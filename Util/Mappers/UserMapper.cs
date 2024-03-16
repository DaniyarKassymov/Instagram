using Instagram.Models;
using Instagram.Util.Services;
using Instagram.ViewModels.UserVms;

namespace Instagram.Util.Mappers;

public static class UserMapper
{
    public static User RegisterVmUser(RegisterVm vm, string avatar)
    {
        return new User()
        {
            UserName = vm.UserName,
            Email = vm.Email,
            Avatar = avatar,
            Name = vm.Name,
            Info = vm.Info,
            PhoneNumber = vm.PhoneNumber,
            Gender = vm.Gender
        };
    }

    public static PersonalCabinetVm UserPersonalCabinetVm(User? user)
    {
        return new PersonalCabinetVm()
        {
            Id = user.Id,
            UserName = user.UserName,
            Email = user.Email,
            Avatar = user.Avatar,
            Gender = user.Gender,
            Info = user.Info,
            Name = user.Name,
            PhoneNumber = user.PhoneNumber,
            Posts = user.Posts,
            Subscribed = user.Subscribed,
            Subscribers = user.Subscribers
        };
    }
}