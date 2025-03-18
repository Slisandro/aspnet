using Microsoft.AspNetCore.Mvc;
using GestionContableYugen.Models;
using GestionContableYugen.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text.pdf;
using iTextSharp.text;
using System.Text.Json;

namespace GestionContableYugen.Controllers
{
    [Authorize(Roles = "Administrador, Asistente del Administrador, Cajero")]
    public class CajaChicaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CajaChicaController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index(DateTime? fechaInicio, DateTime? fechaFin, string usuario, string proveedor)
        {
            var caja = await _context.CajaChicaConfig.FirstOrDefaultAsync();
            ViewBag.SaldoDisponible = caja?.SaldoDisponible ?? 0;

            var movimientos = _context.Caja_Chica
                .Include(c => c.Usuario)
                .Select(c => new CajaChicaViewModel
                {
                    MovimientoID = c.MovimientoID,
                    UsuarioID = c.UsuarioID,
                    NombreUsuario = c.Usuario != null ? c.Usuario.NombreUsuario : "Desconocido",
                    FechaMovimiento = c.FechaMovimiento,
                    Monto = c.Monto,
                    Descripcion = c.Descripcion,
                    NumeroFactura = c.NumeroFactura ?? "Sin Factura",
                    Proveedor = c.Proveedor ?? "No Especificado",
                    FacturaAdjunta = c.FacturaAdjunta ?? "No Adjunta",
                    Anulado = c.Anulado ?? false
                });

            // 🔥 Aplicar Filtros
            if (fechaInicio.HasValue)
                movimientos = movimientos.Where(m => m.FechaMovimiento >= fechaInicio.Value);

            if (fechaFin.HasValue)
                movimientos = movimientos.Where(m => m.FechaMovimiento <= fechaFin.Value);

            if (!string.IsNullOrEmpty(proveedor))
                movimientos = movimientos.Where(m => m.Proveedor.Contains(proveedor));

            var listaFiltrada = await movimientos.ToListAsync();

            HttpContext.Session.SetString("ListaCajaChica", JsonSerializer.Serialize(listaFiltrada));

            return View(listaFiltrada);
        }

        [HttpGet("CajaChica/ExportarPDF")]
        public IActionResult ExportarPDF()
        {
            var listaFiltrada = HttpContext.Session.GetString("ListaCajaChica");

            if (string.IsNullOrEmpty(listaFiltrada))
                return BadRequest("No hay datos filtrados para exportar.");

            var movimientos = JsonSerializer.Deserialize<List<CajaChicaViewModel>>(listaFiltrada);

            return GenerarPDF(movimientos);
        }

