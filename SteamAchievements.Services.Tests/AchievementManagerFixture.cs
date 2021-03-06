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
using SteamAchievements.Data;

namespace SteamAchievements.Services.Tests
{
    [TestFixture]
    public class AchievementManagerFixture
    {
        [SetUp]
        public void SetUp()
        {
            _achievements = new[]
                            {
                                new Achievement
                                {
                                    Id = 1,
                                    GameId = 1,
                                    ApiName = "1",
                                    AchievementNames = new List<AchievementName>
                                                       {
                                                           new AchievementName
                                                           {
                                                               Id = 1,
                                                               AchievementId = 1,
                                                               Name = "Achievement 1 for Game 1",
                                                               Description = ""
                                                           }
                                                       }
                                },
                                new Achievement
                                {
                                    Id = 2,
                                    GameId = 1,
                                    ApiName = "2",
                                    AchievementNames = new List<AchievementName>
                                                       {
                                                           new AchievementName
                                                           {
                                                               Id = 2,
                                                               AchievementId = 2,
                                                               Name = "Achievement 1 for Game 1",
                                                               Description = ""
                                                           }
                                                       }
                                },
                                new Achievement
                                {
                                    Id = 3,
                                    GameId = 1,
                                    ApiName = "3",
                                    AchievementNames = new List<AchievementName>
                                                       {
                                                           new AchievementName
                                                           {
                                                               Id = 3,
                                                               AchievementId = 3,
                                                               Name = "Achievement 2 for Game 1",
                                                               Description = ""
                                                           }
                                                       }
                                },
                                new Achievement
                                {
                                    Id = 4,
                                    GameId = 2,
                                    ApiName = "4",
                                    AchievementNames = new List<AchievementName>
                                                       {
                                                           new AchievementName
                                                           {
                                                               Id = 4,
                                                               AchievementId = 4,
                                                               Name = "Achievement 1 for Game 2",
                                                               Description = ""
                                                           }
                                                       }
                                },
                                new Achievement
                                {
                                    Id = 5,
                                    GameId = 2,
                                    ApiName = "5",
                                    AchievementNames = new List<AchievementName>
                                                       {
                                                           new AchievementName
                                                           {
                                                               Id = 5,
                                                               AchievementId = 5,
                                                               Name = "Achievement 2 for Game 2",
                                                               Description = ""
                                                           }
                                                       }
                                }
                            }.AsQueryable();

            _users = new[]
                     {
                         new User {Id = "1", SteamUserId = "user1"},
                         new User {Id = "2", SteamUserId = "user2"}
                     }.AsQueryable();

            _userAchievements = new[]
                                {
                                    new UserAchievement
                                    {
                                        Id = 1,
                                        AchievementId = 1,
                                        Date = DateTime.Now,
                                        UserId = "1",
                                        Achievement = _achievements.Single(achievement => achievement.Id == 1),
                                        Published = true,
                                        Hidden = false
                                    },
                                    new UserAchievement
                                    {
                                        Id = 2,
                                        AchievementId = 2,
                                        Date = DateTime.Now,
                                        UserId = "1",
                                        Achievement = _achievements.Single(achievement => achievement.Id == 2),
                                        Published = true,
                                        Hidden = false
                                    },
                                    new UserAchievement
                                    {
                                        Id = 3,
                                        AchievementId = 3,
                                        Date = DateTime.Now,
                                        UserId = "1",
                                        Achievement = _achievements.Single(achievement => achievement.Id == 3),
                                        Published = false,
                                        Hidden = false
                                    }
                                }.AsQueryable();

            _repositoryMock = new Mock<ISteamRepository>();
            _manager = new AchievementManager(_repositoryMock.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _manager.Dispose();
        }

        private AchievementManager _manager;
        private IQueryable<Achievement> _achievements;
        private IQueryable<User> _users;
        private IQueryable<UserAchievement> _userAchievements;
        private Mock<ISteamRepository> _repositoryMock;

        [Test]
        public void AddGameAchievements()
        {
            const int gameId = 99;
            var gameAchievements = new List<Achievement>();
            for (var i = 0; i < 10; i++)
            {
                var achievement = new Achievement
                                  {
                                      ImageUrl = "http://example.com/achievement" + i + ".gif",
                                      ApiName = i.ToString(),
                                      AchievementNames = new List<AchievementName>
                                                         {
                                                             new AchievementName
                                                             {
                                                                 Name = "Achievement " + i,
                                                                 Description = "Achievement " + i
                                                             }
                                                         }
                                  };
                gameAchievements.Add(achievement);
            }

            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);

            _manager.AddAchievements(99, gameAchievements);

            _repositoryMock.VerifyGet(rep => rep.Achievements);
            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<Achievement>()),
                                   Times.Exactly(gameAchievements.Count));
            _repositoryMock.Verify(rep => rep.SubmitChanges());

            var expectedAchievements = new List<Achievement>(_achievements);
            expectedAchievements.AddRange(gameAchievements);
            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(expectedAchievements.AsQueryable());

