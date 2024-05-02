using System.Collections;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Tp_Inmobiliaria_Ledesma_Lillo.Models;
using Tp_Inmobiliaria_Ledesma_Lillo.Net.Controllers;

namespace Tp_Inmobiliaria_Ledesma_Lillo.Controllers;

public class ContratoController : Controller 
{
    private readonly ILogger<HomeController> _logger;
    

    public ContratoController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    [Authorize]
    public IActionResult Listado(int? dias, DateTime? fecInf, DateTime? fecSup, string? dir)
    {
        try
        {   ViewBag.dias = dias;
            RepositorioContrato rc = new RepositorioContrato();
            RepositorioInmueble ri = new RepositorioInmueble();
            IList<Contrato> filtrados = new List<Contrato>();
            var lista=rc.ObtenerContratos();
            var listaInm = ri.ObtenerInmuebles();
            ViewBag.Inmuebles = listaInm;
            if(dir!=null){
                foreach (var item in lista)
                {
                    if(item.Inmueble.Direccion==dir){
                       filtrados.Add(item);
                    }
                }
                return View(filtrados);
                
            }
            
            if(fecInf!=null&&fecSup!=null){
               
                
                foreach (var item in lista)
                {
                    if(item.FecInicio>=fecInf && item.FecFin<=fecSup){
                       filtrados.Add(item);
                    }
                }
                return View(filtrados);
                

            }
            if(dias==0||dias==null){
               
                ViewBag.id = TempData["id"];
                // TempData es para pasar datos entre acciones
                    // ViewBag/Data es para pasar datos del controlador a la vista
                    // Si viene alguno valor por el tempdata, lo paso al viewdata/viewbag
                    if (TempData.ContainsKey("Mensaje"))
                        ViewBag.Mensaje = TempData["Mensaje"];
                return View(lista);
            }
            else{
                       
                DateTime fechLim = DateTime.Today.AddDays((double)dias);
                foreach (var item in lista)
                {
                    if(item.FecFin<fechLim){
                       filtrados.Add(item);
                    }
                }
                return View(filtrados);
            }
            
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
            {   RepositorioInmueble repoInmueble = new RepositorioInmueble();
                RepositorioInquilino repoInquilino = new RepositorioInquilino();
                RepositorioContrato rc = new RepositorioContrato();

                ViewBag.Inmuebles = repoInmueble.ObtenerInmuebles();               
                ViewBag.Inquilinos = repoInquilino.ObtenerInquilinos();

                var contrato = rc.ObtenerContrato(id);
                TempData["Mensaje"] = "Datos guardados correctamente";
                return View(contrato);
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
				RepositorioInmueble repoInmueble = new RepositorioInmueble();
                RepositorioInquilino repoInquilino = new RepositorioInquilino();

                ViewBag.Inmuebles = repoInmueble.ObtenerInmuebles();
                ViewBag.Inquilinos = repoInquilino.ObtenerInquilinos();

                return View();
			}
			catch (Exception ex)
			{
                return Json(new { Error = ex.Message });
			}
		}

    [Authorize]
    public IActionResult Guardar(Contrato contrato)
    {
        RepositorioContrato rc = new RepositorioContrato();
        
        try
        {   
            
           if(ModelState.IsValid)
           {
                if(contrato.IdContrato > 0)
                {   
                    rc.ModificaContrato(contrato);
                }
                else{
                    
                    rc.AltaContrato(contrato);
                    TempData["id"] = contrato.IdContrato; 
                }
           }
           else 
           {
            return View(contrato); 
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
        RepositorioContrato rc = new RepositorioContrato();
        try
        {
            rc.EliminaContrato(id);
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
        RepositorioContrato rc = new RepositorioContrato();

        Contrato? contrato = rc.ObtenerContrato(id);

        ViewBag.Inquilino = contrato.Inquilino.Nombre + " " + contrato.Inquilino.Apellido; 
        ViewBag.Inmueble = contrato.Inmueble.Direccion;

        return View(contrato);
    }
}