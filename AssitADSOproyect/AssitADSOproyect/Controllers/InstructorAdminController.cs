using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ClaseDatos;
using iTextSharp.text.pdf;
using iTextSharp.text;
using static AssitADSOproyect.Controllers.LoginController;

namespace AssitADSOproyect.Controllers
{
    public class InstructorAdminController : Controller
    {
        private BDAssistsADSOv4Entities db = new BDAssistsADSOv4Entities();
        string Conexion = "Data Source=Buitrago;Initial Catalog=BDAssistsADSO;Integrated Security=True;trustservercertificate=True;";
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

        public ActionResult Asistencias_Ficha(int fichaId)
        {
            var asistencias = db.Asistencia
                .Where(a => a.Id_ficha == fichaId)
                .ToList();

            ViewBag.fichaId = fichaId;
            return View(asistencias);
        }

        class HeaderFooterEvent : PdfPageEventHelper
        {
            private readonly Image _logo;

            public HeaderFooterEvent(string imagePath)
            {
                _logo = Image.GetInstance(imagePath);
                _logo.ScaleToFit(100f, 50f); // Ajustar tamaño de la imagen
            }

            public override void OnOpenDocument(PdfWriter writer, Document document)
            {
                // Calcular la posición del logo para que no se superponga con el contenido
                float logoY = document.PageSize.Height - document.TopMargin - _logo.ScaledHeight;
                _logo.SetAbsolutePosition(document.LeftMargin, logoY);
                document.Add(_logo); // Agregar el logo al inicio del documento
            }

            public override void OnEndPage(PdfWriter writer, Document document)
            {
                // Pie de página (texto centrado)
                Font footerFont = FontFactory.GetFont(FontFactory.HELVETICA, 8);
                Phrase footerText = new Phrase("Generado el " + DateTime.Now.ToString("yyyy-MM-dd HH:mm") + " © AssistADSO. Todos los derechos reservados.", footerFont);
                float textWidth = footerFont.GetCalculatedBaseFont(false).GetWidthPoint(footerText.Content, footerFont.Size);
                float xPosition = (document.PageSize.Width - textWidth) / 2;

                ColumnText.ShowTextAligned(writer.DirectContent, Element.ALIGN_CENTER, footerText, xPosition, document.BottomMargin, 0);
            }
        }

        [HttpPost]
        public ActionResult GenerarReportePDFAsistenciaFicha(int fichaId)
        {
            try
            {
                var asistencias = db.Asistencia
               .Where(a => a.Id_ficha == fichaId)
               .ToList();


                var ficha = db.Ficha.Find(fichaId); // Busca la ficha por su ID
                string codigoFicha = ficha?.Codigo_ficha.ToString(); // Obtiene el código de la ficha

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    Document document = new Document(PageSize.A4.Rotate(), 50, 50, 50, 35);
                    PdfWriter writer = PdfWriter.GetInstance(document, memoryStream);

                    writer.PageEvent = new HeaderFooterEvent(Server.MapPath("~/assets/images/Logo-remove.png")); // Pasar la ruta de la imagen

                    document.Open();

                    // Título
                    Paragraph titulo = new Paragraph("Reporte Asistencias general de la Ficha " + codigoFicha, new Font(Font.FontFamily.HELVETICA, 14, Font.BOLD));
                    titulo.Alignment = Element.ALIGN_CENTER;
                    document.Add(titulo);

                    document.Add(Chunk.NEWLINE);

                    // Agregar contenido al PDF (tabla con datos de las fichas)
                    PdfPTable table = new PdfPTable(7);
                    table.WidthPercentage = 100;

                    // Encabezados de la tabla
                    Font headerFont = FontFactory.GetFont(FontFactory.HELVETICA_BOLD, 12);
                    table.AddCell(new Phrase("Id de Asistencia", headerFont));
                    table.AddCell(new Phrase("Fecha"));
                    table.AddCell(new Phrase("Hora Inicio"));
                    table.AddCell(new Phrase("Hora Fin"));
                    table.AddCell(new Phrase("Detalles Asistencia"));
                    table.AddCell(new Phrase("Competencia"));
                    table.AddCell(new Phrase("Estado Asistencia"));

                    // Datos de las fichas
                    Font cellFont = FontFactory.GetFont(FontFactory.HELVETICA, 10);
                    foreach (var asistenciasFicha in asistencias)
                    {
                        table.AddCell(new Phrase(asistenciasFicha.Id_asistencia.ToString()));
                        table.AddCell(new Phrase(asistenciasFicha.Fecha_asistencia));
                        table.AddCell(new Phrase(asistenciasFicha.Hora_inicio_asistencia));
                        table.AddCell(new Phrase(asistenciasFicha.Hora_fin_asistencia));
                        table.AddCell(new Phrase(asistenciasFicha.Detalles_asistencia));
                        table.AddCell(new Phrase(asistenciasFicha.Competencia.Nombre_competencia));
                        table.AddCell(new Phrase(asistenciasFicha.Estado_Asistencia.ToString()));
                    }

                    document.Add(table);
                    document.Close();

                    return File(memoryStream.ToArray(), "application/pdf", "ReporteAsistenciasgeneraldeFicha" + codigoFicha + ".pdf");

                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Error al generar el PDF: " + ex.Message); 
                return new HttpStatusCodeResult(500, "Error interno del servidor al generar el PDF.");
            }
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