        //Generar PDF
        private IActionResult GenerarPDF(List<CajaChicaViewModel> lista)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10, 10, 20, 10);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                Font tituloFont = new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD, BaseColor.BLACK);
                Font subtituloFont = new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, BaseColor.DARK_GRAY);
                Font encabezadoFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);
                Font contenidoFont = new Font(Font.FontFamily.HELVETICA, 10);

                Paragraph titulo = new Paragraph("Reporte de Movimientos de Caja Chica", tituloFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(titulo);

                Paragraph fechaReporte = new Paragraph("Generado el: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), subtituloFont)
                {
                    Alignment = Element.ALIGN_CENTER
                };
                document.Add(fechaReporte);

                document.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 2, 2, 3, 2, 2, 2 });

                string[] headers = { "Usuario", "Monto", "Descripción", "Proveedor", "Fecha", "Estado" };
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
                        table.AddCell(new Phrase(item.NombreUsuario, contenidoFont));
                        table.AddCell(new Phrase(item.Monto.ToString("N2"), contenidoFont));
                        table.AddCell(new Phrase(item.Descripcion, contenidoFont));
                        table.AddCell(new Phrase(item.Proveedor, contenidoFont));
                        table.AddCell(new Phrase(item.FechaMovimiento.ToShortDateString(), contenidoFont));
                        table.AddCell(new Phrase(item.Anulado ? "Anulado" : "Activo", contenidoFont));
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

                return File(ms.ToArray(), "application/pdf", "Reporte_CajaChica.pdf");
            }
        }


        //  ANULAR MOVIMIENTO
        [HttpPost]
        public async Task<IActionResult> Anular(int id)
        {
            var movimiento = await _context.Caja_Chica.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound(new { success = false, message = "El movimiento no existe." });
            }

            // Marcar como anulado
            movimiento.Anulado = true;

            // Guardar cambios en la base de datos
            await _context.SaveChangesAsync();

            // Recalcular saldo disponible
            var saldoDisponible = await _context.Caja_Chica
                .Where(m => m.Anulado == false)
                .SumAsync(m => m.Monto);

            return Json(new { success = true, message = "Movimiento anulado correctamente.", saldo = saldoDisponible });
        }


        //  CREAR MOVIMIENTO (GET)
        public async Task<IActionResult> Create()
        {
            var usuarioNombre = HttpContext.Session.GetString("Usuario");
            var usuarioID = HttpContext.Session.GetInt32("UsuarioID");

            if (string.IsNullOrEmpty(usuarioNombre) || usuarioID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cajaChicaVM = new CajaChicaViewModel
            {
                UsuarioID = usuarioID.Value,
                NombreUsuario = usuarioNombre,
                FechaMovimiento = DateTime.Now
            };

            return View(cajaChicaVM);
        }

        // CREAR MOVIMIENTO (POST) 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CajaChicaViewModel cajaChicaVM, IFormFile? FacturaAdjuntaFile)
        {
            Console.WriteLine(" [POST] Create - Recibiendo datos...");

            if (!ModelState.IsValid)
            {
                Console.WriteLine(" Error de validación:");
                return BadRequest(ModelState);
            }

            // Obtener el único registro de configuración de Caja Chica
            var caja = await _context.CajaChicaConfig
                .OrderBy(c => c.ID) // ✅ Asegurar que traemos el primer registro
                .FirstOrDefaultAsync();

            if (caja == null)
            {
                return BadRequest("Error: No se encontró la configuración de Caja Chica.");
            }


            var saldoDisponible = caja.SaldoDisponible;
            // var totalMovimientos = await _context.Caja_Chica.Where(m => m.Anulado == false).SumAsync(m => (decimal?)m.Monto) ?? 0;
            // var saldoReal = saldoDisponible - totalMovimientos;

            // 🔥 VALIDACIÓN: El monto NO puede ser mayor al saldo disponible
            if (cajaChicaVM.Monto > saldoDisponible)
            {
                return BadRequest("El monto del movimiento excede el saldo disponible."); // Envío de error a AJAX
            }

            var usuarioLogueado = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.NombreUsuario == User.Identity.Name);

            if (usuarioLogueado == null)
            {
                return BadRequest("Usuario no encontrado.");
            }

            var movimiento = new CajaChica
            {
                UsuarioID = usuarioLogueado.UsuarioID,
                FechaMovimiento = cajaChicaVM.FechaMovimiento,
                Monto = cajaChicaVM.Monto,
                Descripcion = cajaChicaVM.Descripcion ?? "Sin descripción",
                NumeroFactura = cajaChicaVM.NumeroFactura ?? "Sin Factura",
                Proveedor = cajaChicaVM.Proveedor ?? "No Especificado",
                FacturaAdjunta = "No Adjunta",
                Anulado = false
            };

            // ✅ Aquí restauramos la lógica que antes funcionaba
            if (FacturaAdjuntaFile != null && FacturaAdjuntaFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FacturaAdjuntaFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await FacturaAdjuntaFile.CopyToAsync(fileStream);
                }

                movimiento.FacturaAdjunta = "/uploads/" + uniqueFileName; // 🔥 RESTAURADO a la forma que antes funcionaba
                Console.WriteLine($"✅ Archivo guardado en: {movimiento.FacturaAdjunta}");
            }


            // ✅ Restar el monto al saldo disponible
            caja.SaldoDisponible -= cajaChicaVM.Monto;
            _context.Update(caja); // Guardar el nuevo saldo

     
            _context.Add(movimiento);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Movimiento registrado correctamente. Nuevo saldo: {caja.SaldoDisponible:N2} CRC";

            return RedirectToAction("Index");
        }

        // EDITAR MOVIMIENTO - GET
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var movimiento = await _context.Caja_Chica.FindAsync(id);

            if (movimiento == null)
            {
                return NotFound();
            }

            var usuario = await _context.Usuarios.FindAsync(movimiento.UsuarioID);

            var movimientoVM = new CajaChicaViewModel
            {
                MovimientoID = movimiento.MovimientoID,
                UsuarioID = movimiento.UsuarioID,
                NombreUsuario = usuario != null ? usuario.NombreUsuario : "Usuario no encontrado",
                FechaMovimiento = movimiento.FechaMovimiento,
                Monto = movimiento.Monto,
                Descripcion = movimiento.Descripcion,
                NumeroFactura = movimiento.NumeroFactura,
                Proveedor = movimiento.Proveedor,
                FacturaAdjunta = movimiento.FacturaAdjunta
            };


            return View(movimientoVM);
        }

        // EDITAR MOVIMIENTO - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CajaChicaViewModel cajaChicaVM, IFormFile? FacturaAdjuntaFile)
        {
            if (!ModelState.IsValid)
            {
                return View(cajaChicaVM);
            }

            var movimiento = await _context.Caja_Chica.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }

            // Actualizar campos editables
            movimiento.FechaMovimiento = cajaChicaVM.FechaMovimiento;
            movimiento.Monto = cajaChicaVM.Monto;
            movimiento.Descripcion = cajaChicaVM.Descripcion;
            movimiento.NumeroFactura = cajaChicaVM.NumeroFactura ?? "Sin Factura";
            movimiento.Proveedor = cajaChicaVM.Proveedor ?? "No Especificado";

            //  Solo actualizar la factura si el usuario sube un nuevo archivo
            if (FacturaAdjuntaFile != null && FacturaAdjuntaFile.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");
                Directory.CreateDirectory(uploadsFolder);
                var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(FacturaAdjuntaFile.FileName);
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await FacturaAdjuntaFile.CopyToAsync(fileStream);
                }

                movimiento.FacturaAdjunta = "/uploads/" + uniqueFileName;
            }

            _context.Update(movimiento);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }


        // ===========================
        // 🔥 CALCULAR SALDO DISPONIBLE 
        // ===========================
        public decimal ObtenerSaldoDisponible()
        {
            var saldoConfig = _context.CajaChicaConfig.FirstOrDefault()?.SaldoDisponible ?? 0;
            var totalMovimientos = _context.Caja_Chica
                .Where(m => m.Anulado == false)
                .Sum(m => (decimal?)m.Monto) ?? 0;

            decimal saldoFinal = saldoConfig - totalMovimientos;
            Console.WriteLine($"🔍 [INFO] Cálculo de saldo disponible: {saldoConfig} - {totalMovimientos} = {saldoFinal}");

            return saldoFinal;
        }

    }
}
