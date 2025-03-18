using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionContableYugen.Models;
using GestionContableYugen.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClosedXML.Excel;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace GestionContableYugen.Controllers
{
    public class ReportesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Vista principal de Reportes
        public IActionResult Index()
        {
            ViewBag.Clientes = _context.Clientes
                .Select(c => new SelectListItem { Value = c.ClienteID.ToString(), Text = c.NombreCliente })
                .ToList();

            return View();
        }

        // Cargar formulario de Reporte de Facturación
        [HttpGet]
        public IActionResult ReporteFacturacion()
        {
            ViewBag.Clientes = _context.Clientes
                .Select(c => new SelectListItem { Value = c.ClienteID.ToString(), Text = c.NombreCliente })
                .ToList();

            return View(new List<ReporteFacturacionViewModel>()); // Retorna una lista vacía inicialmente
        }

        // Procesar Reporte de Facturación
        [HttpPost]
        public async Task<IActionResult> ReporteFacturacion(DateTime fechaInicio, DateTime fechaFin, int? clienteId)
        {
            var facturas = await _context.Facturas
                .Include(f => f.Cliente)
                .Where(f => f.FechaFactura >= fechaInicio && f.FechaFactura <= fechaFin)
                .Where(f => clienteId == null || f.ClienteID == clienteId)
                .Select(f => new ReporteFacturacionViewModel
                {
                    FacturaID = f.FacturaID,
                    ClienteNombre = f.Cliente.NombreCliente,
                    Fecha = f.FechaFactura,
                    Total = f.Total
                })
                .ToListAsync();

            ViewBag.Clientes = _context.Clientes
                .Select(c => new SelectListItem { Value = c.ClienteID.ToString(), Text = c.NombreCliente })
                .ToList();

            return View(facturas);
        }

        // Exportar Reporte de Facturación a Excel
        public IActionResult ExportarExcel(DateTime fechaInicio, DateTime fechaFin, int? clienteId)
        {
            var facturas = _context.Facturas
                .Include(f => f.Cliente)
                .Where(f => f.FechaFactura >= fechaInicio && f.FechaFactura <= fechaFin)
                .Where(f => clienteId == null || f.ClienteID == clienteId)
                .Select(f => new ReporteFacturacionViewModel
                {
                    FacturaID = f.FacturaID,
                    ClienteNombre = f.Cliente.NombreCliente,
                    Fecha = f.FechaFactura,
                    Total = f.Total
                })
                .ToList();

            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Facturación");
                worksheet.Cell(1, 1).Value = "ID Factura";
                worksheet.Cell(1, 2).Value = "Cliente";
                worksheet.Cell(1, 3).Value = "Fecha";
                worksheet.Cell(1, 4).Value = "Total";

                for (int i = 0; i < facturas.Count(); i++)
                {
                    worksheet.Cell(i + 2, 1).Value = facturas[i].FacturaID;
                    worksheet.Cell(i + 2, 2).Value = facturas[i].ClienteNombre;
                    worksheet.Cell(i + 2, 3).Value = facturas[i].Fecha.ToShortDateString();
                    worksheet.Cell(i + 2, 4).Value = facturas[i].Total;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "Reporte_Facturacion.xlsx");
                }
            }
        }

        // Exportar Reporte de Facturación a PDF
        public IActionResult ExportarPDF(DateTime fechaInicio, DateTime fechaFin, int? clienteId)
        {
            var facturas = _context.Facturas
                .Include(f => f.Cliente)
                .Where(f => f.FechaFactura >= fechaInicio && f.FechaFactura <= fechaFin)
                .Where(f => clienteId == null || f.ClienteID == clienteId)
                .Select(f => new ReporteFacturacionViewModel
                {
                    FacturaID = f.FacturaID,
                    ClienteNombre = f.Cliente.NombreCliente,
                    Fecha = f.FechaFactura,
                    Total = f.Total
                })
                .ToList();

            using (MemoryStream stream = new MemoryStream())
            {
                Document document = new Document();
                PdfWriter.GetInstance(document, stream);
                document.Open();

                document.Add(new Paragraph("Reporte de Facturación"));
                document.Add(new Paragraph($"Desde: {fechaInicio.ToShortDateString()} Hasta: {fechaFin.ToShortDateString()}"));
                document.Add(new Paragraph("\n"));

                PdfPTable table = new PdfPTable(4);
                table.AddCell("ID Factura");
                table.AddCell("Cliente");
                table.AddCell("Fecha");
                table.AddCell("Total");

                foreach (var factura in facturas)
                {
                    table.AddCell(factura.FacturaID.ToString());
                    table.AddCell(factura.ClienteNombre);
                    table.AddCell(factura.Fecha.ToShortDateString());
                    table.AddCell(factura.Total.ToString("C"));
                }

                document.Add(table);
                document.Close();

                return File(stream.ToArray(), "application/pdf", "Reporte_Facturacion.pdf");
            }
        }
    }
}
