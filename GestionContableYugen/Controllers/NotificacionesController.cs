using Microsoft.AspNetCore.Mvc;
using GestionContableYugen.Models;
using System;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

public class NotificacionesController : Controller
{
    private readonly ApplicationDbContext _context;

    public NotificacionesController(ApplicationDbContext context)
    {
        _context = context;
    }
    public IActionResult GetNotificaciones()
    {
        var hoy = DateTime.Today;
        var cincoDiasDespues = hoy.AddDays(5);

        // Obtener CXP - Cuentas por Pagar próximas a vencer
        var cxp = _context.CuentasPorPagar
            .Where(c => c.Estado == "Pendiente" && c.FechaVencimiento >= hoy && c.FechaVencimiento <= cincoDiasDespues)
            .Select(c => new
            {
                mensaje = $"CXP: Pago a {c.NombreProveedor} vence el {c.FechaVencimiento:dd/MM/yyyy}",
                url = "/CuentaPorPagar/Index"
            })
            .ToList(); 

        // Obtener CXC - Cuentas por Cobrar próximas a vencer
        var cxc = _context.CuentasPorCobrar
            .Include(c => c.Cliente) 
            .Where(c => c.Estado == "Pendiente" && c.FechaVencimiento >= hoy && c.FechaVencimiento <= cincoDiasDespues)
            .Select(c => new
            {
                mensaje = $"CXC: Cobro a {(c.Cliente != null ? c.Cliente.NumeroCedula : "Cliente no encontrado")} vence el {c.FechaVencimiento:dd/MM/yyyy}",
                url = "/CuentaPorCobrar/Index"
            })
            .ToList();

        //Stock

        var stockBajo = _context.Productos
        .Where(p => p.CantidadDisponible < 10)
        .Select(p => new
        {
            mensaje = $" Stock Bajo: {p.NombreProducto} tiene solo {p.CantidadDisponible} unidades.",
            url = "/Producto/Index"
        })
        .ToList();

        var notificaciones = cxp.Concat(cxc).Concat(stockBajo).ToList();

        return Json(notificaciones);
    }



}
