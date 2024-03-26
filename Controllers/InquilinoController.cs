
using Microsoft.AspNetCore.Mvc;
using Tp_Inmobiliaria_Ledesma_Lillo.Models;
using Tp_Inmobiliaria_Ledesma_Lillo.Net.Controllers;

namespace Tp_Inmobiliaria_Ledesma_Lillo.Controllers;

public class InquilinoController : Controller 
{
    private readonly ILogger<HomeController> _logger;
    

    public InquilinoController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Listado()
    {
        try
        {
            RepositorioInquilino rp = new RepositorioInquilino();
            var lista = rp.ObtenerInquilinos();
            ViewBag.id = TempData["id"];
            return View(lista);
        }
        catch(Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }

    public IActionResult Editar(int id)
    {
        try
        {
            if(id > 0)
            {
                RepositorioInquilino ri = new RepositorioInquilino();
                var inquilino = ri.ObtenerInquilino(id);
                return View(inquilino);
            }
            else
                { return View();}
        }
        catch(Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }

    public IActionResult Crear() //carga el formulario vacio
		{
			try
			{
				return View();
			}
			catch (Exception ex)
			{
                return Json(new { Error = ex.Message });
			}
		}

    public IActionResult Guardar(Inquilino inquilino)
    {
        RepositorioInquilino ri = new RepositorioInquilino();
        try
        {
           if(ModelState.IsValid)
           {
                if(inquilino.IdInquilino > 0)
                {
                    ri.ModificaInquilino(inquilino);
                }
                else{
                    ri.AltaInquilino(inquilino);
                    TempData["id"] = inquilino.IdInquilino; 
                }
           }
           else 
           {
            return View(inquilino); 
           }
            return RedirectToAction(nameof(Listado));
        }
        catch(Exception ex)
        {
            return Json(new { Error = ex.Message });
        }

    }

    public IActionResult Eliminar(int id)
    {
        RepositorioInquilino rp = new RepositorioInquilino();
        try
        {
            rp.EliminaInquilino(id);
            TempData["Mensaje"] = "Eliminación realizada correctamente";
            return RedirectToAction(nameof(Listado));
        }
        catch(Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }
}