using AutoMapper;

namespace Hearth.ArcGIS.Samples.Core2
{
    public class PersonProfile : Profile
    {
        public PersonProfile()
        {
            CreateMap<Person, PersonVO>();
            CreateMap<PersonVO, Person>();
        }
    }
}
