using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace SteamAchievements.Web.Helpers
{
    /// <summary>
    /// Based on the example by Phil Haack at http://haacked.com/archive/2011/04/25/conditional-filters.aspx
    /// </summary>
    public class ConditionalFilterProvider : IFilterProvider
    {
        private readonly
            IEnumerable<Func<ControllerContext, ActionDescriptor, object>> _conditions;

        public ConditionalFilterProvider(
            IEnumerable<Func<ControllerContext, ActionDescriptor, object>> conditions)
        {
            _conditions = conditions;
        }

        #region IFilterProvider Members

        public IEnumerable<Filter> GetFilters(ControllerContext controllerContext, ActionDescriptor actionDescriptor)
        {
            return from condition in _conditions
                   select condition(controllerContext, actionDescriptor)
                   into filter
                   where filter != null
                   select new Filter(filter, FilterScope.Global, null);
        }

        #endregion
    }
}