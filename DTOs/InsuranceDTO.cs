using System.ComponentModel.DataAnnotations;

public class InsuranceDTO
{
    public string? FarmerId{get; set;}
    [Required(ErrorMessage = "Season is required.")]
    public string Season{get; set;}
    [Required(ErrorMessage = "Year is required.")]
    public DateTime year{get; set;}
    [Required(ErrorMessage = "Crop Name is required.")]
    public string Crop{get; set;}
    [Required(ErrorMessage = "Area in Hectare is required.")]
    public decimal Area{get; set;}
    public string? InsuranceCompany{get; set;}
    public decimal? SharePremium{get; set;}
    public decimal? PermiumAmount{get; set;}
    [Required(ErrorMessage = "SumInsured is required.")]
    public decimal SumInsured{get; set;}

}