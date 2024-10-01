using HealthTracker.Server;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Health.Models;
using HealthTracker.Server.Modules.PhysicalActivity.Models;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Server.Tests.Models
{
    public class PostModelTests
    {
        [Fact]
        public void ShouldInitializeProperties()
        {
            //Arrange
            var post = new Post();

            //Assert
            Assert.NotNull(post.DateOfCreate);

        }
        [Fact]
        public void ShouldAssignDataToPost()
        {
            //Arrange
            var post = new Post();
            var user = new User() { FirstName= "FirstName" };

            //Act
            post.User = user;

            //Assert
            Assert.Equal("FirstName", post.User.FirstName);
        }
        [Fact]
        public void Content_ShouldHaveMaxLengthOf2500()
        {
            //Arrange
            var post = new Post();
            var context = new ValidationContext(post) { MemberName = nameof(Post.Content)};
            var results = new List<ValidationResult>();
            
            //Act
            var isValid = Validator.TryValidateProperty(new string('z', 2501), context, results);

            //Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("Must be 2500 characters or less!"));
        }

        [Fact]
        public void Content_ShouldBeRequired()
        {
            // Arrange
            var post = new Post();
            var context = new ValidationContext(post) { MemberName = nameof(Post.Content) };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(null, context, results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("The Content field is required."));
        }



    }
}