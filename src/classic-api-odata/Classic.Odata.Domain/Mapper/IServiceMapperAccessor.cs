using AutoMapper;

namespace Classic.Odata.Domain.Mapper
{
    public interface IServiceMapperAccessor
    {
        IMapper Mapper { get; }
    }
}