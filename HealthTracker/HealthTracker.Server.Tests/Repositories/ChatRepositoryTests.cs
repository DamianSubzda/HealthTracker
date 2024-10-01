using AutoMapper;
using HealthTracker.Server.Core.Exceptions;
using HealthTracker.Server.Core.Exceptions.Community;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Modules.Community.DTOs;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Community.Repositories;
using Moq;

namespace HealthTracker.Server.Tests.Repositories
{
    public class ChatRepositoryTests : BaseRepositoryTests
    {
        private readonly ChatRepository _chatRepository;

        public ChatRepositoryTests()
        {
            _mapper = new MapperConfiguration(cfg => {
                cfg.CreateMap<Message, MessageDTO>();
                cfg.CreateMap<CreateMessageDTO, Message>();
            }).CreateMapper();

            _chatRepository = new ChatRepository(_context, _mapper);
        }

        [Fact]
        public async Task CreateMessage_ShouldReturnMessage_WhenValidDataProvided()
        {
            //Arrange
            var userFrom = new User { Id = 1, FirstName = "FirstName", LastName = "FirstName", Email = "test@test.pl" };
            var userTo = new User { Id = 2, FirstName = "FirstName", LastName = "FirstName", Email = "test2@test.pl" };
            var message = new CreateMessageDTO { Text= "test", UserIdFrom=1, UserIdTo=2 };
            await _context.User.AddAsync(userFrom);
            await _context.User.AddAsync(userTo);
            await _context.SaveChangesAsync();

            // Act
            var result = await _chatRepository.CreateMessage(message);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("test", result.Text);
            Assert.Equal(1, result.UserIdFrom);
            Assert.Equal(2, result.UserIdTo);
        }

        [Fact]
        public async Task CreateMessage_ShouldThrowUserNotFoundException_WhenUserDoesNotExist()
        {
            //Arrange
            var userFrom = new User { Id = 1, FirstName = "FirstName", LastName = "FirstName", Email = "test@test.pl" };
            var message = new CreateMessageDTO { Text = "test", UserIdFrom = 1, UserIdTo = 2 };
            await _context.User.AddAsync(userFrom);
            await _context.SaveChangesAsync();

            // Act
            var exception =  await Assert.ThrowsAsync<UserNotFoundException>(() => _chatRepository.CreateMessage(message));

            // Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task GetMessage_ShouldReturnMessage_WhenMessageExists()
        {
            //Arrange
            var message = new Message { Id = 1, Text="test" };
            await _context.Message.AddAsync(message);
            await _context.SaveChangesAsync();

            //Act
            var result = await _chatRepository.GetMessage(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("test", result.Text);
            Assert.Equal(1, result.Id);
        }

        [Fact]
        public async Task GetMessage_ShouldThrowMessageNotFoundException_WhenMessageDoesNotExist()
        {
            //Arrange
            var message = new Message { Id = 1, Text = "test" };
            await _context.Message.AddAsync(message);
            await _context.SaveChangesAsync();

            //Act
            var exception = await Assert.ThrowsAsync<MessageNotFoundException>(()=> _chatRepository.GetMessage(2));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task GetMessages_ShouldReturnMessages_WhenUsersHaveMessages()
        {
            //Arrange
            var userFrom = new User { Id = 1, FirstName = "FirstName", LastName = "FirstName", Email = "test@test.pl" };
            var userTo = new User { Id = 2, FirstName = "FirstName", LastName = "FirstName", Email = "test2@test.pl" };
            await _context.User.AddAsync(userFrom);
            await _context.User.AddAsync(userTo);
            var messages = new Message[] { 
                new() { Id = 1, Text = "test", UserIdFrom=1, UserIdTo=2 }, 
                new () { Id = 2, Text = "test2", UserIdFrom = 2, UserIdTo = 1 } 
            };
            await _context.Message.AddRangeAsync(messages);
            await _context.SaveChangesAsync();

            //Act
            var result = await _chatRepository.GetMessages(1, 2, 1, 10);

            //Assert
            Assert.Equal(2, result.Count);

            Assert.Equal("test2", result[0].Text);
            Assert.Equal(2, result[0].UserIdFrom);
            Assert.Equal(1, result[0].UserIdTo);

            Assert.Equal("test", result[1].Text);
            Assert.Equal(1, result[1].UserIdFrom);
            Assert.Equal(2, result[1].UserIdTo);
        }

        [Fact]
        public async Task GetMessages_ShouldThrowNullPageException_WhenThereIsNoMessages()
        {
            //Arrange
            var userFrom = new User { Id = 1, FirstName = "FirstName", LastName = "FirstName", Email = "test@test.pl" };
            var userTo = new User { Id = 2, FirstName = "FirstName", LastName = "FirstName", Email = "test2@test.pl" };
            await _context.User.AddAsync(userFrom);
            await _context.User.AddAsync(userTo);
            await _context.SaveChangesAsync();

            //Act
            var exception = await Assert.ThrowsAsync<NullPageException>(() => _chatRepository.GetMessages(1, 2, 1, 10));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task GetNumberOfNewMessages_ShouldGetNumberOfNewMessages()
        {
            //Arrange
            var userFrom = new User { Id = 1, FirstName = "FirstName", LastName = "FirstName", Email = "test@test.pl" };
            var userTo = new User { Id = 2, FirstName = "FirstName", LastName = "FirstName", Email = "test2@test.pl" };
            await _context.User.AddAsync(userFrom);
            await _context.User.AddAsync(userTo);
            var messages = new Message[] {
                new() { Id = 1, Text = "test", UserIdFrom=1, UserIdTo=2, IsReaded=true },
                new () { Id = 2, Text = "test2", UserIdFrom = 2, UserIdTo = 1, IsReaded=false }
            };
            await _context.Message.AddRangeAsync(messages);
            await _context.SaveChangesAsync();

            //Act
            var result1 = await _chatRepository.GetNumberOfNewMessages(1, 2);
            var result2 = await _chatRepository.GetNumberOfNewMessages(2, 1);

            //Assert
            Assert.Equal(0 ,result1);
            Assert.Equal(1 ,result2);

        }

        [Fact]
        public async Task UpdateMessagesToReaded_ShouldUpdateMessageToReaded()
        {
            //Arrange
            var userFrom = new User { Id = 1, FirstName = "FirstName", LastName = "FirstName", Email = "test@test.pl" };
            var userTo = new User { Id = 2, FirstName = "FirstName", LastName = "FirstName", Email = "test2@test.pl" };
            await _context.User.AddAsync(userFrom);
            await _context.User.AddAsync(userTo);
            var messages = new Message[] {
                new() { Id = 1, Text = "test", UserIdFrom=1, UserIdTo=2, IsReaded=true },
                new () { Id = 2, Text = "test2", UserIdFrom = 2, UserIdTo = 1, IsReaded=false }
            };
            await _context.Message.AddRangeAsync(messages);
            await _context.SaveChangesAsync();

            //Act
            await _chatRepository.UpdateMessagesToReaded(1, 2);
            await _chatRepository.UpdateMessagesToReaded(2, 1);

            var result = await _chatRepository.GetMessage(1);
            var result2 = await _chatRepository.GetMessage(2);

            //Assert
            Assert.True(result.IsReaded);
            Assert.True(result2.IsReaded);

        }

    }
}