using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

public class MonitoreoApiEstadisticasDto : EntityDto<int>
{
    public float PromedioDuracion { get; set; }
    public int TotalErrores { get; set; }
    public int TotalMonitoreos { get; set; }
}

