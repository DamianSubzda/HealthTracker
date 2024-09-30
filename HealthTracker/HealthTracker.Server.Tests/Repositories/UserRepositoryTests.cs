using AutoMapper;
using HealthTracker.Server.Core.DTOs;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Core.Repositories;
using HealthTracker.Server.Infrastructure.Services;
using Moq;

namespace HealthTracker.Server.Tests.Repositories
{
    public class UserRepositoryTests : BaseRepositoryTests
    {
        private readonly IFileService _fileService;
        private readonly UserRepository _userRepository;

        public UserRepositoryTests()
        {
            _mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<User, UserDTO>();
                cfg.CreateMap<User, UserSearchDTO>();
            }).CreateMapper();

            _fileService = new Mock<IFileService>().Object;
            _userRepository = new UserRepository(_context, _mapper, _fileService);
        }

        [Fact]
        public async Task ShouldReturnUserDTO_WhenUserExists()
        {
            //Arrange
            var userId = 1;
            _context.User.Add(new User { Id = userId, FirstName = "John", LastName = "Doe" });
            await _context.SaveChangesAsync();

            // Act
            var result = await _userRepository.GetUser(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);

        }
    }
}