using System.ComponentModel.DataAnnotations;
using Schemes_for_Farmers.Models;

public class Claim_insurance
{
    [Key]
    public string ClaimId{get; set;}

    public string InsuranceId{get; set;}

    public DateTime DateofLoss{get; set;}

    public string ReasonofLoss{get;set;}

    public string? ClaimStatus{get; set;}

    //Navigation property
    public Insurance Insurances{get; set;}
}