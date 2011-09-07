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
using System.Data.SqlTypes;
using System.Linq;
using Moq;
using NUnit.Framework;

namespace SteamAchievements.Data.Tests
{
    [TestFixture]
    public class AchievementManagerFixture
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            _achievements = (new[]
                                 {
                                     new Achievement
                                         {Id = 1, GameId = 1, Name = "Achievement 1 for Game 1", Description = "", ApiName = "1"},
                                     new Achievement
                                         {Id = 2, GameId = 1, Name = "Achievement 2 for Game 1", Description = "", ApiName = "2"},
                                     new Achievement
                                         {Id = 3, GameId = 1, Name = "Achievement 3 for Game 1", Description = "", ApiName = "3"},
                                     new Achievement
                                         {Id = 4, GameId = 2, Name = "Achievement 1 for Game 2", Description = "", ApiName = "4"},
                                     new Achievement
                                         {Id = 5, GameId = 2, Name = "Achievement 2 for Game 2", Description = "", ApiName = "5"}
                                 }).AsQueryable();

            _users = (new[]
                          {
                              new User {FacebookUserId = 1, SteamUserId = "user1"},
                              new User {FacebookUserId = 2, SteamUserId = "user2"}
                          }).AsQueryable();

            _userAchievements = (new[]
                                     {
                                         new UserAchievement
                                             {
                                                 Id = 1,
                                                 AchievementId = 1,
                                                 Date = DateTime.Now,
                                                 FacebookUserId = 1,
                                                 Achievement = _achievements.Single(achievement => achievement.Id == 1),
                                                 Published = true,
                                                 Hidden = false
                                             },
                                         new UserAchievement
                                             {
                                                 Id = 2,
                                                 AchievementId = 2,
                                                 Date = DateTime.Now,
                                                 FacebookUserId = 1,
                                                 Achievement = _achievements.Single(achievement => achievement.Id == 2),
                                                 Published = true,
                                                 Hidden = false
                                             },
                                         new UserAchievement
                                             {
                                                 Id = 3,
                                                 AchievementId = 3,
                                                 Date = DateTime.Now,
                                                 FacebookUserId = 1,
                                                 Achievement = _achievements.Single(achievement => achievement.Id == 3),
                                                 Published = false,
                                                 Hidden = false
                                             }
                                     }).AsQueryable();

            _repositoryMock = new Mock<ISteamRepository>();
            _manager = new AchievementManager(_repositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _manager.Dispose();
        }

        #endregion

        private AchievementManager _manager;
        private IQueryable<Achievement> _achievements;
        private IQueryable<User> _users;
        private IQueryable<UserAchievement> _userAchievements;
        private Mock<ISteamRepository> _repositoryMock;

