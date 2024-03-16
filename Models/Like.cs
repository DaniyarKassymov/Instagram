namespace Instagram.Models;

public class Like
{
    public int Id { get; set; }

    public required int PostId { get; set; }
    public Post? Post { get; set; }
    public required string UserId { get; set; }
    public User? User { get; set; }
}