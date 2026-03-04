using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BPlatform.Shared.Entities;

[Table("CompanyOwner")]
public class CompanyOwnerEntity
{
    [Key]
    [Column("company_owner_id")]
    public long Id { get; set; }
    [Column("company_id")]
    public long CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public required CompanyEntity Company { get; set; }
    [Column("password")]
    public string? Password { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    [Column("deleted_at")] 
    public DateTime? DeletedAt { get; set; }
}