﻿using System;
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
        private BDAssistsADSOEntities db = new BDAssistsADSOEntities();
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

                if (usuario.Tipo_usuario == "Aprendiz" && usuario.Estado_Usuario == true)
                {
                    return RedirectToAction("Index", "Aprendizs"); // Vista para aprendices
                }
                else if (usuario.Tipo_usuario == "Instructor" && usuario.Estado_Usuario == true)
                {
                    return RedirectToAction("Index", "Instructor"); // Vista para instructores
                }
            }
            else
            {
                ViewData["Mensaje"] = "Correo o contraseña incorrectos";
            }
            //if (usuario.Estado_Usuario == false && usuario == null)
            //{
            //    ViewData["Mensaje"] = "Usuario no Activo";
            //}
            //else if (usuario.Estado_Usuario == false) ;
            //{
            //    ViewData["Mensaje"] = "Usuario no Activo";
            //}
            


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
                // Construir la URL del ActionLink
                var urlHelper = new UrlHelper(filterContext.RequestContext);
                var url = urlHelper.Action("Error401", "Home"); // Asumiendo que tienes un controlador "Error" con una acción "Unauthorized"

                // Redirigir a la URL del ActionLink
                filterContext.Result = new RedirectResult(url);
            }
        }



        //[HttpPost]
        //public ActionResult reg(Usuario usuario)
        //{
        //    usuario.Contrasena_usuario = (usuario.Contrasena_usuario);
        //    bool registrado = false;
        //    string mensaje = string.Empty;

        //    using (SqlConnection cn = new SqlConnection(Conexion))
        //    {
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand("RegistrarUsuario", cn);
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.AddWithValue("@Tipo_Documento_usuario", usuario.Tipo_Documento_usuario);
        //            cmd.Parameters.AddWithValue("@Documento_usuario", usuario.Documento_usuario);
        //            cmd.Parameters.AddWithValue("@Nombre_usuario", usuario.Nombre_usuario);
        //            cmd.Parameters.AddWithValue("@Apellido_usuario", usuario.Apellido_usuario);
        //            cmd.Parameters.AddWithValue("@Telefono_usuario", usuario.Telefono_usuario);
        //            cmd.Parameters.AddWithValue("@Correo_usuario", usuario.Correo_usuario);
        //            cmd.Parameters.AddWithValue("@Contrasena_usuario", usuario.Contrasena_usuario);
        //            cmd.Parameters.AddWithValue("@Tipo_usuario", usuario.Tipo_usuario);
        //            cmd.Parameters.AddWithValue("@Tipo_instructor", usuario.Tipo_instructor);

        //            SqlParameter outputRegistrado = new SqlParameter("@Registrado", SqlDbType.Bit)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmd.Parameters.Add(outputRegistrado);

        //            SqlParameter outputMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200)
        //            {
        //                Direction = ParameterDirection.Output
        //            };
        //            cmd.Parameters.Add(outputMensaje);

        //            cn.Open();
        //            cmd.ExecuteNonQuery();

        //            registrado = (bool)outputRegistrado.Value;
        //            mensaje = outputMensaje.Value.ToString();
        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Error = "Ocurrió un error: " + ex.Message;
        //            return View("Index");
        //        }
        //    }

        //    ViewData["Mensaje"] = mensaje;
        //    if (registrado)
        //    {
        //        return RedirectToAction("Index", "Home");
        //    }
        //    else
        //    {
        //        ViewBag.Error = mensaje;
        //        return View("Index");
        //    }
        //}

        //[HttpPost]
        //public ActionResult ValidarUsuario(string Correo_usuario, string Contrasena_usuario)
        //{
        //    string tipoUsuario = "0";

        //    using (SqlConnection cn = new SqlConnection(Conexion))
        //    {
        //        try
        //        {
        //            SqlCommand cmd = new SqlCommand("ValidarUsuarios", cn);
        //            cmd.CommandType = CommandType.StoredProcedure;

        //            cmd.Parameters.AddWithValue("@Correo_usuario", Correo_usuario);
        //            cmd.Parameters.AddWithValue("@Contrasena_usuario", Contrasena_usuario);

        //            cn.Open();
        //            tipoUsuario = cmd.ExecuteScalar().ToString();

        //        }
        //        catch (Exception ex)
        //        {
        //            ViewBag.Error = "Ocurrió un error: " + ex.Message;
        //            return View("Index");
        //        }
        //    }

        //    if (tipoUsuario != "0")
        //    {

        //        if (tipoUsuario == "Instructor") { return RedirectToAction("Index", "Home"); }
        //        FormsAuthentication.SetAuthCookie(Correo_usuario, false);
        //        return RedirectToAction("Index", "Login");


        //    }
        //    else
        //    {
        //        ViewData["Mensaje"] = "Correo o contraseña incorrectos";
        //        return View("Index");
        //    }
        //}

    }
}
