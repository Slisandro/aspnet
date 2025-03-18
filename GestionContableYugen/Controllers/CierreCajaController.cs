using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using GestionContableYugen.Models;
using GestionContableYugen.ViewModels;
using System.Threading.Tasks;
using System.Linq;

namespace GestionContableYugen.Controllers
{
    public class CierreCajaController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CierreCajaController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Listar Cierres de Caja
        public async Task<IActionResult> Index()
        {
            var cierres = await _context.CierresCaja
                .Select(c => new CierreCajaViewModel
                {
                    CierreID = c.CierreID,
                    FechaCierre = c.FechaCierre,
                    SaldoInicial = c.SaldoInicial,
                    TotalIngresos = c.TotalIngresos,
                    TotalEgresos = c.TotalEgresos,
                    SaldoFinal = c.SaldoFinal
                })
                .ToListAsync();

            return View(cierres);
        }

        // Crear un nuevo Cierre de Caja
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CierreCajaViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var cierreCaja = new CierreCaja
            {
                FechaCierre = model.FechaCierre,
                SaldoInicial = model.SaldoInicial,
                TotalIngresos = model.TotalIngresos,
                TotalEgresos = model.TotalEgresos,
                SaldoFinal = model.SaldoInicial + model.TotalIngresos - model.TotalEgresos // 🔹 Cálculo automático
            };

            _context.CierresCaja.Add(cierreCaja);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Editar un Cierre de Caja
        public async Task<IActionResult> Edit(int id)
        {
            var cierre = await _context.CierresCaja.FindAsync(id);
            if (cierre == null)
                return NotFound();

            var model = new CierreCajaViewModel
            {
                CierreID = cierre.CierreID,
                FechaCierre = cierre.FechaCierre,
                SaldoInicial = cierre.SaldoInicial,
                TotalIngresos = cierre.TotalIngresos,
                TotalEgresos = cierre.TotalEgresos,
                SaldoFinal = cierre.SaldoFinal
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CierreCajaViewModel model)
        {
            if (id != model.CierreID)
                return NotFound();

            if (!ModelState.IsValid)
                return View(model);

            var cierreCaja = await _context.CierresCaja.FindAsync(id);
            if (cierreCaja == null)
                return NotFound();

            cierreCaja.FechaCierre = model.FechaCierre;
            cierreCaja.SaldoInicial = model.SaldoInicial;
            cierreCaja.TotalIngresos = model.TotalIngresos;
            cierreCaja.TotalEgresos = model.TotalEgresos;
            cierreCaja.SaldoFinal = model.SaldoInicial + model.TotalIngresos - model.TotalEgresos;

            _context.Update(cierreCaja);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        // Eliminar un Cierre de Caja
        public async Task<IActionResult> Delete(int id)
        {
            var cierre = await _context.CierresCaja.FindAsync(id);
            if (cierre == null)
                return NotFound();

            return View(cierre);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cierre = await _context.CierresCaja.FindAsync(id);
            if (cierre == null)
                return NotFound();

            _context.CierresCaja.Remove(cierre);
            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
