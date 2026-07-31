using System.Diagnostics;
using DAW_Procesos_Fabricacion_KevinRodriguez.Data;
using Microsoft.AspNetCore.Mvc;
using DAW_Procesos_Fabricacion_KevinRodriguez.Models;
using DAW_Procesos_Fabricacion_KevinRodriguez.ViewModels;
using Microsoft.EntityFrameworkCore;

namespace DAW_Procesos_Fabricacion_KevinRodriguez.Controllers;

public class HomeController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var ordenes = await context.OrdenesProduccion
            .AsNoTracking()
            .Include(orden => orden.Procesos)
            .OrderByDescending(orden => orden.FechaCreacion)
            .ThenByDescending(orden => orden.Id)
            .ToListAsync();

        var dashboard = new DashboardViewModel
        {
            TotalOrdenes = ordenes.Count,
            OrdenesPendientes = ordenes.Count(orden => orden.EstadoGeneral == EstadoOrdenProduccion.Pendiente),
            OrdenesEnProceso = ordenes.Count(orden => orden.EstadoGeneral == EstadoOrdenProduccion.EnProceso),
            OrdenesCompletadas = ordenes.Count(orden => orden.EstadoGeneral == EstadoOrdenProduccion.Completada),
            OrdenesRecientes = ordenes.Take(5).ToList()
        };

        return View(dashboard);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
