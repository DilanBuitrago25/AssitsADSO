using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClaseDatos
{
    public class UsuarioViewModel
    {
        // Propiedades del usuario
        public int Id_usuario { get; set; }
        public string Tipo_Documento_usuario { get; set; }
        public int? Documento_usuario { get; set; }
        public string Nombre_usuario { get; set; }
        public string Apellido_usuario { get; set; }
        public decimal? Telefono_usuario { get; set; }
        public string Correo_usuario { get; set; }
        public string Contrasena_usuario { get; set; }
        public string Tipo_usuario { get; set; }
        public bool? Estado_Usuario { get; set; }

        // Fichas seleccionadas (lista de IDs)
        public List<int> FichasSeleccionadas { get; set; }

        // Lista de todas las fichas disponibles (para el dropdown)
        public IEnumerable<Ficha> FichasDisponibles { get; set; }
    }
}
