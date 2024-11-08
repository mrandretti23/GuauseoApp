using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guauseo.Models
{
    public class PaseoModel
    {
        public decimal Codigo { get; set; }
        public decimal DueñoCodigo { get; set; }
        public decimal? PaseadorCodigo { get; set; }
        public string UbicacionNombre { get; set; }
        public string CoordenadasDueño { get; set; }
        public string? CoordenadasPaseador { get; set; }
        public decimal CodigoMascota1 { get; set; }
        public decimal? CodigoMascota2 { get; set; }
        public string ValorPagar { get; set; }
        public string EstadoPaseo { get; set; }
        public string TiempoSeleccionado { get; set; }
        public DateTime Fecha { get; set; }

    }
}
