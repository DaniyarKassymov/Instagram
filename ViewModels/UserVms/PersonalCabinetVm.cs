using Instagram.Models;
using Instagram.Util.Enums;

namespace Instagram.ViewModels.UserVms;

public class PersonalCabinetVm
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required string Email { get; set; }
    public required string Avatar { get; set; }
    public string? Name { get; set; }
    public string? Info { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender Gender { get; set; }

    public List<User> Subscribed { get; set; } = new();
    public List<User> Subscribers { get; set; } = new();
    public List<Post> Posts { get; set; } = new();
}