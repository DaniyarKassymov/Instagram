using System.ComponentModel.DataAnnotations;
using Instagram.Util.Enums;
using Microsoft.AspNetCore.Mvc;

namespace Instagram.ViewModels.UserVms;

public class RegisterVm
{
    [Required(ErrorMessage = "*Поле обязательно к заполнению")]
    [Remote("CheckUserName", "Validation", ErrorMessage = "*Пользователь с таким логином уже существует")]
    public required string UserName { get; set; }
    [Required(ErrorMessage = "*Поле обязательно к заполнению")]
    [EmailAddress(ErrorMessage = "*Вы ввели адрес в неверном формате")]
    [Remote("CheckEmail", "Validation", ErrorMessage = "*Пользователь с таким эл. адресом уже существует")]
    public required string Email { get; set; }
    [Required(ErrorMessage = "*Поле обязательно к заполнению")]
    [DataType(DataType.Upload)]
    public required IFormFile Avatar { get; set; }
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "*Поле обязательно к заполнению")]
    public required string Password { get; set; }
    [DataType(DataType.Password)]
    [Required(ErrorMessage = "*Поле обязательно к заполнению")]
    [Compare(nameof(Password), ErrorMessage = "*Пароли не совпадают")]
    public required string ConfirmPassword { get; set; }
    public string? Name { get; set; }
    public string? Info { get; set; }
    public string? PhoneNumber { get; set; }
    public Gender Gender { get; set; }
}