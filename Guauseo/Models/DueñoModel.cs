
using Microsoft.VisualBasic;
using System.ComponentModel.DataAnnotations;

namespace Guauseo.Models
{
    public class DueñoModel
    {
        public decimal Codigo { get; set; }

        public string Correo { get; set; }

        public string Contraseña { get; set; }

        public string Nombre { get; set; }

        public string Cedula { get; set; }

        public string Direccion { get; set; }

        public string Telefono { get; set; }

        public DateTime Fecha { get; set; }

        public string Estado { get; set; }

    }
}
