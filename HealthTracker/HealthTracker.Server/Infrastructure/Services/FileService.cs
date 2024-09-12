using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;

namespace HealthTracker.Server.Infrastructure.Services
{
    public interface IFileService
    {
        string SaveFile(IFormFile file, string uploadPath);
        void ResizeImage(string filePath, int maxWidth = 1280, int maxHeight = 720);
        void DeleteFile(string filePath);
    }
    public class FileService : IFileService
    {
        public FileService() { }

        public string SaveFile(IFormFile file, string uploadPath)
        {
            var fileName = Guid.NewGuid().ToString() + "_" + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            }
            ResizeImage(filePath);
            return filePath;
        }

        public async void ResizeImage(string filePath, int maxWidth = 640, int maxHeight = 480)
        {
            using (var image = Image.Load(filePath))
            {
                if (image.Width <= maxWidth && image.Height <= maxHeight) return;
                double ratioX = (double)maxWidth / image.Width;
                double ratioY = (double)maxHeight / image.Height;
                double ratio = Math.Min(ratioX, ratioY);

                int newWidth = (int)(image.Width * ratio);
                int newHeight = (int)(image.Height * ratio);

                image.Mutate(x => x.Resize(newWidth, newHeight));

                var encoder = new PngEncoder() { ColorType = PngColorType.Palette };

                await image.SaveAsPngAsync(filePath, encoder);
            }
        }

        public void DeleteFile(string filePath)
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }

    }
}
