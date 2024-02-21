using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppProject.Model.Entities.ApplicationModules;

public class ApplicationModuleRoot<TKey>:AuditableEntity where TKey:IEquatable<TKey>
{
    [Column("id")]
    [Key]
    [Comment("主键id")]
    public TKey Id { get; set; }

    [Column("parent_id")]
    [Comment("父id")]
    public TKey ParentId { get; set; }
}