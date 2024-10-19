using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities.Auditing;

namespace SerializedStalker.Series
{
    public class Calificacion : FullAuditedEntity<int>
    {
        public float calificacion {  get; set; }
        [Range(1, 5, ErrorMessage = "La calificación debe estar entre 1 y 5")]
        public string comentario { get; set; }
        public DateTime FechaCreacion { get; set; }

        //Foreign key
        public int SerieID { get; set; }

    }
}
