using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BPlatform.Shared.Entities;

[Table("BusinessUnit")]
public class BusinessUnitEntity
{
    [Key]
    [Column("business_unit_id")]
    public long Id { get; set; }
    [Column("business_unit_name")]
    [MaxLength(50)]
    public required string BusinessUnitName { get; set; }
    [Column("company_id")]
    public long CompanyId { get; set; }
    [ForeignKey(nameof(CompanyId))]
    public required CompanyEntity Company { get; set; }
    [Column("company_owner_id")]
    public long CompanyOwnerId { get; set; }
    [ForeignKey(nameof(CompanyOwnerId))]
    public required CompanyOwnerEntity CompanyOwner { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    [Column("deleted_at")] 
    public DateTime? DeletedAt { get; set; }
}