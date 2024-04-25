using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Net.Http.Headers;
using Tp_Inmobiliaria_Ledesma_Lillo.Models;
using Tp_Inmobiliaria_Ledesma_Lillo.Net.Controllers;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.VisualBasic;
using Microsoft.AspNetCore.Http.Features;
using Mysqlx.Crud;


namespace Tp_Inmobiliaria_Ledesma_Lillo.Controllers
{
	public class UsuarioController : Controller
	{
        private readonly IConfiguration configuration;
		private readonly IWebHostEnvironment environment;
		private readonly RepositorioUsuario repositorio;
		
         
		private readonly ILogger<HomeController> _logger;
    

    public UsuarioController( ILogger<HomeController> logger,IWebHostEnvironment environment, IConfiguration configuration)
    {	this.environment = environment;
		this.configuration = configuration;
		
        _logger = logger;
    }
		// GET: Usuarios
		[Authorize(Policy = "Administrador")]
		public ActionResult Listado()
		{
			RepositorioUsuario repositorio = new RepositorioUsuario();
			var usuarios1 = repositorio.ObtenerUsuarios();
			return View(usuarios1);
		}

		// GET: Usuarios/Details/5
		[Authorize(Policy = "Administrador")]
		public ActionResult Details(int id)
		{
			var e = repositorio.ObtenerUsuario(id);
			return View(e);
		}

		// GET: Usuario/Crear
		[Authorize(Policy = "Administrador")]
		public ActionResult Crear()
		{
			
			return View();
		}

		// POST: Usuario/Crear
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrador")]
		public ActionResult Crear(Usuario usuario)
    {
        RepositorioUsuario ru = new RepositorioUsuario();
        try
        {   
			if(!ModelState.IsValid)
			{   
				return View();
			}
			else{
				string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(
							password: usuario.Clave,
							salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
							prf: KeyDerivationPrf.HMACSHA1,
							iterationCount: 1000,
							numBytesRequested: 256 / 8));
				usuario.Clave = hashed;
				
				ru.AltaUsuario(usuario);
				
				if (usuario.AvatarFile != null && usuario.IdUsuario >0)
				{
					string wwwPath = environment.WebRootPath;
					string path = Path.Combine(wwwPath, "Uploads");
					if (!Directory.Exists(path))
					{
						Directory.CreateDirectory(path);
					}
					//Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
					string fileName = "avatar_" + usuario.IdUsuario + Path.GetExtension(usuario.AvatarFile.FileName);
					string pathCompleto = Path.Combine(path, fileName);
					usuario.Avatar = Path.Combine("/Uploads", fileName);
					
					// Esta operación guarda la foto en memoria en la ruta que necesitamos
					using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
					{
						usuario.AvatarFile.CopyTo(stream);
					}
					
					ru.ModificaUsuario(usuario);
				}
				
				
				TempData["id"] = usuario.IdUsuario; 
			}
		
			return RedirectToAction(nameof(Index));
        }
        catch(Exception ex)
        {
            return Json(new { Error = ex.Message });
        }

    }
		// GET: Usuario/Editar/5
		[Authorize]
		public ActionResult Perfil()
		{
			ViewData["Title"] = "Mi perfil";
			RepositorioUsuario ru = new RepositorioUsuario();
			Usuario usuario = ru.ObtenerPorEmail(User.Identity.Name);
			return View("Perfil", usuario);
		}
		[Authorize]
		[HttpPost]
		public ActionResult borrarAvatar()
		{	var vista = nameof(Perfil);
			ViewData["Title"] = "Mi perfil";
			RepositorioUsuario ru = new RepositorioUsuario();
			var user = ru.ObtenerPorEmail(User.Identity.Name);
			user.Avatar=" ";
			ru.ModificaUsuario(user);
			return View(vista, user);
		}
		[HttpPost]
		[Authorize]
		public ActionResult Perfil(Usuario usuario){

			var vista = nameof(Perfil);
			
			RepositorioUsuario ru = new RepositorioUsuario();
			Usuario u = ru.ObtenerPorEmail(User.Identity.Name);
			
			if (usuario.AvatarFile != null && u.IdUsuario >0)
			{
				string wwwPath = environment.WebRootPath;
				string path = Path.Combine(wwwPath, "Uploads");
				if (!Directory.Exists(path))
				{
					Directory.CreateDirectory(path);
				}
				//Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
				string fileName = "avatar_" + u.IdUsuario + Path.GetExtension(usuario.AvatarFile.FileName);
				string pathCompleto = Path.Combine(path, fileName);
				u.Avatar = Path.Combine("/Uploads", fileName);
				
				// Esta operación guarda la foto en memoria en la ruta que necesitamos
				using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
				{
					usuario.AvatarFile.CopyTo(stream);
					stream.Dispose();
				}
				
				
				ru.ModificaUsuario(u);
				TempData["mensaje"] = "datos guardados correctamente.";
				
			}
			return RedirectToAction(vista,u);
		}




		
		

		// GET: Usuario/Editar/5
		[Authorize(Policy = "Administrador")]
		public ActionResult Editar(int id)
		{
			ViewData["Title"] = "Editar usuario";
			RepositorioUsuario ru = new RepositorioUsuario();
			Usuario u = ru.ObtenerUsuario(id);
			ViewBag.Roles = u.ObtenerRoles();
			return View("Editar",u);
		}

