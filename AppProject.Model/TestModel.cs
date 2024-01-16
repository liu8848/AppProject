using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AppProject.Model;

[Table("test_model")]
public class TestModel
{
    [Key] [Column("id")] public int id { get; set; }

    [Column("desc")] public string Desc { get; set; }
}