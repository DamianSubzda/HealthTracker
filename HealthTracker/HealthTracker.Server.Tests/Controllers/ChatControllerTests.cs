using HealthTracker.Server.Core.Controllers;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Exceptions;
using HealthTracker.Server.Core.Exceptions.Community;
using HealthTracker.Server.Core.Repositories;
using HealthTracker.Server.Modules.Community.Controllers;
using HealthTracker.Server.Modules.Community.DTOs;
using HealthTracker.Server.Modules.Community.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Server.Tests.Controllers
{
    public class ChatControllerTests
    {
        private readonly Mock<IChatRepository> _chatRepositoryMock;
        private readonly Mock<ILogger<ChatController>> _loggerMock;
        private readonly ChatController _chatController;
        public ChatControllerTests()
        {
            _chatRepositoryMock = new Mock<IChatRepository>();
            _loggerMock = new Mock<ILogger<ChatController>>();
            
            _chatController = new ChatController(
                _chatRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        
        [Fact]
        public async Task CreateMessage_ShouldReturnCreatedAtAction()
        {
            //Arrange
            var sendMessageDTO = new CreateMessageDTO { UserIdFrom = 1, UserIdTo = 2, Text= "Test message." };
            var messageDto = new MessageDTO { Id=1, Text = sendMessageDTO.Text, UserIdFrom=sendMessageDTO.UserIdFrom, UserIdTo=sendMessageDTO.UserIdTo };

            _chatRepositoryMock.Setup(x => x.CreateMessage(sendMessageDTO)).ReturnsAsync(messageDto);

            //Act
            var result = await _chatController.CreateMessage(sendMessageDTO);

            //Assert
            var createAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_chatController.GetMessage), createAtActionResult.ActionName);
            Assert.Equal(messageDto, createAtActionResult.Value);
        }

        [Fact]
        public async Task CreateMessage_ShouldReturnBadRequest()
        {
            //Arrange
            var sendMessageDTO = new CreateMessageDTO { UserIdFrom = 1, UserIdTo = 2, Text = "Test message." };

            _chatRepositoryMock.Setup(x => x.CreateMessage(sendMessageDTO)).ThrowsAsync(new DbUpdateException());

            //Act
            var result = await _chatController.CreateMessage(sendMessageDTO);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Database error.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateMessage_ShouldReturnNotFound()
        {
            //Arrange
            var sendMessageDTO = new CreateMessageDTO { UserIdFrom = 1, UserIdTo = 2, Text = "Test message." };

            _chatRepositoryMock.Setup(x => x.CreateMessage(sendMessageDTO)).ThrowsAsync(new UserNotFoundException(sendMessageDTO.UserIdTo));

            //Act
            var result = await _chatController.CreateMessage(sendMessageDTO);

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetMessage_ShouldReturnOk()
        {
            //Arrange
            var messageDto = new MessageDTO { Id = 1, UserIdFrom = 1, UserIdTo = 2, Text = "Test message." };

            _chatRepositoryMock.Setup(x => x.GetMessage(messageDto.Id)).ReturnsAsync(messageDto);

            //Act
            var result = await _chatController.GetMessage(messageDto.Id);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<MessageDTO>(okResult.Value);
            Assert.Equal(messageDto, returnValue);
        }

        [Fact]
        public async Task GetMessage_ShouldReturnNotFound()
        {
            //Arrange
            var messageId = 1;
            _chatRepositoryMock.Setup(x => x.GetMessage(messageId)).ThrowsAsync(new MessageNotFoundException(messageId));

            //Act
            var result = await _chatController.GetMessage(messageId);

            //Assert
            var actionResult = Assert.IsType<ActionResult<MessageDTO>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetMessages_ShouldReturnOk_WhenMessagesExist()
        {
            //Arrange
            var userIdFrom = 1;
            var userIdTo = 2;
            var pageNr = 1;
            var messages = new List<MessageDTO>()
            {
                new() { Id = 1, Text="Test message 1."},
                new() { Id = 2, Text="Test message 2."}
            };
            _chatRepositoryMock.Setup(x => x.GetMessages(userIdFrom, userIdTo, pageNr, 10)).ReturnsAsync(messages);

            //Act
            var result = await _chatController.GetMessages(userIdFrom, userIdTo, pageNr);

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<MessageDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<MessageDTO>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Contains(returnValue, u => u.Id == 1);
            Assert.Contains(returnValue, u => u.Id == 2);
            Assert.Contains(returnValue, u => u.Text == messages[0].Text);
            Assert.Contains(returnValue, u => u.Text == messages[1].Text);
        }

        [Fact]
        public async Task GetMessages_ShouldReturnOk_WhenNoMessagesExist()
        {
            //Arrange
            var userIdFrom = 1;
            var userIdTo = 2;
            var pageNr = 1;
            var messages = new List<MessageDTO>();
            _chatRepositoryMock.Setup(x => x.GetMessages(userIdFrom, userIdTo, pageNr, 10)).ReturnsAsync(messages);

            //Act
            var result = await _chatController.GetMessages(userIdFrom, userIdTo, pageNr);

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<MessageDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<MessageDTO>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Empty(returnValue);
        }

        [Fact]
        public async Task GetMessages_ShouldReturnNotFound()
        {
            //Arrange
            var userIdFrom = 1;
            var userIdTo = 2;
            var pageNr = 1;
            _chatRepositoryMock.Setup(x => x.GetMessages(userIdFrom, userIdTo, pageNr, 10)).ThrowsAsync(new UserNotFoundException(userIdFrom));

            //Act
            var result = await _chatController.GetMessages(userIdFrom, userIdTo, pageNr);

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<MessageDTO>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetNumberOfNewMessages_ShouldReturnOk()
        {
            //Arrange
            var userIdFrom = 1;
            var userIdTo = 2;
            var numberOfNewMessages = 5;

            _chatRepositoryMock.Setup(x => x.GetNumberOfNewMessages(userIdFrom, userIdTo)).ReturnsAsync(numberOfNewMessages);

            //Act
            var result = await _chatController.GetNumberOfNewMessages(userIdFrom, userIdTo);

            //Assert
            var actionResult = Assert.IsType<ActionResult<int>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(numberOfNewMessages, okResult.Value);
        }

        [Fact]
        public async Task GetNumberOfNewMessages_ShouldReturnNotFOund()
        {
            //Arrange
            var userIdFrom = 1;
            var userIdTo = 2;

            _chatRepositoryMock.Setup(x => x.GetNumberOfNewMessages(userIdFrom, userIdTo)).ThrowsAsync(new UserNotFoundException(userIdTo));

            //Act
            var result = await _chatController.GetNumberOfNewMessages(userIdFrom, userIdTo);

            //Assert
            var actionResult = Assert.IsType<ActionResult<int>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task UpdateMessagesToReaded_ShouldReturnOk()
        {
            //Arrange
            var userIdFrom = 1;
            var userIdTo = 2;

            //Act
            var result = await _chatController.UpdateMessagesToReaded(userIdFrom, userIdTo);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task UpdateMessagesToReaded_ShouldReturnNotFound()
        {
            //Arrange
            var userIdFrom = 1;
            var userIdTo = 2;

            _chatRepositoryMock.Setup(x => x.UpdateMessagesToReaded(userIdFrom, userIdTo)).ThrowsAsync(new UserNotFoundException(userIdTo));

            //Act
            var result = await _chatController.UpdateMessagesToReaded(userIdFrom, userIdTo);

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

    }
}