		// POST: Usuarios/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize]
		public ActionResult Editar(int id, Usuario usuario)
		{
			var vista = nameof(Editar);//de que vista provengo
			try
			{
				if (User.IsInRole("Empleado"))//no soy admin
				{
					vista = nameof(Perfil);//solo puedo ver mi perfil
					var usuarioActual = repositorio.ObtenerPorEmail(User.Identity.Name);
					if (usuarioActual.IdUsuario != id)//si no es admin, solo puede modificarse él mismo
						return RedirectToAction("Home");
				}
				else{
					Usuario u = new Usuario();
					RepositorioUsuario ru = new RepositorioUsuario();
					u=ru.ObtenerUsuario(id);
					u.Nombre = usuario.Nombre;
					u.Apellido = usuario.Apellido;
					u.Email = usuario.Email;
					u.Clave = usuario.Clave;
					u.Rol = usuario.Rol;
					if (usuario.AvatarFile != null && usuario.IdUsuario >0)
					{
						string wwwPath = environment.WebRootPath;
						string path = Path.Combine(wwwPath, "Uploads");
						if (!Directory.Exists(path))
						{
							Directory.CreateDirectory(path);
						}
						//Path.GetFileName(u.AvatarFile.FileName);//este nombre se puede repetir
						string fileName = "avatar_" + usuario.IdUsuario + Path.GetExtension(usuario.AvatarFile.FileName);
						string pathCompleto = Path.Combine(path, fileName);
						u.Avatar = Path.Combine("/Uploads", fileName);
						
						// Esta operación guarda la foto en memoria en la ruta que necesitamos
						using (FileStream stream = new FileStream(pathCompleto, FileMode.Create))
						{
							usuario.AvatarFile.CopyTo(stream);
							stream.Dispose();
						}
						
						
						ru.ModificaUsuario(u);
						TempData["mensaje"] = "datos guardados correctamente.";
						return RedirectToAction(vista,u);
					}
				}
				// TODO: Add update logic here

				return RedirectToAction(vista);
			}
			catch (Exception ex)
			{//colocar breakpoints en la siguiente línea por si algo falla
				throw;
			}
		}

		// GET: Usuarios/Delete/5
		[Authorize(Policy = "Administrador")]
		public ActionResult Eliminar(int id)
		{
			return View();
		}

		// POST: Usuarios/Delete/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		[Authorize(Policy = "Administrador")]
		public ActionResult Eliminar(int id, Usuario usuario)
		{
			try
			{
				// TODO: Add delete logic here
				var ruta = Path.Combine(environment.WebRootPath, "Uploads", $"avatar_{id}" + Path.GetExtension(usuario.Avatar));
				if (System.IO.File.Exists(ruta))
					System.IO.File.Delete(ruta);
				return RedirectToAction(nameof(Index));
			}
			catch
			{
				return View();
			}
		}

		[AllowAnonymous]
		// GET: Usuarios/Login/
		public IActionResult LoginModal()
		{
			return PartialView("_LoginModal", new LoginView());
		}

		[AllowAnonymous]
		// GET: Usuarios/Login/
		public IActionResult Login(string returnUrl)
		{
			TempData["returnUrl"] = returnUrl;
			return View();
		}

		// POST: Usuarios/Login/
		
		[AllowAnonymous]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Login(LoginView login)
		{
			
			try{
				var  returnUrl = String.IsNullOrEmpty(TempData["returnUrl"] as string) ? "/Home" : TempData["returnUrl"].ToString();
				if (ModelState.IsValid)
				{
					string hashed =  Convert.ToBase64String(KeyDerivation.Pbkdf2(
							password: login.Clave,
							salt: System.Text.Encoding.ASCII.GetBytes(configuration["Salt"]),
							prf: KeyDerivationPrf.HMACSHA1,
							iterationCount: 1000,
							numBytesRequested: 256 / 8));
					RepositorioUsuario rep = new RepositorioUsuario();
					var e = rep.ObtenerPorEmail(login.Usuario);
					if (e == null || e.Clave != hashed)
					{
						ModelState.AddModelError("", "El email o la clave no son correctos");
						TempData["returnUrl"] = returnUrl;
						return View();
					}else{
						Console.Write("Usuario logueado correctamente");
					}

					var claims = new List<Claim>
					{
						new Claim(ClaimTypes.Name, e.Email),
						new Claim("FullName", e.Nombre + " " + e.Apellido),
						new Claim(ClaimTypes.Role, e.RolNombre),
					};

					var claimsIdentity = new ClaimsIdentity(
							claims, CookieAuthenticationDefaults.AuthenticationScheme);

					await HttpContext.SignInAsync(
							CookieAuthenticationDefaults.AuthenticationScheme,
							new ClaimsPrincipal(claimsIdentity));
					
					TempData.Remove("returnUrl");
					return Redirect(returnUrl);
				}
				TempData.Remove("returnUrl");
				return Redirect(returnUrl);
			}
			catch (Exception ex)
			{
				Console.WriteLine("error en login", ex.Message);
				return View();
			}
			
		}

		// GET: /salir
		[Route("salir", Name = "logout")]
		public async Task<ActionResult> Logout()
		{
			await HttpContext.SignOutAsync(
					CookieAuthenticationDefaults.AuthenticationScheme);
			return RedirectToAction("Index", "Home");
		}
	}
}