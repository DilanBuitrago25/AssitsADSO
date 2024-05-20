using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ClaseDatos;

namespace AssitADSOproyect.Controllers
{
    public class LoginController : Controller
    {
        static string Conexion = "Data Source=DESKTOP-057421\\SQLEXPRESS;Initial Catalog=BDAssistsADSO;Integrated Security=True;MultipleActiveResultSets=True;";

        // GET: Login
        public ActionResult Index()   // Hago el metodo 
        {
            return View();
        }

        [HttpPost]
        public ActionResult reg(Usuario usuario)
        {
            usuario.Contrasena_usuario = (usuario.Contrasena_usuario);
            bool registrado = false;
            string mensaje = string.Empty;

            using (SqlConnection cn = new SqlConnection(Conexion))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("RegistrarUsuario", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Tipo_Documento_usuario", usuario.Tipo_Documento_usuario);
                    cmd.Parameters.AddWithValue("@Documento_usuario", usuario.Documento_usuario);
                    cmd.Parameters.AddWithValue("@Nombre_usuario", usuario.Nombre_usuario);
                    cmd.Parameters.AddWithValue("@Apellido_usuario", usuario.Apellido_usuario);
                    cmd.Parameters.AddWithValue("@Telefono_usuario", usuario.Telefono_usuario);
                    cmd.Parameters.AddWithValue("@Correo_usuario", usuario.Correo_usuario);
                    cmd.Parameters.AddWithValue("@Contrasena_usuario", usuario.Contrasena_usuario);
                    cmd.Parameters.AddWithValue("@Tipo_usuario", usuario.Tipo_usuario);
                    cmd.Parameters.AddWithValue("@Tipo_instructor", usuario.Tipo_instructor);

                    SqlParameter outputRegistrado = new SqlParameter("@Registrado", SqlDbType.Bit)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputRegistrado);

                    SqlParameter outputMensaje = new SqlParameter("@Mensaje", SqlDbType.VarChar, 200)
                    {
                        Direction = ParameterDirection.Output
                    };
                    cmd.Parameters.Add(outputMensaje);

                    cn.Open();
                    cmd.ExecuteNonQuery();

                    registrado = (bool)outputRegistrado.Value;
                    mensaje = outputMensaje.Value.ToString();
                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Ocurrió un error: " + ex.Message;
                    return View("Index");
                }
            }

            ViewData["Mensaje"] = mensaje;
            if (registrado)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = mensaje;
                return View("Index");
            }
        }

        [HttpPost]
        public ActionResult ValidarUsuario(string Correo_usuario, string Contrasena_usuario)
        {
            string tipoUsuario = "0";

            using (SqlConnection cn = new SqlConnection(Conexion))
            {
                try
                {
                    SqlCommand cmd = new SqlCommand("ValidarUsuarios", cn);
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@Correo_usuario", Correo_usuario);
                    cmd.Parameters.AddWithValue("@Contrasena_usuario", Contrasena_usuario);

                    cn.Open();
                    tipoUsuario = cmd.ExecuteScalar().ToString();

                }
                catch (Exception ex)
                {
                    ViewBag.Error = "Ocurrió un error: " + ex.Message;
                    return View("Index");
                }
            }

            if (tipoUsuario != "0")
            {

                if (tipoUsuario == "Instructor") { return RedirectToAction("Index", "Instructor"); }
                FormsAuthentication.SetAuthCookie(Correo_usuario, false);
                return RedirectToAction("Index", "Home");


            }
            else
            {
                ViewData["Mensaje"] = "Correo o contraseña incorrectos";
                return View("Index");
            }
        }

    }
}
