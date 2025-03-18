using Microsoft.AspNetCore.Mvc;
using GestionContableYugen.Models;
using GestionContableYugen.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text.Json;

namespace GestionContableYugen.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class ProveedorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProveedorController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //*********************//
        [HttpGet]
        [Route("Proveedores/GetProveedorData/{id}")]
        public async Task<IActionResult> GetProveedorData(int id)
        {
            var proveedor = await _context.Proveedores
                .Where(p => p.ProveedorID == id)
                .Select(p => new
                {
                    NombreProveedor = p.NombreProveedor,
                    NumeroCedula = p.NumeroCedula,
                    Telefono = p.Telefono,
                    CorreoElectronico = p.CorreoElectronico
                })
                .FirstOrDefaultAsync();

            if (proveedor == null)
            {
                return NotFound();
            }

            return Json(proveedor);
        }

        //*********************//

        // Listar Proveedores con Filtros
        public async Task<IActionResult> Index(string nombreProveedor, string numeroCedula, string estado)
        {
            var proveedores = _context.Proveedores.AsQueryable();

            // Aplicar filtros
            if (!string.IsNullOrEmpty(nombreProveedor))
            {
                proveedores = proveedores.Where(p => p.NombreProveedor.Contains(nombreProveedor));
            }

            if (!string.IsNullOrEmpty(numeroCedula))
            {
                proveedores = proveedores.Where(p => p.NumeroCedula.Contains(numeroCedula));
            }

            if (!string.IsNullOrEmpty(estado))
            {
                bool estadoBool = estado == "Activo";
                proveedores = proveedores.Where(p => p.Estado == estadoBool);
            }

            var proveedoresList = await proveedores.ToListAsync();
            var proveedoresVM = _mapper.Map<List<ProveedorViewModel>>(proveedoresList);

            // 🔥 Guardar la lista filtrada en sesión para exportar PDF
            HttpContext.Session.SetString("ListaProveedores", JsonSerializer.Serialize(proveedoresVM));

            return View(proveedoresVM);
        }

        // 🔥 Exportar PDF
        [HttpGet("Proveedor/ExportarPDF")]
        public IActionResult ExportarPDF()
        {
            var listaFiltrada = HttpContext.Session.GetString("ListaProveedores");

            if (string.IsNullOrEmpty(listaFiltrada))
                return BadRequest("No hay datos filtrados para exportar.");

            var proveedores = JsonSerializer.Deserialize<List<ProveedorViewModel>>(listaFiltrada);

            return GenerarPDF(proveedores);
        }

        // 🔥 Método para Generar PDF 🔥
        private IActionResult GenerarPDF(List<ProveedorViewModel> lista)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10, 10, 20, 10);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                // 🔹 Agregar título y fecha de generación
                Font tituloFont = new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD, BaseColor.BLACK);
                Font subtituloFont = new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, BaseColor.DARK_GRAY);
                Font encabezadoFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);
                Font contenidoFont = new Font(Font.FontFamily.HELVETICA, 10);

                Paragraph titulo = new Paragraph("Reporte de Proveedores", tituloFont);
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                Paragraph fechaReporte = new Paragraph("Generado el: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), subtituloFont);
                fechaReporte.Alignment = Element.ALIGN_CENTER;
                document.Add(fechaReporte);

                document.Add(new Paragraph("\n"));

                // 🔹 Crear tabla con 5 columnas
                PdfPTable table = new PdfPTable(5);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 3, 2, 2, 3, 2 });

                string[] headers = { "Nombre", "Cédula", "Teléfono", "Correo Electrónico", "Estado" };
                foreach (var header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, encabezadoFont))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    table.AddCell(cell);
                }

                if (lista.Count > 0)
                {
                    foreach (var item in lista)
                    {
                        table.AddCell(new Phrase(item.NombreProveedor, contenidoFont));
                        table.AddCell(new Phrase(item.NumeroCedula, contenidoFont));
                        table.AddCell(new Phrase(item.Telefono, contenidoFont));
                        table.AddCell(new Phrase(item.CorreoElectronico, contenidoFont));
                        table.AddCell(new Phrase(item.Estado ? "Activo" : "Inactivo", contenidoFont));
                    }
                }
                else
                {
                    PdfPCell emptyCell = new PdfPCell(new Phrase("No hay datos disponibles", contenidoFont))
                    {
                        Colspan = 5,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    table.AddCell(emptyCell);
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(), "application/pdf", "Reporte_Proveedores.pdf");
            }
        }

        // Crear Proveedor (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Crear Proveedor (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProveedorViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var proveedor = new Proveedor
            {
                NombreProveedor = model.NombreProveedor,
                NumeroCedula = model.NumeroCedula,
                Telefono = model.Telefono,
                CorreoElectronico = model.CorreoElectronico,
                Direccion = model.Direccion,
                Estado = true,  
                FechaRegistro = DateTime.Now
            };

            _context.Proveedores.Add(proveedor);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Editar Proveedor
        public async Task<IActionResult> Edit(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            var proveedorVM = new ProveedorViewModel
            {
                ProveedorID = proveedor.ProveedorID,
                NombreProveedor = proveedor.NombreProveedor,
                NumeroCedula = proveedor.NumeroCedula,
                Telefono = proveedor.Telefono,
                CorreoElectronico = proveedor.CorreoElectronico,
                Direccion = proveedor.Direccion,
                Estado = proveedor.Estado  // Agregar Estado para que aparezca el CheckBox
            };

            return View(proveedorVM);
        }

        // POST: Guardar cambios en Proveedor
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProveedorViewModel model)
        {
            if (id != model.ProveedorID) return NotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            proveedor.NombreProveedor = model.NombreProveedor;
            proveedor.NumeroCedula = model.NumeroCedula;
            proveedor.Telefono = model.Telefono;
            proveedor.CorreoElectronico = model.CorreoElectronico;
            proveedor.Direccion = model.Direccion;
            proveedor.Estado = model.Estado; // Ahora guarda el estado correctamente

            _context.Update(proveedor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // Deshabilitar Proveedor (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor == null) return NotFound();

            var proveedorVM = _mapper.Map<ProveedorViewModel>(proveedor);
            return View(proveedorVM);
        }

        // Deshabilitar Proveedor (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor != null)
            {
                proveedor.Estado = false; 
                _context.Update(proveedor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }


        // Reactivar Proveedor
        [HttpPost]
        public async Task<IActionResult> Reactivar(int id)
        {
            var proveedor = await _context.Proveedores.FindAsync(id);
            if (proveedor != null)
            {
                proveedor.Estado = true;
                _context.Update(proveedor);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
