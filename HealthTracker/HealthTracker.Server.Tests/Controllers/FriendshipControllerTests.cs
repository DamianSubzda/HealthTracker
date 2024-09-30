using HealthTracker.Server.Core.Controllers;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Exceptions;
using HealthTracker.Server.Core.Exceptions.Community;
using HealthTracker.Server.Core.Repositories;
using HealthTracker.Server.Modules.Community.Controllers;
using HealthTracker.Server.Modules.Community.DTOs;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Community.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Server.Tests.Controllers
{
    public class FriendshipControllerTests
    {
        private readonly Mock<IFriendRepository> _friendshipRepositoryMock;
        private readonly Mock<ILogger<FriendshipController>> _loggerMock;
        private readonly FriendshipController _friendshipController;
        public FriendshipControllerTests()
        {
            _friendshipRepositoryMock = new Mock<IFriendRepository>();
            _loggerMock = new Mock<ILogger<FriendshipController>>();

            _friendshipController = new FriendshipController(
                _friendshipRepositoryMock.Object,
                _loggerMock.Object
            );
        }


        [Fact]
        public async Task CreateFriendship_ShouldReturnCreatedAtAction()
        {
            //Arrange
            var createFriendshipDTo = new CreateFriendshipDTO { UserId = 1, FriendId = 2 };
            var friendshipDto = new FriendshipDTO { Id = 1, UserId = createFriendshipDTo.UserId, FriendId = createFriendshipDTo.FriendId };

            _friendshipRepositoryMock.Setup(x => x.CreateFriendshipRequest(createFriendshipDTo)).ReturnsAsync(friendshipDto);

            //Act
            var result = await _friendshipController.CreateFriendship(createFriendshipDTo);

            //Assert
            var createAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createAtActionResult.StatusCode);
            Assert.Equal(nameof(_friendshipController.GetFriendship), createAtActionResult.ActionName);
            Assert.Equal(friendshipDto, createAtActionResult.Value);
        }

        [Fact]
        public async Task CreateFriendship_ShouldReturnBadRequest()
        {
            //Arrange
            var createFriendshipDTo = new CreateFriendshipDTO { UserId = 1, FriendId = 2 };

            _friendshipRepositoryMock.Setup(x => x.CreateFriendshipRequest(createFriendshipDTo)).ThrowsAsync(new DbUpdateException());

            //Act
            var result = await _friendshipController.CreateFriendship(createFriendshipDTo);

            //Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Database error.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreateFriendship_ShouldReturnNotFound()
        {
            //Arrange
            var createFriendshipDTo = new CreateFriendshipDTO { UserId = 1, FriendId = 2 };

            _friendshipRepositoryMock.Setup(x => x.CreateFriendshipRequest(createFriendshipDTo)).ThrowsAsync(new UserNotFoundException());

            //Act
            var result = await _friendshipController.CreateFriendship(createFriendshipDTo);

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task CreateFriendship_ShouldReturnAlreadyExist()
        {
            //Arrange
            var createFriendshipDTo = new CreateFriendshipDTO { UserId = 1, FriendId = 2 };

            _friendshipRepositoryMock.Setup(x => x.CreateFriendshipRequest(createFriendshipDTo)).ThrowsAsync(new FriendshipAlreadyExistsException());

            //Act
            var result = await _friendshipController.CreateFriendship(createFriendshipDTo);

            //Assert
            var alreadyExistResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal(409, alreadyExistResult.StatusCode);
        }

        [Fact]
        public async Task GetFriendship_ShouldReturnOk()
        {
            //Arrange
            var friendshipId = 1;
            var friendshipDto = new FriendshipDTO { Id = 1, FriendId = 1, UserId = 2 };

            _friendshipRepositoryMock.Setup(x => x.GetFriendship(friendshipId)).ReturnsAsync(friendshipDto);

            //Act
            var result = await _friendshipController.GetFriendship(friendshipId);

            //Asssert
            var actionResult = Assert.IsType<ActionResult<FriendshipDTO>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<FriendshipDTO>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(friendshipDto, returnValue);
        }

        [Fact]
        public async Task GetFriendship_ShouldReturnNotFound()
        {
            //Arrange
            var friendshipId = 1;

            _friendshipRepositoryMock.Setup(x => x.GetFriendship(friendshipId)).ThrowsAsync(new FriendshipNotFoundException());

            //Act
            var result = await _friendshipController.GetFriendship(friendshipId);

            //Asssert
            var actionResult = Assert.IsType<ActionResult<FriendshipDTO>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetFriendshipByUsersId_ShouldReturnOk()
        {
            //Arrange
            var userId = 1;
            var friendId = 2;
            var friendshipDto = new FriendshipDTO { Id = 1, UserId = userId, FriendId = friendId };

            _friendshipRepositoryMock.Setup(x => x.GetFriendshipByUsersId(userId, friendId)).ReturnsAsync(friendshipDto);

            //Act
            var result = await _friendshipController.GetFriendshipByUsersId(userId, friendId);

            //Assert
            var actionResult = Assert.IsType<ActionResult<FriendshipDTO>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<FriendshipDTO>(okResult.Value);

            Assert.Equal(friendshipDto, returnValue);
        }

        [Fact]
        public async Task GetFriendshipByUsersId_ShouldReturnNotFound()
        {
            //Arrange
            var userId = 1;
            var friendId = 2;

            _friendshipRepositoryMock.Setup(x => x.GetFriendshipByUsersId(userId, friendId)).ThrowsAsync(new UserNotFoundException());

            //Act
            var result = await _friendshipController.GetFriendshipByUsersId(userId, friendId);

            //Assert
            var actionResult = Assert.IsType<ActionResult<FriendshipDTO>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetFriendList_ShouldReturnOk()
        {
            //Arrange
            var userId = 1;
            var friends = new List<FriendDTO>()
            {
                new FriendDTO() { UserId = 1, FirstName = "FirstName 1"},
                new FriendDTO() { UserId = 2, FirstName = "FirstName 2"},
            };

            _friendshipRepositoryMock.Setup(x => x.GetFriendList(userId)).ReturnsAsync(friends);

            //Act
            var result = await _friendshipController.GetFriendList(userId);

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<FriendDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<FriendDTO>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(friends, returnValue);
            Assert.Contains(returnValue, f => f.UserId == friends[0].UserId);
            Assert.Contains(returnValue, f => f.UserId == friends[1].UserId);
            Assert.Equal(2, returnValue.Count);
        }

        [Fact]
        public async Task GetFriendList_ShouldReturnNotFound()
        {
            //Arrange
            var userId = 1;

            _friendshipRepositoryMock.Setup(x => x.GetFriendList(userId)).ThrowsAsync(new UserNotFoundException());

            //Act
            var result = await _friendshipController.GetFriendList(userId);

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<FriendDTO>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetFriendshipRequestsForUser_ShouldReturnOk()
        {
            //Arrange
            var userId = 1;
            var friendshipRequests = new List<FriendDTO>
            {
                new FriendDTO { UserId = 1 },
                new FriendDTO { UserId = 2 }
            };

            _friendshipRepositoryMock.Setup(x => x.GetFriendshipRequestsForUser(userId)).ReturnsAsync(friendshipRequests);

            //Act
            var result = await _friendshipController.GetFriendshipRequestsForUser(userId);

            //Asert
            var actionResult = Assert.IsType<ActionResult<List<FriendDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<FriendDTO>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Contains(returnValue, f => f.UserId == friendshipRequests[0].UserId);
            Assert.Contains(returnValue, f => f.UserId == friendshipRequests[1].UserId);
        }

        [Fact]
        public async Task GetFriendshipRequestsForUser_ShouldReturnNotFound()
        {
            //Arrange
            var userId = 1;
            

            _friendshipRepositoryMock.Setup(x => x.GetFriendshipRequestsForUser(userId)).ThrowsAsync(new UserNotFoundException());

            //Act
            var result = await _friendshipController.GetFriendshipRequestsForUser(userId);

            //Asert
            var actionResult = Assert.IsType<ActionResult<List<FriendDTO>>>(result);
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(actionResult.Result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task AcceptFriendshipStatus_ShouldReturnOk()
        {
            //Arrange
            var userId = 1;
            var friendId = 2;
            
            //Act
            var result = await _friendshipController.AcceptFriendshipStatus(userId, friendId);

            //Assert
            var okResult = Assert.IsType<NoContentResult>(result);

            Assert.Equal(204, okResult.StatusCode);
        }

        [Fact]
        public async Task DeclineFriendshipStatus_ShouldReturnOk()
        {
            //Arrange
            var userId = 1;
            var friendId = 2;

            //Act
            var result = await _friendshipController.DeclineFriendshipStatus(userId, friendId);

            //Assert
            var okResult = Assert.IsType<NoContentResult>(result);

            Assert.Equal(204, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteFriendship_ShouldReturnOk()
        {
            //Arrange
            var userId = 1;
            var friendId = 2;

            //Act
            var result = await _friendshipController.DeleteFriendship(userId, friendId);

            //Assert
            var okResult = Assert.IsType<NoContentResult>(result);

            Assert.Equal(204, okResult.StatusCode);
        }
    }
}