#region License

//  Copyright  John Rummell
// 
//  This file is part of SteamAchievements.
// 
//      SteamAchievements is free software: you can redistribute it and/or modify
//      it under the terms of the GNU General Public License as published by
//      the Free Software Foundation, either version 3 of the License, or
//      (at your option) any later version.
// 
//      SteamAchievements is distributed in the hope that it will be useful,
//      but WITHOUT ANY WARRANTY; without even the implied warranty of
//      MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//      GNU General Public License for more details.
// 
//      You should have received a copy of the GNU General Public License
//      along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Humanizer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace SteamAchievements.Web.Spa.Controllers
{
    /// <summary>
    /// Gets localized resources
    /// </summary>
    /// <seealso cref="SteamAchievements.Web.Spa.Controllers.ApiController" />
    public class ResourceController : ApiController
    {
        private readonly IStringLocalizer<SharedResource> _localizer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ResourceController"/> class.
        /// </summary>
        /// <param name="localizer">The localizer.</param>
        public ResourceController(IStringLocalizer<SharedResource> localizer)
        {
            _localizer = localizer;
        }

        /// <summary>
        /// Gets localized resources.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IDictionary<string, string>>> Get()
        {
            return _localizer.GetAllStrings().ToDictionary(s => s.Name.Camelize(), s => s.Value);
        }
    }
}
