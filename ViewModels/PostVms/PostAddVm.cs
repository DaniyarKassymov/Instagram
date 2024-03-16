using System.ComponentModel.DataAnnotations;

namespace Instagram.ViewModels.PostVms;

public class PostAddVm
{
    public int Id { get; set; }
    [Required(ErrorMessage = "*Поле обязательно к заполнению")]
    [DataType(DataType.Upload)]
    public required IFormFile Image { get; set; }
    [Required(ErrorMessage = "*Поле обязательно к заполнению")]
    public string? Description { get; set; }

    public required string UserId { get; set; }
}