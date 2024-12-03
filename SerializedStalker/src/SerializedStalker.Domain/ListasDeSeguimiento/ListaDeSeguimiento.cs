using Microsoft.VisualBasic;
using SerializedStalker.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace SerializedStalker.ListasDeSeguimiento
{
    public class ListaDeSeguimiento : AggregateRoot<int>, IMustHaveCreator<Guid>
    {
        //Agregar usuario
        public List<Serie> Series { get; set; } = new List<Serie>(); // Inicializar aquí
        public DateOnly FechaModificacion { get; set; }
        //Usuario
        public Guid Creator { get; set; }
        public Guid CreatorId { get; set; }
    }
}
