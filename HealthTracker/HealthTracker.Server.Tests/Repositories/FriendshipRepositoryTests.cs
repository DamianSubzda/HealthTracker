using AutoMapper;
using HealthTracker.Server;
using HealthTracker.Server.Core.Exceptions.Community;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Modules.Community.DTOs;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Community.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace HealthTracker.Server.Tests.Repositories
{
    public class FriendshipRepositoryTests : BaseRepositoryTests
    {
        private IFriendRepository _friendRepository;
        public FriendshipRepositoryTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreateFriendshipDTO, Friendship>();
                cfg.CreateMap<Friendship, FriendshipDTO>();
            }).CreateMapper();

            _friendRepository = new FriendshipRepository(_context, _mapper);
        }

        private async Task<User> CreateUserHelper(int id, string email, string firstName = "FirstName", string lastName = "LastName")
        {
            var user = new User { Id = id, FirstName = firstName, LastName = lastName, Email = email };
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        [Fact]
        public async Task CreateFriendshipRequest_ShouldReturnFriendship()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend = await CreateUserHelper(2, "test2@test.pl");
            var friendship = new CreateFriendshipDTO { FriendId = friend.Id, UserId = user.Id };

            //Act
            var result = await _friendRepository.CreateFriendshipRequest(friendship);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(result.FriendId, friend.Id);
            Assert.Equal(result.UserId, user.Id);
            Assert.NotNull(result.CreatedAt);

        }

        [Fact]
        public async Task CreateFriendshipRequest_ShouldThrowFriendshipAlreadyExistsException()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend = await CreateUserHelper(2, "test2@test.pl");
            var friendship = new CreateFriendshipDTO { FriendId = friend.Id, UserId = user.Id };

            await _context.Friendship.AddAsync(new Friendship { FriendId = friend.Id, UserId = user.Id });
            await _context.SaveChangesAsync();

            //Act
            var exception = await Assert.ThrowsAsync<FriendshipAlreadyExistsException>(() => _friendRepository.CreateFriendshipRequest(friendship));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task GetFriendship_ShouldGetFriendship()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend = await CreateUserHelper(2, "test2@test.pl");

            await _context.Friendship.AddAsync(new Friendship { Id = 1, FriendId = friend.Id, UserId = user.Id });
            await _context.SaveChangesAsync();

            //Act
            var result = await _friendRepository.GetFriendship(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.FriendId);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task GetFriendship_ShouldThrowFriendshipNotFoundException()
        {
            //Act
            var exception = await Assert.ThrowsAsync<FriendshipNotFoundException>(() => _friendRepository.GetFriendship(1));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task GetFriendshipByUsersId_ShouldGetFriendship()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend = await CreateUserHelper(2, "test2@test.pl");

            await _context.Friendship.AddAsync(new Friendship { Id = 1, FriendId = friend.Id, UserId = user.Id });
            await _context.SaveChangesAsync();

            //Act
            var result = await _friendRepository.GetFriendshipByUsersId(1, 2);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.FriendId);
            Assert.Equal(1, result.UserId);
        }

        [Fact]
        public async Task GetFriendshipByUsersId_ShouldThrowFriendshipNotFoundException()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend = await CreateUserHelper(2, "test2@test.pl");

            //Act
            var exception = await Assert.ThrowsAsync<FriendshipNotFoundException>(() => _friendRepository.GetFriendshipByUsersId(1, 2));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task GetFriendList_ShouldGetListOfAcceptedFriendsForUser()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend1 = await CreateUserHelper(2, "test2@test.pl");
            var friend2 = await CreateUserHelper(3, "test3@test.pl");
            var friend3 = await CreateUserHelper(4, "test4@test.pl");

            await _context.Friendship.AddAsync(new Friendship { Id = 1, FriendId = friend1.Id, UserId = user.Id, Status = Status.Accepted });
            await _context.Friendship.AddAsync(new Friendship { Id = 2, FriendId = friend2.Id, UserId = user.Id, Status = Status.Accepted });
            await _context.Friendship.AddAsync(new Friendship { Id = 3, FriendId = friend3.Id, UserId = user.Id, Status = Status.Requested });
            await _context.SaveChangesAsync();

            //Act
            var result = await _friendRepository.GetFriendList(user.Id);

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);

            Assert.Equal(2, result[0].UserId);
            Assert.Equal(3, result[1].UserId);
        }

        [Fact]
        public async Task GetFriendList_ShouldGetEmptyList()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");

            //Act
            var result = await _friendRepository.GetFriendList(user.Id);

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetFriendshipRequestsForUser_ShouldGetListOfRequestedFriendshipForUser()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend1 = await CreateUserHelper(2, "test2@test.pl");
            var friend2 = await CreateUserHelper(3, "test3@test.pl");
            var friend3 = await CreateUserHelper(4, "test4@test.pl");

            await _context.Friendship.AddAsync(new Friendship { Id = 1, FriendId = user.Id, UserId = friend1.Id, Status = Status.Requested });
            await _context.Friendship.AddAsync(new Friendship { Id = 2, FriendId = user.Id, UserId = friend2.Id, Status = Status.Accepted });
            await _context.Friendship.AddAsync(new Friendship { Id = 3, FriendId = user.Id, UserId = friend3.Id, Status = Status.Requested });
            await _context.SaveChangesAsync();

            //Act
            var result = await _friendRepository.GetFriendshipRequestsForUser(user.Id);

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);

            Assert.Equal(2, result[0].UserId);
            Assert.Equal(4, result[1].UserId);
        }

        [Fact]
        public async Task AcceptFriendship_ShouldAcceptRequestedFriendshipAndCreateSymmetricalFriendship()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend = await CreateUserHelper(2, "test2@test.pl");

            await _context.Friendship.AddAsync(new Friendship { Id = 1, FriendId = friend.Id, UserId = user.Id, Status = Status.Requested });
            await _context.SaveChangesAsync();

            //Act
            await _friendRepository.AcceptFriendship(friend.Id, user.Id);
            var friendships = await _context.Friendship.Where(f => f.Status == Status.Accepted).ToListAsync();

            //Assert
            Assert.Equal(2, friendships.Count);
            Assert.Contains(friendships, f => f.UserId == user.Id && f.FriendId == friend.Id && f.Status == Status.Accepted);
            Assert.Contains(friendships, f => f.UserId == friend.Id && f.FriendId == user.Id && f.Status == Status.Accepted);

        }

        [Fact]
        public async Task DeclineFriendship_ShouldDeclineRequestedFriendshipAndCreateSymmetricalFriendshipd()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend = await CreateUserHelper(2, "test2@test.pl");

            await _context.Friendship.AddAsync(new Friendship { Id = 1, FriendId = friend.Id, UserId = user.Id, Status = Status.Requested });
            await _context.SaveChangesAsync();

            //Act
            await _friendRepository.DeclineFriendship(friend.Id, user.Id); //friend accepts friendship
            var friendships = await _context.Friendship.Where(f => f.Status == Status.Declined).ToListAsync();

            //Assert
            Assert.Equal(2, friendships.Count);
            Assert.Contains(friendships, f => f.UserId == user.Id && f.FriendId == friend.Id && f.Status == Status.Declined);
            Assert.Contains(friendships, f => f.UserId == friend.Id && f.FriendId == user.Id && f.Status == Status.Declined);
        }

        [Fact]
        public async Task DeleteFriendship_ShouldDeleteFriendship()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend = await CreateUserHelper(2, "test2@test.pl");

            await _context.Friendship.AddAsync(new Friendship { Id = 1, FriendId = friend.Id, UserId = user.Id, Status = Status.Requested });
            await _context.SaveChangesAsync();

            //Act
            await _friendRepository.DeleteFriendship(user.Id, friend.Id);
            var friendships = await _context.Friendship.ToListAsync();

            //Assert
            Assert.Empty(friendships);
        }

        [Fact]
        public async Task DeleteFriendship_ShouldThrowFriendshipNotFoundException()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend = await CreateUserHelper(2, "test2@test.pl");

            //Act
            var exception = await Assert.ThrowsAsync<FriendshipNotFoundException>(() => _friendRepository.DeleteFriendship(user.Id, friend.Id));

            //Assert
            Assert.NotNull(exception.Message);
        }

    }
}