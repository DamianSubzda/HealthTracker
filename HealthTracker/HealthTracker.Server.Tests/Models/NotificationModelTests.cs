using HealthTracker.Server;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Health.Models;
using HealthTracker.Server.Modules.PhysicalActivity.Models;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Server.Tests.Models
{
    public class NotificationModelTests
    {
        [Fact]
        public void Content_ShowuldHaveMaxLengthOf100()
        {
            //Arrange 
            var notification = new Notification();
            var context = new ValidationContext(notification) { MemberName = nameof(Notification.Content) };
            var results = new List<ValidationResult>();
            
            //Act
            var isValid = Validator.TryValidateProperty(new string('z', 1001), context, results);

            //Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "Must be 1000 characters or less!");
        }

        [Fact]
        public void ShouldSetNotificationProperties()
        {
            //Arrange
            var notification = new Notification
            {
                Id = 1,
                UserId = 2,
                StatusId = 0,
                Content = "Notification content!",
                DateOfCreate = new DateTime(2024, 1, 1)
            };

            Assert.Equal(1, notification.Id);
            Assert.Equal(2, notification.UserId);
            Assert.Equal(0, notification.StatusId);
            Assert.Equal("Notification content!", notification.Content);
            Assert.Equal(new DateTime(2024, 1, 1), notification.DateOfCreate);

        }


        [Fact]
        public void ShouldAssignDataToNotification()
        {
            //Arrange
            var notification = new Notification();
            var user = new User() { FirstName= "FirstName", LastName="LastName"};
            var status = Status.Accepted;

            //Act
            notification.User = user;
            notification.Status = status;

            //Assert
            Assert.Equal(status, notification.Status);
            Assert.Equal("FirstName", notification.User.FirstName);
            Assert.Equal("LastName", notification.User.LastName);
        }


    }
}