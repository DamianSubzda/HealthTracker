using Xunit;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Repositories;
using HealthTracker.Server.Infrastructure.Data;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Moq;
using System.Security.Claims;

namespace HealthTracker.Server.Tests.Repositories
{
    public class AuthRepositoryTests : BaseRepositoryTests
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole<int>> _roleManager;
        private readonly IConfiguration _configuration;
        private readonly AuthRepository _authRepository;

        public AuthRepositoryTests()
        {

            var userStore = new UserStore<User, IdentityRole<int>, ApplicationDbContext, int>(_context);
            _userManager = new UserManager<User>(userStore, null, new PasswordHasher<User>(), null, null, null, null, null, null);

            var roleStore = new RoleStore<IdentityRole<int>, ApplicationDbContext, int>(_context);
            _roleManager = new RoleManager<IdentityRole<int>>(roleStore, null, null, null, null);

            _roleManager.CreateAsync(new IdentityRole<int> { Name = "User" }).Wait();

            var userClaimsPrincipalFactoryMock = new Mock<IUserClaimsPrincipalFactory<User>>();
            var httpContextAccessorMock = new Mock<IHttpContextAccessor>();

            var signInManagerMock = new Mock<SignInManager<User>>(
                _userManager,
                httpContextAccessorMock.Object,
                userClaimsPrincipalFactoryMock.Object,
                null, null, null, null
            );

            signInManagerMock
                .Setup(s => s.PasswordSignInAsync(It.IsAny<string>(), It.IsAny<string>(), false, false))
                .ReturnsAsync(SignInResult.Success);

            _signInManager = signInManagerMock.Object;

            var inMemorySettings = new Dictionary<string, string> {
                {"Jwt:Key", "verylongsupersecretkey!!!!!!!!!!!!!!!!"},
                {"Jwt:Issuer", "testIssuer"},
                {"Jwt:Audience", "testAudience"}
            };

            _configuration = new ConfigurationBuilder()
                .AddInMemoryCollection(inMemorySettings)
                .Build();

            _authRepository = new AuthRepository(_userManager, _signInManager, _configuration, _mapper);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnSuccess_WhenUserIsRegisteredSuccessfully()
        {
            // Arrange
            var registerDto = new RegisterDTO { FirstName = "FirstName1", LastName = "LastName1", Email = "test1@test.com", Password = "Password123!", UserName = "testUser1" };

            // Act
            var result = await _authRepository.RegisterUserAsync(registerDto);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public async Task RegisterUserAsync_ShouldReturnFailed_WhenEmailIsAlreadyTaken()
        {
            // Arrange
            var existingUser = new User { FirstName = "FirstName2", LastName = "LastName2", Email = "test2@test.com", UserName = "existingUser2" };
            await _userManager.CreateAsync(existingUser, "Password123!");

            var registerDto = new RegisterDTO { FirstName = "FirstName3", LastName = "LastName3", Email = "test2@test.com", Password = "Password123!", UserName = "newUser3" };

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
            var user = new User { FirstName = "FirstName4", LastName = "LastName4", Email = "test4@test.com", UserName = "testUser4", EmailConfirmed = true };
            await _userManager.CreateAsync(user, "Password123!");

            var loginDto = new LoginDto { EmailUserName = "test4@test.com", Password = "Password123!" };

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
            var user = new User { FirstName = "FirstName5", LastName = "LastName5", Email = "test5@test.com" };
            await _userManager.CreateAsync(user, "Password123!");
            await _userManager.AddToRoleAsync(user, "User");

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
