using Microsoft.AspNetCore.Mvc;
using Tp_Inmobiliaria_Ledesma_Lillo.Models;
using Tp_Inmobiliaria_Ledesma_Lillo.Net.Controllers;

namespace Tp_Inmobiliaria_Ledesma_Lillo.Controllers;

public class InmuebleController : Controller 
{
    private readonly ILogger<HomeController> _logger;
    

    public InmuebleController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Listado()
    {
        try
        {
            RepositorioInmueble rinmu = new RepositorioInmueble();
            var lista = rinmu.ObtenerInmuebles();
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
            {   RepositorioPropietario repoPropietario = new RepositorioPropietario();
                ViewBag.Propietarios = repoPropietario.ObtenerPropietarios();
                RepositorioInmueble rinmu = new RepositorioInmueble();
                var inmueble = rinmu.ObtenerInmueble(id);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return View(inmueble);
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
				RepositorioPropietario repoPropietario = new RepositorioPropietario();
                ViewBag.Propietarios = repoPropietario.ObtenerPropietarios();
                return View();
			}
			catch (Exception ex)
			{
                return Json(new { Error = ex.Message });
			}
		}

    public IActionResult Guardar(Inmueble inmueble)
    {
        RepositorioInmueble rinmu = new RepositorioInmueble();
        
        try
        {   
            
           if(ModelState.IsValid)
           {
                if(inmueble.IdInmueble > 0)
                {   
                    rinmu.ModificaInmueble(inmueble);
                }
                else{
                    
                    rinmu.AltaInmueble(inmueble);
                    TempData["id"] = inmueble.IdInmueble; 
                }
           }
           else 
           {
            return View(inmueble); 
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
        RepositorioInmueble rinmu = new RepositorioInmueble();
        try
        {
            rinmu.EliminaInmueble(id);
            TempData["Mensaje"] = "Eliminación realizada correctamente";
            return RedirectToAction(nameof(Listado));
        }
        catch(Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }
}