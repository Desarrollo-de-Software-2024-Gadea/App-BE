﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Entities;

namespace SerializedStalker.Series
{
    public class MonitoreoApi: Entity<int>
    {
        public DateTime HoraEntrada { get; set; }
        public DateTime HoraSalida { get; set; }
        public float TiempoDuracion { get; set; }
        public List<string> Errores { get; set; } = new List<string>();
    }
}
