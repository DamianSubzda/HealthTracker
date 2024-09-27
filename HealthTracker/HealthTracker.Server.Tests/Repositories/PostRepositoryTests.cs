using AutoMapper;
using HealthTracker.Server;
using HealthTracker.Server.Core.Exceptions;
using HealthTracker.Server.Core.Exceptions.Community;
using HealthTracker.Server.Core.Models;
using HealthTracker.Server.Infrastructure.Services;
using HealthTracker.Server.Modules.Community.DTOs;
using HealthTracker.Server.Modules.Community.Models;
using HealthTracker.Server.Modules.Community.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Runtime.ConstrainedExecution;

namespace HealthTracker.Server.Tests.Repositories
{
    public class PostRepositoryTests : BaseRepositoryTests
    {
        private IPostRepository _postRepository;
        private readonly IFileService _fileService;
        public PostRepositoryTests()
        {
            _mapper = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<CreatePostDTO, Post>();
                cfg.CreateMap<Post, PostDTO>();

                cfg.CreateMap<CreateCommentDTO, Comment>();
                cfg.CreateMap<Comment, CommentDTO>()
                    .ForMember(dest => dest.UserFirstName, opt => opt.MapFrom(src => src.User.FirstName))
                    .ForMember(dest => dest.UserLastName, opt => opt.MapFrom(src => src.User.LastName));

                cfg.CreateMap<Like, LikeDTO>();
                cfg.CreateMap<LikeDTO, Like>();

            }).CreateMapper();

