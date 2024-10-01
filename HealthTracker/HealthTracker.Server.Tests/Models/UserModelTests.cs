using HealthTracker.Server;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Health.Models;
using HealthTracker.Server.Modules.PhysicalActivity.Models;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Server.Tests.Models
{
    public class UserModelTests
    {
        [Fact]
        public void FirstName_ShowuldHaveMaxLengthOf100()
        {
            //Arrange 
            var user = new User();
            var context = new ValidationContext(user) { MemberName = nameof(User.FirstName) };
            var results = new List<ValidationResult>();
            
            //Act
            var isValid = Validator.TryValidateProperty(new string('z', 101), context, results);

            //Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "Must be 100 characters or less!");
        }

        [Fact]
        public void LastName_ShowuldHaveMaxLengthOf100()
        {
            //Arrange
            var user = new User();
            var context = new ValidationContext(user) { MemberName = nameof(User.LastName) };
            var results = new List<ValidationResult>();

            //Act
            var isValid = Validator.TryValidateProperty(new string('z', 101), context, results);

            //Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "Must be 100 characters or less!");
        }

        [Fact]
        public void About_ShowuldHaveMaxLengthOf1000()
        {
            //Arrange
            var user = new User();
            var context = new ValidationContext(user) { MemberName = nameof(User.About) };
            var results = new List<ValidationResult>();

            //Act
            var isValid = Validator.TryValidateProperty(new string('z', 1001), context, results);

            //Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "Must be 1000 characters or less!");
        }

        [Fact]
        public void CollentionsShouldBeEmptyByDefault()
        {
            //Arrange
            var user = new User();

            //Assert
            Assert.Empty(user.Goals);
            Assert.Empty(user.Workouts);
            Assert.Empty(user.Posts);
            Assert.Empty(user.Comments);
            Assert.Empty(user.Notifications);
            Assert.Empty(user.Friendships);
            Assert.Empty(user.HealthMeasurements);
        }

        [Fact]
        public void CollectionsShouldAddElement()
        {
            //Arrange
            var user = new User();
            var goal = new Goal();
            var workout = new Workout();
            var post = new Post();
            var comment = new Comment() { Content=""};
            var notification = new Notification();
            var friendship = new Friendship();
            var healthMeasurement = new HealthMeasurement();

            //Act
            user.Goals.Add(goal);
            user.Workouts.Add(workout);
            user.Posts.Add(post);
            user.Comments.Add(comment);
            user.Notifications.Add(notification);
            user.Friendships.Add(friendship);
            user.HealthMeasurements.Add(healthMeasurement);

            //Assert
            Assert.Contains(goal, user.Goals);
            Assert.Contains(workout, user.Workouts);
            Assert.Contains(post, user.Posts);
            Assert.Contains(comment, user.Comments);
            Assert.Contains(notification, user.Notifications);
            Assert.Contains(healthMeasurement, user.HealthMeasurements);

        }
    }
}