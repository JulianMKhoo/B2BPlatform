using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BPlatform.Shared.Entities;

[Table("EmployeeData")]
public class EmployeeDataEntity
{
    [Key]
    [Column("employee_data_id")]
    public long Id { get; set; }
    [Column("email")]
    [EmailAddress(ErrorMessage =  "Not a valid email address")]
    [MaxLength(50)]
    public required string Email { get; set; }
    [Column("password")]
    [MaxLength(50)]
    public required string Password { get; set; }
    [Column("team_name")]
    [MaxLength(50)]
    public required string TeamName { get; set; }
    [Column("role")]
    public required short Role { get; set; }
    [Column("employee_id")]
    public long EmployeeId { get; set; }
    [ForeignKey(nameof(EmployeeId))]
    public required EmployeeEntity Employee { get; set; }
    [Column("unit_id")]
    public long UnitId { get; set; }
    [ForeignKey(nameof(UnitId))]
    public required BusinessUnitEntity BusinessUnit { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    [Column("deleted_at")] 
    public DateTime? DeletedAt { get; set; }
}