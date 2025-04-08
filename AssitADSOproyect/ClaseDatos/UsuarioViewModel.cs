using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ClaseDatos
{
    public class UsuarioViewModel
    {
   
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


        public List<int> FichasSeleccionadas { get; set; }

        public IEnumerable<Ficha> FichasDisponibles { get; set; }
    }

    public class RegistroAsistenciaViewModel
    {
        public RegistroAsistencia RegistroAsistencia { get; set; }
        public bool TieneSoporte { get; set; }
    }

    public class CambiarContrasenaViewModel
    {
        [Required]
        [DataType(DataType.Password)]
        public string Contrasena_actual { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Contrasena_nueva { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Contrasena_nueva", ErrorMessage = "La nueva contraseña y su confirmación no coinciden.")]
        public string Contrasena_confirmar { get; set; }
    }
}
