using AgriStream_ST10157545_Practium.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AgriStream_ST10157545_Practium.Controllers
{
    [Authorize(Roles = AppRoles.AllRoles)]

    public class ImageController
    {
        private const string ContainerName = "drone-images";
        private const long MaxBytes = 20 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".tif", ".tiff" };
        private readonly BlobServiceClient _blobs;
        private readonly ILogger _logger;
        public ImagesController(BlobServiceClient blobs, ILogger logger)
        {
            _blobs = blobs;
            _logger = logger;
        }
        [HttpGet]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var container = _blobs.GetBlobContainerClient(ContainerName);
            await container.CreateIfNotExistsAsync(cancellationToken: ct);
            var names = new List<string>();
            await foreach (var item in container.GetBlobsAsync(cancellationToken: ct))
            {
                names.Add(item.Name);
            }
            return View(new ImagesViewModel { BlobNames = names });
        }
        [HttpPost]
        [RequestSizeLimit(MaxBytes)]
        public async Task<IActionResult> Upload(IFormFile? image, CancellationToken ct)
        {
            if (image is null || image.Length == 0)
            {
                TempData["StatusMessage"] = "Please choose a file.";
                return RedirectToAction(nameof(Index));
            }
            var extension = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension) || image.Length > MaxBytes)
            {
                TempData["StatusMessage"] = "Unsupported file type or file too large.";
                return RedirectToAction(nameof(Index));
            }
            var container = _blobs.GetBlobContainerClient(ContainerName);
            await container.CreateIfNotExistsAsync(cancellationToken: ct);
            var blobName = $"{Guid.NewGuid()}{extension}";
            await using var stream = image.OpenReadStream();
            await container.GetBlobClient(blobName).UploadAsync(stream, overwrite: false, cancellationToken: ct);
            TempData["StatusMessage"] = $"Uploaded {blobName}. The blob-trigger function will log it.";
            return RedirectToAction(nameof(Index));
        }
    }
}
