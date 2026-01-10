using System.Security.Cryptography;

namespace MagicMatchTracker.Features.Shared.Services;

public sealed class ImageCachingService(HttpClient httpClient, IWebHostEnvironment webHostEnvironment)
{
	/// <summary>
	/// Downloads an image from the provided URL and caches it in the specified subfolder in <c>/wwwroot/img/cache</c> folder.
	/// </summary>
	/// <returns>The path to the cached image file.</returns>
	public async Task<string> CacheImageAsync(string folder, string imageUrl, CancellationToken cancellationToken = default)
	{
		if (!Uri.IsWellFormedUriString(imageUrl, UriKind.Absolute))
			throw new ArgumentException("Image URL must be a valid absolute URI string", nameof(imageUrl));

		var imageBytes = await httpClient.GetByteArrayAsync(imageUrl, cancellationToken);

		var extension = Path.GetExtension(new Uri(imageUrl).LocalPath).TrimToNull();
		var fileName = CreateFileName(imageBytes, extension);

		var relativePath = Path.Combine("img", "cache", folder, fileName);
		var fullPath = Path.Combine(webHostEnvironment.WebRootPath, relativePath);

		var directory = Path.GetDirectoryName(fullPath);
		if (directory.IsNotEmpty())
			Directory.CreateDirectory(directory);

		await File.WriteAllBytesAsync(fullPath, imageBytes, cancellationToken);

		return $"/{relativePath.Replace("\\", "/")}";
	}

	private static string CreateFileName(byte[] data, string? extension)
	{
		var hashBytes = SHA256.HashData(data);
		var hash = Convert.ToHexString(hashBytes).ToLowerInvariant();

		return $"{hash}{extension ?? ".jpg"}";
	}
}