using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BPlatform.Shared.Entities;

[Table("Employee")]
public class EmployeeEntity
{
    [Key]
    [Column("employee_id")]
    public long Id { get; set; }
    [Column("first_name")]
    [MaxLength(50)]
    public required string FirstName { get; set; }
    [Column("last_name")]
    [MaxLength(50)]
    public required string LastName { get; set; }
    [Column("position")]
    public required short Position { get; set; }
    [Column("company_id")]
    public long CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public required CompanyEntity Company { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    [Column("deleted_at")] 
    public DateTime? DeletedAt { get; set; }
}