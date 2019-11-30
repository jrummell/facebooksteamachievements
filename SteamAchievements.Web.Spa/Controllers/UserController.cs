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

using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Web.Spa.Controllers
{
    public class UserController : ApiController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{facebookUserId}")]
        public async Task<ActionResult<UserModel>> Get(long facebookUserId)
        {
            var user = _userService.GetByFacebookUserId(facebookUserId);

            if (user == null)
            {
                return NotFound();
            }

            return user;
        }

        [HttpPost]
        public async Task<ActionResult<UserModel>> Post(CreateUserModel model)
        {
            _userService.CreateUser(new UserModel{FacebookUserId = model.FacebookUserId});

            return _userService.GetByFacebookUserId(model.FacebookUserId);
        }
    }
}
