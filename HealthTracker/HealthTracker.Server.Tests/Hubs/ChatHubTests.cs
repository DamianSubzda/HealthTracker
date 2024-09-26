using HealthTracker.Server;
using HealthTracker.Server.Infrastructure.Data;
using HealthTracker.Server.Infrastructure.Hubs;
using HealthTracker.Server.Modules.Community.DTOs;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Community.Repositories;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Server.Tests.Hubs
{
    public class ChatHubTests
    {
        private readonly Mock<IChatRepository> _chatRepositoryMock;
        private readonly Mock<ILogger<ChatHub>> _loggerMock;
        private readonly Mock<IHubCallerClients> _clientsMock;
        private readonly Mock<IClientProxy> _clientProxyMock;
        private readonly ChatHub _chatHub;

        public ChatHubTests()
        {
            _chatRepositoryMock = new Mock<IChatRepository>();
            _loggerMock = new Mock<ILogger<ChatHub>>();
            _clientsMock = new Mock<IHubCallerClients>();
            _clientProxyMock = new Mock<IClientProxy>();

            _clientsMock.Setup(c => c.User(It.IsAny<string>())).Returns(_clientProxyMock.Object);

            _chatHub = new ChatHub(_chatRepositoryMock.Object, _loggerMock.Object)
            {
                Clients = _clientsMock.Object
            };
        }

        [Fact]
        public async Task SendMessageToUser_ShouldSendMessagesToBothUsers_WhenMessageIsSentSuccessfully()
        {
            // Arrange
            var userFrom = 1;
            var userTo = 2;
            var messageText = "Hello!";
            var createdMessage = new MessageDTO { Id = 100, Text = messageText, UserIdFrom = userFrom, UserIdTo = userTo };

            _chatRepositoryMock
                .Setup(repo => repo.CreateMessage(It.IsAny<CreateMessageDTO>()))
                .ReturnsAsync(createdMessage);

            // Act
            await _chatHub.SendMessageToUser(userFrom, userTo, messageText);

            // Assert
            _clientProxyMock.Verify(client => client.SendCoreAsync(
                "ReceiveMessage",
                It.Is<object[]>(args => (int)args[0] == createdMessage.Id &&
                                        (int)args[1] == userFrom &&
                                        (int)args[2] == userTo &&
                                        (string)args[3] == messageText),
                default(CancellationToken)), Times.Exactly(2));
        }

        [Fact]
        public async Task SendMessageToUser_ShouldLogError_WhenExceptionIsThrown()
        {
            // Arrange
            var userFrom = 1;
            var userTo = 2;
            var messageText = "Hello!";
            var exception = new Exception("Database error");

            _chatRepositoryMock
                .Setup(repo => repo.CreateMessage(It.IsAny<CreateMessageDTO>()))
                .ThrowsAsync(exception);

            // Act
            await _chatHub.SendMessageToUser(userFrom, userTo, messageText);

            // Assert
            _loggerMock.Verify(logger => logger.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("An error occurred while sending a message")),
                exception,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()),
                Times.Once);
        }
    }
}