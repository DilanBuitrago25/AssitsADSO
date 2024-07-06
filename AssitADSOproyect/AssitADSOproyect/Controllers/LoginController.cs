﻿using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using ClaseDatos;
using System.Web.Security;
using System.Linq;
using System.Web;
using System.Data.Entity;

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

                if (TempData["ReturnUrl"] != null)
                {
                    string returnUrl = TempData["ReturnUrl"].ToString();
                    TempData.Remove("ReturnUrl"); // Eliminar el valor de TempData
                    return Redirect(returnUrl);
                }

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

        public ActionResult EditarPerfil()
        {
            if (Session["Idusuario"] != null)
            {
                int usuarioId = (int)Session["Idusuario"];
                var usuario = db.Usuario.Find(usuarioId);

                if (usuario != null)
                {
                    return View(usuario);
                }
            }

            return RedirectToAction("Index"); // O a donde quieras redirigir si no hay sesión o usuario
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditarPerfil([Bind(Include = "Id_usuario,Tipo_Documento_usuario,Documento_usuario,Nombre_usuario,Apellido_usuario,Telefono_usuario,Correo_usuario,Contrasena_usuario,Tipo_usuario,Tipo_instructor,Id_ficha,Estado_usuario")] Usuario usuario, Ficha_has_Usuario ficha_Has_Usuario)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario).State = EntityState.Modified;
                db.SaveChanges();

                if (usuario.Tipo_usuario == "Aprendiz")
                {
                    return RedirectToAction("Index", "Aprendizs"); // Redirigir al controlador Aprendizs
                }
                else if (usuario.Tipo_usuario == "Instructor" || usuario.Tipo_usuario == "InstructorAdmin")
                {
                    return RedirectToAction("Index", "Instructor"); // Redirigir al controlador Instructors
                }
                else
                {
                    // Manejar otros tipos de usuario si es necesario
                    return RedirectToAction("Index"); // Redirigir al índice actual por defecto
                }
            }

            return View(usuario);
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
