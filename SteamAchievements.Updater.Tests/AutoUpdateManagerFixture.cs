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
using Moq;
using NUnit.Framework;
using SteamAchievements.Services;
using SteamAchievements.Services.Models;

namespace SteamAchievements.Updater.Tests
{
    [TestFixture]
    public class AutoUpdateManagerFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _achievementServiceMock = new Mock<IAchievementService>();
            _userServiceMock = new Mock<IUserService>();
            _loggerMock = new Mock<IAutoUpdateLogger>();
            _publisherMock = new Mock<IFacebookPublisher>();

            IAchievementService achievementService = _achievementServiceMock.Object;
            IUserService userService = _userServiceMock.Object;
            IAutoUpdateLogger logger = _loggerMock.Object;
            IFacebookPublisher publisher = _publisherMock.Object;

            _manager = new AutoUpdateManager(achievementService, userService, publisher, logger);
        }

        #endregion

        private AutoUpdateManager _manager;
        private Mock<IAchievementService> _achievementServiceMock;
        private Mock<IAutoUpdateLogger> _loggerMock;
        private Mock<IUserService> _userServiceMock;
        private Mock<IFacebookPublisher> _publisherMock;

        [Test]
        public void GetAutoUpdateUsers()
        {
            User[] expectedUsers = new[] {new User {SteamUserId = "user1"}, new User {SteamUserId = "user2"}};
            _userServiceMock.Setup(service => service.GetAutoUpdateUsers())
                .Returns(expectedUsers)
                .Verifiable();

            ICollection<User> users = _manager.GetAutoUpdateUsers();

            Assert.That(users, Is.Not.Null.Or.Empty);
            Assert.That(users.Count, Is.EqualTo(2));

            _userServiceMock.Verify();
        }

        [Test]
        public void PublishUserAchievements()
        {
            User user = new User {SteamUserId = "user1", FacebookUserId = 1, AccessToken = "token", AutoUpdate = true};

            _achievementServiceMock.Setup(service => service.UpdateAchievements(user.FacebookUserId, null))
                .Returns(1)
                .Verifiable();
            _achievementServiceMock.Setup(
                service => service.GetUnpublishedAchievements(user.FacebookUserId, DateTime.UtcNow.AddDays(-2).Date, null))
                .Returns(
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
                        })
                .Verifiable();

            _publisherMock.Setup(pub => pub.Publish(It.IsAny<User>(), It.IsAny<IDictionary<string, object>>()))
                .Verifiable();

            _achievementServiceMock.Setup(service => service.PublishAchievements(user.FacebookUserId, new List<int> {1}))
                .Returns(true)
                .Verifiable();

            _manager.PublishUserAchievements(user);

            _achievementServiceMock.Verify();
            _publisherMock.Verify();
        }
    }
}