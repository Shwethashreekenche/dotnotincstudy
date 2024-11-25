using System.ComponentModel.DataAnnotations;

namespace Schemes_for_Farmers.Models;
public class Farmer
{
    [Key]
    public string FarmerId{get; set;}
    [Required]
    public string FullName{get; set;}
    [Required]
    public string ContactNo{get; set;}
    [Required]
    public string EmailId{get; set;}

    public string Address{get; set;}

    public double LandArea{get; set;}
    public string LandAddress{get; set;}
    [Required]
    public string AccountNo{get; set;}
    [Required]
    public string IFSCCode{get; set;}
    [Required]
    public byte[] Aadhaar{get; set;}
    [Required]
    public byte[] Pan{get; set;}
    [Required]
    public byte[] Certificate{get; set;}
    [Required]
    public string Password{get; set;}  

    public string? GrantAccess{get; set;}

    public ICollection<Crop> Crops { get; set; } = new List<Crop>();
    public ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
}