using Microsoft.AspNetCore.Mvc;
using GestionContableYugen.Models;
using GestionContableYugen.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Collections.Generic;
using Microsoft.IdentityModel.Tokens;

namespace GestionContableYugen.Controllers
{
    [Authorize(Roles = "Administrador, Asistente del Administrador, Cajero")]
    [Route("Factura")]
    public class FacturaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacturaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📌 Listar Facturas
        [HttpGet("Index")]
        public async Task<IActionResult> Index()
        {
            var facturas = await _context.Facturas
                .Include(f => f.Cliente)
                .Include(f => f.TipoPago)
                .Select(f => new FacturaViewModel
                {
                    FacturaID = f.FacturaID,
                    ClienteNombre = f.Cliente != null ? f.Cliente.NombreCliente : "N/A",
                    UsuarioNombre = f.Usuario != null ? f.Usuario.NombreUsuario : "N/A",
                    TipoPagoDescripcion = f.TipoPago != null ? f.TipoPago.Descripcion : "N/A",
                    FechaFactura = f.FechaFactura,
                    Total = f.Total
                })
                .ToListAsync();

            return View(facturas);
        }

        // 📌 Obtener Producto por Código de Barras
        [HttpGet("ObtenerProductoPorCodigo")]
        public async Task<IActionResult> ObtenerProductoPorCodigo(string codigoBarras)
        {
            Console.WriteLine($"📥 Buscando producto con código: {codigoBarras}");

            if (string.IsNullOrEmpty(codigoBarras))
            {
                Console.WriteLine("❌ Código de barras vacío.");
                return BadRequest(new { success = false, message = "Código de barras no puede estar vacío." });
            }

            var producto = await _context.Productos
                .Where(p => p.CodigoBarras == codigoBarras)
                .Select(p => new
                {
                    p.CodigoBarras,
                    p.NombreProducto,
                    p.Precio
                })
                .FirstOrDefaultAsync();

            if (producto == null)
            {
                Console.WriteLine("🚨 Producto no encontrado en la base de datos.");
                return Json(new { success = false, message = "Producto no encontrado." });
            }

            Console.WriteLine($"✅ Producto encontrado: {producto.NombreProducto}");
            return Json(new { success = true, producto });
        }


        //  Crear Factura (GET)
        [HttpGet("Create")]
        public IActionResult Create()
        {
            var usuarioNombre = HttpContext.Session.GetString("Usuario");
            var usuarioID = HttpContext.Session.GetInt32("UsuarioID");

            if (string.IsNullOrEmpty(usuarioNombre) || usuarioID == null)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.Clientes = _context.Clientes.ToList();
            ViewBag.TiposPago = _context.TipoPago.ToList();
            ViewBag.Descuentos = _context.Descuentos.Where(d => d.Activo).ToList();
            ViewBag.TiposFactura = _context.TiposFactura.ToList();

            var facturaVM = new FacturaViewModel
            {
                UsuarioID = usuarioID.Value,
                UsuarioNombre = usuarioNombre,
                FechaFactura = DateTime.Today
            };

            return View(facturaVM);
        }

        // 📌 Crear Factura (POST)
        [HttpPost("Create")]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromBody] FacturaViewModel facturaVM)
        {
            Console.WriteLine("📥 Recibiendo datos en el backend...");
            Console.WriteLine($"UsuarioID: {facturaVM.UsuarioID}");
            Console.WriteLine($"ClienteID: {facturaVM.ClienteID}");
            Console.WriteLine($"Tipo de Pago: {facturaVM.idTipoPago}");
            Console.WriteLine($"Tipo de Factura: {facturaVM.idTipoFactura}");
            Console.WriteLine($"Descuento ID: {facturaVM.idDescuento}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("❌ Error en ModelState.");
                return BadRequest(new { error = "⚠️ Error en los datos de la factura." });
            }

            if (facturaVM.DetalleFacturas == null || facturaVM.DetalleFacturas.Count == 0)
            {
                Console.WriteLine("🚨 No se recibieron productos en la factura.");
                return BadRequest(new { error = "⚠️ No se enviaron productos en la factura." });
            }

            decimal subtotal = facturaVM.DetalleFacturas.Sum(d => d.Cantidad * d.Precio);
            decimal descuento = 0;

            if (facturaVM.idDescuento.HasValue && facturaVM.idDescuento > 0)
            {
                var descuentoObj = await _context.Descuentos.FindAsync(facturaVM.idDescuento);
                if (descuentoObj != null)
                {
                    descuento = subtotal * (descuentoObj.Porcentaje / 100);
                }
            }

            decimal impuesto = subtotal * 0.13m;
            decimal totalFinal = subtotal - descuento + impuesto;

            var factura = new Factura
            {
                UsuarioID = facturaVM.UsuarioID,
                ClienteID = facturaVM.ClienteID,
                idTipoPago = facturaVM.idTipoPago,
                idTipoFactura = facturaVM.idTipoFactura,
                FechaFactura = DateTime.Today,
                Total = totalFinal,
                Impuesto = impuesto,
                idDescuento = facturaVM.idDescuento,
                MontoDescuento = descuento
            };

            _context.Add(factura);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = "Factura creada correctamente." });
        }


    }
}