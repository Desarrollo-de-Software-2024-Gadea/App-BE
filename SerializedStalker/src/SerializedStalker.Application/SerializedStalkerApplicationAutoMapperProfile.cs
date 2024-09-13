using AutoMapper;
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
        CreateMap<CreateUpdateSerieDto, Serie>();

        //Episodio
        CreateMap<Episodio, EpisodioDto>();
        CreateMap<CreateUpdateEpisodioDto, Episodio>();
    }
}
