using Microsoft.Extensions.FileProviders;

namespace Instagram.Models;

public class Post
{
    public int Id { get; set; }
    public required string Image { get; init; }
    public string? Description { get; set; }

    public required string UserId { get; set; }
    public User? User { get; set; }

    public List<Like> Likes { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
}