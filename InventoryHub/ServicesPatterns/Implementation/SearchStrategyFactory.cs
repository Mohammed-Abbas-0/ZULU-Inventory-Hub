using InventoryHub.Context;
using InventoryHub.ModelViews;
using InventoryHub.ServicesPatterns.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.Implementation
{
    public class SearchStrategyFactory: ISearchStrategyFactory
    {

        private readonly IServiceProvider _serviceProvider;

        public SearchStrategyFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }


        public ISearchStrategy GetSearchStrategy(TypeOfSearch typeOfSearch)
        {
            Type strategyType = typeOfSearch switch
            {
                TypeOfSearch.Previous => typeof(PreviousSearchStrategy),
                TypeOfSearch.Next => typeof(NextSearchStrategy),
                TypeOfSearch.ByCode => typeof(ByCodeSearchStrategy),
                _ => throw new ArgumentException("Invalid search type", nameof(typeOfSearch))
            };

            return (ISearchStrategy)_serviceProvider.GetService(strategyType);
        }
    }
}
