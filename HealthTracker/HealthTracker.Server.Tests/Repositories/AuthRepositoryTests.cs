using AutoMapper;
using HealthTracker.Server;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Core.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using System.IdentityModel.Tokens.Jwt;

namespace HealthTracker.Server.Tests
{
    public class AuthRepositoryTests
    {
        private readonly Mock<UserManager<User>> _userManagerMock;
        private readonly Mock<SignInManager<User>> _signInManagerMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly AuthRepository _authRepository;

        public AuthRepositoryTests()
        {
            _userManagerMock = new Mock<UserManager<User>>(
                Mock.Of<IUserStore<User>>(), null, null, null, null, null, null, null, null);
            _signInManagerMock = new Mock<SignInManager<User>>(
                _userManagerMock.Object, Mock.Of<IHttpContextAccessor>(), Mock.Of<IUserClaimsPrincipalFactory<User>>(), null, null, null, null);
            _configurationMock = new Mock<IConfiguration>();
            _mapperMock = new Mock<IMapper>();

            _authRepository = new AuthRepository(_userManagerMock.Object, _signInManagerMock.Object, _configurationMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var registerDto = new RegisterDTO { Email = "test@test.com", Password = "Password123!", UserName = "testUser" };
            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);
            _userManagerMock.Setup(u => u.CreateAsync(It.IsAny<User>(), It.IsAny<string>())).ReturnsAsync(IdentityResult.Success);
            _userManagerMock.Setup(u => u.AddToRoleAsync(It.IsAny<User>(), "User")).ReturnsAsync(IdentityResult.Success);

            // Act
            var result = await _authRepository.RegisterUserAsync(registerDto);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailed_WhenEmailIsAlreadyTaken()
        {
            // Arrange
            var existingUser = new User { Email = "test@test.com" };
            var registerDto = new RegisterDTO { Email = "test@test.com", Password = "Password123!" };
            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(existingUser);

            // Act
            var result = await _authRepository.RegisterUserAsync(registerDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("400", result.Errors.First().Code);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnSuccess_WhenLoginIsSuccessful()
        {
            // Arrange
            var loginDto = new LoginDto { EmailUserName = "test@test.com", Password = "Password123!" };
            var user = new User { Email = loginDto.EmailUserName, EmailConfirmed = true };
            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _signInManagerMock.Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
                              .ReturnsAsync(SignInResult.Success);

            // Act
            var result = await _authRepository.LoginAsync(loginDto);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnFailed_WhenUserNotFound()
        {
            // Arrange
            var loginDto = new LoginDto { EmailUserName = "nonexistent@test.com", Password = "Password123!" };
            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync((User)null);

            // Act
            var result = await _authRepository.LoginAsync(loginDto);

            // Assert
            Assert.False(result.Succeeded);
            Assert.Equal("404", result.Errors.First().Code);
        }

        [Fact]
        public async Task GenerateJwtToken_ShouldReturnToken_WhenUserExists()
        {
            // Arrange
            var user = new User { Email = "test@test.com", Id = 1, FirstName = "John", LastName = "Doe" };
            _userManagerMock.Setup(u => u.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            _userManagerMock.Setup(u => u.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

            _configurationMock.Setup(c => c["Jwt:Key"]).Returns("verylongsupersecretkey!!!!!!!!!!!!!!!!");
            _configurationMock.Setup(c => c["Jwt:Issuer"]).Returns("testIssuer");
            _configurationMock.Setup(c => c["Jwt:Audience"]).Returns("testAudience");

            // Act
            var token = await _authRepository.GenerateJwtToken(user.Email);

            // Assert
            Assert.NotNull(token);
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;
            Assert.Equal("testIssuer", jsonToken.Issuer);
        }
    }
}