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

            Mock<IDependencyResolver> mockDependencyResolver = new Mock<IDependencyResolver>();
            mockDependencyResolver.Setup(dr => dr.GetService(typeof (IAchievementService)))
                .Returns(mockAchievementService.Object)
                .Verifiable();

            DependencyResolver.SetResolver(mockDependencyResolver.Object);

            IEnumerable<ValidationResult> validationResults =
                model.Validate(new ValidationContext(model, null, null)).ToArray();

            Assert.That(validationResults.Where(result => result.ErrorMessage == Strings.SettingsInvalidCustomUrl).Any());

            mockAchievementService.Verify();
            mockDependencyResolver.Verify();
        }
    }
}