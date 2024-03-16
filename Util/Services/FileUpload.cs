namespace Instagram.Util.Services;

public static class FileUpload
{
    public static async Task<string> Upload(string? name, IFormFile file)
    {
        name = name?.Replace(' ', '_');
        var basePath = Path.Combine("wwwroot", "uploads");

        if (!Directory.Exists(basePath))
        {
            Directory.CreateDirectory(basePath);
        }

        var extension = Path.GetExtension(file.FileName);
        var filePath = Path.Combine(basePath, $"{name}{extension}");
        
        await using var stream = File.Create(filePath);
        await file.CopyToAsync(stream);
        
        return Path.Combine("uploads", $"{name}{extension}");
    }
}