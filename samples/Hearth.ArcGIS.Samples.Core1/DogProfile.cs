using AutoMapper;

namespace Hearth.ArcGIS.Samples.Core1
{
    public class DogProfile : Profile
    {
        public DogProfile()
        {
            CreateMap<Dog, DogVO>();
            CreateMap<DogVO, Dog>();
        }
    }
}
