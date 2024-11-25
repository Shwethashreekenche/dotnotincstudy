using System.ComponentModel.DataAnnotations;
namespace Schemes_for_Farmers.Models;
public class Insurance
{
    [Key]
    public string InsuranceId{get; set;}
    public string FarmerId{get; set;}
    public string Season{get; set;}
    
    public string Crop{get; set;}
    public decimal Area{get; set;}

    public DateTime year{get; set;}
    public string InsuranceCompany{get; set;}
    public decimal SharePremium{get; set;}
    public decimal PermiumAmount{get; set;}
    public decimal SumInsured{get; set;}
    


    //navigation property
    public Farmer Farmers{get; set;}


    public ICollection<Claim_insurance> Claim_Insurances { get; set; } = new List<Claim_insurance>();
}