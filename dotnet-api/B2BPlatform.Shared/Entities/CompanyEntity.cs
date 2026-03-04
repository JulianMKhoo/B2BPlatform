using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace B2BPlatform.Shared.Entities;

[Table("Company")]
public class CompanyEntity
{
    [Key]
    [Column("company_id")]
    public long Id { get; set; }
    [Column("company_name")]
    [MaxLength(50)]
    public required string CompanyName { get; set; }
    [Column("company_address")]
    [MaxLength(50)]
    public string? CompanyAddress { get; set; }
    [Column("contract_number")]
    public int ContractNumber { get; set; }
    [Column("contact_number")]
    [MaxLength(20)]
    public string? ContactNumber { get; set; }
    [Column("created_at")]
    public DateTime CreatedAt { get; set; }
    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
    [Column("deleted_at")] 
    public DateTime? DeletedAt { get; set; }
}