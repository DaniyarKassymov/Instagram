using System.ComponentModel.DataAnnotations;

namespace Instagram.ViewModels.PostVms;

public class EditVm
{
    public int Id { get; set; }
    [Required(ErrorMessage = "Поле описание обязательно")]
    [StringLength(50, MinimumLength = 10, ErrorMessage = "Минимум 10 символов")]
    public required string Description { get; set; }
}