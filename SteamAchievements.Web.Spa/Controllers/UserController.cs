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
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SteamAchievements.Data;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Web.Spa.Controllers
{
    /// <summary>
    ///     Manages users
    /// </summary>
    /// <seealso cref="SteamAchievements.Web.Spa.Controllers.ApiController" />
    public class UserController : ApiController
    {
        private readonly IMapper _mapper;
        private readonly UserManager<User> _userManager;
        private readonly IUserService _userService;

        /// <summary>
        ///     Initializes a new instance of the <see cref="UserController" /> class.
        /// </summary>
        /// <param name="userManager">The user manager.</param>
        /// <param name="userService">The user service.</param>
        /// <param name="mapper">The mapper.</param>
        public UserController(UserManager<User> userManager, IUserService userService, IMapper mapper)
        {
            _userManager = userManager;
            _userService = userService;
            _mapper = mapper;
        }

        /// <summary>
        ///     Gets the user by facebook user identifier.
        /// </summary>
        /// <param name="facebookUserId">The facebook user identifier.</param>
        /// <returns></returns>
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

        /// <summary>
        ///     Creates a user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<UserModel>> Post(CreateUserModel model)
        {
            var user = new User {UserName = model.FacebookUserId.ToString(), FacebookUserId = model.FacebookUserId};
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
            {
                return InvalidRequest(result);
            }

            return await GetById(user.Id);
        }

        /// <summary>
        ///     Updates a user.
        /// </summary>
        /// <param name="model">The model.</param>
        /// <returns></returns>
        [HttpPut]
        public async Task<ActionResult<UserModel>> Put(UserModel model)
        {
            var user = await _userManager.FindByIdAsync(model.Id);

            user.FacebookUserId = model.FacebookUserId;
            user.PublishDescription = model.PublishDescription;
            user.SteamUserId = model.SteamUserId;

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return InvalidRequest(result);
            }

            return await GetById(user.Id);
        }

        private ActionResult<UserModel> InvalidRequest(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return BadRequest(ModelState);
        }

        private async Task<UserModel> GetById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);

            return _mapper.Map<UserModel>(user);
        }
    }
}
