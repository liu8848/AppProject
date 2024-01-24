using System.ComponentModel.DataAnnotations;

namespace AppProject.Model.Entities.Identities;

public record UserForAuthentication
{
    [Required(ErrorMessage = "用户名为必填项")]
    public string UserName { get; set; }
    [Required(ErrorMessage = "密码为必填项")]
    public string Password { get; set; }
}