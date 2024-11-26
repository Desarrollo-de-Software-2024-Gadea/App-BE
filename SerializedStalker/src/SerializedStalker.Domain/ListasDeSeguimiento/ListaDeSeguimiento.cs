using Microsoft.VisualBasic;
using SerializedStalker.Series;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace SerializedStalker.ListasDeSeguimiento
{
    public class ListaDeSeguimiento : AggregateRoot<int>
    {
        public List<Serie> Series { get; set; }
        public DateOnly FechaModificacion {  get; set; } 
    }
}
