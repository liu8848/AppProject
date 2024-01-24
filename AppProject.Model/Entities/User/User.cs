using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppProject.Model.Entities.User;

[Table("user")]
public class User:AuditableEntity
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public int Id { get; set; }
    
    [Column("user_name")]
    [Comment("用户名")]
    public string UserName { get; set; }
    
    [Column("user_nickname")]
    [Comment("用户别名")]
    public string UserNickName { get; set; }
    
    [Column("password")]
    [Comment("用户密码")]
    public string Password { get; set; }
}