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
    public interface IMockSteamRepository : ISteamRepository
    {
        new IQueryable<Achievement> Achievements { get; set; }

        new IQueryable<UserAchievement> UserAchievements { get; set; }

        new IQueryable<User> Users { get; set; }
    }

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
                                         {Id = 1, GameId = 1, Name = "Achievement 1 for Game 1", Description = ""},
                                     new Achievement
                                         {Id = 2, GameId = 1, Name = "Achievement 2 for Game 1", Description = ""},
                                     new Achievement
                                         {Id = 3, GameId = 1, Name = "Achievement 3 for Game 1", Description = ""},
                                     new Achievement
                                         {Id = 4, GameId = 2, Name = "Achievement 1 for Game 2", Description = ""},
                                     new Achievement
                                         {Id = 5, GameId = 2, Name = "Achievement 2 for Game 2", Description = ""}
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
                                                 Published = true
                                             },
                                         new UserAchievement
                                             {
                                                 Id = 2,
                                                 AchievementId = 2,
                                                 Date = DateTime.Now,
                                                 FacebookUserId = 1,
                                                 Achievement = _achievements.Single(achievement => achievement.Id == 2),
                                                 Published = true
                                             },
                                         new UserAchievement
                                             {
                                                 Id = 3,
                                                 AchievementId = 3,
                                                 Date = DateTime.Now,
                                                 FacebookUserId = 1,
                                                 Achievement = _achievements.Single(achievement => achievement.Id == 3),
                                                 Published = false
                                             }
                                     }).AsQueryable();

            _repositoryMock = new Mock<IMockSteamRepository>();
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
        private Mock<IMockSteamRepository> _repositoryMock;

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
                                                  ImageUrl = "http://example.com/achievement" + i + ".gif"
                                              };
                gameAchievements.Add(achievement);
            }

            foreach (Achievement achievement in gameAchievements)
            {
                _repositoryMock.SetupProperty(rep => rep.Achievements, _achievements);
                _repositoryMock.Setup(rep => rep.InsertOnSubmit(achievement));
            }

            _repositoryMock.Setup(rep => rep.SubmitChanges());

            _manager.AddAchievements(99, gameAchievements);

            _repositoryMock.Verify();

            List<Achievement> expectedAchievements = new List<Achievement>(_achievements);
            expectedAchievements.AddRange(gameAchievements);
            _repositoryMock.SetupProperty(rep => rep.Achievements, expectedAchievements.AsQueryable());

            Assert.That(_repositoryMock.Object.Achievements.Count(a => a.GameId == gameId), Is.EqualTo(10));

            _repositoryMock.Verify();
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

            _repositoryMock.Verify();

            IEnumerable<UserAchievement> userAchievements =
                from achievement in achievements
                select new UserAchievement
                           {
                               FacebookUserId = facebookUserId,
                               Achievement = achievement
                           };

            _repositoryMock.Setup(rep => rep.InsertAllOnSubmit(userAchievements));
            _repositoryMock.Setup(rep => rep.SubmitChanges());

            _manager.AssignAchievements(userAchievements);

            _repositoryMock.Verify();

            foreach (Achievement achievement in achievements)
            {
                _repositoryMock.SetupProperty(rep => rep.UserAchievements, _userAchievements);

                int achievementId = achievement.Id;
                int count =
                    _repositoryMock.Object.UserAchievements.Count(
                        ua => ua.FacebookUserId == facebookUserId && ua.AchievementId == achievementId);
                Assert.That(count, Is.EqualTo(1));
            }

            _repositoryMock.Verify();
        }

        [Test]
        public void GetMissingAchievements()
        {
            Achievement achievement1Game5 = new Achievement {GameId = 5, Name = "Achievement 1 for Game 5"};
            Achievement achievement2Game5 = new Achievement {GameId = 5, Name = "Achievement 2 for Game 5"};
            ICollection<Achievement> communityAchievements =
                new[]
                    {
                        new Achievement {GameId = 1, Name = "Achievement 1 for Game 1"},
                        new Achievement {GameId = 1, Name = "Achievement 2 for Game 1"},
                        new Achievement {GameId = 1, Name = "Achievement 3 for Game 1"},
                        achievement1Game5,
                        achievement2Game5
                    };

            _repositoryMock.SetupProperty(rep => rep.Achievements, _achievements);
            ICollection<Achievement> missingAchievements =
                _manager.GetMissingAchievements(communityAchievements.ToList());

            _repositoryMock.Verify();

            Assert.That(missingAchievements.Count(), Is.EqualTo(2));

            CollectionAssert.Contains(missingAchievements, achievement1Game5);
            CollectionAssert.Contains(missingAchievements, achievement2Game5);
        }

        [Test]
        public void GetUnassignedAchievements()
        {
            _repositoryMock.SetupProperty(rep => rep.Achievements, _achievements);
            _repositoryMock.SetupProperty(rep => rep.UserAchievements, _userAchievements);

            IEnumerable<Achievement> achievements = _manager.GetUnassignedAchievements(1, _achievements);

            _repositoryMock.Verify();

            Assert.That(achievements.Count(), Is.EqualTo(2));
            Assert.That(achievements.Count(a => a.Id == 4), Is.EqualTo(1));
            Assert.That(achievements.Count(a => a.Id == 5), Is.EqualTo(1));
        }

        [Test]
        public void GetUnpublishedAchievements()
        {
            _repositoryMock.SetupProperty(rep => rep.UserAchievements, _userAchievements);
            _repositoryMock.SetupProperty(rep => rep.Users, _users);

            ICollection<Achievement> achievements = _manager.GetUnpublishedAchievements("user1");

            _repositoryMock.Verify();

            Assert.That(achievements.Count(), Is.EqualTo(1));
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
                        ImageUrl = ""
                    };
            Achievement achievement2Game5 =
                new Achievement
                    {
                        Id = ++maxId,
                        GameId = 5,
                        Name = "Achievement 2 for Game 5",
                        Description = "",
                        ImageUrl = ""
                    };

            Achievement[] missingAchievements = new[] {achievement1Game5, achievement2Game5};

            foreach (Achievement achievement in missingAchievements)
            {
                _repositoryMock.Setup(rep => rep.InsertOnSubmit(achievement));
            }
            _repositoryMock.Setup(rep => rep.SubmitChanges());

            _manager.InsertMissingAchievements(missingAchievements);

            _repositoryMock.Verify();
        }

        [Test]
        public void UpdateAchievements()
        {
            const long facebookUserId = 1;

            ICollection<UserAchievement> achievements =
                new[]
                    {
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                Achievement = new Achievement
                                                  {
                                                      Id = 1,
                                                      GameId = 1,
                                                      Name = "Achievement 1 for Game 1",
                                                      Description = ""
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                Achievement = new Achievement
                                                  {
                                                      Id = 2,
                                                      GameId = 1,
                                                      Name = "Achievement 2 for Game 1",
                                                      Description = ""
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                Achievement = new Achievement
                                                  {
                                                      Id = 3,
                                                      GameId = 1,
                                                      Name = "Achievement 3 for Game 1",
                                                      Description = ""
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                Achievement = new Achievement
                                                  {
                                                      Id = 4,
                                                      GameId = 2,
                                                      Name = "Achievement 1 for Game 2",
                                                      Description = ""
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                Achievement = new Achievement
                                                  {
                                                      Id = 5,
                                                      GameId = 2,
                                                      Name = "Achievement 2 for Game 2",
                                                      Description = ""
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                Achievement = new Achievement
                                                  {
                                                      Id = 6,
                                                      GameId = 3,
                                                      Name = "Achievement for Game",
                                                      Description = "Game 3 Achievement 1"
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                Achievement = new Achievement
                                                  {
                                                      Id = 7,
                                                      GameId = 3,
                                                      Name = "Achievement for Game",
                                                      Description = "Game 3 Achievement 2"
                                                  }
                            },
                        new UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                Achievement = new Achievement
                                                  {
                                                      Id = 8,
                                                      GameId = 4,
                                                      Name = "Achievement for Game",
                                                      Description = "Game 4 Achievement 1"
                                                  }
                            }
                    };

            _repositoryMock.SetupProperty(rep => rep.Achievements, new Achievement[0].AsQueryable());
            _repositoryMock.SetupProperty(rep => rep.UserAchievements, new UserAchievement[0].AsQueryable());
            _repositoryMock.SetupProperty(rep => rep.Users, _users);

            _manager.UpdateAchievements(achievements.ToList());

            foreach (UserAchievement userAchievement in achievements)
            {
                // assert that the new achievements were added
                Achievement achievement = userAchievement.Achievement;
                int achievementId = achievement.Id;
                int gameId = achievement.GameId;
                string name = achievement.Name;
                string description = achievement.Description;
                int achievementCount =
                    _repositoryMock.Object.Achievements.Count(
                        a =>
                        a.Id == achievementId && a.GameId == gameId && a.Name == name && a.Description == description);
                Assert.That(achievementCount, Is.EqualTo(1));

                // assert that the new achievements were assigned
                UserAchievement achievement1 = userAchievement;
                int userAchievementCount =
                    _repositoryMock.Object.UserAchievements.Count(
                        ua => ua.FacebookUserId == achievement1.FacebookUserId && ua.AchievementId == achievementId);
                Assert.That(userAchievementCount, Is.EqualTo(1));
            }
        }

        [Test]
        public void UpdateHidden()
        {
            _repositoryMock.SetupProperty(rep => rep.Users, _users);
            _repositoryMock.SetupProperty(rep => rep.UserAchievements, _userAchievements);
            _repositoryMock.SetupProperty(rep => rep.Achievements, _achievements);

            const string steamUserId = "user1";
            ICollection<Achievement> achievements = _manager.GetUnpublishedAchievements(steamUserId);
            _manager.UpdateHidden(steamUserId, achievements.Select(acheivement => acheivement.Id).ToList());

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
            _manager.UpdatePublished(steamUserId, achievements.Select(acheivement => acheivement.Id).ToList());

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

            const string steamUserId = "userxxx";
            const int facebookUserId = 1;

            User user = new User {SteamUserId = steamUserId, FacebookUserId = facebookUserId};
            _manager.UpdateUser(user);

            _repositoryMock.Setup(rep => rep.SubmitChanges());

            _repositoryMock.Verify();
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