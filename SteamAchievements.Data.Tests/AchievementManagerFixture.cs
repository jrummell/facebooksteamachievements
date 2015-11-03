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
                                     new steam_Achievement
                                         {
                                             Id = 1,
                                             GameId = 1,
                                             ApiName = "1",
                                             AchievementNames = new EntitySet<steam_AchievementName>
                                                                    {
                                                                        new steam_AchievementName
                                                                            {
                                                                                Id = 1,
                                                                                AchievementId = 1,
                                                                                Name = "Achievement 1 for Game 1",
                                                                                Description = ""
                                                                            }
                                                                    }
                                         },
                                     new steam_Achievement
                                         {
                                             Id = 2,
                                             GameId = 1,
                                             ApiName = "2",
                                             AchievementNames = new EntitySet<steam_AchievementName>
                                                                    {
                                                                        new steam_AchievementName
                                                                            {
                                                                                Id = 2,
                                                                                AchievementId = 2,
                                                                                Name = "Achievement 1 for Game 1",
                                                                                Description = ""
                                                                            }
                                                                    }
                                         },
                                     new steam_Achievement
                                         {
                                             Id = 3,
                                             GameId = 1,
                                             ApiName = "3",
                                             AchievementNames = new EntitySet<steam_AchievementName>
                                                                    {
                                                                        new steam_AchievementName
                                                                            {
                                                                                Id = 3,
                                                                                AchievementId = 3,
                                                                                Name = "Achievement 2 for Game 1",
                                                                                Description = ""
                                                                            }
                                                                    }
                                         },
                                     new steam_Achievement
                                         {
                                             Id = 4,
                                             GameId = 2,
                                             ApiName = "4",
                                             AchievementNames = new EntitySet<steam_AchievementName>
                                                                    {
                                                                        new steam_AchievementName
                                                                            {
                                                                                Id = 4,
                                                                                AchievementId = 4,
                                                                                Name = "Achievement 1 for Game 2",
                                                                                Description = ""
                                                                            }
                                                                    }
                                         },
                                     new steam_Achievement
                                         {
                                             Id = 5,
                                             GameId = 2,
                                             ApiName = "5",
                                             AchievementNames = new EntitySet<steam_AchievementName>
                                                                    {
                                                                        new steam_AchievementName
                                                                            {
                                                                                Id = 5,
                                                                                AchievementId = 5,
                                                                                Name = "Achievement 2 for Game 2",
                                                                                Description = ""
                                                                            }
                                                                    }
                                         }
                                 }).AsQueryable();

            _users = (new[]
                          {
                              new steam_User {FacebookUserId = 1, SteamUserId = "user1"},
                              new steam_User {FacebookUserId = 2, SteamUserId = "user2"}
                          }).AsQueryable();

            _userAchievements = (new[]
                                     {
                                         new steam_UserAchievement
                                             {
                                                 Id = 1,
                                                 AchievementId = 1,
                                                 Date = DateTime.Now,
                                                 FacebookUserId = 1,
                                                 Achievement = _achievements.Single(achievement => achievement.Id == 1),
                                                 Published = true,
                                                 Hidden = false
                                             },
                                         new steam_UserAchievement
                                             {
                                                 Id = 2,
                                                 AchievementId = 2,
                                                 Date = DateTime.Now,
                                                 FacebookUserId = 1,
                                                 Achievement = _achievements.Single(achievement => achievement.Id == 2),
                                                 Published = true,
                                                 Hidden = false
                                             },
                                         new steam_UserAchievement
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
        private IQueryable<steam_Achievement> _achievements;
        private IQueryable<steam_User> _users;
        private IQueryable<steam_UserAchievement> _userAchievements;
        private Mock<ISteamRepository> _repositoryMock;

        [Test]
        public void AddGameAchievements()
        {
            const int gameId = 99;
            List<steam_Achievement> gameAchievements = new List<steam_Achievement>();
            for (int i = 0; i < 10; i++)
            {
                steam_Achievement achievement = new steam_Achievement
                                              {
                                                  ImageUrl = "http://example.com/achievement" + i + ".gif",
                                                  ApiName = i.ToString(),
                                                  AchievementNames = new EntitySet<steam_AchievementName>
                                                                         {
                                                                             new steam_AchievementName
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
            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<steam_Achievement>()),
                                   Times.Exactly(gameAchievements.Count));
            _repositoryMock.Verify(rep => rep.SubmitChanges());

            List<steam_Achievement> expectedAchievements = new List<steam_Achievement>(_achievements);
            expectedAchievements.AddRange(gameAchievements);
            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(expectedAchievements.AsQueryable());

            Assert.That(_repositoryMock.Object.Achievements.Count(a => a.GameId == gameId), Is.EqualTo(10));
        }

        [Test]
        public void AssignAchievements()
        {
            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);
            _repositoryMock.SetupGet(rep => rep.UserAchievements).Returns(_userAchievements);

            const long facebookUserId = 1;
            IEnumerable<steam_Achievement> achievements =
                _manager.GetUnassignedAchievements(facebookUserId, _achievements);
            Assert.That(achievements.Any());

            _repositoryMock.VerifyGet(rep => rep.Achievements);
            _repositoryMock.VerifyGet(rep => rep.UserAchievements);

            IEnumerable<steam_UserAchievement> userAchievements =
                from achievement in achievements
                select new steam_UserAchievement
                           {
                               FacebookUserId = facebookUserId,
                               Achievement = achievement
                           };

            _manager.AssignAchievements(userAchievements);

            _repositoryMock.Verify(rep => rep.InsertAllOnSubmit(It.IsAny<IEnumerable<steam_UserAchievement>>()));
            _repositoryMock.Verify(rep => rep.SubmitChanges());
        }

        [Test]
        public void GetMissingAchievements()
        {
            steam_Achievement achievement1Game5 = new steam_Achievement {GameId = 5, ApiName = "1"};
            steam_Achievement achievement2Game5 = new steam_Achievement {GameId = 5, ApiName = "2"};
            ICollection<steam_Achievement> communityAchievements =
                new[]
                    {
                        new steam_Achievement {GameId = 1, ApiName = "1"},
                        new steam_Achievement {GameId = 1, ApiName = "2"},
                        new steam_Achievement {GameId = 1, ApiName = "3"},
                        achievement1Game5,
                        achievement2Game5
                    };

            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);
            ICollection<steam_Achievement> missingAchievements =
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

            ICollection<steam_Achievement> achievements = _manager.GetUnassignedAchievements(1, _achievements);

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

            ICollection<steam_Achievement> achievements = _manager.GetUnpublishedAchievements(1);

            _repositoryMock.VerifyGet(rep => rep.UserAchievements);

            Assert.That(achievements.Count, Is.EqualTo(1));
            Assert.That(achievements.Count(a => a.Id == 3), Is.EqualTo(1));
        }

        [Test]
        public void InsertMissingAchievements()
        {
            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);
            int maxId = _achievements.Select(a => a.Id).Max();

            steam_Achievement achievement1Game5 =
                new steam_Achievement
                    {
                        Id = ++maxId,
                        GameId = 5,
                        ImageUrl = "",
                        ApiName = "1",
                        AchievementNames = new EntitySet<steam_AchievementName> {new steam_AchievementName()}
                    };
            steam_Achievement achievement2Game5 =
                new steam_Achievement
                    {
                        Id = ++maxId,
                        GameId = 5,
                        ImageUrl = "",
                        ApiName = "2",
                        AchievementNames = new EntitySet<steam_AchievementName> {new steam_AchievementName()}
                    };

            steam_Achievement[] missingAchievements = new[] {achievement1Game5, achievement2Game5};

            _manager.InsertMissingAchievements(missingAchievements);

            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<steam_Achievement>()),
                                   Times.Exactly(missingAchievements.Length));
            _repositoryMock.Verify(rep => rep.SubmitChanges());
        }

        [Test]
        public void UpdateAchievements()
        {
            long facebookUserId = _users.First().FacebookUserId;

            ICollection<steam_UserAchievement> userAchievements =
                new[]
                    {
                        new steam_UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                Achievement = new steam_Achievement
                                                  {
                                                      GameId = 1,
                                                      ApiName = "1",
                                                      AchievementNames =
                                                          new EntitySet<steam_AchievementName>
                                                              {
                                                                  new steam_AchievementName
                                                                      {Language = "english"}
                                                              }
                                                  }
                            },
                        new steam_UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                Achievement = new steam_Achievement
                                                  {
                                                      GameId = 1,
                                                      ApiName = "2",
                                                      AchievementNames =
                                                          new EntitySet<steam_AchievementName>
                                                              {
                                                                  new steam_AchievementName
                                                                      {Language = "english"}
                                                              }
                                                  }
                            },
                        new steam_UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 3,
                                Achievement = new steam_Achievement
                                                  {
                                                      Id = 3,
                                                      GameId = 1,
                                                      ApiName = "3",
                                                      AchievementNames =
                                                          new EntitySet<steam_AchievementName>
                                                              {
                                                                  new steam_AchievementName
                                                                      {AchievementId = 3, Language = "english"}
                                                              }
                                                  }
                            },
                        new steam_UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 4,
                                Achievement = new steam_Achievement
                                                  {
                                                      Id = 4,
                                                      GameId = 2,
                                                      ApiName = "4",
                                                      AchievementNames =
                                                          new EntitySet<steam_AchievementName>
                                                              {
                                                                  new steam_AchievementName
                                                                      {AchievementId = 4, Language = "english"}
                                                              }
                                                  }
                            },
                        new steam_UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 5,
                                Achievement = new steam_Achievement
                                                  {
                                                      Id = 5,
                                                      GameId = 2,
                                                      ApiName = "5",
                                                      AchievementNames =
                                                          new EntitySet<steam_AchievementName>
                                                              {
                                                                  new steam_AchievementName
                                                                      {AchievementId = 5, Language = "english"}
                                                              }
                                                  }
                            },
                        new steam_UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 6,
                                Achievement = new steam_Achievement
                                                  {
                                                      Id = 6,
                                                      GameId = 3,
                                                      ApiName = "6",
                                                      AchievementNames =
                                                          new EntitySet<steam_AchievementName>
                                                              {
                                                                  new steam_AchievementName
                                                                      {AchievementId = 6, Language = "english"}
                                                              }
                                                  }
                            },
                        new steam_UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 7,
                                Achievement = new steam_Achievement
                                                  {
                                                      Id = 7,
                                                      GameId = 3,
                                                      ApiName = "7",
                                                      AchievementNames =
                                                          new EntitySet<steam_AchievementName>
                                                              {
                                                                  new steam_AchievementName
                                                                      {AchievementId = 7, Language = "german"}
                                                              }
                                                  }
                            },
                        new steam_UserAchievement
                            {
                                FacebookUserId = facebookUserId,
                                AchievementId = 8,
                                Achievement = new steam_Achievement
                                                  {
                                                      Id = 8,
                                                      GameId = 4,
                                                      ApiName = "8",
                                                      AchievementNames =
                                                          new EntitySet<steam_AchievementName>
                                                              {
                                                                  new steam_AchievementName
                                                                      {AchievementId = 8, Language = "german"}
                                                              }
                                                  }
                            }
                    };

            foreach (steam_UserAchievement userAchievement in userAchievements)
            {
                userAchievement.Achievement.UserAchievements = new EntitySet<steam_UserAchievement> {userAchievement};
                foreach (steam_AchievementName name in userAchievement.Achievement.AchievementNames)
                {
                    name.Achievement = userAchievement.Achievement;
                }
            }

            _repositoryMock.SetupSequence(rep => rep.Achievements)
                .Returns(new steam_Achievement[0].AsQueryable())
                .Returns(userAchievements.Select(a => a.Achievement).AsQueryable())
                .Returns(userAchievements.Select(a => a.Achievement).AsQueryable());

            _repositoryMock.SetupSequence(rep => rep.UserAchievements)
                .Returns(new steam_UserAchievement[0].AsQueryable())
                .Returns(userAchievements.AsQueryable())
                .Returns(userAchievements.AsQueryable());

            _repositoryMock.Setup(rep => rep.InsertAllOnSubmit(It.IsAny<IEnumerable<steam_UserAchievement>>()))
                .Verifiable();

            _repositoryMock.Setup(rep => rep.SubmitChanges())
                .Verifiable();

            _manager.UpdateAchievements(userAchievements);

            _repositoryMock.VerifyGet(rep => rep.Achievements);
            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<steam_Achievement>()),
                                   Times.Exactly(userAchievements.Count));
            _repositoryMock.Verify();
        }

        [Test]
        public void UpdateHidden()
        {
            _repositoryMock.SetupGet(rep => rep.Users).Returns(_users);
            _repositoryMock.SetupGet(rep => rep.UserAchievements).Returns(_userAchievements);
            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);

            const int userId = 1;
            ICollection<steam_Achievement> achievements = _manager.GetUnpublishedAchievements(userId);
            _manager.UpdateHidden(userId, achievements.Select(acheivement => acheivement.Id));

            IEnumerable<int> achievementIds = from achievement in achievements
                                              select achievement.Id;
            IQueryable<steam_UserAchievement> userAchievements =
                from achievement in _repositoryMock.Object.UserAchievements
                where achievement.FacebookUserId == userId && achievementIds.Contains(achievement.AchievementId)
                select achievement;

            Assert.That(userAchievements.Any(achievement => !achievement.Hidden), Is.False);
        }

        [Test]
        public void UpdatePublished()
        {
            _repositoryMock.SetupGet(rep => rep.Users).Returns(_users);
            _repositoryMock.SetupGet(rep => rep.UserAchievements).Returns(_userAchievements);
            _repositoryMock.SetupGet(rep => rep.Achievements).Returns(_achievements);

            const int userId = 1;
            ICollection<steam_Achievement> achievements = _manager.GetUnpublishedAchievements(userId);
            _manager.UpdatePublished(userId, achievements.Select(acheivement => acheivement.Id));

            IEnumerable<int> achievementIds = from achievement in achievements
                                              select achievement.Id;

            IQueryable<steam_UserAchievement> userAchievements =
                from achievement in _repositoryMock.Object.UserAchievements
                where achievement.FacebookUserId == userId && achievementIds.Contains(achievement.AchievementId)
                select achievement;

            Assert.That(userAchievements.Any(achievement => !achievement.Published), Is.False);
        }

        [Test]
        public void UpdateUser()
        {
            _repositoryMock.SetupGet(rep => rep.Users).Returns(_users);
            _repositoryMock.Setup(rep => rep.SubmitChanges());

            const string steamUserId = "user1";
            const int facebookUserId = 1;

            steam_User user = new steam_User {SteamUserId = steamUserId, FacebookUserId = facebookUserId, AccessToken = "asdf"};
            _manager.UpdateUser(user);

            _repositoryMock.VerifyGet(rep => rep.Users, Times.Exactly(2));
            _repositoryMock.Verify(rep => rep.SubmitChanges());
            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<steam_User>()), Times.Never());
        }
        
        [Test]
        public void UpdateUserWithNewSteamUserId()
        {
        	_repositoryMock.SetupGet(rep => rep.Users)
        		.Returns(new[]{new steam_User {SteamUserId = "user1", FacebookUserId = 1}}.AsQueryable())
        		.Verifiable();
        	_repositoryMock.SetupGet(rep => rep.UserAchievements)
        		.Returns(new[]{new steam_UserAchievement {FacebookUserId = 1}}.AsQueryable())
        		.Verifiable();
            _repositoryMock.Setup(rep => rep.DeleteAllOnSubmit(It.IsAny<IEnumerable<steam_UserAchievement>>()))
            	.Verifiable();
            _repositoryMock.Setup(rep => rep.SubmitChanges())
            	.Verifiable();

            steam_User user = new steam_User {SteamUserId = "userxxx", FacebookUserId = 1};
            _manager.UpdateUser(user);

            _repositoryMock.Verify();
            _repositoryMock.VerifyGet(rep => rep.Users, Times.Exactly(2));
            _repositoryMock.Verify(rep => rep.InsertOnSubmit(It.IsAny<steam_User>()), Times.Never());
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