using AutoMapper;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Classic.Odata.Domain.Mapper
{
    public class ServiceMapperAccessor : IServiceMapperAccessor
    {
        public ServiceMapperAccessor()
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Include,
                MissingMemberHandling = MissingMemberHandling.Ignore,
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                ContractResolver = new DefaultContractResolver(),

            };

            settings.Converters.Add(new StringEnumConverter());
            Mapper = new MapperConfiguration(m =>
            {


            }).CreateMapper();

        }

        public IMapper Mapper { get; }

    }
}