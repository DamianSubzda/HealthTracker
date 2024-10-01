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
    public class PostControllerTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<ILogger<PostController>> _loggerMock;
        private readonly PostController _postController;
        public PostControllerTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _loggerMock = new Mock<ILogger<PostController>>();

            _postController = new PostController(
                _postRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreatePost_ShouldReturnCreateAtAction()
        {
            //Arrange
            var createPost = new CreatePostDTO { UserId = 1, Content = "Test post." };
            var post = new PostDTO { Id = 1, UserId = 1, Content = "Test post." };

            _postRepositoryMock.Setup(x => x.CreatePost(createPost)).ReturnsAsync(post);

            //Act
            var result = await _postController.CreatePost(createPost);

            //Assert
            var actionResult = Assert.IsType<ActionResult<PostDTO>>(result);
            var createAtActionResut = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

            Assert.Equal(nameof(_postController.GetPost), createAtActionResut.ActionName);
            Assert.Equal(post, createAtActionResut.Value);
            Assert.Equal(201, createAtActionResut.StatusCode);
        }


        [Fact]
        public async Task CreatePost_ShouldReturnBadRequest()
        {
            // Arrange
            var createPostDto = new CreatePostDTO { UserId = 1, Content = "Test post" };
            _postRepositoryMock.Setup(x => x.CreatePost(createPostDto)).ThrowsAsync(new DbUpdateException());

            // Act
            var result = await _postController.CreatePost(createPostDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result.Result);
            Assert.Equal(400, badRequestResult.StatusCode);
            Assert.Equal("Database error.", badRequestResult.Value);
        }

        [Fact]
        public async Task CreatePost_ShouldReturnNotFound()
        {
            // Arrange
            var createPostDto = new CreatePostDTO { UserId = 999, Content = "Test post" };
            _postRepositoryMock.Setup(x => x.CreatePost(createPostDto)).ThrowsAsync(new UserNotFoundException());

            // Act
            var result = await _postController.CreatePost(createPostDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetPost_ShouldReturnOk()
        {
            // Arrange
            var postId = 1;
            var postDto = new PostDTO { Id = postId, Content = "Test post" };

            _postRepositoryMock.Setup(x => x.GetPost(postId)).ReturnsAsync(postDto);

            // Act
            var result = await _postController.GetPost(postId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<PostDTO>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(postDto, returnValue);
        }

        [Fact]
        public async Task GetPost_ShouldReturnNotFound()
        {
            // Arrange
            var postId = 999;
            _postRepositoryMock.Setup(x => x.GetPost(postId)).ThrowsAsync(new PostNotFoundException());

            // Act
            var result = await _postController.GetPost(postId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetPosts_ShouldReturnOk()
        {
            //Arrange
            var userId = 1;
            var pageNr = 1;
            var posts = new List<PostDTO>
            {
                new PostDTO { Id = 1},
                new PostDTO { Id = 2}
            };

            _postRepositoryMock.Setup(x => x.GetPosts(userId, pageNr, 10)).ReturnsAsync(posts);

            //Act
            var result = await _postController.GetPosts(userId, pageNr);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<PostDTO>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Contains(returnValue, p => p.Id == posts[0].Id);
            Assert.Contains(returnValue, p => p.Id == posts[1].Id);

        }

        [Fact]
        public async Task GetUserPosts_ShouldReturnOk()
        {
            //Arrange
            var userId = 1;
            var pageNr = 1;
            var posts = new List<PostDTO>
            {
                new PostDTO { Id = 1},
                new PostDTO { Id = 2}
            };

            _postRepositoryMock.Setup(x => x.GetUserPosts(userId, pageNr, 10)).ReturnsAsync(posts);

            //Act
            var result = await _postController.GetUserPosts(userId, pageNr, 10);

            //Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<PostDTO>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Contains(returnValue, p => p.Id == posts[0].Id);
            Assert.Contains(returnValue, p => p.Id == posts[1].Id);

        }

        [Fact]
        public async Task DeletePost_ShouldReturnOk()
        {
            // Arrange
            var postId = 1;
            _postRepositoryMock.Setup(x => x.DeletePost(postId)).Returns(Task.CompletedTask);

            // Act
            var result = await _postController.DeletePost(postId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task DeletePost_ShouldReturnNotFound()
        {
            // Arrange
            var postId = 999;
            _postRepositoryMock.Setup(x => x.DeletePost(postId)).ThrowsAsync(new PostNotFoundException());

            // Act
            var result = await _postController.DeletePost(postId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task CreateLike_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var likeDto = new LikeDTO { UserId = 1, PostId = 1 };
            _postRepositoryMock.Setup(x => x.CreateLike(likeDto)).ReturnsAsync(likeDto);

            // Act
            var result = await _postController.CreateLike(likeDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
            Assert.Equal(likeDto, createdAtActionResult.Value);
        }

        [Fact]
        public async Task CreateLike_ShouldReturnBadRequest()
        {
            // Arrange
            var likeDto = new LikeDTO { UserId = 1, PostId = 1 };
            _postRepositoryMock.Setup(x => x.CreateLike(likeDto)).ThrowsAsync(new LikeAlreadyExistsException());

            // Act
            var result = await _postController.CreateLike(likeDto);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task CreateLike_ShouldReturnNotFound()
        {
            // Arrange
            var likeDto = new LikeDTO { UserId = 1, PostId = 1 };
            _postRepositoryMock.Setup(x => x.CreateLike(likeDto)).ThrowsAsync(new PostNotFoundException());

            // Act
            var result = await _postController.CreateLike(likeDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetLike_ShouldReturnOk()
        {
            // Arrange
            var postId = 1;
            var userId = 1;
            var like = new LikeDTO { UserId = 1, PostId = 1 };

            _postRepositoryMock.Setup(x => x.GetLike(userId, postId)).ReturnsAsync(like);

            // Act
            var result = await _postController.GetLike(userId, postId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<LikeDTO>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(like, returnValue);
        }

        [Fact]
        public async Task GetLike_ShouldReturnNotFound()
        {
            // Arrange
            var postId = 1;
            var userId = 1;

            _postRepositoryMock.Setup(x => x.GetLike(userId, postId)).ThrowsAsync(new LikeNotFoundException());

            // Act
            var result = await _postController.GetLike(userId, postId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);

            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetLikesFromPost_ShouldReturnOk()
        {
            // Arrange
            var postId = 1;
            var likes = new List<LikeDTO>
            {
                new LikeDTO { UserId = 1, PostId = 1 },
                new LikeDTO { UserId = 2, PostId = 1 }
            };

            _postRepositoryMock.Setup(x => x.GetLikesFromPost(postId)).ReturnsAsync(likes);

            // Act
            var result = await _postController.GetLikesFromPost(postId);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnValue = Assert.IsType<List<LikeDTO>>(okResult.Value);

            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(likes, returnValue);
        }

        [Fact]
        public async Task DeleteLike_ShouldReturnOk()
        {
            // Arrange
            var postId = 1;
            var userId = 1;

            // Act
            var result = await _postController.DeleteLike(userId, postId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);

            Assert.Equal(200, okResult.StatusCode);
        }

    }
}