            Assert.That(_repositoryMock.Object.Achievements.Count(a => a.GameId == gameId), Is.EqualTo(10));
        }

        [Test]
        public void AssignAchievements()
        {
            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);
            _repositoryMock.SetupGet(rep => rep.UserAchievements).Returns(_userAchievements);

            const string userId = "1";
            IEnumerable<Achievement> achievements =
                _manager.GetUnassignedAchievements(userId, _achievements);
            Assert.That(achievements.Any());

            _repositoryMock.VerifyGet(rep => rep.Achievements);
            _repositoryMock.VerifyGet(rep => rep.UserAchievements);

            var userAchievements =
                from achievement in achievements
                select new UserAchievement
                       {
                           UserId = userId,
                           Achievement = achievement
                       };

            _manager.AssignAchievements(userAchievements);

            _repositoryMock.Verify(rep => rep.InsertAllOnSubmit(It.IsAny<IEnumerable<UserAchievement>>()));
            _repositoryMock.Verify(rep => rep.SubmitChanges());
        }

        [Test]
        public void GetMissingAchievements()
        {
            var achievement1Game5 = new Achievement {GameId = 5, ApiName = "1"};
            var achievement2Game5 = new Achievement {GameId = 5, ApiName = "2"};
            ICollection<Achievement> communityAchievements =
                new[]
                {
                    new Achievement {GameId = 1, ApiName = "1"},
                    new Achievement {GameId = 1, ApiName = "2"},
                    new Achievement {GameId = 1, ApiName = "3"},
                    achievement1Game5,
                    achievement2Game5
                };

            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);
            var missingAchievements =
                _manager.GetMissingAchievements(communityAchievements.ToList());

            _repositoryMock.VerifyGet(rep => rep.Achievements);

            Assert.That(missingAchievements.Count(), Is.EqualTo(2));

            CollectionAssert.Contains(missingAchievements, achievement1Game5);
            CollectionAssert.Contains(missingAchievements, achievement2Game5);
        }

        [Test]
        public void GetUnassignedAchievements()
        {
            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);
            _repositoryMock.SetupGet(rep => rep.UserAchievements).Returns(_userAchievements);

            var achievements = _manager.GetUnassignedAchievements("1", _achievements);

            _repositoryMock.VerifyGet(rep => rep.UserAchievements);
            _repositoryMock.VerifyGet(rep => rep.Achievements);

            Assert.That(achievements.Count, Is.EqualTo(2));
            Assert.That(achievements.Count(a => a.Id == 4), Is.EqualTo(1));
            Assert.That(achievements.Count(a => a.Id == 5), Is.EqualTo(1));
        }

        [Test]
        public void GetUnpublishedAchievements()
        {
            _repositoryMock.SetupGet(rep => rep.UserAchievements).Returns(_userAchievements);
            _repositoryMock.SetupGet(rep => rep.Users).Returns(_users);

            var achievements = _manager.GetUnpublishedAchievements("1");

            _repositoryMock.VerifyGet(rep => rep.UserAchievements);

            Assert.That(achievements.Count, Is.EqualTo(1));
            Assert.That(achievements.Count(a => a.Id == 3), Is.EqualTo(1));
        }

        [Test]
        public void InsertMissingAchievements()
        {
            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);
            var maxId = _achievements.Select(a => a.Id).Max();

            var achievement1Game5 =
                new Achievement
                {
                    Id = ++maxId,
                    GameId = 5,
                    ImageUrl = "",
                    ApiName = "1",
                    AchievementNames = new List<AchievementName> {new AchievementName()}
                };
            var achievement2Game5 =
                new Achievement
                {
                    Id = ++maxId,
                    GameId = 5,
                    ImageUrl = "",
                    ApiName = "2",
                    AchievementNames = new List<AchievementName> {new AchievementName()}
                };

            Achievement[] missingAchievements = {achievement1Game5, achievement2Game5};

            _manager.InsertMissingAchievements(missingAchievements);

            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<Achievement>()),
                                   Times.Exactly(missingAchievements.Length));
            _repositoryMock.Verify(rep => rep.SubmitChanges());
        }

        [Test]
        public void UpdateAchievements()
        {
            var userId = _users.First().Id;

            ICollection<UserAchievement> userAchievements =
                new[]
                {
                    new UserAchievement
                    {
                        UserId = userId,
                        Achievement = new Achievement
                                      {
                                          GameId = 1,
                                          ApiName = "1",
                                          AchievementNames =
                                              new List<AchievementName>
                                              {
                                                  new AchievementName
                                                  {Language = "english"}
                                              }
                                      }
                    },
                    new UserAchievement
                    {
                        UserId = userId,
                        Achievement = new Achievement
                                      {
                                          GameId = 1,
                                          ApiName = "2",
                                          AchievementNames =
                                              new List<AchievementName>
                                              {
                                                  new AchievementName
                                                  {Language = "english"}
                                              }
                                      }
                    },
                    new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = 3,
                        Achievement = new Achievement
                                      {
                                          Id = 3,
                                          GameId = 1,
                                          ApiName = "3",
                                          AchievementNames =
                                              new List<AchievementName>
                                              {
                                                  new AchievementName
                                                  {AchievementId = 3, Language = "english"}
                                              }
                                      }
                    },
                    new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = 4,
                        Achievement = new Achievement
                                      {
                                          Id = 4,
                                          GameId = 2,
                                          ApiName = "4",
                                          AchievementNames =
                                              new List<AchievementName>
                                              {
                                                  new AchievementName
                                                  {AchievementId = 4, Language = "english"}
                                              }
                                      }
                    },
                    new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = 5,
                        Achievement = new Achievement
                                      {
                                          Id = 5,
                                          GameId = 2,
                                          ApiName = "5",
                                          AchievementNames =
                                              new List<AchievementName>
                                              {
                                                  new AchievementName
                                                  {AchievementId = 5, Language = "english"}
                                              }
                                      }
                    },
                    new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = 6,
                        Achievement = new Achievement
                                      {
                                          Id = 6,
                                          GameId = 3,
                                          ApiName = "6",
                                          AchievementNames =
                                              new List<AchievementName>
                                              {
                                                  new AchievementName
                                                  {AchievementId = 6, Language = "english"}
                                              }
                                      }
                    },
                    new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = 7,
                        Achievement = new Achievement
                                      {
                                          Id = 7,
                                          GameId = 3,
                                          ApiName = "7",
                                          AchievementNames =
                                              new List<AchievementName>
                                              {
                                                  new AchievementName
                                                  {AchievementId = 7, Language = "german"}
                                              }
                                      }
                    },
                    new UserAchievement
                    {
                        UserId = userId,
                        AchievementId = 8,
                        Achievement = new Achievement
                                      {
                                          Id = 8,
                                          GameId = 4,
                                          ApiName = "8",
                                          AchievementNames =
                                              new List<AchievementName>
                                              {
                                                  new AchievementName
                                                  {AchievementId = 8, Language = "german"}
                                              }
                                      }
                    }
                };

            foreach (var userAchievement in userAchievements)
            {
                userAchievement.Achievement.UserAchievements = new List<UserAchievement> {userAchievement};
                foreach (var name in userAchievement.Achievement.AchievementNames)
                {
                    name.Achievement = userAchievement.Achievement;
                }
            }

            _repositoryMock.SetupSequence(rep => rep.Achievements)
                           .Returns(new Achievement[0].AsQueryable())
                           .Returns(userAchievements.Select(a => a.Achievement).AsQueryable())
                           .Returns(userAchievements.Select(a => a.Achievement).AsQueryable());

            _repositoryMock.SetupSequence(rep => rep.UserAchievements)
                           .Returns(new UserAchievement[0].AsQueryable())
                           .Returns(userAchievements.AsQueryable())
                           .Returns(userAchievements.AsQueryable());

            _repositoryMock.Setup(rep => rep.InsertAllOnSubmit(It.IsAny<IEnumerable<UserAchievement>>()))
                           .Verifiable();

            _repositoryMock.Setup(rep => rep.SubmitChanges())
                           .Verifiable();

            _manager.UpdateAchievements(userAchievements);

            _repositoryMock.VerifyGet(rep => rep.Achievements);
            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<Achievement>()),
                                   Times.Exactly(userAchievements.Count));
            _repositoryMock.Verify();
        }

        [Test]
        public void UpdateHidden()
        {
            _repositoryMock.SetupGet(rep => rep.Users).Returns(_users);
            _repositoryMock.SetupGet(rep => rep.UserAchievements).Returns(_userAchievements);
            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);

            const string userId = "1";
            var achievements = _manager.GetUnpublishedAchievements(userId);
            _manager.UpdateHidden(userId, achievements.Select(acheivement => acheivement.Id));

            var achievementIds = from achievement in achievements
                                 select achievement.Id;
            var userAchievements =
                from achievement in _repositoryMock.Object.UserAchievements
                where achievement.UserId == userId && achievementIds.Contains(achievement.AchievementId)
                select achievement;

            Assert.That(userAchievements.Any(achievement => !achievement.Hidden), Is.False);
        }

        [Test]
        public void UpdatePublished()
        {
            _repositoryMock.SetupGet(rep => rep.Users).Returns(_users);
            _repositoryMock.SetupGet(rep => rep.UserAchievements).Returns(_userAchievements);
            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);

            const string userId = "1";
            var achievements = _manager.GetUnpublishedAchievements(userId);
            _manager.UpdatePublished(userId, achievements.Select(acheivement => acheivement.Id));

            var achievementIds = from achievement in achievements
                                 select achievement.Id;

            var userAchievements =
                from achievement in _repositoryMock.Object.UserAchievements
                where achievement.UserId == userId && achievementIds.Contains(achievement.AchievementId)
                select achievement;

            Assert.That(userAchievements.Any(achievement => !achievement.Published), Is.False);
        }

        [Test]
        public void ValidateDate()
        {
            var date = AchievementManager.ValidateDate(DateTime.MinValue);

            var sqlMinTicks = SqlDateTime.MinValue.Value.Ticks;
            Assert.That(date.Ticks, Is.GreaterThan(sqlMinTicks));
        }
    }
}