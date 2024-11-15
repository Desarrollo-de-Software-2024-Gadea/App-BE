import type { EntityDto } from '@abp/ng.core';

export interface CalificacionDto {
  calificacion: number;
  comentario?: string;
  fechaCreacion?: string;
  serieID: number;
  usuarioId?: string;
}

export interface CreateUpdateSerieDto {
  titulo?: string;
  clasificacion?: string;
  fechaEstreno?: string;
  duracion?: string;
  generos?: string;
  directores?: string;
  escritores?: string;
  actores?: string;
  sinopsis?: string;
  idiomas?: string;
  pais?: string;
  poster?: string;
  imdbPuntuacion?: string;
  imdbVotos: number;
  imdbIdentificator?: string;
  tipo?: string;
  totalTemporadas: number;
}

export interface EpisodioDto {
  numeroEpisodio: number;
  fechaEstreno?: string;
  titulo?: string;
  directores?: string;
  escritores?: string;
  duracion?: string;
  resumen?: string;
  temporadaID: number;
}

export interface SerieDto extends EntityDto<number> {
  titulo?: string;
  clasificacion?: string;
  fechaEstreno?: string;
  duracion?: string;
  generos?: string;
  directores?: string;
  escritores?: string;
  actores?: string;
  sinopsis?: string;
  idiomas?: string;
  pais?: string;
  poster?: string;
  imdbPuntuacion?: string;
  imdbVotos: number;
  imdbIdentificator?: string;
  tipo?: string;
  totalTemporadas: number;
  temporadas: TemporadaDto[];
  calificaciones: CalificacionDto[];
}

export interface TemporadaDto extends EntityDto<number> {
  titulo?: string;
  fechaLanzamiento?: string;
  numeroTemporada: number;
  serieID: number;
  episodios: EpisodioDto[];
}
