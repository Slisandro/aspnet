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
    
    public class CuentaPorCobrarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        

        public CuentaPorCobrarController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
            
        }

        [HttpGet("CuentaPorCobrar/Index")]
        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, string clienteNombre)
        {
            var cuentas = _context.CuentasPorCobrar
                .Include(c => c.Cliente)
                .Select(c => new CuentaPorCobrarViewModel
                {
                    CuentaID = c.CuentaID,
                    ClienteID = c.ClienteID,
                    NombreCliente = c.Cliente.NombreCliente,
                    NumeroCedula = c.Cliente.NumeroCedula,
                    Telefono = c.Cliente.Telefono,
                    CorreoElectronico = c.Cliente.CorreoElectronico,
                    Monto = c.Monto,
                    FechaVencimiento = c.FechaVencimiento,
                    Estado = c.Estado
                });

            List<CuentaPorCobrarViewModel> listaFiltrada = await cuentas.ToListAsync();

            if (fechaInicio.HasValue)
                listaFiltrada = listaFiltrada.Where(c => c.FechaVencimiento >= fechaInicio.Value).ToList();

            if (fechaFin.HasValue)
                listaFiltrada = listaFiltrada.Where(c => c.FechaVencimiento <= fechaFin.Value).ToList();

            if (!string.IsNullOrEmpty(clienteNombre))
                listaFiltrada = listaFiltrada.Where(c => c.NombreCliente.Contains(clienteNombre, StringComparison.OrdinalIgnoreCase)).ToList();

            // 🔥 GUARDAR EN SESSION LA LISTA FILTRADA 🔥
            HttpContext.Session.SetString("ListaFiltrada", JsonSerializer.Serialize(listaFiltrada));

            return View(listaFiltrada);
        }

        [HttpGet("CuentaPorCobrar/ExportarPDF")]
        public IActionResult ExportarPDF()
        {
            // 🔥 RECUPERAR LA LISTA FILTRADA 🔥
            var listaFiltrada = HttpContext.Session.GetString("ListaFiltrada");

            if (string.IsNullOrEmpty(listaFiltrada))
                return BadRequest("No hay datos filtrados para exportar."); // Evitar errores si la sesión está vacía

            var cuentas = JsonSerializer.Deserialize<List<CuentaPorCobrarViewModel>>(listaFiltrada);

            return GenerarPDF(cuentas);
        }

        //GENERAR PDF
        private IActionResult GenerarPDF(List<CuentaPorCobrarViewModel> lista)
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

                Paragraph titulo = new Paragraph("Reporte de Cuentas por Cobrar", tituloFont);
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

                string[] headers = { "Cliente", "Cédula", "Teléfono", "Monto", "Fecha Vencimiento", "Estado" };
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

                return File(ms.ToArray(), "application/pdf", "Reporte_CuentasPorCobrar.pdf");
            }
        }


        // CREAR CUENTA POR COBRAR - GET
        public IActionResult Create()
        {
            ViewBag.Clientes = _context.Clientes
                .Select(c => new SelectListItem { Value = c.ClienteID.ToString(), Text = c.NombreCliente })
                .ToList();

            return View();
        }

        // CREAR CUENTA POR COBRAR - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CuentaPorCobrarViewModel cuentaVM)
        {
            if (cuentaVM.FechaVencimiento < DateTime.Today)
            {
                ModelState.AddModelError("FechaVencimiento", "La fecha de vencimiento no puede ser pasada.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var cliente = await _context.Clientes.FindAsync(cuentaVM.ClienteID);
            if (cliente == null)
            {
                ModelState.AddModelError("ClienteID", "El cliente seleccionado no existe.");
                return BadRequest(ModelState);
            }

            var cuenta = new CuentaPorCobrar
            {
                ClienteID = cliente.ClienteID,
                NumeroCedula = cliente.NumeroCedula,
                Telefono = cliente.Telefono,
                CorreoElectronico = cliente.CorreoElectronico,
                Monto = cuentaVM.Monto,
                FechaVencimiento = cuentaVM.FechaVencimiento,
                Estado = cuentaVM.Estado ?? "Pendiente"
            };

            try
            {
                _context.CuentasPorCobrar.Add(cuenta);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cuenta guardada correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Error al guardar: {ex.Message}");
                return StatusCode(500, "Error interno en el servidor.");
            }
        }

        // EDITAR CUENTA - GET
        public async Task<IActionResult> Edit(int id)
        {
            var cuenta = await _context.CuentasPorCobrar.FindAsync(id);
            if (cuenta == null)
            {
                return NotFound();
            }

            ViewBag.Clientes = _context.Clientes
                .Select(c => new SelectListItem { Value = c.ClienteID.ToString(), Text = c.NombreCliente })
                .ToList();

            var cuentaVM = new CuentaPorCobrarViewModel
            {
                CuentaID = cuenta.CuentaID,
                ClienteID = cuenta.ClienteID,
                NumeroCedula = cuenta.NumeroCedula,
                Telefono = cuenta.Telefono,
                CorreoElectronico = cuenta.CorreoElectronico,
                Monto = cuenta.Monto,
                FechaVencimiento = cuenta.FechaVencimiento,
                Estado = cuenta.Estado
            };

            return View(cuentaVM);
        }

        // EDITAR CUENTA - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CuentaPorCobrarViewModel cuentaVM)
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

            var cuenta = await _context.CuentasPorCobrar.FindAsync(id);
            if (cuenta == null)
            {
                return NotFound();
            }

            var cliente = await _context.Clientes.FirstOrDefaultAsync(c => c.ClienteID == cuentaVM.ClienteID);
            if (cliente == null)
            {
                ModelState.AddModelError("ClienteID", "El cliente no existe.");
                return BadRequest(ModelState);
            }

            // Actualizar los datos
            cuenta.ClienteID = cliente.ClienteID;
            cuenta.NumeroCedula = cliente.NumeroCedula;
            cuenta.Telefono = cliente.Telefono;
            cuenta.CorreoElectronico = cliente.CorreoElectronico;
            cuenta.Monto = cuentaVM.Monto;
            cuenta.FechaVencimiento = cuentaVM.FechaVencimiento;
            cuenta.Estado = cuentaVM.Estado;

            try
            {
                _context.Update(cuenta);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cuenta actualizada correctamente" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar: {ex.Message}");
                return StatusCode(500, "Error interno en el servidor.");
            }
        }

        //  ELIMINAR CUENTA (GET)
        public async Task<IActionResult> Delete(int id)
        {
            var cuenta = await _context.CuentasPorCobrar
                .Include(c => c.Cliente)
                .FirstOrDefaultAsync(c => c.CuentaID == id);

            if (cuenta == null)
            {
                return NotFound();
            }

            var cuentaVM = new CuentaPorCobrarViewModel
            {
                CuentaID = cuenta.CuentaID,
                ClienteID = cuenta.ClienteID,
                NombreCliente = cuenta.Cliente?.NombreCliente ?? "Cliente no encontrado", // Ahora se muestra el nombre del cliente
                NumeroCedula = cuenta.Cliente?.NumeroCedula ?? "N/A",
                Telefono = cuenta.Cliente?.Telefono ?? "N/A",
                CorreoElectronico = cuenta.Cliente?.CorreoElectronico ?? "N/A",
                Monto = cuenta.Monto,
                FechaVencimiento = cuenta.FechaVencimiento,
                Estado = cuenta.Estado
            };

            return View(cuentaVM);
        }

        //  ELIMINAR CUENTA (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cuenta = await _context.CuentasPorCobrar.FindAsync(id);

            if (cuenta == null)
            {
                return NotFound();
            }

            try
            {
                _context.CuentasPorCobrar.Remove(cuenta);
                await _context.SaveChangesAsync();
                return Ok(new { message = "Cuenta eliminada correctamente." }); // Se retorna un JSON en lugar de redirección
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno al eliminar: {ex.Message}");
            }
        }


        // Notificaciones
        [HttpGet]
        public async Task<IActionResult> GetPendingNotifications()
        {
            var hoy = DateTime.Today;
            var enCincoDias = hoy.AddDays(5);

            var cuentasPendientes = await _context.CuentasPorCobrar
                .Where(c => c.Estado == "Pendiente" && c.FechaVencimiento >= hoy && c.FechaVencimiento <= enCincoDias)
                .ToListAsync();

            return Json(new { cantidad = cuentasPendientes.Count });
        }

    }
}
