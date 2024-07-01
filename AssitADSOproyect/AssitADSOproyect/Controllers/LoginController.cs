using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ClaseDatos;
using System.Web.Security;
using System.Linq;
using System.Web;

namespace AssitADSOproyect.Controllers
{
    public class LoginController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        // GET: Login
        public ActionResult Index() 
        {
            return View();
        }

        // POST: Auth/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(string correo, string contrasena)
        {
            var usuario = db.Usuario.FirstOrDefault(u => u.Correo_usuario == correo && u.Contrasena_usuario == contrasena);

            if (usuario != null)
            {
                Session["Idusuario"] = usuario.Id_usuario; // Almacenar el ID en sesión
                Session["TipoUsuario"] = usuario.Tipo_usuario;
                Session["NombreUsuario"] = usuario.Nombre_usuario + " " + usuario.Apellido_usuario;
                ViewBag.TipoUsuario = usuario.Tipo_usuario;

                if (usuario.Tipo_usuario == "Aprendiz" && usuario.Estado_Usuario == true)
                {
                    return RedirectToAction("Index", "Aprendizs"); // Vista para aprendices
                }
                else if (usuario.Tipo_usuario == "Instructor" && usuario.Estado_Usuario == true)
                {
                    return RedirectToAction("Index", "Instructor"); // Vista para instructores
                }
                else if (usuario.Tipo_usuario == "InstructorAdmin" && usuario.Estado_Usuario == true)
                {
                    return RedirectToAction("Index", "InstructorAdmin"); // Vista para instructores Administradores 
                }
            }
            if (usuario == null) 
            {
                ViewData["Mensaje"] = "Correo o contraseña incorrectos";
            }
            else
            {
                ViewData["Mensaje"] = "Usuario no Activo";
            }

            


            ModelState.AddModelError("", "Credenciales inválidas.");
            return View();




        }

        // GET: Auth/Logout
        public ActionResult Logout()
        {
            Session.Abandon(); // Cerrar sesión
            return RedirectToAction("Index");
        }


        public class AutorizarTipoUsuarioAttribute : AuthorizeAttribute
        {
            private readonly string[] _tiposPermitidos;
            private readonly string _vistaNoAutorizado = "~/Views/Home/Error401.cshtml";

            public AutorizarTipoUsuarioAttribute(params string[] tiposPermitidos)
            {
                _tiposPermitidos = tiposPermitidos;
            }

            protected override bool AuthorizeCore(HttpContextBase httpContext)
            {
                var tipoUsuario = httpContext.Session["TipoUsuario"] as string;
                return tipoUsuario != null && _tiposPermitidos.Contains(tipoUsuario);
            }

            protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
            {
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                var url = urlHelper.Action("Error401", "Home");
                filterContext.Result = new RedirectResult(url);
            }
        }
    }
}
