﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerializedStalker.Series
{
    public class CreateUpdateEpisodioDto
    {
        public string Titulo { get; set; }
        public DateOnly FechaLanzamiento { get; set; }
        public int NumeroEpisodio { get; set; }
        public string Resumen { get; set; }
        public string Duracion { get; set; }
        public string Directores { get; set; }
        public string Escritores { get; set; }
    }
}