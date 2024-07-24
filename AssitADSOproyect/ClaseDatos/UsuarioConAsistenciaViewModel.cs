using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaseDatos
{
    public class UsuarioConAsistenciaViewModel
    {
        public ClaseDatos.Usuario Usuario { get; set; }
        public int? Id_Registroasistencia { get; set; }
        public bool Asistio_registro { get; set; }
        public string fecha_registro { get; set; }
        public string hora_registro { get; set; }
    }
}
