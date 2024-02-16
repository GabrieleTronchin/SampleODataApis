using ComplexFilters.Abstractions;
using ComplexFilters.Abstractions.Models;
using Kata.QueryBuilder.ComplexFilters.FilterTypeParsers;
using Microsoft.Extensions.DependencyInjection;
using SqlKata;

namespace Kata.QueryBuilder.ComplexFilters
{
    public class KataComplexFilterBuilder : IComplexFilterBuilder
    {
        private readonly IServiceProvider _serviceProvider;

        public KataComplexFilterBuilder(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public Query BuildComplexFilter(Query mainEntity, ComplexFilter filter)
        {

            FilterBuilder filterBuilder = new();

            for (int i = 0; i < filter.EntityFilter.Count; i++)
            {
                var eFilter = filter.EntityFilter[i];

                if (!eFilter.FilterJoiner) continue;

                mainEntity.Join($"{filter.RelatedEntityName} as {filter.CorrelatorName}", j =>
                {
                    j.On($"target.{filter.TargetEntityId}", $"{filter.CorrelatorName}.{filter.RelatedEntityId}"); //set relation between PK/FK
                    return j;
                });

                var filterParser = RetrieveFilterParser(filter.FilterType);
                filterBuilder = filterParser.Parse(filterBuilder, eFilter);




                foreach (var item in filterBuilder.JoinExpression)
                {
                    switch (item.LogicalOperator)
                    {
                        case LogicalOperator.Or:
                            switch (item.Operation)
                            {
                                case LogialComparator.Contains:
                                    mainEntity.OrWhereContains($"{filter.CorrelatorName}.{item.FilterColumn}", item.ValueToFilter);
                                    break;
                                case LogialComparator.StartWith:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.EndWith:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.NotEquals:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.GreaterThan:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.LessThan:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.GreaterThanOrEqual:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.LessThanOrEqual:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.IsNull:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.IsNotNull:
                                    throw new NotImplementedException();
                                    break;
                                default:
                                    mainEntity.OrWhere($"{filter.CorrelatorName}.{item.FilterColumn}", $"{item.ValueToFilter}"); //set filter relation
                                    break;
                            }
                            break;
                        default:
                            switch (item.Operation)
                            {
                                case LogialComparator.Contains:
                                    mainEntity.WhereContains($"{filter.CorrelatorName}.{item.FilterColumn}", item.ValueToFilter);
                                    break;
                                case LogialComparator.StartWith:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.EndWith:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.NotEquals:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.GreaterThan:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.LessThan:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.GreaterThanOrEqual:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.LessThanOrEqual:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.IsNull:
                                    throw new NotImplementedException();
                                    break;
                                case LogialComparator.IsNotNull:
                                    throw new NotImplementedException();
                                    break;
                                default:
                                    mainEntity.Where($"{filter.CorrelatorName}.{item.FilterColumn}", $"{item.ValueToFilter}"); //set filter relation
                                    break;

                            }
                            break;
                    }



                }

            }

            return mainEntity;
        }

        private IFilterTypeParser RetrieveFilterParser(FilterType filterType)
        {
            return filterType switch
            {
                FilterType.CheckBox or FilterType.DropDown => _serviceProvider.GetService<SingleSelectionTypeParser>() ?? throw new NotImplementedException($"{filterType} is not registered on DI."),
                FilterType.Range => _serviceProvider.GetService<RangeSelectionTypeParser>() ?? throw new NotImplementedException($"{filterType} is not registered on DI."),
                _ => _serviceProvider.GetService<DefaultFilterTypeParser>() ?? throw new NotImplementedException($"{filterType} is not registered on DI."),
            };
        }

    }
}