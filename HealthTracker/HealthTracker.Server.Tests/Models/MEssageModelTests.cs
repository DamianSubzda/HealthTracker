using HealthTracker.Server;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Health.Models;
using HealthTracker.Server.Modules.PhysicalActivity.Models;
using Moq;
using System.ComponentModel.DataAnnotations;

namespace HealthTracker.Server.Tests.Models
{
    public class MessageModelTests
    {
        [Fact]
        public void ShouldInitializeProperties()
        {
            //Arrange
            var message = new Message() { Text=""};

            //Assert
            Assert.False(message.IsReaded);
            Assert.NotNull(message.SendTime);

        }
        [Fact]
        public void ShouldAssignDataToMessage()
        {
            //Arrange
            var message = new Message
            { 
                Text = ""
            };
            var userFrom = new User() { FirstName="Sender"};
            var userTo = new User() { FirstName="Receiver"}; 

            //Act
            message.UserFrom = userFrom;
            message.UserTo = userTo;

            //Assert
            Assert.Equal("Sender", message.UserFrom.FirstName);
            Assert.Equal("Receiver", message.UserTo.FirstName);
        }

        
    }
}