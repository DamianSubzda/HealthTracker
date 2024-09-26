using HealthTracker.Server;
using HealthTracker.Server.Infrastructure.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace HealthTracker.Server.Tests.Services
{
    public class FileServiceTests
    {
        private readonly Mock<IFormFile> _fileMock;
        private readonly FileService _fileService;
        private readonly string _uploadPath = "";
        private readonly string _filePath = "test_image.png";

        public FileServiceTests()
        {
            _fileMock = new Mock<IFormFile>();
            _fileService = new FileService();
        }

        [Fact]
        public void SaveFile_ShouldSaveFileAndResizeImage()
        {
            // Arrange
            var fileName = "test_image.png";
            var ms = new MemoryStream();
            using (var image = new Image<Rgba32>(100, 100))
            {
                image.SaveAsPng(ms);
                ms.Position = 0;
            }

            _fileMock.Setup(f => f.FileName).Returns(fileName);
            _fileMock.Setup(f => f.CopyTo(It.IsAny<Stream>())).Callback<Stream>(s => ms.CopyTo(s));

            // Act
            var savedFilePath = _fileService.SaveFile(_fileMock.Object, _uploadPath);

            // Assert
            Assert.True(File.Exists(savedFilePath));
            File.Delete(savedFilePath);
        }

        [Fact]
        public async Task ResizeImage_ShouldResizeLargerImages()
        {
            // Arrange
            var imagePath = _filePath;
            using (var image = new Image<Rgba32>(2000, 1500))
            {
                await image.SaveAsPngAsync(imagePath);
            }

            // Act
            _fileService.ResizeImage(imagePath, 1280, 720);

            // Assert
            using (var resizedImage = Image.Load(imagePath))
            {
                Assert.True(resizedImage.Width <= 1280);
                Assert.True(resizedImage.Height <= 720);
            }

            File.Delete(imagePath);
        }

        [Fact]
        public void DeleteFile_ShouldDeleteExistingFile()
        {
            // Arrange
            var filePath = _filePath;
            File.WriteAllText(filePath, "test content");

            // Act
            _fileService.DeleteFile(filePath);

            // Assert
            Assert.False(File.Exists(filePath));
        }

        [Fact]
        public void DeleteFile_ShouldNotThrow_WhenFileDoesNotExist()
        {
            // Arrange
            var nonExistentFilePath = "non_existent_file.png";

            // Act
            var exception = Record.Exception(() => _fileService.DeleteFile(nonExistentFilePath));

            //Assert
            Assert.Null(exception);
        }
    }
}