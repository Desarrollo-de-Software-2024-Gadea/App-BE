using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializedStalker.Series
{
    public class Califiacacion
    {
        public float calificacion {  get; set; }
        public string comentario { get; set; }
        public DateTime FechaCreacion { get; set; }

        //Foreign key
        public int SerieID { get; set; }

    }
}
