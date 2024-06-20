using ClaseDatos;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using WebGrease.Configuration;

namespace AssitADSOproyect
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Error(object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            HttpException httpEx = ex as HttpException;

            if (httpEx != null && httpEx.GetHttpCode() == 401)
            {
                // Lógica personalizada antes de redirigir (opcional)
                Server.ClearError(); // Limpia el error para evitar la página de error genérica
                Response.Redirect("~/Views/Home/Error401.cshtml");
            }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            System.Timers.Timer timer = new System.Timers.Timer(3600000); // 1 hora en milisegundos
            timer.Elapsed += (sender, e) => GenerarRegistrosAsistencia();
            timer.Enabled = true;
        }

        public void GenerarRegistrosAsistencia()
        {
            using (var db = new BDAssistsADSOEntities())
            {
                var fechaHoraActual = DateTime.Now;

                // Obtener asistencias que potencialmente han finalizado
                var posiblesAsistenciasFinalizadas = db.Asistencia.Include(a => a.Usuario).ToList();

                // Filtrar asistencias finalizadas y no registradas
                var asistenciasFinalizadas = posiblesAsistenciasFinalizadas
                    .Where(a =>
                    {
                        DateTime fechaFin;
                        TimeSpan horaFin;
                        return DateTime.TryParseExact(a.Fecha_fin_asistencia, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fechaFin) &&
                               TimeSpan.TryParseExact(a.Hora_fin_asistencia, "hh\\:mm", CultureInfo.InvariantCulture, out horaFin) &&
                               fechaFin < fechaHoraActual.Date && horaFin < fechaHoraActual.TimeOfDay &&
                               a.Usuario.Tipo_usuario == "Instructor";
                    })
                    .ToList();

                foreach (var asistencia in asistenciasFinalizadas)
                {
                    int? idFicha = asistencia.Id_ficha; // Obtener el Id_ficha fuera de la consulta

                    // Obtener aprendices de la ficha que no se han registrado
                    var aprendicesSinRegistro = db.Usuario
                        .Where(u => u.Tipo_usuario == "Aprendiz" && u.Id_ficha == idFicha &&
                                    !db.RegistroAsistencia.Any(ra => ra.Id_asistencia == asistencia.Id_asistencia && ra.Id_usuario == u.Id_usuario))
                        .ToList();

                    foreach (var aprendiz in aprendicesSinRegistro)
                    {
                        // Crear registro de asistencia por defecto
                        var nuevoRegistro = new RegistroAsistencia
                        {
                            Fecha_registro = null,
                            Hora_registro = null,
                            Id_asistencia = asistencia.Id_asistencia,
                            Id_usuario = aprendiz.Id_usuario,
                            Asistio_registro = false
                        };
                        db.RegistroAsistencia.Add(nuevoRegistro);
                    }
                }

                db.SaveChanges(); // Guardar los nuevos registros
            }
        }







    }
}