        [Test]
        public void AddGameAchievements()
        {
            const int gameId = 99;
            List<Achievement> gameAchievements = new List<Achievement>();
            for (int i = 0; i < 10; i++)
            {
                Achievement achievement = new Achievement
                                              {
                                                  Name = "Achievement " + i,
                                                  Description = "Achievement " + i,
                                                  ImageUrl = "http://example.com/achievement" + i + ".gif",
                                                  ApiName = i.ToString()
                                              };
                gameAchievements.Add(achievement);
            }

            _repositoryMock.SetupProperty(rep => rep.Achievements, _achievements);

            _manager.AddAchievements(99, gameAchievements);

            _repositoryMock.VerifyGet(rep => rep.Achievements);
            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<Achievement>()),
                                   Times.Exactly(gameAchievements.Count));
            _repositoryMock.Verify(rep => rep.SubmitChanges());

            List<Achievement> expectedAchievements = new List<Achievement>(_achievements);
            expectedAchievements.AddRange(gameAchievements);
            _repositoryMock.SetupProperty(rep => rep.Achievements, expectedAchievements.AsQueryable());

            Assert.That(_repositoryMock.Object.Achievements.Count(a => a.GameId == gameId), Is.EqualTo(10));
        }

        [Test]
        public void AssignAchievements()
        {
            _repositoryMock.SetupProperty(rep => rep.Achievements, _achievements);
            _repositoryMock.SetupProperty(rep => rep.UserAchievements, _userAchievements);

            const long facebookUserId = 1;
            IEnumerable<Achievement> achievements =
                _manager.GetUnassignedAchievements(facebookUserId, _achievements);
            Assert.That(achievements.Any());

            _repositoryMock.VerifyGet(rep => rep.Achievements);
            _repositoryMock.VerifyGet(rep => rep.UserAchievements);

            IEnumerable<UserAchievement> userAchievements =
                from achievement in achievements
                select new UserAchievement
                           {
                               FacebookUserId = facebookUserId,
                               Achievement = achievement
                           };

            _manager.AssignAchievements(userAchievements);

            _repositoryMock.Verify(rep => rep.InsertAllOnSubmit(It.IsAny<IEnumerable<UserAchievement>>()));
            _repositoryMock.Verify(rep => rep.SubmitChanges());
        }

        [Test]
        public void GetMissingAchievements()
        {
            Achievement achievement1Game5 = new Achievement {GameId = 5, Name = "Achievement 1 for Game 5", ApiName = "1"};
            Achievement achievement2Game5 = new Achievement {GameId = 5, Name = "Achievement 2 for Game 5", ApiName = "2"};
            ICollection<Achievement> communityAchievements =
                new[]
                    {
                        new Achievement {GameId = 1, Name = "Achievement 1 for Game 1", ApiName = "1"},
                        new Achievement {GameId = 1, Name = "Achievement 2 for Game 1", ApiName = "2"},
                        new Achievement {GameId = 1, Name = "Achievement 3 for Game 1", ApiName = "3"},
                        achievement1Game5,
                        achievement2Game5
                    };

            _repositoryMock.SetupProperty(rep => rep.Achievements, _achievements);
            ICollection<Achievement> missingAchievements =
                _manager.GetMissingAchievements(communityAchievements.ToList());

            _repositoryMock.VerifyGet(rep => rep.Achievements);

            Assert.That(missingAchievements.Count(), Is.EqualTo(2));

            CollectionAssert.Contains(missingAchievements, achievement1Game5);
            CollectionAssert.Contains(missingAchievements, achievement2Game5);
        }

        [Test]
        public void GetUnassignedAchievements()
        {
            _repositoryMock.SetupProperty(rep => rep.Achievements, _achievements);
            _repositoryMock.SetupProperty(rep => rep.UserAchievements, _userAchievements);

            ICollection<Achievement> achievements = _manager.GetUnassignedAchievements(1, _achievements);

            _repositoryMock.VerifyGet(rep => rep.UserAchievements);
            _repositoryMock.VerifyGet(rep => rep.Achievements);

            Assert.That(achievements.Count, Is.EqualTo(2));
            Assert.That(achievements.Count(a => a.Id == 4), Is.EqualTo(1));
            Assert.That(achievements.Count(a => a.Id == 5), Is.EqualTo(1));
        }

        [Test]
        public void GetUnpublishedAchievements()
        {
            _repositoryMock.SetupProperty(rep => rep.UserAchievements, _userAchievements);
            _repositoryMock.SetupProperty(rep => rep.Users, _users);

            ICollection<Achievement> achievements = _manager.GetUnpublishedAchievements("user1");

            _repositoryMock.VerifyGet(rep => rep.UserAchievements);
            _repositoryMock.VerifyGet(rep => rep.Users);

            Assert.That(achievements.Count, Is.EqualTo(1));
            Assert.That(achievements.Count(a => a.Id == 3), Is.EqualTo(1));
        }

        [Test]
        public void InsertMissingAchievements()
        {
            _repositoryMock.SetupProperty(rep => rep.Achievements, _achievements);
            int maxId = _achievements.Select(a => a.Id).Max();

            Achievement achievement1Game5 =
                new Achievement
                    {
                        Id = ++maxId,
                        GameId = 5,
                        Name = "Achievement 1 for Game 5",
                        Description = "",
                        ImageUrl = "",
                        ApiName = "1"
                    };
            Achievement achievement2Game5 =
                new Achievement
                    {
                        Id = ++maxId,
                        GameId = 5,
                        Name = "Achievement 2 for Game 5",
                        Description = "",
                        ImageUrl = "",
                        ApiName = "2"
                    };

            Achievement[] missingAchievements = new[] {achievement1Game5, achievement2Game5};

            _manager.InsertMissingAchievements(missingAchievements);

            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<Achievement>()),
                                   Times.Exactly(missingAchievements.Length));
            _repositoryMock.Verify(rep => rep.SubmitChanges());
        }

        [Test]
        public void UpdateAchievements()
        {
            long facebookUserId = _users.First().FacebookUserId;

            ICollection<UserAchievement> achievements =
                new[]
                    {
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 1,
                                Achievement = new Achievement
                                                  {
                                                      Id = 1,
                                                      GameId = 1,
                                                      Name = "Achievement 1 for Game 1",
                                                      Description = "",
                                                      ApiName = "1"
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 2,
                                Achievement = new Achievement
                                                  {
                                                      Id = 2,
                                                      GameId = 1,
                                                      Name = "Achievement 2 for Game 1",
                                                      Description = "",
                                                      ApiName = "2"
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 3,
                                Achievement = new Achievement
                                                  {
                                                      Id = 3,
                                                      GameId = 1,
                                                      Name = "Achievement 3 for Game 1",
                                                      Description = "",
                                                      ApiName = "3"
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 4,
                                Achievement = new Achievement
                                                  {
                                                      Id = 4,
                                                      GameId = 2,
                                                      Name = "Achievement 1 for Game 2",
                                                      Description = "",
                                                      ApiName = "4"
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 5,
                                Achievement = new Achievement
                                                  {
                                                      Id = 5,
                                                      GameId = 2,
                                                      Name = "Achievement 2 for Game 2",
                                                      Description = "",
                                                      ApiName = "5"
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 6,
                                Achievement = new Achievement
                                                  {
                                                      Id = 6,
                                                      GameId = 3,
                                                      Name = "Achievement for Game",
                                                      Description = "Game 3 Achievement 1",
                                                      ApiName = "6"
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 7,
                                Achievement = new Achievement
                                                  {
                                                      Id = 7,
                                                      GameId = 3,
                                                      Name = "Achievement for Game",
                                                      Description = "Game 3 Achievement 2",
                                                      ApiName = "7"
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 8,
                                Achievement = new Achievement
                                                  {
                                                      Id = 8,
                                                      GameId = 4,
                                                      Name = "Achievement for Game",
                                                      Description = "Game 4 Achievement 1",
                                                      ApiName = "8"
                                                  }
                            }
                    };

            _repositoryMock.SetupSequence(rep => rep.Achievements)
                .Returns(new Achievement[0].AsQueryable())
                .Returns(achievements.Select(a => a.Achievement).AsQueryable());
            _repositoryMock.SetupProperty(rep => rep.UserAchievements, new UserAchievement[0].AsQueryable());

            _manager.UpdateAchievements(achievements);

            _repositoryMock.VerifyGet(rep => rep.Achievements);
            _repositoryMock.VerifyGet(rep => rep.UserAchievements);
            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<Achievement>()), Times.Exactly(achievements.Count));
            _repositoryMock.Verify(rep => rep.InsertAllOnSubmit(It.IsAny<IEnumerable<UserAchievement>>()));
            _repositoryMock.Verify(rep => rep.SubmitChanges(), Times.Exactly(2));
        }

        [Test]
        public void UpdateHidden()
        {
            _repositoryMock.SetupProperty(rep => rep.Users, _users);
            _repositoryMock.SetupProperty(rep => rep.UserAchievements, _userAchievements);
            _repositoryMock.SetupProperty(rep => rep.Achievements, _achievements);

            const string steamUserId = "user1";
            ICollection<Achievement> achievements = _manager.GetUnpublishedAchievements(steamUserId);
            _manager.UpdateHidden(steamUserId, achievements.Select(acheivement => acheivement.Id));

            IEnumerable<int> achievementIds = from achievement in achievements
                                              select achievement.Id;
            long facebookUserId =
                _repositoryMock.Object.Users.Where(user => user.SteamUserId == steamUserId).Select(
                    user => user.FacebookUserId).
                    Single();
            IQueryable<UserAchievement> userAchievements =
                from achievement in _repositoryMock.Object.UserAchievements
                where achievement.FacebookUserId == facebookUserId && achievementIds.Contains(achievement.AchievementId)
                select achievement;

            Assert.That(userAchievements.Any(achievement => !achievement.Hidden), Is.False);
        }

        [Test]
        public void UpdatePublished()
        {
            _repositoryMock.SetupProperty(rep => rep.Users, _users);
            _repositoryMock.SetupProperty(rep => rep.UserAchievements, _userAchievements);
            _repositoryMock.SetupProperty(rep => rep.Achievements, _achievements);

            const string steamUserId = "user1";
            ICollection<Achievement> achievements = _manager.GetUnpublishedAchievements(steamUserId);
            _manager.UpdatePublished(steamUserId, achievements.Select(acheivement => acheivement.Id));

            IEnumerable<int> achievementIds = from achievement in achievements
                                              select achievement.Id;
            long facebookUserId =
                _repositoryMock.Object.Users.Where(user => user.SteamUserId == steamUserId).Select(
                    user => user.FacebookUserId).
                    Single();
            IQueryable<UserAchievement> userAchievements =
                from achievement in _repositoryMock.Object.UserAchievements
                where achievement.FacebookUserId == facebookUserId && achievementIds.Contains(achievement.AchievementId)
                select achievement;

            Assert.That(userAchievements.Any(achievement => !achievement.Published), Is.False);
        }

        [Test]
        public void UpdateUser()
        {
            _repositoryMock.SetupProperty(rep => rep.Users, _users);
            _repositoryMock.Setup(rep => rep.SubmitChanges());

            const string steamUserId = "userxxx";
            const int facebookUserId = 1;

            User user = new User {SteamUserId = steamUserId, FacebookUserId = facebookUserId};
            _manager.UpdateUser(user);

            _repositoryMock.VerifyGet(rep => rep.Users, Times.Exactly(2));
            _repositoryMock.Verify(rep => rep.SubmitChanges());
            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<User>()), Times.Never());
        }

        [Test]
        public void ValidateDate()
        {
            DateTime date = AchievementManager.ValidateDate(DateTime.MinValue);

            long sqlMinTicks = SqlDateTime.MinValue.Value.Ticks;
            Assert.That(date.Ticks, Is.GreaterThan(sqlMinTicks));
        }
    }
}