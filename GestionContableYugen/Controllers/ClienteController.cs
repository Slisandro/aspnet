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
using System.IO;
using System.Text.Json;

namespace GestionContableYugen.Controllers
{
    [Authorize(Roles = "Administrador, Asistente del Administrador")]
    public class ClienteController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ClienteController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        //**********//
        [HttpGet]
        [Route("Clientes/GetClienteData/{id}")]
        public async Task<IActionResult> GetClienteData(int id)
        {
            var cliente = await _context.Clientes
                .Where(c => c.ClienteID == id)
                .Select(c => new
                {
                    NumeroCedula = c.NumeroCedula,
                    Telefono = c.Telefono,
                    CorreoElectronico = c.CorreoElectronico
                })
                .FirstOrDefaultAsync();

            if (cliente == null)
            {
                return NotFound();
            }

            return Json(cliente);
        }

        //**********//


        // Listar Clientes
        public async Task<IActionResult> Index(string nombreCliente, string numeroCedula, string estado)
        {
            var clientes = _context.Clientes.AsQueryable();

            // 🔹 Aplicar filtros dinámicos
            if (!string.IsNullOrEmpty(nombreCliente))
            {
                clientes = clientes.Where(c => c.NombreCliente.Contains(nombreCliente));
            }

            if (!string.IsNullOrEmpty(numeroCedula))
            {
                clientes = clientes.Where(c => c.NumeroCedula.Contains(numeroCedula));
            }

            if (!string.IsNullOrEmpty(estado))
            {
                bool estadoBool = estado == "Activo";
                clientes = clientes.Where(c => c.Estado == estadoBool);
            }

            var clientesList = await clientes.ToListAsync();

            var clientesViewModel = clientesList.Select(c => new ClienteViewModel
            {
                ClienteID = c.ClienteID,
                NumeroCedula = c.NumeroCedula,
                NombreCliente = c.NombreCliente,
                Telefono = c.Telefono,
                CorreoElectronico = c.CorreoElectronico,
                Direccion = c.Direccion,
                Estado = c.Estado
            }).ToList();

            // 🔥 Guardamos la lista filtrada en Session para exportación a PDF
            HttpContext.Session.SetString("ListaClientes", JsonSerializer.Serialize(clientesViewModel));

            return View(clientesViewModel);
        }

        //Exportar lista de clientes a PDF
        [HttpGet("Cliente/ExportarPDF")]
        public IActionResult ExportarPDF()
        {
            //Recuperar la lista filtrada de session
            var listaFiltrada = HttpContext.Session.GetString("ListaClientes");

            if (string.IsNullOrEmpty(listaFiltrada))
                return BadRequest("No hay datos filtrados para exportar.");

            var clientes = JsonSerializer.Deserialize<List<ClienteViewModel>>(listaFiltrada);

            return GenerarPDF(clientes);
        }

        //Generar PDF
        private IActionResult GenerarPDF(List<ClienteViewModel> lista)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10, 10, 20, 10);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                //Agregar título y fecha de generación en el reporte
                Font tituloFont = new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD, BaseColor.BLACK);
                Font subtituloFont = new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, BaseColor.DARK_GRAY);
                Font encabezadoFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);
                Font contenidoFont = new Font(Font.FontFamily.HELVETICA, 10);

                Paragraph titulo = new Paragraph("Reporte de Clientes", tituloFont);
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                Paragraph fechaReporte = new Paragraph("Generado el: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), subtituloFont);
                fechaReporte.Alignment = Element.ALIGN_CENTER;
                document.Add(fechaReporte);

                document.Add(new Paragraph("\n"));

                //Crear una tabla con 5 columnas
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
                        table.AddCell(new Phrase(item.NombreCliente, contenidoFont));
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

                return File(ms.ToArray(), "application/pdf", "Reporte_Clientes.pdf");
            }
        }


        // Crear Cliente (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Crear Cliente (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ClienteViewModel clienteVM)
        {
            if (!ModelState.IsValid)
            {
                return View(clienteVM);
            }

            var cliente = new Cliente
            {
                NombreCliente = clienteVM.NombreCliente,
                NumeroCedula = clienteVM.NumeroCedula,
                Telefono = clienteVM.Telefono,
                CorreoElectronico = clienteVM.CorreoElectronico,
                Direccion = clienteVM.Direccion,
                Estado = true,
                FechaRegistro = DateTime.Now 
            };

            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Editar Cliente (GET)
        public async Task<IActionResult> Edit(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            var clienteVM = new ClienteViewModel
            {
                ClienteID = cliente.ClienteID,
                NombreCliente = cliente.NombreCliente,
                NumeroCedula = cliente.NumeroCedula,
                Telefono = cliente.Telefono,
                CorreoElectronico = cliente.CorreoElectronico,
                Direccion = cliente.Direccion,
                Estado = cliente.Estado
            };

            return View(clienteVM);
        }

        // Editar Cliente (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ClienteViewModel clienteVM)
        {
            if (id != clienteVM.ClienteID) return NotFound();

            // 🔍 Agregamos esto para ver si hay errores en el ModelState
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine(error.ErrorMessage);  // Muestra errores en la consola del backend
                }
                return View(clienteVM);
            }

            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null) return NotFound();

            cliente.NombreCliente = clienteVM.NombreCliente;
            cliente.NumeroCedula = clienteVM.NumeroCedula;
            cliente.Telefono = clienteVM.Telefono;
            cliente.CorreoElectronico = clienteVM.CorreoElectronico;
            cliente.Direccion = clienteVM.Direccion;
            cliente.Estado = clienteVM.Estado;

            try
            {
                _context.Update(cliente);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Error al actualizar el cliente en la base de datos.");
                return View(clienteVM);
            }

            return RedirectToAction(nameof(Index));
        }

        // Eliminar Cliente (GET) -  Deshabilitar
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ClienteID == id);
            if (cliente == null) return NotFound();

            var clienteVM = _mapper.Map<ClienteViewModel>(cliente);
            return View(clienteVM);
        }

        // Deshabilitar Cliente (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Estado = false;  // Se deshabilita en lugar de eliminar
                _context.Update(cliente);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Reactivar Cliente
        [HttpPost]
        public async Task<IActionResult> Reactivar(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente != null)
            {
                cliente.Estado = true;
                _context.Update(cliente);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

    }
}
