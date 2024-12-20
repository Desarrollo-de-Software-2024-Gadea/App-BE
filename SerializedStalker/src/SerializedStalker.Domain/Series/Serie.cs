﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Auditing;
using Volo.Abp.Domain.Entities;

namespace SerializedStalker.Series
{
    public class Serie : AggregateRoot<int>, IMustHaveCreator<Guid>, ISoftDelete
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
        public ICollection<Temporada> Temporadas { get; set; }

        //Usuario
        public Guid Creator { get; set; }
        public Guid CreatorId { get; set; }

        //Manejo de Calificaciones
        public ICollection<Calificacion> Calificaciones { get; set; }

        public Serie()
        {
            Calificaciones = new List<Calificacion>();
        }

        //Esta borrada
        public Boolean IsDeleted { get; set; }
    }
}

