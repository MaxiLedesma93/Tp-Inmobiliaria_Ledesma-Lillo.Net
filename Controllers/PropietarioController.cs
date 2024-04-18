
using Microsoft.AspNetCore.Mvc;
using Tp_Inmobiliaria_Ledesma_Lillo.Models;
using Tp_Inmobiliaria_Ledesma_Lillo.Net.Controllers;

namespace Tp_Inmobiliaria_Ledesma_Lillo.Controllers;

public class PropietarioController : Controller 
{
    private readonly ILogger<HomeController> _logger;
    

    public PropietarioController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Listado()
    {
        try
        {
            RepositorioPropietario rp = new RepositorioPropietario();
            var lista = rp.ObtenerPropietarios();
            ViewBag.id = TempData["id"];
            // TempData es para pasar datos entre acciones
				// ViewBag/Data es para pasar datos del controlador a la vista
				// Si viene alguno valor por el tempdata, lo paso al viewdata/viewbag
				if (TempData.ContainsKey("Mensaje"))
					ViewBag.Mensaje = TempData["Mensaje"];
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
                RepositorioPropietario rp = new RepositorioPropietario();
                var propietario = rp.ObtenerPropietario(id);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return View(propietario);
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

    public IActionResult Guardar(Propietario propietario)
    {
        RepositorioPropietario rp = new RepositorioPropietario();
        try
        {
           if(ModelState.IsValid)
           {
                if(propietario.IdPropietario > 0)
                {
                    rp.ModificaPropietario(propietario);
                }
                else{
                    rp.AltaPropietario(propietario);
                    TempData["id"] = propietario.IdPropietario; 
                }
           }
           else 
           {
            return View(propietario); 
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
        RepositorioPropietario rp = new RepositorioPropietario();
        try
        {
            rp.EliminaPersona(id);
            TempData["Mensaje"] = "Eliminación realizada correctamente";
            return RedirectToAction(nameof(Listado));
        }
        catch(Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }
}