using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guauseo.Models
{
    public class CalificacionModel
    {

        public decimal Codigo { get; set; }
        public decimal? PaseadorCodigo { get; set; }
        public decimal PaseoCodigo { get; set; }
        public string Calificacion { get; set; }
        public string Comentario { get; set; }
        public DateTime Fecha { get; set; }


    }
}
