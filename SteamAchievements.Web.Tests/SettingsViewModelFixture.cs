using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;
using Moq;
using NUnit.Framework;
using SteamAchievements.Services;
using SteamAchievements.Web.Models;
using SteamAchievements.Web.Resources;

namespace SteamAchievements.Web.Tests
{
    [TestFixture]
    public class SettingsViewModelFixture
    {
        [Test]
        public void Validate()
        {
            SettingsViewModel model =
                new SettingsViewModel
                    {
                        AutoUpdate = true,
                        PublishDescription = true,
                        SteamUserId = "NullReference"
                    };

            Mock<IAchievementService> mockAchievementService = new Mock<IAchievementService>();
            mockAchievementService.Setup(service => service.GetProfile(model.SteamUserId))
                .Returns(() => null)
                .Verifiable();

            Mock<IUserService> mockUserService = new Mock<IUserService>();
            mockUserService.Setup(service => service.GetUser(model.FacebookUserId))
                .Returns(new User {SteamUserId = model.SteamUserId, FacebookUserId = model.FacebookUserId})
                .Verifiable();
            mockUserService.Setup(service => service.GetUser(model.SteamUserId))
                .Returns(new User {SteamUserId = model.SteamUserId, FacebookUserId = 999})
                .Verifiable();

            Mock<IDependencyResolver> mockDependencyResolver = new Mock<IDependencyResolver>();
            mockDependencyResolver.Setup(dr => dr.GetService(typeof (IAchievementService)))
                .Returns(mockAchievementService.Object)
                .Verifiable();
            mockDependencyResolver.Setup(dr => dr.GetService(typeof (IUserService)))
                .Returns(mockUserService.Object)
                .Verifiable();

            DependencyResolver.SetResolver(mockDependencyResolver.Object);

            IEnumerable<ValidationResult> validationResults = model.Validate(new ValidationContext(model, null, null)).ToArray();

            Assert.That(validationResults.Where(result => result.ErrorMessage == Strings.SettingsDuplicateCustomUrl).Any());
            Assert.That(validationResults.Where(result => result.ErrorMessage == Strings.SettingsInvalidCustomUrl).Any());

            mockAchievementService.Verify();
            mockDependencyResolver.Verify();
            mockUserService.Verify();
        }
    }
}