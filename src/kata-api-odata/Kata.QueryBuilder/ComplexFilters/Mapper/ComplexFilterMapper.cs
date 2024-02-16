using AutoMapper;

namespace Kata.QueryBuilder.ComplexFilters.Mapper
{
    public class ComplexFilterMapper : IComplexFilterMapper<ComplexFilterMapper>
    {
        public ComplexFilterMapper()
        {
            //Mapper = new MapperConfiguration(m =>
            //{
            //    m.CreateMap<Dictionary.Shared.Models.EntitySettings.ComplexFilter, ComplexFilter>();
            //    m.CreateMap<Dictionary.Shared.Models.UserSettings.ComplexFilter, ComplexFilter>();

            //    m.CreateMap<Dictionary.Shared.Models.EntitySettings.Filter, Filter>();
            //    m.CreateMap<Dictionary.Shared.Models.UserSettings.Filter, Filter>();

            //}).CreateMapper();
        }

        public IMapper Mapper { get; }
    }
}
