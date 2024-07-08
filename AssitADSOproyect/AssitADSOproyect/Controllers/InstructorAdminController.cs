using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClaseDatos;
using static AssitADSOproyect.Controllers.LoginController;

namespace AssitADSOproyect.Controllers
{
    public class InstructorAdminController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        string Conexion = "Data Source=DESKTOP-057421\\SQLEXPRESS;Initial Catalog=BDAssistsADSOvFinal;Integrated Security=True;trustservercertificate=True;";
        // GET: Instructor
        [AutorizarTipoUsuario("InstructorAdmin")]
        public ActionResult Index(string estadoFiltro = "")
        {
            int Total_Aprendices;

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = "(select count(*) from Usuario where Tipo_usuario = 'Aprendiz')";

                SqlCommand comando = new SqlCommand(query, connection);
                connection.Open();
                Total_Aprendices = (int)comando.ExecuteScalar();
            }
            ViewBag.Total_Aprendices = Total_Aprendices;
            int Total_Competencias;

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = "(select count(*) from Competencia)";

                SqlCommand comando = new SqlCommand(query, connection);
                connection.Open();
                Total_Competencias = (int)comando.ExecuteScalar();
            }
            ViewBag.Total_Competencias = Total_Competencias;

            int Total_Asistencias;

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = "(select count(*) from Asistencia)";

                SqlCommand comando = new SqlCommand(query, connection);
                connection.Open();
                Total_Asistencias = (int)comando.ExecuteScalar();
            }
            ViewBag.Total_Asistencias = Total_Asistencias;

            int Total_Fichas;

            using (SqlConnection connection = new SqlConnection(Conexion))
            {
                string query = "(select count(*) from Ficha)";

                SqlCommand comando = new SqlCommand(query, connection);
                connection.Open();
                Total_Fichas = (int)comando.ExecuteScalar();
            }
            ViewBag.Total_Fichas = Total_Fichas;

            var fichasFiltradas = db.Ficha
                 .Where(f => f.Estado_ficha == true) // Filtrar por Estado_Ficha = true
                 .Where(f => estadoFiltro == "" || f.Estado_ficha.ToString() == estadoFiltro) // Filtrar por estado (opcional)
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

        // GET: InstructorAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario usuario = db.Usuario.Find(id);
            if (usuario == null)
            {
                return HttpNotFound();
            }
            return View(usuario);
        }

      
        

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
