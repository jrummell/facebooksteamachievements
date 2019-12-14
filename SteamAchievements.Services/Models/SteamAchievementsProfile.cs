#region License

//  Copyright 2015 John Rummell
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
using AutoMapper;
using SteamAchievements.Data;

namespace SteamAchievements.Services.Models
{
    public class SteamAchievementsProfile : Profile
    {
        public SteamAchievementsProfile()
        {
            // User
            CreateMap<User, UserModel>(MemberList.Destination);

            CreateMap<UserModel, User>(MemberList.Source);

            // UserAchievement
            CreateMap<UserAchievement, Data.UserAchievement>()
               .ForMember(entity => entity.User, options => options.Ignore())
               .ForMember(entity => entity.Id, options => options.Ignore())
               .ForMember(entity => entity.Hidden, options => options.Ignore())
               .ForMember(entity => entity.Published, options => options.Ignore());

            // Achievement
            CreateMap<AchievementModel, Achievement>()
               .ForMember(entity => entity.UserAchievements, options => options.Ignore())
               .ForMember(entity => entity.AchievementNames,
                          options => options.MapFrom(model =>
                                                         new List<AchievementName>
                                                         {
                                                             new AchievementName
                                                             {
                                                                 AchievementId = model.Id,
                                                                 Language = model.Language,
                                                                 Name = model.Name,
                                                                 Description = model.Description
                                                             }
                                                         }));

            var language = CultureHelper.GetLanguage();
            CreateMap<Achievement, AchievementModel>()
               .ForMember(model => model.Name, options => options.Ignore())
               .ForMember(model => model.Description, options => options.Ignore())
               .ForMember(model => model.Game, options => options.Ignore())
               .ForMember(model => model.Language, options => options.MapFrom(entity => language))
               .AfterMap((entity, model) =>
                         {
                             var name = entity.AchievementNames
                                              .Where(n => n.Language == language)
                                              .SingleOrDefault() ??
                                        entity.AchievementNames.FirstOrDefault();
                             if (name != null)
                             {
                                 model.Name = name.Name;
                                 model.Description = name.Description;
                             }
                         });
        }
    }
}