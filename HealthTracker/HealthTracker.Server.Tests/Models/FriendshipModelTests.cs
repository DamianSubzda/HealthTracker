using HealthTracker.Server;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Health.Models;
using HealthTracker.Server.Modules.PhysicalActivity.Models;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Server.Tests.Models
{
    public class FriendshipModelTests
    {
        [Fact]
        public void ShouldInitialCreatedAt()
        {
            //Arrange
            var friendship = new Friendship();

            //Assert
            Assert.NotNull(friendship.CreatedAt);
        }

        [Fact]
        public void ShouldAssignDataToFriendship()
        {
            //Arrange
            var friendship = new Friendship();
            var user = new User() { FirstName = "FirstName"};
            var friend = new User() { FirstName = "FriendFirstName"};
            var status = Status.Accepted;

            //Act
            friendship.Friend = friend;
            friendship.User = user;
            friendship.Status = status;

            //Assert
            Assert.Equal(Status.Accepted, friendship.Status);
            Assert.Equal("FirstName", friendship.User.FirstName);
            Assert.Equal("FriendFirstName", friendship.Friend.FirstName);
        }
    }
}