using Instagram.Models;
using Instagram.Util.Services;
using Instagram.ViewModels.PostVms;

namespace Instagram.Util.Mappers;

public static class PostMapper
{
    public static Post PostAddVmPost(PostAddVm vm, string image)
    {
        return new Post()
        {
            Image = image,
            UserId = vm.UserId,
            Description = vm.Description
        };
    }

    public static PostDetailsVm PostPostDetailsVm(Post post)
    {
        return new PostDetailsVm()
        {
            Image = post.Image,
            UserId = post.UserId,
            Description = post.Description,
            User = post.User,
            Comments = post.Comments,
            Id = post.Id,
            Likes = post.Likes
        };
    }
}