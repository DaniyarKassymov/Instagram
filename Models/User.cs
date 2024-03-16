using Instagram.Util.Enums;
using Microsoft.AspNetCore.Identity;        

namespace Instagram.Models;

public class User : IdentityUser
{
    public required string Avatar { get; set; }
    public string? Name { get; set; }
    public string? Info { get; set; }
    public Gender Gender { get; set; }

    public List<User> Subscribed { get; set; } = new();
    public List<User> Subscribers { get; set; } = new();
    public List<Post> Posts { get; set; } = new();
    public List<Like> Likes { get; set; } = new();
    public List<Comment> Comments { get; set; } = new();
}