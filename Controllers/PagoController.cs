using Microsoft.AspNetCore.Mvc;
using Tp_Inmobiliaria_Ledesma_Lillo.Models;
using Tp_Inmobiliaria_Ledesma_Lillo.Net.Controllers;

namespace Tp_Inmobiliaria_Ledesma_Lillo.Controllers;

public class PagoController : Controller
{
    private readonly ILogger<HomeController> _logger;
    

    public PagoController(ILogger<HomeController> logger)
    {
        _logger = logger;
    }

    public IActionResult Listado()
    {
        try
        {
            RepositorioPago rp = new RepositorioPago();
            var lista = rp.ObtenerTodosPagos();
            if(lista.Count != 0)
            { validaEstado(lista); }
            
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
    [HttpGet]
    public IActionResult Crear(int id)
	{
			RepositorioPago rp = new RepositorioPago();
            RepositorioContrato rc = new RepositorioContrato();
            try
			{
                Contrato c = rc.ObtenerContrato(id);
                IList<Pago> lista = rp.ObtenerPagosPorContrato(id);
                ViewBag.tamanio = lista.Count + 1;
                ViewBag.monto = c.Monto;
                ViewBag.idContrato = c.IdContrato;
                ViewBag.ApellidoInq = c.Inquilino.Apellido;
                return View();
			}
			catch (Exception ex)
			{
                return Json(new { Error = ex.Message });
			}
	}

    public IActionResult Eliminar(int id)
    {
        RepositorioPago rp = new RepositorioPago();
        try
        {
            rp.EliminaPago(id);
            Pago p = rp.ObtenerPago(id);
            p.Activo = "Inactivo";
            TempData["Mensaje"] = "Eliminación realizada correctamente";
            return RedirectToAction(nameof(Listado));
        }
        catch(Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }

    [HttpPost]
    public IActionResult Guardar(Pago pago)
    {
        RepositorioPago rp = new RepositorioPago();
        try
        {
           if(ModelState.IsValid)
           {
                if(pago.IdPago > 0)
                {
                    rp.ModificaPago(pago);
                }
                else
                {
                    rp.AltaPago(pago);
                    pago.Est = 0;
                    TempData["id"] = pago.IdPago;
                }
           }
           else 
           {
             return View(pago); 
           }
            return RedirectToAction(nameof(Listado));
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
                RepositorioPago rp = new RepositorioPago();
                var pago = rp.ObtenerPago(id);
                pago.FechaPago = DateTime.Today;
                TempData["Mensaje"] = "Datos guardados correctamente";
                return View(pago);
            }
            else
                { return View();}
        }
        catch(Exception ex)
        {
            return Json(new { Error = ex.Message });
        }
    }

    public IActionResult Detalle(int id)
    {
        RepositorioPago rp = new RepositorioPago();

        Pago? pago = rp.ObtenerPago(id);

        return View(pago);
    }

    public IActionResult PagosEliminados()
    {
        RepositorioPago rp = new RepositorioPago();
        var lista = rp.ObtenerPagosEliminados();
        validaEstado(lista);
        if(lista.Count == 0)
        {
            ViewBag.Mensaje = "No se encontraron registros";
        }
        return View(lista);
    }

    private void validaEstado(IList<Pago> lista)
    {
        foreach(Pago p in lista)
            {
                if(p.Est == 0)
                {
                    p.Activo = "Activo";
                }
                else
                {
                    p.Activo = "Inactivo";
                }
            }
    }
}