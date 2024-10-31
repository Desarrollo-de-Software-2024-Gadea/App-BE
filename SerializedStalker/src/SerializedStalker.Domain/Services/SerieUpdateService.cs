using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Domain.Services;
using SerializedStalker.Series;
using SerializedStalker.Notificaciones;
using System.Collections.Generic;
using Volo.Abp.Users;
using System;
using AutoMapper.Internal.Mappers;
using Volo.Abp.ObjectMapping;
using Microsoft.AspNetCore.Authorization;

namespace SerializedStalker.Series
{
    [Authorize]
    public class SerieUpdateService : DomainService, ISerieUpdateService // Implementa la interfaz
    {
        private readonly ISeriesApiService _seriesApiService;
        private readonly IRepository<Serie, int> _serieRepository;
        private readonly INotificacionService _notificacionService;
        private readonly ICurrentUser _currentUser;

        public SerieUpdateService(
            ISeriesApiService seriesApiService,
            IRepository<Serie, int> serieRepository,
            ICurrentUser currentUser,
            INotificacionService notificacionService) // Inyección del servicio de notificaciones
        {
            _seriesApiService = seriesApiService;
            _serieRepository = serieRepository;
            _currentUser = currentUser;
            _notificacionService = notificacionService; // Asignación del servicio
        }

        public async Task VerificarYActualizarSeriesAsync()
        {
            var series = await _serieRepository.GetListAsync();
            foreach (var serie in series)
            {
                var apiSeries = await _seriesApiService.BuscarSerieAsync(serie.Titulo, serie.Generos);

                if (apiSeries != null && apiSeries.Length > 0)
                {
                    var apiSerie = apiSeries.FirstOrDefault();

                    // Persistir la serie en la base de datos si es nueva o necesita actualizarse
                    //await PersistirSeriesAsync(new[] { apiSerie }); No tiene sentido llamar esto, la series checkeadas deben esta ya persistidas, solo debemos actualizar

                    // Si la serie tiene más temporadas, se agrega la nueva temporada
                    if (apiSerie.TotalTemporadas > serie.TotalTemporadas)
                    {
                        var nuevaTemporadaNumero = serie.TotalTemporadas + 1;
                        var nuevaTemporadaApi = await _seriesApiService.BuscarTemporadaAsync(apiSerie.ImdbIdentificator, nuevaTemporadaNumero);

                        if (nuevaTemporadaApi != null)
                        {
                            //var nuevaTemporada = ObjectMapper.Map<TemporadaDto, Temporada>(nuevaTemporadaApi);
                            //ObjectMapper no funciona, por alguna razón
                            var nuevaTemporada = new Temporada
                            {
                                NumeroTemporada = nuevaTemporadaNumero,
                                Episodios = nuevaTemporadaApi.Episodios.Select(e => new Episodio
                                {
                                    Titulo = e.Titulo,
                                    NumeroEpisodio = e.NumeroEpisodio,
                                    FechaEstreno = e.FechaEstreno
                                }).ToList()
                            };

                            serie.Temporadas.Add(nuevaTemporada);
                            serie.TotalTemporadas = apiSerie.TotalTemporadas;

                            await _serieRepository.UpdateAsync(serie);

                            // Notificar al usuario sobre la nueva temporada
                            var tituloNotificacionTemporada = $"Nueva temporada disponible de {serie.Titulo}";
                            var mensajeNotificacionTemporada = $"La temporada {nuevaTemporadaNumero} ya está disponible en {serie.Titulo}.";

                            var usuarioId = 001; // Suponiendo un usuario por defecto

                            // Los usuarios pueden elegir si quieren notificaciones por mail o pantalla
                            await _notificacionService.CrearYEnviarNotificacionAsync(
                                usuarioId, tituloNotificacionTemporada, mensajeNotificacionTemporada, TipoNotificacion.Email);
                            await _notificacionService.CrearYEnviarNotificacionAsync(
                                usuarioId, tituloNotificacionTemporada, mensajeNotificacionTemporada, TipoNotificacion.Pantalla);
                        }
                    }

                    // Obtener la última temporada local
                    var ultimaTemporadaLocal = serie.Temporadas.OrderByDescending(t => t.NumeroTemporada).FirstOrDefault();
                    if (ultimaTemporadaLocal != null)
                    {
                        // Obtener la última temporada desde la API
                        var apiUltimaTemporada = await _seriesApiService.BuscarTemporadaAsync(apiSerie.ImdbIdentificator, ultimaTemporadaLocal.NumeroTemporada);

                        if (apiUltimaTemporada != null)
                        {
                            // Comparar la cantidad de episodios
                            if (apiUltimaTemporada.Episodios.Count > ultimaTemporadaLocal.Episodios.Count)
                            {
                                // Detectar episodios nuevos
                                var episodiosLocales = ultimaTemporadaLocal.Episodios.Select(e => e.NumeroEpisodio).ToHashSet();
                                var episodiosNuevos = apiUltimaTemporada.Episodios
                                    .Where(e => !episodiosLocales.Contains(e.NumeroEpisodio))
                                    .ToList();

                                if (episodiosNuevos.Any())
                                {
                                    // Lógica para manejar los episodios nuevos
                                    foreach (var episodioNuevo in episodiosNuevos)
                                    {
                                        //var nuevoEpisodio = ObjectMapper.Map<EpisodioDto, Episodio>(episodioNuevo);
                                        var nuevoEpisodio = new Episodio
                                        {
                                            Titulo = episodioNuevo.Titulo,
                                            NumeroEpisodio = episodioNuevo.NumeroEpisodio,
                                            FechaEstreno = episodioNuevo.FechaEstreno,
                                            TemporadaID = ultimaTemporadaLocal.Id
                                        };

                                        // Agregar a la colección de episodios de la temporada local
                                        ultimaTemporadaLocal.Episodios.Add(nuevoEpisodio);
                                    }

                                    // Reemplazo de la última temporada
                                    var ultimaTemporadaSerie = serie.Temporadas.OrderByDescending(t => t.NumeroTemporada).FirstOrDefault();
                                    var listaTemporadas = serie.Temporadas.ToList();
                                    var indiceUltimaTemporadaSerie = listaTemporadas.IndexOf(ultimaTemporadaSerie);
                                    listaTemporadas[indiceUltimaTemporadaSerie] = ultimaTemporadaLocal;
                                    serie.Temporadas = listaTemporadas;

                                    await _serieRepository.UpdateAsync(serie);

                                    // Generar y persistir la notificación para la serie
                                    var tituloNotificacion = $"Nuevos episodios en {serie.Titulo}";
                                    var mensajeNotificacion = $"Se han añadido {episodiosNuevos.Count} nuevos episodios en la serie {serie.Titulo}.";

                                    var usuarioId = 001; // Suponiendo un usuario por defecto

                                    // Notificar al usuario sobre los nuevos episodios
                                    await _notificacionService.CrearYEnviarNotificacionAsync(
                                        usuarioId, tituloNotificacion, mensajeNotificacion, TipoNotificacion.Email);
                                    await _notificacionService.CrearYEnviarNotificacionAsync(
                                        usuarioId, tituloNotificacion, mensajeNotificacion, TipoNotificacion.Pantalla);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
