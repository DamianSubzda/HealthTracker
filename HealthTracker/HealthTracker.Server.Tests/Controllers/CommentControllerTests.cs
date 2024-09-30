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
    public class CommentControllerTests
    {
        private readonly Mock<IPostRepository> _postRepositoryMock;
        private readonly Mock<ILogger<CommentController>> _loggerMock;
        private readonly CommentController _commentController;
        public CommentControllerTests()
        {
            _postRepositoryMock = new Mock<IPostRepository>();
            _loggerMock = new Mock<ILogger<CommentController>>();

            _commentController = new CommentController(
                _postRepositoryMock.Object,
                _loggerMock.Object
            );
        }

        [Fact]
        public async Task CreateComment_ShouldReturnCreatedAtAction()
        {
            // Arrange
            var commentDto = new CreateCommentDTO { Content = "Test comment", UserId = 1, PostId = 1 };
            var createdComment = new CommentDTO { Id = 1, Content = "Test comment", UserId = 1, PostId = 1 };

            _postRepositoryMock.Setup(x => x.CreateComment(null, commentDto)).ReturnsAsync(createdComment);

            // Act
            var result = await _commentController.CreateComment(null, commentDto);

            // Assert
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(201, createdAtActionResult.StatusCode);
            Assert.Equal(nameof(_commentController.GetComment), createdAtActionResult.ActionName);
            Assert.Equal(createdComment, createdAtActionResult.Value);
        }

        [Fact]
        public async Task CreateComment_ShouldReturnNotFound()
        {
            // Arrange
            var commentDto = new CreateCommentDTO { Content = "Test comment", UserId = 999, PostId = 1 };

            _postRepositoryMock.Setup(x => x.CreateComment(null, commentDto)).ThrowsAsync(new UserNotFoundException());

            // Act
            var result = await _commentController.CreateComment(null, commentDto);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetComment_ShouldReturnOk()
        {
            // Arrange
            var commentDto = new CommentDTO { Id = 1, Content = "Test comment", UserId = 1, PostId = 1 };

            _postRepositoryMock.Setup(x => x.GetComment(commentDto.Id)).ReturnsAsync(commentDto);

            // Act
            var result = await _commentController.GetComment(commentDto.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(commentDto, okResult.Value);
        }

        [Fact]
        public async Task GetComment_ShouldReturnNotFound()
        {
            // Arrange
            var commentId = 1;

            _postRepositoryMock.Setup(x => x.GetComment(commentId)).ThrowsAsync(new CommentNotFoundException());

            // Act
            var result = await _commentController.GetComment(commentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task DeleteComment_ShouldReturnOk()
        {
            // Arrange
            var commentId = 1;

            _postRepositoryMock.Setup(x => x.DeleteComment(commentId)).Returns(Task.CompletedTask);

            // Act
            var result = await _commentController.DeleteComment(commentId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteComment_ShouldReturnNotFound()
        {
            // Arrange
            var commentId = 999;

            _postRepositoryMock.Setup(x => x.DeleteComment(commentId)).ThrowsAsync(new CommentNotFoundException());

            // Act
            var result = await _commentController.DeleteComment(commentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task GetCommentsByPostId_ShouldReturnOk()
        {
            // Arrange
            var postId = 1;
            var comments = new CommentFromPostDTO
            {
                Comments = new List<CommentDTO>
        {
            new CommentDTO { Id = 1, Content = "Test comment" },
            new CommentDTO { Id = 2, Content = "Another comment" }
        }
            };

            _postRepositoryMock.Setup(x => x.GetCommentsByPostId(postId, 1, 10)).ReturnsAsync(comments);

            // Act
            var result = await _commentController.GetCommentsByPostId(postId, 1, 10);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            Assert.Equal(200, okResult.StatusCode);
            Assert.Equal(comments, okResult.Value);
        }

        [Fact]
        public async Task GetCommentsByPostId_ShouldReturnNotFound()
        {
            // Arrange
            var postId = 1;

            _postRepositoryMock.Setup(x => x.GetCommentsByPostId(postId, 1, 10)).ThrowsAsync(new PostNotFoundException());

            // Act
            var result = await _commentController.GetCommentsByPostId(postId, 1, 10);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result.Result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCommentFromPost_ShouldReturnOk()
        {
            // Arrange
            var postId = 1;

            _postRepositoryMock.Setup(x => x.DeleteCommentsFromPost(postId)).Returns(Task.CompletedTask);

            // Act
            var result = await _commentController.DeleteCommentFromPost(postId);

            // Assert
            var okResult = Assert.IsType<OkResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }

        [Fact]
        public async Task DeleteCommentFromPost_ShouldReturnNotFound()
        {
            // Arrange
            var postId = 999;

            _postRepositoryMock.Setup(x => x.DeleteCommentsFromPost(postId)).ThrowsAsync(new PostNotFoundException());

            // Act
            var result = await _commentController.DeleteCommentFromPost(postId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(404, notFoundResult.StatusCode);
        }

    }
}