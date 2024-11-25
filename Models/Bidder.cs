
using System.ComponentModel.DataAnnotations;

namespace Schemes_for_Farmers.Models;
public class Bidder
{
    [Key]
    public string BidderId{get; set;}

    public string FullName{get; set;}
    
    public string ContactNo{get; set;}

    public string EmailId{get; set;}
    public string Address{get; set;}

    public string AccountNo{get; set;}

    public string IFSCCode{get; set;}

    public byte[] Aadhaar{get; set;}

    public byte[] Pan{get; set;}

    public byte[] TraderLicense{get; set;}

    public string Password{get; set;}

    public string? GrantAccess{get; set;}

    public ICollection<BiddingCrops> BiddingCrops { get; set; } = new List<BiddingCrops>();

}