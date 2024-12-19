using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Entities.Auditing;

namespace SerializedStalker.Series
{
    public class Calificacion : Entity<int>
    {
        public float NroCalificacion {  get; set; }
        public string Comentario { get; set; }
        public DateTime FechaCreacion { get; set; }

        //Foreign key
        public int SerieID { get; set; }
        public Serie Serie { get; set; }
        
        // UsuarioId para rastrear el usuario que hizo la calificación
        public Guid UsuarioId { get; set; }
    }
}