            _fileService = new Mock<IFileService>().Object;
            _postRepository = new PostRepository(_context, _mapper, _fileService);
        }

        private async Task<User> CreateUserHelper(int id, string email, string firstName = "FirstName", string lastName = "LastName")
        {
            var user = new User { Id = id, FirstName = firstName, LastName = lastName, Email = email };
            await _context.User.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        private async Task<Post> CreatePostHelper(int id, int userId, string content = "Test Post")
        {
            var post = new Post { Id = id, Content = content, UserId = userId };
            await _context.Post.AddAsync(post);
            await _context.SaveChangesAsync();
            return post;
        }

        private async Task<Comment> CreateCommentHelper(int id, int userId, int postId, int? parenCommentId = null, string content = "Test Comment")
        {
            var comment = new Comment { Id = id, Content = content, PostId = postId, UserId = userId, ParentCommentId = parenCommentId };
            await _context.Comment.AddAsync(comment);
            await _context.SaveChangesAsync();
            return comment;
        }

        private async Task<Like> CreateLikeHelper(int userId, int postId)
        {
            var like = new Like { PostId = postId, UserId = userId };
            await _context.Like.AddAsync(like);
            await _context.SaveChangesAsync();
            return like;
        }

        [Fact]
        public async Task CreatePost_ShouldReturnPostDTO()
        {
            // Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var createPostDTO = new CreatePostDTO { UserId = 1, Content = "Test Post" };

            // Act
            var result = await _postRepository.CreatePost(createPostDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Post", result.Content);
            Assert.Equal(user.FirstName, result.UserFirstName);
            Assert.Equal(user.LastName, result.UserLastName);
        }

        [Fact]
        public async Task CreatePost_ShouldThrowUserNotFoundException()
        {
            // Arrange
            var createPostDTO = new CreatePostDTO { UserId = 999, Content = "Test Post" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<UserNotFoundException>(() => _postRepository.CreatePost(createPostDTO));
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetPost_ShouldReturnPost()
        {
            // Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var post = await CreatePostHelper(1, 1);

            // Act
            var result = await _postRepository.GetPost(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Post", result.Content);
            Assert.Equal(user.FirstName, result.UserFirstName);
            Assert.Equal(user.LastName, result.UserLastName);
        }

        [Fact]
        public async Task GetPost_ShouldThrowPostNotFoundException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<PostNotFoundException>(() => _postRepository.GetPost(999));
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task DeletePost_ShouldDeletePost()
        {
            // Arrange
            await CreateUserHelper(1, "test@test.pl");
            await CreatePostHelper(1, 1);

            // Act
            await _postRepository.DeletePost(1);

            // Assert
            var deletedPost = await _context.Post.FindAsync(1);
            Assert.Null(deletedPost);
        }

        [Fact]
        public async Task DeletePost_ShouldThrowPostNotFoundException()
        {
            // Act & Assert
            var exception = await Assert.ThrowsAsync<PostNotFoundException>(() => _postRepository.DeletePost(999));
            Assert.NotNull(exception);
        }

        [Fact]
        public async Task GetPosts_ShouldGetListOfPosts()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend = await CreateUserHelper(2, "test2@test.pl");
            var friendship = new Friendship { Id = 1, FriendId = friend.Id, UserId = user.Id, Status = Status.Accepted };
            await _context.Friendship.AddAsync(friendship);
            await _context.SaveChangesAsync();

            var post = CreatePostHelper(1, friend.Id);
            var post2 = CreatePostHelper(2, friend.Id);
            var post3 = CreatePostHelper(3, friend.Id);

            //Act
            var result = await _postRepository.GetPosts(user.Id, 10, 1);

            //Assert
            Assert.True(result.Any());
            Assert.Equal(3, result.Count);

            Assert.Equal(post.Id, result[2].Id);
            Assert.Equal(post2.Id, result[1].Id);
            Assert.Equal(post3.Id, result[0].Id);
        }

        [Fact]
        public async Task GetPosts_ShouldThrowNullPageException()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var friend = await CreateUserHelper(2, "test2@test.pl");
            var friendship = new Friendship { Id = 1, FriendId = friend.Id, UserId = user.Id, Status = Status.Accepted };
            await _context.Friendship.AddAsync(friendship);
            await _context.SaveChangesAsync();

            //Act
            var exception = await Assert.ThrowsAsync<NullPageException>(() => _postRepository.GetPosts(user.Id, 10, 1));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task GetUsersPosts_ShouldGetListOfPosts()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");

            var post = await CreatePostHelper(1, user.Id);
            var post2 = await CreatePostHelper(2, user.Id);
            var post3 = await CreatePostHelper(3, user.Id);

            //Act
            var result = await _postRepository.GetUsersPosts(user.Id, 10, 1);

            //Assert
            Assert.True(result.Any());
            Assert.Equal(3, result.Count);

            Assert.Equal(post.Id, result[2].Id);
            Assert.Equal(post2.Id, result[1].Id);
            Assert.Equal(post3.Id, result[0].Id);
        }

        [Fact]
        public async Task GetUsersPosts_ShouldThrowNullPageException()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");

            //Act
            var exception = await Assert.ThrowsAsync<NullPageException>(() => _postRepository.GetUsersPosts(user.Id, 10, 1));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task CreateComment_ShouldReturnComment()
        {
            // Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var post = await CreatePostHelper(1, user.Id);

            var createCommentDTO = new CreateCommentDTO { UserId = user.Id, PostId = post.Id, Content = "Test Comment" };

            // Act
            var result = await _postRepository.CreateComment(null, createCommentDTO);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Test Comment", result.Content);
            Assert.Equal(user.FirstName, result.UserFirstName);
            Assert.Equal(user.LastName, result.UserLastName);
        }

        [Fact]
        public async Task CreateComment_ShouldThrowPostNotFoundException()
        {
            // Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var createCommentDTO = new CreateCommentDTO { UserId = 1, PostId = 1, Content = "Test Comment" };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<PostNotFoundException>(() => _postRepository.CreateComment(null, createCommentDTO));
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task GetComment_ShouldReturnComment()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            await CreateCommentHelper(1, user.Id, 1);

            //Act
            var result = await _postRepository.GetComment(1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal("Test Comment", result.Content);
            Assert.Equal("FirstName", result.UserFirstName);
            Assert.Equal("LastName", result.UserLastName);
        }

        [Fact]
        public async Task GetComment_ShouldThrowCommentNotFoundException()
        {
            //Act
            var exception = await Assert.ThrowsAsync<CommentNotFoundException>(() => _postRepository.GetComment(1));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task GetCommentsByPostId_ShouldReturnCommentFromPost()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var post = await CreatePostHelper(1, user.Id);
            var comment = await CreateCommentHelper(1, user.Id, post.Id);
            var comment2 = await CreateCommentHelper(2, user.Id, post.Id);
            var comment3 = await CreateCommentHelper(3, user.Id, post.Id);

            //Act
            var result = await _postRepository.GetCommentsByPostId(post.Id, 1, 10);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(3, result.Comments.Count);
            Assert.True(result.TotalCommentsLeft == 0);

            Assert.Equal(comment.Id, result.Comments.ToList()[2].Id);
            Assert.Equal(comment2.Id, result.Comments.ToList()[1].Id);
            Assert.Equal(comment3.Id, result.Comments.ToList()[0].Id);
        }

        [Fact]
        public async Task GetCommentsByParentCommentId_ShouldReturnListOfCommentsFromParentComment()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var post = await CreatePostHelper(1, user.Id);
            var comment = await CreateCommentHelper(1, user.Id, post.Id);
            var childComment = await CreateCommentHelper(2, user.Id, post.Id, comment.Id);
            var childComment2 = await CreateCommentHelper(3, user.Id, post.Id, comment.Id);

            //Act
            var result = await _postRepository.GetCommentsByParentCommentId(post.Id, 1);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);

            Assert.Equal(childComment.Id, result.ToList()[1].Id);
            Assert.Equal(childComment2.Id, result.ToList()[0].Id);
        }

        [Fact]
        public async Task DeleteComment_ShouldDeleteComment()
        {
            //Arrange
            var comment = await CreateCommentHelper(1, 1, 1);

            //Act
            await _postRepository.DeleteComment(comment.Id);
            var post = await _context.Comment.FirstOrDefaultAsync(c => c.Id == comment.Id);

            //Assert
            Assert.Null(post);
        }

        [Fact]
        public async Task DeleteComment_ShouldThrowCommentNotFoundException()
        {
            //Act
            var exception = await Assert.ThrowsAsync<CommentNotFoundException>(() => _postRepository.DeleteComment(1));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task DeleteCommentsFromPost_ShouldDeleteComments()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var post = await CreatePostHelper(1, user.Id);
            var comment = await CreateCommentHelper(1, user.Id, post.Id);
            var childComment = await CreateCommentHelper(2, user.Id, post.Id, comment.Id);
            var childComment2 = await CreateCommentHelper(3, user.Id, post.Id, comment.Id);
            var childComment3 = await CreateCommentHelper(4, user.Id, post.Id, childComment.Id);

            //Act
            await _postRepository.DeleteCommentsFromPost(post.Id);
            var comments = await _context.Comment.Where(c => c.PostId == post.Id).ToListAsync();

            //
            Assert.Empty(comments);
        }

        [Fact]
        public async Task DeleteUserComments_ShouldDeleteComments()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@test.pl");
            var post = await CreatePostHelper(1, user.Id);
            var comment = await CreateCommentHelper(1, user.Id, post.Id);
            var childComment = await CreateCommentHelper(2, user.Id, post.Id, comment.Id);
            var childComment2 = await CreateCommentHelper(3, user.Id, post.Id, comment.Id);
            var childComment3 = await CreateCommentHelper(4, user.Id, post.Id, childComment.Id);

            //Act
            await _postRepository.DeleteUserComments(user.Id);
            var comments = await _context.Comment.Where(c => c.PostId == post.Id).ToListAsync();

            //
            Assert.Empty(comments);
        }

        [Fact]
        public async Task CreateLike_ShouldCreateLike()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@tes.pl");
            var post = await CreatePostHelper(1, user.Id);
            var like = new LikeDTO { PostId = 1, UserId = user.Id };

            //Act
            var result = await _postRepository.CreateLike(like);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(post.Id, result.PostId);
        }

        [Fact]
        public async Task CreateLike_ShouldThrowPostNotFoundException()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@tes.pl");
            var like = new LikeDTO { PostId = 1, UserId = user.Id };

            //Act
            var exception = await Assert.ThrowsAsync<PostNotFoundException>(() => _postRepository.CreateLike(like));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task CreateLike_ShouldThrowLikeAlreadyExistsException()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@tes.pl");
            var post = await CreatePostHelper(1, user.Id);
            var like = await CreateLikeHelper(user.Id, post.Id);
            var like2 = new LikeDTO { PostId = 1, UserId = user.Id };

            //Act
            var exception = await Assert.ThrowsAsync<LikeAlreadyExistsException>(() => _postRepository.CreateLike(like2));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task GetLike_ShouldReturnLike()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@tes.pl");
            var post = await CreatePostHelper(1, user.Id);
            var like = await CreateLikeHelper(user.Id, post.Id);

            //Act
            var result = await _postRepository.GetLike(user.Id, post.Id);

            //Assert
            Assert.NotNull(result);
            Assert.Equal(user.Id, result.UserId);
            Assert.Equal(post.Id, result.PostId);
        }

        [Fact]
        public async Task GetLike_ShouldThrowLikeNotFoundException()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@tes.pl");
            var post = await CreatePostHelper(1, user.Id);

            //Act
            var exception = await Assert.ThrowsAsync<LikeNotFoundException>(() => _postRepository.GetLike(user.Id, post.Id));

            //Assert
            Assert.NotNull(exception.Message);
        }

        [Fact]
        public async Task GetLikesFromPost_ShouldReturnLikes()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@tes.pl");
            var user2 = await CreateUserHelper(2, "test2@tes.pl");
            var post = await CreatePostHelper(1, user.Id);
            var like = await CreateLikeHelper(user.Id, post.Id);
            var like2 = await CreateLikeHelper(user2.Id, post.Id);

            //Act
            var result = await _postRepository.GetLikesFromPost(post.Id);

            //Assert
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Equal(like.UserId, result[0].UserId);
            Assert.Equal(like2.UserId, result[1].UserId);
        }

        [Fact]
        public async Task GetLikesFromPost_ShouldReturnemptyList()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@tes.pl");
            var post = await CreatePostHelper(1, user.Id);

            //Act
            var result = await _postRepository.GetLikesFromPost(post.Id);

            //Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task DeleteLike_ShouldDeleteLike()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@tes.pl");
            var post = await CreatePostHelper(1, user.Id);
            var like = await CreateLikeHelper(user.Id, post.Id);

            //Act
            await _postRepository.DeleteLike(user.Id, post.Id);
            like = await _context.Like.FindAsync(user.Id, post.Id);
            //Assert
            Assert.Null(like);
        }

        [Fact]
        public async Task DeleteLike_ShouldThrowLikeNotFoundException()
        {
            //Arrange
            var user = await CreateUserHelper(1, "test@tes.pl");
            var post = await CreatePostHelper(1, user.Id);

            //Act
            var exception = await Assert.ThrowsAsync<LikeNotFoundException>(()=> _postRepository.DeleteLike(user.Id, post.Id));

            //Assert
            Assert.NotNull(exception.Message);
        }
    }
}