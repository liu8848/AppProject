using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AppProject.Model.Attributes;
using Microsoft.EntityFrameworkCore;

namespace AppProject.Model.Entities.ApplicationModules;

[TableEntity]
[Table("application_module")]
public class ApplicationModule:ApplicationModuleRoot<long>
{
    [Column("is_deleted")]
    [Comment("是否删除")]
    public bool? IsDeleted { get; set; }

    [Column("module_name")]
    [Comment("模块名称")]
    [MaxLength(50)]
    public string ModuleName { get; set; }

    [Column("link_url")]
    [Comment("菜单链接地址")]
    public string LinkUrl { get; set; }

    [Column("controller")]
    [Comment("控制器名称")]
    [MaxLength(200)]
    public string Controller { get; set; }

    [Column("action")]
    [Comment("Action 名称")]
    public string Action { get; set; }
    
    
}