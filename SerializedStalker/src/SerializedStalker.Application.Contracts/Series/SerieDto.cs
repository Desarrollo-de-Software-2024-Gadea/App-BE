﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Dtos;

namespace SerializedStalker.Series
{
    public class SerieDto : EntityDto<int>
    {
        public string Titulo { get; set; }
        public string Clasificacion { get; set; }
        public string FechaEstreno { get; set; }
        public string Duracion { get; set; }
        public string Generos { get; set; }
        public string Directores { get; set; }
        public string Escritores { get; set; }
        public string Actores { get; set; }
        public string Sinopsis { get; set; }
        public string Idiomas { get; set; }
        public string Pais { get; set; }
        public string Poster { get; set; }
        public string ImdbPuntuacion { get; set; }
        public int ImdbVotos { get; set; }
        public string ImdbIdentificator { get; set; }
        public string Tipo { get; set; }
        public int TotalTemporadas { get; set; }

        public ICollection<TemporadaDto> Temporadas { get; set; }

        // Manejo de Calificaciones
        public ICollection<CalificacionDto> Calificaciones { get; set; }
    }
}
