using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace AppProject.Model;

public abstract class AuditableEntity
{
    [Column("created_time")]
    [Comment("创建时间")]
    public DateTime CreatedTime { get; set; }
    
    [Column("created_by")]
    [Comment("创建人")]
    public string? CreatedBy { get; set; }
    
    [Column("last_modified")]
    [Comment("修改时间")]
    public DateTime? LastModified { get; set; }
    
    [Column("last_modified_by")]
    [Comment("修改人")]
    public string? LastModifiedBy { get; set; }
}