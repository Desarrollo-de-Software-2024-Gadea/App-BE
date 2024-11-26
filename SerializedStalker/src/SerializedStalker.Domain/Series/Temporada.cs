using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace SerializedStalker.Series
{
    public class Temporada : Entity<int>
    {
        public string Titulo { get; set; }
        public DateOnly FechaLanzamiento { get; set; }
        public int NumeroTemporada { get; set; }

       //Foreign key
        public int SerieID { get; set; }
        public Serie Serie { get; set; }

        //Relación uno a muchos con Episodio
        public ICollection<Episodio> Episodios { get; set; }

    }
}
