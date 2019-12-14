#region License

// Copyright 2010 John Rummell
// 
// This file is part of SteamAchievements.
// 
//     SteamAchievements is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     SteamAchievements is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with SteamAchievements.  If not, see <http://www.gnu.org/licenses/>.

#endregion

using System;
using System.Linq;
using AutoMapper;
using SteamAchievements.Data;
using SteamAchievements.Services.Models;
using UserAchievement = SteamAchievements.Data.UserAchievement;

namespace SteamAchievements.Services
{
    public class UserService : Disposable, IUserService
    {
        private readonly IMapper _mapper;
        private readonly ISteamRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserService" /> class.
        /// </summary>
        /// <param name="mapper">The mapper.</param>
        /// <param name="repository">The repository.</param>
        /// <exception cref="ArgumentNullException">manager</exception>
        public UserService(IMapper mapper, ISteamRepository repository)
        {
            _mapper = mapper;
            _repository = repository;
        }

        #region IUserService Members

        /// <inheritdoc />
        public UserModel GetUser(string userId)
        {
            Data.User user = _repository.Users.SingleOrDefault(e => e.Id == userId);

            return Map(user);
        }

        /// <inheritdoc />
        public UserModel GetByFacebookUserId(long facebookUserId)
        {
            var user = _repository.Users.SingleOrDefault(e => e.FacebookUserId == facebookUserId);;

            return Map(user);
        }

        /// <inheritdoc />
        public void ChangeSteamUserId(string userId, string steamUserId)
        {
            User existingUser = _repository.Users.Single(u => u.Id == userId);
            
            // if the user changed steam IDs, remove their achievements
            if (!string.Equals(existingUser.SteamUserId, steamUserId, StringComparison.CurrentCultureIgnoreCase))
            {
                var existingAchievements = _repository.UserAchievements
                                                      .Where(ua => ua.UserId == userId)
                                                      .ToArray();
                if (existingAchievements.Length > 0)
                {
                    _repository.DeleteAllOnSubmit(existingAchievements);
                }
            }
            
            existingUser.SteamUserId = steamUserId;

            _repository.SubmitChanges();
        }

        /// <inheritdoc />
        public void DeauthorizeUser(string userId)
        {
            User user = _repository.Users.SingleOrDefault(u => u.Id == userId);
            if (user == null)
            {
                return;
            }

            IQueryable<UserAchievement> userAchievements =
                _repository.UserAchievements.Where(ua => ua.UserId == userId);
            _repository.DeleteAllOnSubmit(userAchievements);
            _repository.SubmitChanges();

            _repository.DeleteOnSubmit(user);
            _repository.SubmitChanges();
        }

        #endregion

        protected override void DisposeManaged()
        {
            _repository.Dispose();
        }

        /// <summary>
        /// Maps the specified user.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <returns></returns>
        private Models.UserModel Map(Data.User user)
        {
            if (user == null)
            {
                return null;
            }

            return _mapper.Map<Models.UserModel>(user);
        }
    }
}