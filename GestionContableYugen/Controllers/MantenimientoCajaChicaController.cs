using Microsoft.AspNetCore.Mvc;
using GestionContableYugen.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace GestionContableYugen.Controllers
{
    [Authorize(Roles = "Administrador")]
    public class MantenimientoCajaChicaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MantenimientoCajaChicaController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        public async Task<IActionResult> Index()
        {
            var caja = await _context.CajaChicaConfig.FirstOrDefaultAsync();
            if (caja == null)
            {
                caja = new CajaChicaConfig { SaldoDisponible = 0 };
                _context.CajaChicaConfig.Add(caja);
                await _context.SaveChangesAsync();
            }

            ViewBag.SaldoDisponible = caja.SaldoDisponible;
            return View("MantenimientoSaldo", caja); 
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActualizarSaldo(CajaChicaConfig caja)
        {
            if (caja.SaldoDisponible < 0)
            {
                ModelState.AddModelError("SaldoDisponible", "El saldo no puede ser negativo.");
                return View("MantenimientoSaldo", caja);
            }

            var cajaActual = await _context.CajaChicaConfig.FirstOrDefaultAsync();

            if (cajaActual != null)
            {
                cajaActual.SaldoDisponible += caja.SaldoDisponible;
                _context.Entry(cajaActual).State = EntityState.Modified;
            }
            else
            {
                _context.CajaChicaConfig.Add(caja);
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Saldo actualizado correctamente. Nuevo saldo: {cajaActual.SaldoDisponible.ToString("N2")} ₡";

            return RedirectToAction("Index", "MantenimientoCajaChica");
        }

    }
}
