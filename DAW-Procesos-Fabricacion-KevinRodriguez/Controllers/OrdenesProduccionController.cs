using DAW_Procesos_Fabricacion_KevinRodriguez.Data;
using DAW_Procesos_Fabricacion_KevinRodriguez.Models;
using DAW_Procesos_Fabricacion_KevinRodriguez.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAW_Procesos_Fabricacion_KevinRodriguez.Controllers;

public class OrdenesProduccionController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var ordenes = await context.OrdenesProduccion
            .AsNoTracking()
            .Include(orden => orden.Procesos)
            .OrderByDescending(orden => orden.FechaCreacion)
            .ThenBy(orden => orden.NumeroOrden)
            .ToListAsync();

        return View(ordenes);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var orden = await context.OrdenesProduccion
            .AsNoTracking()
            .Include(orden => orden.Procesos)
            .ThenInclude(relacion => relacion.ProcesoFabricacion)
            .SingleOrDefaultAsync(orden => orden.Id == id);

        return orden is null ? NotFound() : View(orden);
    }

    public async Task<IActionResult> Create()
    {
        var hoy = DateOnly.FromDateTime(DateTime.Today);
        var model = new OrdenProduccionFormViewModel
        {
            FechaCreacion = hoy,
            FechaEntregaEstimada = hoy.AddDays(7)
        };

        await CargarProcesosDisponiblesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(OrdenProduccionFormViewModel model)
    {
        NormalizarCampos(model);
        var procesosSeleccionados = await ValidarProcesosSeleccionadosAsync(model);

        if (await NumeroOrdenEnUsoAsync(model.NumeroOrden))
        {
            ModelState.AddModelError(nameof(model.NumeroOrden), "Ya existe una orden con este número.");
        }

        if (!ModelState.IsValid || procesosSeleccionados is null)
        {
            await CargarProcesosDisponiblesAsync(model);
            return View(model);
        }

        var orden = new OrdenProduccion
        {
            NumeroOrden = model.NumeroOrden,
            ModeloCalzado = model.ModeloCalzado,
            Cantidad = model.Cantidad,
            FechaCreacion = model.FechaCreacion,
            FechaEntregaEstimada = model.FechaEntregaEstimada,
            Procesos = procesosSeleccionados
                .Select(procesoId => new OrdenProceso { ProcesoFabricacionId = procesoId })
                .ToList()
        };

        try
        {
            context.OrdenesProduccion.Add(orden);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.NumeroOrden), "No fue posible guardar la orden. Verifica que el número no esté repetido.");
            await CargarProcesosDisponiblesAsync(model);
            return View(model);
        }

        TempData["Success"] = "La orden de producción fue creada correctamente.";
        return RedirectToAction(nameof(Details), new { id = orden.Id });
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var orden = await context.OrdenesProduccion
            .AsNoTracking()
            .Include(orden => orden.Procesos)
            .SingleOrDefaultAsync(orden => orden.Id == id);

        if (orden is null)
        {
            return NotFound();
        }

        var model = new OrdenProduccionFormViewModel
        {
            Id = orden.Id,
            NumeroOrden = orden.NumeroOrden,
            ModeloCalzado = orden.ModeloCalzado,
            Cantidad = orden.Cantidad,
            FechaCreacion = orden.FechaCreacion,
            FechaEntregaEstimada = orden.FechaEntregaEstimada,
            ProcesosSeleccionados = orden.Procesos.Select(relacion => relacion.ProcesoFabricacionId).ToList()
        };

        await CargarProcesosDisponiblesAsync(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, OrdenProduccionFormViewModel model)
    {
        if (id != model.Id)
        {
            return NotFound();
        }

        NormalizarCampos(model);
        var procesosSeleccionados = await ValidarProcesosSeleccionadosAsync(model);

        if (await NumeroOrdenEnUsoAsync(model.NumeroOrden, model.Id))
        {
            ModelState.AddModelError(nameof(model.NumeroOrden), "Ya existe una orden con este número.");
        }

        if (!ModelState.IsValid || procesosSeleccionados is null)
        {
            await CargarProcesosDisponiblesAsync(model);
            return View(model);
        }

        var orden = await context.OrdenesProduccion
            .Include(orden => orden.Procesos)
            .SingleOrDefaultAsync(orden => orden.Id == id);

        if (orden is null)
        {
            return NotFound();
        }

        orden.NumeroOrden = model.NumeroOrden;
        orden.ModeloCalzado = model.ModeloCalzado;
        orden.Cantidad = model.Cantidad;
        orden.FechaCreacion = model.FechaCreacion;
        orden.FechaEntregaEstimada = model.FechaEntregaEstimada;

        var idsActuales = orden.Procesos.Select(relacion => relacion.ProcesoFabricacionId).ToHashSet();
        var relacionesAEliminar = orden.Procesos
            .Where(relacion => !procesosSeleccionados.Contains(relacion.ProcesoFabricacionId))
            .ToList();

        context.OrdenesProcesos.RemoveRange(relacionesAEliminar);

        foreach (var procesoId in procesosSeleccionados.Where(procesoId => !idsActuales.Contains(procesoId)))
        {
            orden.Procesos.Add(new OrdenProceso { ProcesoFabricacionId = procesoId });
        }

        try
        {
            await context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(model.NumeroOrden), "No fue posible guardar la orden. Verifica que el número y los procesos seleccionados sean válidos.");
            await CargarProcesosDisponiblesAsync(model);
            return View(model);
        }

        TempData["Success"] = "La orden de producción fue actualizada correctamente.";
        return RedirectToAction(nameof(Details), new { id });
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var orden = await context.OrdenesProduccion
            .AsNoTracking()
            .Include(orden => orden.Procesos)
            .SingleOrDefaultAsync(orden => orden.Id == id);

        return orden is null ? NotFound() : View(orden);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var orden = await context.OrdenesProduccion.FindAsync(id);
        if (orden is null)
        {
            return NotFound();
        }

        context.OrdenesProduccion.Remove(orden);
        await context.SaveChangesAsync();
        TempData["Success"] = "La orden de producción y sus asociaciones fueron eliminadas correctamente.";

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ActualizarEstadoProceso(int ordenId, int procesoId, EstadoProceso estado)
    {
        var relacion = await context.OrdenesProcesos
            .SingleOrDefaultAsync(relacion =>
                relacion.OrdenProduccionId == ordenId &&
                relacion.ProcesoFabricacionId == procesoId);

        if (relacion is null)
        {
            return NotFound();
        }

        relacion.Estado = estado;
        relacion.FechaCompletado = estado == EstadoProceso.Completado
            ? DateOnly.FromDateTime(DateTime.Today)
            : null;

        await context.SaveChangesAsync();
        TempData["Success"] = "El estado del proceso fue actualizado correctamente.";

        return RedirectToAction(nameof(Details), new { id = ordenId });
    }

    private async Task<HashSet<int>?> ValidarProcesosSeleccionadosAsync(OrdenProduccionFormViewModel model)
    {
        model.ProcesosSeleccionados ??= [];
        var idsSeleccionados = model.ProcesosSeleccionados.ToHashSet();

        if (idsSeleccionados.Count == 0)
        {
            ModelState.AddModelError(nameof(model.ProcesosSeleccionados), "Debes asociar al menos un proceso a la orden.");
            return null;
        }

        if (idsSeleccionados.Count != model.ProcesosSeleccionados.Count)
        {
            ModelState.AddModelError(nameof(model.ProcesosSeleccionados), "No se puede repetir el mismo proceso en una orden.");
        }

        var procesosExistentes = await context.ProcesosFabricacion
            .Where(proceso => idsSeleccionados.Contains(proceso.Id))
            .CountAsync();

        if (procesosExistentes != idsSeleccionados.Count)
        {
            ModelState.AddModelError(nameof(model.ProcesosSeleccionados), "Uno o más procesos seleccionados ya no están disponibles.");
        }

        return ModelState.IsValid ? idsSeleccionados : null;
    }

    private async Task CargarProcesosDisponiblesAsync(OrdenProduccionFormViewModel model)
    {
        model.ProcesosDisponibles = await context.ProcesosFabricacion
            .AsNoTracking()
            .OrderBy(proceso => proceso.Nombre)
            .Select(proceso => new ProcesoSeleccionableViewModel
            {
                Id = proceso.Id,
                Nombre = proceso.Nombre,
                Descripcion = proceso.Descripcion
            })
            .ToListAsync();
    }

    private static void NormalizarCampos(OrdenProduccionFormViewModel model)
    {
        model.NumeroOrden = model.NumeroOrden?.Trim() ?? string.Empty;
        model.ModeloCalzado = model.ModeloCalzado?.Trim() ?? string.Empty;
    }

    private Task<bool> NumeroOrdenEnUsoAsync(string numeroOrden, int? idExcluido = null)
    {
        return context.OrdenesProduccion.AnyAsync(orden =>
            orden.NumeroOrden == numeroOrden && (!idExcluido.HasValue || orden.Id != idExcluido.Value));
    }
}
