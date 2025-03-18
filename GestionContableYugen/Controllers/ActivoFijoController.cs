using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AutoMapper;
using GestionContableYugen.Models;
using GestionContableYugen.ViewModels;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Text.Json;

namespace GestionContableYugen.Controllers
{
    [Authorize(Roles = "Administrador, Asistente del Administrador")]
    public class ActivoFijoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        // Constructor con la inyección de dependencias
        public ActivoFijoController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Listar Activos Fijos
        public async Task<IActionResult> Index(string nombreActivo, string estado)
        {
            var activos = _context.ActivosFijos.AsQueryable();

            // Filtros dinámicos
            if (!string.IsNullOrEmpty(nombreActivo))
            {
                activos = activos.Where(a => a.NombreActivo.Contains(nombreActivo));
            }

            if (!string.IsNullOrEmpty(estado) && estado != "Todos")
            {
                activos = activos.Where(a => a.Estado != null && a.Estado == estado);
            }

            var activosList = await activos.ToListAsync();
            var activosVM = _mapper.Map<List<ActivoFijoViewModel>>(activosList);

            // Aplicar el cálculo de depreciación en memoria
            foreach (var activo in activosVM)
            {
                activo.ValorDepreciado = CalcularDepreciacion(activo.ValorInicial, activo.VidaUtil);
            }

            // GUARDAR LA LISTA FILTRADA EN SESSION PARA EXPORTAR PDF
            HttpContext.Session.SetString("ListaActivos", JsonSerializer.Serialize(activosVM));

            return View(activosVM);
        }

        [HttpGet("ActivoFijo/ExportarPDF")]
        public IActionResult ExportarPDF()
        {
            //  RECUPERAR LA LISTA FILTRADA 
            var listaFiltrada = HttpContext.Session.GetString("ListaActivos");

            if (string.IsNullOrEmpty(listaFiltrada))
                return BadRequest("No hay datos filtrados para exportar.");

            var activos = JsonSerializer.Deserialize<List<ActivoFijoViewModel>>(listaFiltrada);

            return GenerarPDF(activos);
        }

        //  Método para Generar PDF 
        private IActionResult GenerarPDF(List<ActivoFijoViewModel> lista)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                Document document = new Document(PageSize.A4, 10, 10, 20, 10);
                PdfWriter.GetInstance(document, ms);
                document.Open();

                // 🔹 Agregar encabezado del reporte
                Font tituloFont = new Font(Font.FontFamily.HELVETICA, 16, Font.BOLD, BaseColor.BLACK);
                Font subtituloFont = new Font(Font.FontFamily.HELVETICA, 12, Font.NORMAL, BaseColor.DARK_GRAY);
                Font encabezadoFont = new Font(Font.FontFamily.HELVETICA, 12, Font.BOLD);
                Font contenidoFont = new Font(Font.FontFamily.HELVETICA, 10);

                Paragraph titulo = new Paragraph("Reporte de Activos Fijos", tituloFont);
                titulo.Alignment = Element.ALIGN_CENTER;
                document.Add(titulo);

                // 🔹 Agregar subtítulo con fecha de generación del reporte
                Paragraph fechaReporte = new Paragraph("Generado el: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), subtituloFont);
                fechaReporte.Alignment = Element.ALIGN_CENTER;
                document.Add(fechaReporte);

                // 🔹 Espacio entre el título y la tabla
                document.Add(new Paragraph("\n"));

                // 🔹 Crear la tabla
                PdfPTable table = new PdfPTable(6);
                table.WidthPercentage = 100;
                table.SetWidths(new float[] { 3, 3, 2, 2, 2, 3 });

                // 🔹 Encabezados de la tabla
                string[] headers = { "Nombre", "Fecha Adquisición", "Valor Inicial", "Vida Útil", "Depreciado", "Estado" };
                foreach (var header in headers)
                {
                    PdfPCell cell = new PdfPCell(new Phrase(header, encabezadoFont))
                    {
                        BackgroundColor = BaseColor.LIGHT_GRAY,
                        HorizontalAlignment = Element.ALIGN_CENTER
                    };
                    table.AddCell(cell);
                }

                // 🔹 Agregar datos a la tabla
                if (lista.Count > 0)
                {
                    foreach (var item in lista)
                    {
                        table.AddCell(new Phrase(item.NombreActivo, contenidoFont));
                        table.AddCell(new Phrase(item.FechaAdquisicion.ToShortDateString(), contenidoFont));
                        table.AddCell(new Phrase(item.ValorInicial.ToString("N2"), contenidoFont));
                        table.AddCell(new Phrase($"{item.VidaUtil} años", contenidoFont));
                        table.AddCell(new Phrase(item.ValorDepreciado.ToString("N2"), contenidoFont));
                        table.AddCell(new Phrase(item.Estado ?? "Inactivo", contenidoFont));
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

                return File(ms.ToArray(), "application/pdf", "Reporte_ActivosFijos.pdf");
            }
        }


        // Crear Activo Fijo (GET)
        public IActionResult Create()
        {
            return View();
        }

        //  Crear Activo Fijo (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ActivoFijoViewModel activoVM)
        {
            if (ModelState.IsValid)
            {
                var activo = _mapper.Map<ActivoFijo>(activoVM);

                // Se inicializan estos campos como nulos al crear un activo, ya que se llenan hasta despues de adquirido el activo
                activo.UltimaFechaMantenimiento = null;
                activo.ComentarioMantenimiento = null;

                _context.Add(activo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(activoVM);
        }

        // Editar Activo Fijo (GET)
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var activo = await _context.ActivosFijos.FindAsync(id);
            if (activo == null) return NotFound();

            var activoVM = _mapper.Map<ActivoFijoViewModel>(activo);
            return View(activoVM);
        }

        // Editar Activo Fijo (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ActivoFijoViewModel activoVM)
        {
            if (id != activoVM.ActivoID) return NotFound();

            if (ModelState.IsValid)
            {
                var activo = _mapper.Map<ActivoFijo>(activoVM);

                _context.Update(activo);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(activoVM);
        }


        // Eliminar Activo Fijo (GET)
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var activo = await _context.ActivosFijos
                .AsNoTracking()  // 🔥 Trae siempre la última versión de los datos
                .FirstOrDefaultAsync(a => a.ActivoID == id);

            if (activo == null) return NotFound();

            var activoVM = _mapper.Map<ActivoFijoViewModel>(activo);
            return View(activoVM);
        }


        // Eliminar Activo Fijo (POST)
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activo = await _context.ActivosFijos.FindAsync(id);
            if (activo != null)
            {
                _context.ActivosFijos.Remove(activo);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // Método para calcular la depreciación
        private static decimal CalcularDepreciacion(decimal valorInicial, int vidaUtil)
        {
            return vidaUtil > 0 ? valorInicial / vidaUtil : 0;
        }
    }
}
