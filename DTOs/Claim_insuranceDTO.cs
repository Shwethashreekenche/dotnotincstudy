using System.ComponentModel.DataAnnotations;

public class Claim_insuranceDTO
{
    public string? InsuranceId{get; set;}
    [Required(ErrorMessage = "Season is required.")]
    public DateTime DateofLoss{get; set;}
    [Required(ErrorMessage = "Season is required.")]
    public string ReasonofLoss{get;set;}
    [Required(ErrorMessage = "Season is required.")]
    public string? ClaimStatus{get; set;}
}