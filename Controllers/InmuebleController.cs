using Microsoft.AspNetCore.Authorization;
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

    [Authorize]
    public IActionResult Listado()
    {
        try
        {
            RepositorioTipo rp = new RepositorioTipo();
            ViewBag.Tipos = rp.ObtenerTiposInmuebles();
            
            RepositorioInmueble rinmu = new RepositorioInmueble();
            var lista = rinmu.ObtenerInmuebles();
            ViewBag.id = TempData["id"];
            // TempData es para pasar datos entre acciones
				// ViewBag/Data es para pasar datos del controlador a la vista
				// Si viene alguno valor por el tempdata, lo paso al viewdata/viewbag
				if (TempData.ContainsKey("Mensaje"))
					ViewBag.Mensaje = TempData["Mensaje"];
            validaDisponibles(lista);
            return View(lista);
        }
        catch(Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }

    [Authorize]
    public IActionResult Editar(int id)
    {
        try
        {
            if(id > 0)
            {   
                IList<String> listaDisponibilidad = new List<String>();
                IList<String> listaUso = new List<String>();
                cargarLista(listaDisponibilidad, listaUso);
                
                RepositorioPropietario repoPropietario = new RepositorioPropietario();
                ViewBag.Propietarios = repoPropietario.ObtenerPropietarios();
                RepositorioTipo rt = new RepositorioTipo();
                ViewBag.Tipos = rt.ObtenerTiposInmuebles();
                RepositorioInmueble rinmu = new RepositorioInmueble();
                var inmueble = rinmu.ObtenerInmueble(id);
                ViewBag.Disponibilidad = listaDisponibilidad;
                ViewBag.Uso = listaUso;
                validaDisponible(inmueble);
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

    [Authorize]
    public IActionResult Crear() //carga el formulario vacio
		{
			try
			{
				RepositorioTipo rt = new RepositorioTipo();
                ViewBag.Tipos = rt.ObtenerTiposInmuebles();
                RepositorioPropietario repoPropietario = new RepositorioPropietario();
                ViewBag.Propietarios = repoPropietario.ObtenerPropietarios();
                return View();
			}
			catch (Exception ex)
			{
                return Json(new { Error = ex.Message });
			}
		}

    [Authorize]
    public IActionResult Guardar(Inmueble inmueble)
    {
        RepositorioInmueble rinmu = new RepositorioInmueble();
        
        try
        {   
            
           if(ModelState.IsValid)
           {
                if(inmueble.IdInmueble > 0)
                {   
                    disp(inmueble);
                    rinmu.ModificaInmueble(inmueble);
                }
                else{
                    
                    rinmu.AltaInmueble(inmueble);
                    disp(inmueble);
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

    [Authorize(Policy = "Administrador")]
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
    
    [Authorize]
    public IActionResult Detalle(int id)
    {
        RepositorioInmueble rinmu = new RepositorioInmueble();

        Inmueble? inmu = rinmu.ObtenerInmueble(id);
        ViewBag.Propietario = inmu.Duenio.Nombre + " " + inmu.Duenio.Apellido;
        ViewBag.Tipo = inmu.TipoInmueble.Descripcion;
        validaDisponible(inmu);
        return View(inmu);
    }

    [Authorize]
    public IActionResult InmueblesSuspendidos()
    {
        RepositorioInmueble rinmu = new RepositorioInmueble();
        var lista = rinmu.obtenerInmueblesSuspendidos();
        validaDisponibles(lista);

        return View(lista);
    }

    private void validaDisponibles(IList<Inmueble> lista)
    {
        foreach(Inmueble inmu in lista)
        {
            validaDisponible(inmu);
        }
    }

    public void validaDisponible(Inmueble inmu)
    {
        if(inmu.Disponible == 0)
        {
           inmu.Disp = "Disponible"; 
        }
        else{
                inmu.Disp = "Suspendido";
            }
    }

    private void disp(Inmueble inmu)
    {
        if(inmu.Disp == "Disponible")
        {
            inmu.Disponible = 0;
        }
        else {
            inmu.Disponible = 1;
        }
    }

    private void cargarLista(IList<String> listaD, IList<String> listaU)
    {
        listaD.Add("Suspendido");
        listaD.Add("Disponible");
        listaU.Add("Residencial");
        listaU.Add("Comercial");
    }
}