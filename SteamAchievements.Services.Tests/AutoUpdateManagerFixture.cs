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

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class AutoUpdateManagerFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _achievementServiceMock = new DynamicMock(typeof (IAchievementService));
            _userServiceMock = new DynamicMock(typeof (IUserService));
            _loggerMock = new DynamicMock(typeof (IAutoUpdateLogger));
            _publisherMock = new DynamicMock(typeof (IFacebookPublisher));

            IAchievementService achievementService = (IAchievementService) _achievementServiceMock.MockInstance;
            IUserService userService = (IUserService) _userServiceMock.MockInstance;
            IAutoUpdateLogger logger = (IAutoUpdateLogger) _loggerMock.MockInstance;
            IFacebookPublisher publisher = (IFacebookPublisher) _publisherMock.MockInstance;

            _manager = new AutoUpdateManager(achievementService, userService, publisher, logger);
        }

        #endregion

        private AutoUpdateManager _manager;
        private DynamicMock _achievementServiceMock;
        private DynamicMock _loggerMock;
        private DynamicMock _userServiceMock;
        private DynamicMock _publisherMock;

        [Test]
        public void GetAutoUpdateUsers()
        {
            string[] users = new[] {"user1", "user2"};
            _userServiceMock.ExpectAndReturn("GetAutoUpdateUsers", users);

            IEnumerable<string> userIds = _manager.GetAutoUpdateUsers();

            Assert.That(userIds, Is.Not.Null.Or.Empty);
            Assert.That(String.Join(", ", userIds), Is.EquivalentTo("user1, user2"));

            _userServiceMock.Verify();
        }

        [Test]
        public void PublishUserAchievements()
        {
            User user = new User {SteamUserId = "user1", FacebookUserId = 1, AccessToken = "token", AutoUpdate = true};
            _userServiceMock.ExpectAndReturn("GetUser", user, "user1");
            _achievementServiceMock.ExpectAndReturn("UpdateAchievements", 1, "user1");
            _achievementServiceMock.ExpectAndReturn("GetUnpublishedAchievements",
                                                    new List<Achievement>
                                                        {
                                                            new Achievement
                                                                {
                                                                    Id = 1,
                                                                    Name = "achievement 1",
                                                                    Description = "x",
                                                                    ImageUrl = "http://example.com/a.jpg",
                                                                    Game =
                                                                        new Game
                                                                            {
                                                                                Id = 1,
                                                                                Name = "game 1",
                                                                                ImageUrl = "http://example.com/g.jpg",
                                                                                StatsUrl = "http://example.com/stats",
                                                                                StoreUrl = "http://example.com/store"
                                                                            }
                                                                }
                                                        },
                                                    "user1", DateTime.UtcNow.AddDays(-2).Date);
            _publisherMock.Expect("Publish");
            _achievementServiceMock.ExpectAndReturn("PublishAchievements", true, "user1", new List<int> {1});

            _manager.PublishUserAchievements(user.SteamUserId);

            _achievementServiceMock.Verify();
            _publisherMock.Verify();
            _userServiceMock.Verify();
        }
    }
}