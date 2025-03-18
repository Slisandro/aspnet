using GestionContableYugen.Models;
using GestionContableYugen.ViewModels;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Text.Json;
using System.Threading.Tasks;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace GestionContableYugen.Controllers
{
    [Authorize(Roles = "Administrador, Asistente del Administrador, Bodeguero")]
    public class ProductoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ProductoController(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // 🔹 Listar productos con filtros
    public async Task<IActionResult> Index(string nombreProducto, string codigoBarras, string categoria)
    {
        var productos = _context.Productos.AsQueryable();

        // 🔹 Aplicar filtros dinámicos
        if (!string.IsNullOrEmpty(nombreProducto))
        {
            productos = productos.Where(p => p.NombreProducto.Contains(nombreProducto));
        }

        if (!string.IsNullOrEmpty(codigoBarras))
        {
            productos = productos.Where(p => p.CodigoBarras.Contains(codigoBarras));
        }

        if (!string.IsNullOrEmpty(categoria))
        {
            productos = productos.Where(p => p.Categoria.Contains(categoria));
        }

        var productosList = await productos.ToListAsync();

        var productosViewModel = productosList.Select(p => new ProductoViewModel
        {
            CodigoBarras = p.CodigoBarras,
            NombreProducto = p.NombreProducto,
            Precio = p.Precio,
            CantidadDisponible = p.CantidadDisponible,
            Categoria = p.Categoria,
            Descripcion = p.Descripcion,
            FechaIngreso = p.FechaIngreso

        }).ToList();

        // 🔥 Guardamos la lista filtrada en Session para exportación a PDF
        HttpContext.Session.SetString("ListaProductos", JsonSerializer.Serialize(productosViewModel));

        return View(productosViewModel);
    }

    // 🔥 Exportar Inventario a PDF
    [HttpGet("Producto/ExportarPDF")]
    public IActionResult ExportarPDF()
    {
        // 🔥 Recuperar la lista filtrada de Session
        var listaFiltrada = HttpContext.Session.GetString("ListaProductos");

        if (string.IsNullOrEmpty(listaFiltrada))
            return BadRequest("No hay datos filtrados para exportar.");

        var productos = JsonSerializer.Deserialize<List<ProductoViewModel>>(listaFiltrada);

        return GenerarPDF(productos);
    }

    /// 🔹 GENERAR PDF - INVENTARIO
    private IActionResult GenerarPDF(List<ProductoViewModel> lista)
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

            Paragraph titulo = new Paragraph("Reporte de Inventario", tituloFont);
            titulo.Alignment = Element.ALIGN_CENTER;
            document.Add(titulo);

            Paragraph fechaReporte = new Paragraph("Generado el: " + DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss"), subtituloFont);
            fechaReporte.Alignment = Element.ALIGN_CENTER;
            document.Add(fechaReporte);

            document.Add(new Paragraph("\n"));

            // 🔹 Crear tabla con 6 columnas
            PdfPTable table = new PdfPTable(6);
            table.WidthPercentage = 100;
            table.SetWidths(new float[] { 3, 2, 2, 2, 2, 3 });

            string[] headers = { "Nombre", "Código de Barras", "Categoría", "Precio", "Cantidad", "Fecha de Ingreso" };
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
                    table.AddCell(new Phrase(item.NombreProducto, contenidoFont));
                    table.AddCell(new Phrase(item.CodigoBarras, contenidoFont));
                    table.AddCell(new Phrase(item.Categoria, contenidoFont));
                    table.AddCell(new Phrase(item.Precio.ToString("C"), contenidoFont));
                    table.AddCell(new Phrase(item.CantidadDisponible.ToString(), contenidoFont));
                    table.AddCell(new Phrase(item.FechaIngreso.ToShortDateString(), contenidoFont));
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

            return File(ms.ToArray(), "application/pdf", "Reporte_Inventario.pdf");
        }
    }

        // Crear producto (GET)
        public IActionResult Create()
        {
            return View();
        }

        // Crear producto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductoViewModel productoVM)
        {
            if (!ModelState.IsValid)
            {
                return View(productoVM);
            }
            //Verificar si el código de barras ya existe en la BD
            var existeProducto = await _context.Productos.AnyAsync(p => p.CodigoBarras == productoVM.CodigoBarras);
            if (existeProducto)
            {
                ModelState.AddModelError("CodigoBarras", "Ya existe un producto con este código de barras.");
                return View(productoVM);
            }

            //Asegurar que los valores numéricos sean válidos
            if (productoVM.CantidadDisponible < 0 || productoVM.Precio < 0 ||
                productoVM.DemandaDiariaPromedio < 0 || productoVM.TiempoEntregaProveedor < 0 || productoVM.StockSeguridad < 0)
            {
                ModelState.AddModelError("", "Los valores numéricos no pueden ser negativos.");
                return View(productoVM);
            }

            var producto = new Producto
            {
                NombreProducto = productoVM.NombreProducto,
                CodigoBarras = productoVM.CodigoBarras,
                Precio = productoVM.Precio,
                CantidadDisponible = productoVM.CantidadDisponible,
                Categoria = productoVM.Categoria,
                Descripcion = productoVM.Descripcion,
                FechaIngreso = DateTime.Now,
                DemandaDiariaPromedio = productoVM.DemandaDiariaPromedio,
                TiempoEntregaProveedor = productoVM.TiempoEntregaProveedor,
                StockSeguridad = productoVM.StockSeguridad
            };

            _context.Productos.Add(producto);
            await _context.SaveChangesAsync(); // Guardar el producto en la BD

            //Registro del movimiento en tabla de movimientos
            var movimiento = new MovimientoInventario
            {
                CodigoBarras = producto.CodigoBarras,
                Cantidad = producto.CantidadDisponible,
                TipoMovimiento = "Entrada",
                Descripcion = "Stock inicial",
                FechaMovimiento = DateTime.Now
            };

            _context.MovimientoInventario.Add(movimiento);
            await _context.SaveChangesAsync(); // Guardar movimiento de inventario

            TempData["SuccessMessage"] = "Producto creado correctamente con su movimiento de entrada.";
            return RedirectToAction(nameof(Index));
        }

        // Editar producto (GET)
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            var productoVM = _mapper.Map<ProductoViewModel>(producto);
            return View(productoVM);
        }

        // Editar producto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, ProductoViewModel productoVM)
        {
            if (id != productoVM.CodigoBarras)
            {
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                var productoAnterior = await _context.Productos.AsNoTracking()
                    .FirstOrDefaultAsync(p => p.CodigoBarras == id);

                if (productoAnterior == null)
                {
                    return NotFound();
                }

                // Mapeo manual del ViewModel a la entidad Producto
                var productoEditado = new Producto
                {
                    NombreProducto = productoVM.NombreProducto,
                    CodigoBarras = productoVM.CodigoBarras,
                    Precio = productoVM.Precio,
                    CantidadDisponible = productoVM.CantidadDisponible,
                    Categoria = productoVM.Categoria,
                    Descripcion = productoVM.Descripcion,
                    FechaIngreso = productoAnterior.FechaIngreso, // Mantener la fecha de ingreso

                    // Guardar los valores para calcular ROP
                    DemandaDiariaPromedio = productoVM.DemandaDiariaPromedio,
                    TiempoEntregaProveedor = productoVM.TiempoEntregaProveedor,
                    StockSeguridad = productoVM.StockSeguridad
                };

                _context.Update(productoEditado);
                await _context.SaveChangesAsync();

                // 🔹 REGISTRO AUTOMÁTICO DE MOVIMIENTO DE INVENTARIO (AJUSTE DE STOCK)
                var cantidadDiferencia = productoEditado.CantidadDisponible - productoAnterior.CantidadDisponible;
                if (cantidadDiferencia != 0)
                {
                    var tipoMovimiento = cantidadDiferencia > 0 ? "Entrada" : "Salida";

                    var movimiento = new MovimientoInventario
                    {
                        CodigoBarras = productoEditado.CodigoBarras,
                        Cantidad = Math.Abs(cantidadDiferencia),
                        TipoMovimiento = tipoMovimiento,
                        Descripcion = "Ajuste de stock manual",
                        FechaMovimiento = DateTime.Now
                    };

                    _context.MovimientoInventario.Add(movimiento);
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            return View(productoVM);
        }

        // Eliminar producto (GET)
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }

            var productoVM = _mapper.Map<ProductoViewModel>(producto);
            return View(productoVM);
        }

        // Eliminar producto (POST)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var producto = await _context.Productos.FirstOrDefaultAsync(p => p.CodigoBarras == id);

            if (producto == null)
            {
                return NotFound();
            }

            // REGISTRO AUTOMÁTICO DE MOVIMIENTO DE INVENTARIO (SALIDA)
            var movimiento = new MovimientoInventario
            {
                CodigoBarras = producto.CodigoBarras,
                Cantidad = producto.CantidadDisponible,
                TipoMovimiento = "Salida",
                Descripcion = "Producto eliminado",
                FechaMovimiento = DateTime.Now
            };

            _context.MovimientoInventario.Add(movimiento);

            // Eliminar el producto
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        //Notificaciones

        [HttpGet]
        public async Task<IActionResult> GetStockBajo()
        {
            var productosStockBajo = await _context.Productos
                .Where(p => p.CantidadDisponible < ((p.DemandaDiariaPromedio * p.TiempoEntregaProveedor) + p.StockSeguridad))
                .ToListAsync();

            return Json(new { cantidad = productosStockBajo.Count, productos = productosStockBajo });
        }


    }
}
