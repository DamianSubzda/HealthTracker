using HealthTracker.Server;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Health.Models;
using HealthTracker.Server.Modules.PhysicalActivity.Models;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Server.Tests.Models
{
    public class LikeModelTests
    {
        [Fact]
        public void ShouldSetProperties()
        {
            //Arrange
            var like = new Like
            {
                PostId = 1,
                UserId = 2,
            };

            //Assert
            Assert.Equal(1, like.PostId);
            Assert.Equal(2, like.UserId);

        }
        [Fact]
        public void ShouldAssignDataToLike()
        {
            //Arrange
            var like = new Like();
            var user = new User() { FirstName = "FirstName"};
            var post = new Post() { Content = "Test post"};

            //Act
            like.User = user;
            like.Post = post;

            //Assert
            Assert.Equal("FirstName", like.User.FirstName);
            Assert.Equal("Test post", like.Post.Content);
        }

        
    }
}