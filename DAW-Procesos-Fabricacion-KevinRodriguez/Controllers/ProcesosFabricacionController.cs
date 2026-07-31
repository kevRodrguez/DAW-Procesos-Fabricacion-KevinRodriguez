using DAW_Procesos_Fabricacion_KevinRodriguez.Data;
using DAW_Procesos_Fabricacion_KevinRodriguez.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DAW_Procesos_Fabricacion_KevinRodriguez.Controllers;

public class ProcesosFabricacionController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var procesos = await context.ProcesosFabricacion
            .AsNoTracking()
            .OrderBy(proceso => proceso.Nombre)
            .ToListAsync();

        return View(procesos);
    }

    public async Task<IActionResult> Details(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var proceso = await context.ProcesosFabricacion
            .AsNoTracking()
            .Include(proceso => proceso.Ordenes)
            .ThenInclude(relacion => relacion.OrdenProduccion)
            .SingleOrDefaultAsync(proceso => proceso.Id == id);

        return proceso is null ? NotFound() : View(proceso);
    }

    public IActionResult Create()
    {
        return View(new ProcesoFabricacion());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([Bind("Nombre,Descripcion")] ProcesoFabricacion proceso)
    {
        NormalizarCampos(proceso);

        if (await NombreEnUsoAsync(proceso.Nombre))
        {
            ModelState.AddModelError(nameof(ProcesoFabricacion.Nombre), "Ya existe un proceso con este nombre.");
        }

        if (!ModelState.IsValid)
        {
            return View(proceso);
        }

        context.Add(proceso);
        await context.SaveChangesAsync();
        TempData["Success"] = "El proceso de fabricación fue creado correctamente.";

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var proceso = await context.ProcesosFabricacion.FindAsync(id);
        return proceso is null ? NotFound() : View(proceso);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, [Bind("Id,Nombre,Descripcion")] ProcesoFabricacion proceso)
    {
        if (id != proceso.Id)
        {
            return NotFound();
        }

        NormalizarCampos(proceso);

        if (await NombreEnUsoAsync(proceso.Nombre, proceso.Id))
        {
            ModelState.AddModelError(nameof(ProcesoFabricacion.Nombre), "Ya existe un proceso con este nombre.");
        }

        if (!ModelState.IsValid)
        {
            return View(proceso);
        }

        try
        {
            context.Update(proceso);
            await context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!await ProcesoExisteAsync(proceso.Id))
            {
                return NotFound();
            }

            throw;
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(nameof(ProcesoFabricacion.Nombre), "No fue posible guardar el proceso. Verifica que el nombre no esté repetido.");
            return View(proceso);
        }

        TempData["Success"] = "El proceso de fabricación fue actualizado correctamente.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int? id)
    {
        if (id is null)
        {
            return NotFound();
        }

        var proceso = await context.ProcesosFabricacion
            .AsNoTracking()
            .Include(proceso => proceso.Ordenes)
            .SingleOrDefaultAsync(proceso => proceso.Id == id);

        return proceso is null ? NotFound() : View(proceso);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var proceso = await context.ProcesosFabricacion
            .Include(proceso => proceso.Ordenes)
            .SingleOrDefaultAsync(proceso => proceso.Id == id);

        if (proceso is null)
        {
            return NotFound();
        }

        if (proceso.Ordenes.Count > 0)
        {
            TempData["Error"] = "No se puede eliminar el proceso porque está asociado a una o más órdenes.";
            return RedirectToAction(nameof(Index));
        }

        try
        {
            context.ProcesosFabricacion.Remove(proceso);
            await context.SaveChangesAsync();
            TempData["Success"] = "El proceso de fabricación fue eliminado correctamente.";
        }
        catch (DbUpdateException)
        {
            TempData["Error"] = "No se puede eliminar el proceso porque está asociado a una o más órdenes.";
        }

        return RedirectToAction(nameof(Index));
    }

    private static void NormalizarCampos(ProcesoFabricacion proceso)
    {
        proceso.Nombre = proceso.Nombre?.Trim() ?? string.Empty;
        proceso.Descripcion = proceso.Descripcion?.Trim() ?? string.Empty;
    }

    private Task<bool> NombreEnUsoAsync(string nombre, int? idExcluido = null)
    {
        return context.ProcesosFabricacion.AnyAsync(proceso =>
            proceso.Nombre == nombre && (!idExcluido.HasValue || proceso.Id != idExcluido.Value));
    }

    private Task<bool> ProcesoExisteAsync(int id)
    {
        return context.ProcesosFabricacion.AnyAsync(proceso => proceso.Id == id);
    }
}
