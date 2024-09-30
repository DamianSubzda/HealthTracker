using HealthTracker.Server.Core.Controllers;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Exceptions;
using HealthTracker.Server.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Server.Tests.Controllers
{
    public class ProfileControllerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ILogger<ProfileController>> _loggerMock;
        private readonly ProfileController _profileController;
        public ProfileControllerTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _loggerMock = new Mock<ILogger<ProfileController>>();
            
            _profileController = new ProfileController(
                _userRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task GetUser_ShouldReturnOk()
        {
            //Arrange
            var user = new UserDTO { Id = 1, FirstName = "FirstName", LastName = "LastName", Email = "test@test.pl" };
            _userRepositoryMock.Setup(x=> x.GetUser(user.Id)).ReturnsAsync(user);

            //Act
            var result = await _profileController.GetUser(user.Id);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<UserDTO>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(user.Id, returnValue.Id);
            Assert.Equal(user.FirstName, returnValue.FirstName);
            Assert.Equal(user.LastName, returnValue.LastName);

        }

        [Fact]
        public async Task GetUser_ShouldReturnNotFound()
        {
            //Arrange
            var user = new UserDTO { Id = 1, FirstName = "FirstName", LastName = "LastName", Email = "test@test.pl" };
            _userRepositoryMock.Setup(x => x.GetUser(user.Id)).ThrowsAsync(new UserNotFoundException(user.Id));

            //Act
            var result = await _profileController.GetUser(user.Id);

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetUsers_ShouldReturnOk()
        {
            //Arrange
            var users = new List<UserSearchDTO>()
            {
                new UserSearchDTO { Id = 1},
                new UserSearchDTO { Id = 2}
            };
            _userRepositoryMock.Setup(x => x.GetUsers(1, "test")).ReturnsAsync(users);

            //Act
            var result = await _profileController.GetUsers(1, "test");

            //Assert
            var actionResult = Assert.IsType<ActionResult<List<UserSearchDTO>>>(result);
            var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
            var returnValue = Assert.IsType<List<UserSearchDTO>>(okResult.Value);

            Assert.NotEmpty(returnValue);
            Assert.Contains(returnValue, u => u.Id == 1);
            Assert.Contains(returnValue, u => u.Id == 2);
        }

        [Fact]
        public async Task SetUserPhoto_ShouldReturnOk()
        {
            //Arrange
            var userId = 1;
            var photoPath = "Core\\Assets\\ProfilePictures";
            var photoMock = new Mock<IFormFile>();

            _userRepositoryMock.Setup(x => x.SetPhotoUser(userId, photoMock.Object)).ReturnsAsync(photoPath);

            //Act
            var result = await _profileController.SetUserPhoto(userId, photoMock.Object);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);

            Assert.Equal(photoPath, okResult.Value);
        }

        [Fact]
        public async Task SetUserPhoto_ShouldReturnNotFound()
        {
            //Arrange
            var userId = 1;
            var photoMock = new Mock<IFormFile>();

            _userRepositoryMock.Setup(x => x.SetPhotoUser(userId, photoMock.Object)).ThrowsAsync(new UserNotFoundException(userId));

            //Act
            var result = await _profileController.SetUserPhoto(userId, photoMock.Object);

            //Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }


    }
}