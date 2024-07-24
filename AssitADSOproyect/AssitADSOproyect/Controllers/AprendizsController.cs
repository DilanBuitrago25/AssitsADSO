using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClaseDatos;
using static AssitADSOproyect.Controllers.LoginController;
using System.Web.Services;
using System.Data.SqlClient;

namespace AssitADSOproyect.Controllers
{
    public class AprendizsController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        string Conexion = "Data Source=Buitrago;Initial Catalog=BDAssistsADSOreal;Integrated Security=True;trustservercertificate=True;";
        // GET: Aprendizs
        [AutorizarTipoUsuario("Aprendiz")]
        public ActionResult Index(string estadoFiltro = "")
        {
            int usuarioId = (int)Session["Idusuario"];
            int TotalFichas;

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = @"
                    SELECT COUNT(DISTINCT fhu.Id_ficha) 
                    FROM Ficha_has_Usuario fhu
                    WHERE fhu.Id_usuario = @UsuarioId
                ";

                SqlCommand comando = new SqlCommand(query, connection);
                comando.Parameters.AddWithValue("@UsuarioId", usuarioId);
                connection.Open();
                TotalFichas = (int)comando.ExecuteScalar();
            }
            ViewBag.TotalFichas = TotalFichas;
            int Total_Soporte;

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM Soporte 
                    WHERE Id_Aprendiz = @UsuarioId
                ";

                SqlCommand comando = new SqlCommand(query, connection);
                comando.Parameters.AddWithValue("@UsuarioId", usuarioId); // Agrega el parámetro
                connection.Open();
                Total_Soporte = (int)comando.ExecuteScalar();
            }
            ViewBag.Total_Soporte = Total_Soporte;

            int Total_Asistencias;

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = @"
                   SELECT COUNT(*) 
                    FROM RegistroAsistencia ra
                    INNER JOIN Asistencia a ON ra.Id_asistencia = a.Id_asistencia
                    WHERE ra.Id_Aprendiz = @UsuarioId AND ra.Asistio_registro = 1 
                ";

                SqlCommand comando = new SqlCommand(query, connection);
                comando.Parameters.AddWithValue("@UsuarioId", usuarioId); // Agrega el parámetro
                connection.Open();
                Total_Asistencias = (int)comando.ExecuteScalar();
            }
            ViewBag.Total_Asistencias = Total_Asistencias;

            int Total_Inasistencias;

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = @"
                    SELECT COUNT(*) 
                    FROM RegistroAsistencia ra
                    INNER JOIN Asistencia a ON ra.Id_asistencia = a.Id_asistencia
                    WHERE ra.Id_Aprendiz = @UsuarioId AND ra.Asistio_registro = 0 
                ";

                SqlCommand comando = new SqlCommand(query, connection);
                comando.Parameters.AddWithValue("@UsuarioId", usuarioId); // Agrega el parámetro
                connection.Open();
                Total_Inasistencias = (int)comando.ExecuteScalar();
            }
            ViewBag.Total_Inasistencias = Total_Inasistencias;

            var idUsuarioLoggeado = (int)Session["Idusuario"]; // Obtener el ID del usuario loggeado

            var fichasFiltradas = db.Ficha
                .Where(f => f.Estado_ficha == true) // Filtrar por Estado_Ficha = true
                .Where(f => estadoFiltro == "" || f.Estado_ficha.ToString() == estadoFiltro) // Filtrar por estado (opcional)
                .Where(f => f.Ficha_has_Usuario.Any(fu => fu.Id_usuario == idUsuarioLoggeado && fu.TipoUsuario == "Aprendiz")) // Filtrar por relación con el usuario
                .ToList();


            ViewBag.EstadoFiltro = estadoFiltro;
            var asistenciasPorFicha = ContarAsistenciasPorFicha();
            ViewBag.AsistenciasPorFicha = asistenciasPorFicha;
            var registrosFaltantesPorFicha = ContarRegistrosFaltantesPorFicha();
            ViewBag.RegistrosFaltantesPorFicha = registrosFaltantesPorFicha;
            var registrosSoportePorFicha = ContarRegistrosSoportePorFicha();
            ViewBag.RegistrosSoportePorFicha = registrosSoportePorFicha;
            return View(fichasFiltradas);
        }

        public Dictionary<int, int> ContarAsistenciasPorFicha()
        {
            Dictionary<int, int> asistenciasPorFicha = new Dictionary<int, int>();

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = @"
            SELECT f.Id_ficha, COUNT(a.Id_asistencia) AS TotalAsistencias
            FROM Ficha f
            LEFT JOIN Asistencia a ON f.Id_ficha = a.Id_ficha
            GROUP BY f.Id_ficha";

                SqlCommand comando = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idFicha = (int)reader["Id_ficha"];
                        int totalAsistencias = (int)reader["TotalAsistencias"];
                        asistenciasPorFicha[idFicha] = totalAsistencias;
                    }
                }
            }

            return asistenciasPorFicha;
        }

        public Dictionary<int, int> ContarRegistrosFaltantesPorFicha()
        {
            Dictionary<int, int> registrosFaltantesPorFicha = new Dictionary<int, int>();

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = @"
            SELECT f.Id_ficha, COUNT(ra.Id_Registroasistencia	) AS TotalRegistrosFaltantes
            FROM Ficha f
            LEFT JOIN Asistencia a ON f.Id_ficha = a.Id_ficha
            LEFT JOIN RegistroAsistencia ra ON a.Id_asistencia = ra.Id_Registroasistencia AND ra.Asistio_registro = 0 -- Filtrar por asistencias faltantes
            GROUP BY f.Id_ficha;";

                SqlCommand comando = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idFicha = (int)reader["Id_ficha"];
                        int totalRegistrosFaltantes = (int)reader["TotalRegistrosFaltantes"];
                        registrosFaltantesPorFicha[idFicha] = totalRegistrosFaltantes;
                    }
                }
            }

            return registrosFaltantesPorFicha;
        }

        public Dictionary<int, int> ContarRegistrosSoportePorFicha()
        {
            Dictionary<int, int> registrosSoportePorFicha = new Dictionary<int, int>();

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = @"
            SELECT f.Id_ficha, COUNT(s.Id_soporte) AS TotalRegistrosSoporte
            FROM Ficha f
            LEFT JOIN Asistencia a ON f.Id_ficha = a.Id_ficha
            LEFT JOIN Soporte s ON a.Id_asistencia = s.Id_asistencia 
            GROUP BY f.Id_ficha";

                SqlCommand comando = new SqlCommand(query, connection);
                connection.Open();

                using (SqlDataReader reader = comando.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        int idFicha = (int)reader["Id_ficha"];
                        int totalRegistrosSoporte = (int)reader["TotalRegistrosSoporte"];
                        registrosSoportePorFicha[idFicha] = totalRegistrosSoporte;
                    }
                }
            }

            return registrosSoportePorFicha;
        }

        //// GET: Aprendizs/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Usuario usuario = db.Usuario.Find(id);
        //    if (usuario == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(usuario);
        //}

       

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
