using Microsoft.AspNetCore.Mvc;
using GestionContableYugen.Models;
using GestionContableYugen.ViewModels;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authorization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using System.Text.Json;




namespace GestionContableYugen.Controllers
{
    [Authorize(Roles = "Administrador, Asistente")]
    public class CuentaPorPagarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        


        public CuentaPorPagarController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
           
        }

        // Listar Cuentas por Pagar
        [HttpGet("CuentaPorPagar/Index")]
        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, string proveedorNombre)
        {
            var cuentas = _context.CuentasPorPagar
                .Include(c => c.Proveedor)
                .Select(c => new CuentaPorPagarViewModel
                {
                    CuentaID = c.CuentaID,
                    ProveedorID = c.ProveedorID,
                    NombreProveedor = c.Proveedor.NombreProveedor,
                    NumeroCedula = c.Proveedor.NumeroCedula,
                    Telefono = c.Proveedor.Telefono,
                    CorreoElectronico = c.Proveedor.CorreoElectronico,
                    Monto = c.Monto,
                    FechaVencimiento = c.FechaVencimiento,
                    Estado = c.Estado
                });

            List<CuentaPorPagarViewModel> listaFiltrada = await cuentas.ToListAsync();

            if (fechaInicio.HasValue)
                listaFiltrada = listaFiltrada.Where(c => c.FechaVencimiento >= fechaInicio.Value).ToList();

            if (fechaFin.HasValue)
                listaFiltrada = listaFiltrada.Where(c => c.FechaVencimiento <= fechaFin.Value).ToList();

            if (!string.IsNullOrEmpty(proveedorNombre))
                listaFiltrada = listaFiltrada.Where(c => c.NombreProveedor.Contains(proveedorNombre, StringComparison.OrdinalIgnoreCase)).ToList();

            //Guardar la lista filtrada en sesion
            HttpContext.Session.SetString("ListaFiltrada", JsonSerializer.Serialize(listaFiltrada));

            return View(listaFiltrada);
        }

        [HttpGet("CuentaPorPagar/ExportarPDF")]
        public IActionResult ExportarPDF()
        {
            // 🔥 RECUPERAR LA LISTA FILTRADA 🔥
            var listaFiltrada = HttpContext.Session.GetString("ListaFiltrada");

            if (string.IsNullOrEmpty(listaFiltrada))
                return BadRequest("No hay datos filtrados para exportar.");

            var cuentas = JsonSerializer.Deserialize<List<CuentaPorPagarViewModel>>(listaFiltrada);

            return GenerarPDF(cuentas);
        }

        private IActionResult GenerarPDF(List<CuentaPorPagarViewModel> lista)
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

                Paragraph titulo = new Paragraph("Reporte de Cuentas por Pagar", tituloFont);
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                Paragraph fechaReporte = new Paragraph("Generado el: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), subtituloFont);
                fechaReporte.Alignment = Element.ALIGN_CENTER;
                document.Add(fechaReporte);

                document.Add(new Paragraph("\n"));

                // 🔹 Crear tabla
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2, 2, 2, 2, 2, 2 });

                string[] headers = { "Proveedor", "Cédula", "Teléfono", "Monto", "Fecha Vencimiento", "Estado" };
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
                        table.AddCell(new Phrase(item.Monto.ToString("N2"), contenidoFont));
                        table.AddCell(new Phrase(item.FechaVencimiento.ToShortDateString(), contenidoFont));
                        table.AddCell(new Phrase(item.Estado, contenidoFont));
                    }
                }
                else
                {
                    PdfPCell emptyCell = new PdfPCell(new Phrase("No hay datos disponibles", contenidoFont))
                    {
                        Colspan = 6,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    table.AddCell(emptyCell);
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(), "application/pdf", "Reporte_CuentasPorPagar.pdf");
            }
        }


        // Crear Nueva Cuenta por Pagar (GET)
        public IActionResult Create()
        {
            ViewBag.Proveedores = _context.Proveedores
                .Select(p => new SelectListItem { Value = p.ProveedorID.ToString(), Text = p.NombreProveedor })
                .ToList();

            return View();
        }

        // Crear Nueva Cuenta por Pagar (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CuentaPorPagarViewModel cuentaVM)
        {
            if (cuentaVM.FechaVencimiento < DateTime.Today)
            {
                ModelState.AddModelError("FechaVencimiento", "La fecha de vencimiento no puede ser pasada.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var proveedor = await _context.Proveedores.FindAsync(cuentaVM.ProveedorID);
            if (proveedor == null)
            {
                ModelState.AddModelError("ProveedorID", "El proveedor seleccionado no existe.");
                return BadRequest(ModelState);
            }

            var cuenta = new CuentaPorPagar
            {
                ProveedorID = proveedor.ProveedorID,
                NombreProveedor = proveedor.NombreProveedor,
                NumeroCedula = proveedor.NumeroCedula,
                Telefono = proveedor.Telefono,
                CorreoElectronico = proveedor.CorreoElectronico,
                Monto = cuentaVM.Monto,
                FechaVencimiento = cuentaVM.FechaVencimiento,
                Estado = cuentaVM.Estado ?? "Pendiente"
            };

            try
            {
                _context.CuentasPorPagar.Add(cuenta);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cuenta por Pagar guardada correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al guardar: {ex.Message}");
                return StatusCode(500, "Error interno en el servidor.");
            }
        }

        // EDITAR CUENTA POR PAGAR - GET
        public async Task<IActionResult> Edit(int id)
        {
            var cuenta = await _context.CuentasPorPagar
                .Include(c => c.Proveedor)
                .FirstOrDefaultAsync(c => c.CuentaID == id);

            if (cuenta == null)
            {
                return NotFound();
            }

            ViewBag.Proveedores = _context.Proveedores
                .Select(p => new SelectListItem { Value = p.ProveedorID.ToString(), Text = p.NombreProveedor })
                .ToList();

            var cuentaVM = new CuentaPorPagarViewModel
            {
                CuentaID = cuenta.CuentaID,
                ProveedorID = cuenta.ProveedorID,
                NombreProveedor = cuenta.Proveedor?.NombreProveedor ?? "Proveedor no encontrado",
                NumeroCedula = cuenta.Proveedor?.NumeroCedula ?? "",
                Telefono = cuenta.Proveedor?.Telefono ?? "",
                CorreoElectronico = cuenta.Proveedor?.CorreoElectronico ?? "",
                Monto = cuenta.Monto,
                FechaVencimiento = cuenta.FechaVencimiento,
                Estado = cuenta.Estado
            };

            return View(cuentaVM);
        }


        // EDITAR CUENTA POR PAGAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CuentaPorPagarViewModel cuentaVM)
        {

            if (cuentaVM.FechaVencimiento < DateTime.Today)
            {
                ModelState.AddModelError("FechaVencimiento", "La fecha de vencimiento no puede ser pasada.");
            }

            if (cuentaVM.CuentaID == 0)
            {
                return BadRequest("El ID de la cuenta no se envió correctamente.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cuenta = await _context.CuentasPorPagar.FindAsync(id);
            if (cuenta == null)
            {
                return NotFound();
            }

            var proveedor = await _context.Proveedores.FirstOrDefaultAsync(p => p.ProveedorID == cuentaVM.ProveedorID);
            if (proveedor == null)
            {
                ModelState.AddModelError("ProveedorID", "El proveedor no existe.");
                return BadRequest(ModelState);
            }

            // Actualizar los datos
            cuenta.ProveedorID = proveedor.ProveedorID;
            cuenta.NumeroCedula = proveedor.NumeroCedula;
            cuenta.Telefono = proveedor.Telefono;
            cuenta.CorreoElectronico = proveedor.CorreoElectronico;
            cuenta.Monto = cuentaVM.Monto;
            cuenta.FechaVencimiento = cuentaVM.FechaVencimiento;
            cuenta.Estado = cuentaVM.Estado;

            try
            {
                _context.Update(cuenta);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cuenta por Pagar actualizada correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar: {ex.Message}");
                return StatusCode(500, "Error interno en el servidor.");
            }
        }

        // ELIMINAR CUENTA POR PAGAR - GET
        public async Task<IActionResult> Delete(int id)
        {
            var cuenta = await _context.CuentasPorPagar
                .Include(c => c.Proveedor)
                .FirstOrDefaultAsync(c => c.CuentaID == id);

            if (cuenta == null)
            {
                return NotFound();
            }

            var cuentaVM = new CuentaPorPagarViewModel
            {
                CuentaID = cuenta.CuentaID,
                ProveedorID = cuenta.ProveedorID,
                NombreProveedor = cuenta.Proveedor?.NombreProveedor ?? "Proveedor no encontrado",
                NumeroCedula = cuenta.Proveedor?.NumeroCedula ?? "N/A",
                Telefono = cuenta.Proveedor?.Telefono ?? "N/A",
                CorreoElectronico = cuenta.Proveedor?.CorreoElectronico ?? "N/A",
                Monto = cuenta.Monto,
                FechaVencimiento = cuenta.FechaVencimiento,
                Estado = cuenta.Estado
            };

            return View(cuentaVM);
        }

        // ELIMINAR CUENTA POR PAGAR - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cuenta = await _context.CuentasPorPagar.FindAsync(id);

            if (cuenta == null)
            {
                return NotFound();
            }

            try
            {
                _context.CuentasPorPagar.Remove(cuenta);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cuenta eliminada correctamente." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al eliminar: {ex.Message}");
            }
        }

        //Notificaciones

        [HttpGet]
        public async Task<IActionResult> GetPendingNotifications()
        {
            var hoy = DateTime.Today;
            var enCincoDias = hoy.AddDays(5);

            var cuentasPendientes = await _context.CuentasPorPagar
                .Where(c => c.Estado == "Pendiente" && c.FechaVencimiento >= hoy && c.FechaVencimiento <= enCincoDias)
                .ToListAsync();

            return Json(new { cantidad = cuentasPendientes.Count });
        }
    }
}
