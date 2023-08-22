using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Core.Entities.Base;
//[Table("Companies", Schema ="Base")]
public class Company
{
    [Key]
    public int Id {get; set;}
    [Required]
    [MaxLength(128)]
    public string CompanyName { get; set; }
    [MaxLength(256)]
    public string Address {get; set;}
    [MaxLength(32)]
    public string Phone {get; set;}
    public byte[]? Logo {get; set;}
    [MaxLength(256)]
    public string LogoFileName { get; set; }
}