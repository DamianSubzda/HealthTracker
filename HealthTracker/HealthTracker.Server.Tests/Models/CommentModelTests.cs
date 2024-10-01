using HealthTracker.Server;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Health.Models;
using HealthTracker.Server.Modules.PhysicalActivity.Models;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Server.Tests.Models
{
    public class CommentModelTests
    {
        [Fact]
        public void Content_ShowuldHaveMaxLengthOf100()
        {
            var comment = new Comment() { Content = ""};
            var context = new ValidationContext(comment) { MemberName = nameof(Comment.Content) };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(new string('z', 1001), context, results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage == "Must be 1000 characters or less!");
        }

        [Fact]
        public void Content_ShouldBeRequired()
        {
            // Arrange
            var comment = new Comment
            {
                Content = "This is a required content"
            };
            var context = new ValidationContext(comment) { MemberName = nameof(Comment.Content) };
            var results = new List<ValidationResult>();

            // Act
            var isValid = Validator.TryValidateProperty(null, context, results);

            // Assert
            Assert.False(isValid);
            Assert.Contains(results, r => r.ErrorMessage.Contains("The Content field is required."));
        }

        [Fact]
        public void ShouldAssignDataToComment()
        {
            //Arrange
            var comment = new Comment() { Content = "" };
            var post = new Post() { Content="Test post"};
            var user = new User() { FirstName = "FirstName", LastName= "LastName" };
            var parentComment = new Comment() { Content = "Parent comment" };

            //Act
            comment.Post = post;
            comment.User = user;
            comment.ParentComment = parentComment;

            //Assert
            Assert.Equal("Test post", comment.Post.Content);
            Assert.Equal("FirstName", comment.User.FirstName);
            Assert.Equal("LastName", comment.User.LastName);
            Assert.Equal("Parent comment", comment.ParentComment.Content);
        }

        [Fact]
        public void ShouldInitialDataOfCreate()
        {
            //Arange
            var comment = new Comment() { Content = "" };

            //Assert
            Assert.NotNull(comment.DateOfCreate);
        }
    }
}