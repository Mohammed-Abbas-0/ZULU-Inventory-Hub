using InventoryHub.ModelViews;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.ServicesPatterns.IService
{
    public interface ISearchStrategyFactory
    {
        ISearchStrategy GetSearchStrategy(TypeOfSearch typeOfSearch);
    }

}
