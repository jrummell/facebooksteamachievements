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
using NUnit.Framework;
using NUnit.Mocks;
using SteamAchievements.Data;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class AutoUpdateManagerFixture
    {
        private AutoUpdateManager _manager;
        private DynamicMock _achievementServiceMock;
        private DynamicMock _loggerMock;
        private DynamicMock _achievementManagerMock;
        private DynamicMock _publisherMock;

        [SetUp]
        public void SetUp()
        {
            _achievementServiceMock = new DynamicMock(typeof(IAchievementService));
            _achievementManagerMock = new DynamicMock(typeof(IAchievementManager));
            _loggerMock = new DynamicMock(typeof(IAutoUpdateLogger));
            _publisherMock = new DynamicMock(typeof(IFacebookPublisher));

            IAchievementService achievementService = (IAchievementService)_achievementServiceMock.MockInstance;
            IAchievementManager achievementManager = (IAchievementManager)_achievementManagerMock.MockInstance;
            IAutoUpdateLogger logger = (IAutoUpdateLogger)_loggerMock.MockInstance;
            IFacebookPublisher publisher = (IFacebookPublisher)_publisherMock.MockInstance;

            _manager = new AutoUpdateManager(achievementService, achievementManager, publisher, logger);
        }

        [Test]
        public void GetAutoUpdateUsers()
        {
            User[] users = new[]{new User{SteamUserId = "user1"}, new User{SteamUserId = "user2"}};
        	 _achievementManagerMock.ExpectAndReturn("GetAutoUpdateUsers", users);

            string userIds = _manager.GetAutoUpdateUsers();

            Assert.That(userIds, Is.Not.Null.Or.Empty);
            Assert.That(userIds, Is.EquivalentTo("user1;user2"));

            _achievementManagerMock.Verify();
        }
        
        [Test]
        public void PublishUserAchievements()
        {
            User user = new User{SteamUserId = "user1", FacebookUserId = 1, AccessToken = "token", AutoUpdate = true};
        	 _achievementManagerMock.ExpectAndReturn("GetUser", user, "user1");
        	 _achievementServiceMock.ExpectAndReturn("UpdateAchievements", 1, "user1");
        	 _achievementServiceMock.ExpectAndReturn("GetUnpublishedAchievements", new List<SimpleAchievement> {new SimpleAchievement { Id = 1, Name = "achievement 1", Description = "x", ImageUrl = "http://example.com/a.jpg", Game = new SimpleGame{ Id = 1, Name = "game 1", ImageUrl = "http://example.com/g.jpg", StatsUrl = "http://example.com/stats", StoreUrl = "http://example.com/store" } }}, "user1", DateTime.UtcNow.AddDays(-2).Date);
        	 _publisherMock.Expect("Publish");
        	 _achievementServiceMock.ExpectAndReturn("PublishAchievements", true, "user1", new List<int> { 1 });
        	
        	 _manager.PublishUserAchievements(user.SteamUserId);
        	 
        	 _achievementServiceMock.Verify();
        	 _publisherMock.Verify();
        	 _achievementManagerMock.Verify();
        }
    }
}
