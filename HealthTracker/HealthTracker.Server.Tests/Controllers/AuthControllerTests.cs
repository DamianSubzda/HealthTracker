using AutoMapper;
using Castle.Core.Logging;
using HealthTracker.Server;
using HealthTracker.Server.Core.Controllers;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Core.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace HealthTracker.Server.Tests.Controllers
{
    public class AuthControllerTests
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<ILogger<AuthController>> _loggerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly AuthController _authController;
        public AuthControllerTests()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            _mapperMock = new Mock<IMapper>();
            _loggerMock = new Mock<ILogger<AuthController>>();
            _configurationMock = new Mock<IConfiguration>();

            _authController = new AuthController(
                _authRepositoryMock.Object,
                _userManagerMock.Object,
                _signInManagerMock.Object,
                _mapperMock.Object,
                _loggerMock.Object,
                _configurationMock.Object);
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginDto = new LoginDto { EmailUserName = "test@test.com", Password = "Password123!" };
            var user = new User { Email = "test@test.com", UserName = "testUser" };
            var successLoginDto = new SuccessLoginDto { Token = "valid_token" };

            _authRepositoryMock.Setup(x => x.LoginAsync(loginDto)).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(x => x.FindByNameAsync(It.IsAny<string>())).ReturnsAsync(user);
            _mapperMock.Setup(x => x.Map<SuccessLoginDto>(user)).Returns(successLoginDto);
            _authRepositoryMock.Setup(x => x.GenerateJwtToken(It.IsAny<string>())).ReturnsAsync("valid_token");

            //Act
            var result = await _authController.Login(loginDto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnValue = Assert.IsType<SuccessLoginDto>(okResult.Value);
            Assert.Equal("valid_token", returnValue.Token);
            Assert.Equal(200, okResult.StatusCode);

        }

        [Fact]
        public async Task Login_ShouldReturnBadRequest_WhenLoginFails()
        {
            //Arrange
            var loginDto = new LoginDto { EmailUserName = "test@test.com", Password = "wrongpassword" };
            var failedResult = IdentityResult.Failed(new IdentityError { Code = "InvalidCredentials", Description = "Invalid credentials" });

            _authRepositoryMock.Setup(x => x.LoginAsync(loginDto)).ReturnsAsync(failedResult);

            //Act
            var result = await _authController.Login(loginDto);

            //Assert
            var badRequestedResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestedResult.StatusCode);

        }

        [Fact]
        public async Task Register_ShouldReturnOk_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var registerDto = new RegisterDTO { Email = "test@test.com", Password = "Password123!" };
            var successResult = IdentityResult.Success;

            _authRepositoryMock.Setup(x => x.RegisterUserAsync(registerDto)).ReturnsAsync(successResult);

            //Act
            var result = await _authController.Register(registerDto);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenRegistrationFails()
        {
            // Arrange
            var registerDto = new RegisterDTO { Email = "test@test.com", Password = "Password123!" };
            var failedResult = IdentityResult.Failed(new IdentityError { Code = "EmailInUse", Description = "Email already in use" });

            _authRepositoryMock.Setup(x => x.RegisterUserAsync(registerDto)).ReturnsAsync(failedResult);

            //Act
            var result = await _authController.Register(registerDto);

            //Arrange
            var badRequestedResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestedResult.StatusCode);
        }

        [Fact]
        public async Task AssignAdminRole_ShouldReturnNotFound_WhenUserDoesNotExist()
        {
            // Arrange
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            // Act
            var result = await _authController.AssignAdminRole("nonexistent_user_id");

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal("User not found", notFoundResult.Value);
        }

        [Fact]
        public async Task AssignAdminRole_ShouldReturnOk_WhenAdminRoleIsAssigned()
        {
            // Arrange
            var user = new User { Id = 1, UserName = "testUser" };
            _userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(x => x.AddToRoleAsync(user, "Admin")).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authController.AssignAdminRole("valid_user_id");

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}