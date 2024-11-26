using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializedStalker.Series
{
    public class EpisodioDto
    {
        public int NumeroEpisodio { get; set; }
        public DateOnly FechaEstreno { get; set; }
        public string Titulo { get; set; }
        public string Directores { get; set; }
        public string Escritores { get; set; }
        public string Duracion { get; set; }
        public string Resumen { get; set; }

        //Foreign key
        public int TemporadaID { get; set; }
    }
}
