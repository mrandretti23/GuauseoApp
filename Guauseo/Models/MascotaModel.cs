using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Guauseo.Models
{
    public class MascotaModel
    {
        public decimal Codigo { get; set; }

        public decimal DueñoCodigo { get; set; }

        public string Nombre { get; set; }

        public string Raza { get; set; }

        public int Edad { get; set; }

        public string Sexo { get; set; }

        public string Tamaño { get; set; }

        public string Agresividad { get; set; }

        public string Necesidades { get; set; }

        public string Estado { get; set; }

        public string Foto { get; set; }
    }
}
