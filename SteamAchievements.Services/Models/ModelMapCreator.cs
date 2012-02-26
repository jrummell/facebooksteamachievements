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
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using AutoMapper;
using SteamAchievements.Data;

namespace SteamAchievements.Services.Models
{
	public class ModelMapCreator
	{
		public void CreateMappings()
		{
			// User
			Mapper.CreateMap<Data.User, Models.User>();
			
			Mapper.CreateMap<Models.User, Data.User>()
				.ForMember(entity => entity.UserAchievements, options => options.Ignore())
				.ForMember(entity => entity.AccessToken, 
				           options => options.MapFrom(model => model.AccessToken ?? String.Empty));
			
			// UserAchievement
			Mapper.CreateMap<Models.UserAchievement, Data.Achievement>()
				.ForMember(entity => entity.Id, options => options.MapFrom(model => model.Achievement.Id))
				.ForMember(entity => entity.UserAchievements, options => options.Ignore())
				.ForMember(entity => entity.ApiName, options => options.MapFrom(model => model.Achievement.ApiName))
				.ForMember(entity => entity.GameId, options => options.MapFrom(model => model.Achievement.Game.Id))
				.ForMember(entity => entity.ImageUrl, options => options.MapFrom(model => model.Achievement.ImageUrl))
				.ForMember(entity => entity.AchievementNames,
				           options => options.MapFrom(model => 
				                                      new EntitySet<Data.AchievementName>
				                                      {
				                                      	new AchievementName
				                                      	{
				                                      		AchievementId = model.Achievement.Id,
				                                      		Language = model.Achievement.Language,
				                                      		Name = model.Achievement.Name,
				                                      		Description = model.Achievement.Description
				                                      	}
				                                      }));
			
			// Achievement
			string language = CultureHelper.GetLanguage();
			Mapper.CreateMap<Data.Achievement, Models.Achievement>()
				.ForMember(model => model.Name, options => options.Ignore())
				.ForMember(model => model.Description, options => options.Ignore())
				.ForMember(model => model.Game, options => options.Ignore())
				.ForMember(model => model.Language, options => options.MapFrom(entity => language))
				.AfterMap((entity, model) => 
				          {
				          	AchievementName name = entity.AchievementNames
				          		.Where(n => n.Language == language)
                                .SingleOrDefault() ?? entity.AchievementNames.FirstOrDefault();
				          	if (name != null)
				          	{
				          		model.Name = name.Name;
				          		model.Description = name.Description;
				          	}
				          });
		}
	}
}
