//------------------------------------------------------------------------------
// <auto-generated>
//     Este código se generó a partir de una plantilla.
//
//     Los cambios manuales en este archivo pueden causar un comportamiento inesperado de la aplicación.
//     Los cambios manuales en este archivo se sobrescribirán si se regenera el código.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ClaseDatos
{
    using System;
    using System.Collections.Generic;
    
    public partial class RegistroAsistencia
    {
        public int Id_Registroasistencia { get; set; }
        public string Fecha_registro { get; set; }
        public string Hora_registro { get; set; }
        public Nullable<int> Id_asistencia { get; set; }
        public Nullable<int> Id_usuario { get; set; }
    
        public virtual Asistencia Asistencia { get; set; }
        public virtual Usuario Usuario { get; set; }
    }
}