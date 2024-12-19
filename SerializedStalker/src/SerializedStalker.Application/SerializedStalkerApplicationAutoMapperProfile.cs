using AutoMapper;
using SerializedStalker.Application.Contracts.Notificaciones;
using SerializedStalker.Domain.Notificaciones;
using SerializedStalker.Series;

namespace SerializedStalker;

public class SerializedStalkerApplicationAutoMapperProfile : Profile
{
    public SerializedStalkerApplicationAutoMapperProfile()
    {
        /* You can configure your AutoMapper mapping configuration here.
         * Alternatively, you can split your mapping configurations
         * into multiple profile classes for a better organization. */

        //Serie
        CreateMap<Serie, SerieDto>();
        CreateMap<SerieDto, Serie>();
        CreateMap<CreateUpdateSerieDto, Serie>();

        //Temporada
        CreateMap<Temporada, TemporadaDto>();
        CreateMap<TemporadaDto, Temporada>();

        //Episodio
        CreateMap<Episodio, EpisodioDto>();
        CreateMap<EpisodioDto, Episodio>();

        //Monitoreo
        CreateMap<MonitoreoApiDto, MonitoreoApi>();
        CreateMap<MonitoreoApi, MonitoreoApiDto>();

        //Notificacion
        CreateMap<NotificacionDto, Notificacion>();


    }
}
