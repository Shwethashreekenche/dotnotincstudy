using System.ComponentModel.DataAnnotations;

namespace Schemes_for_Farmers.Models;
public class Crop
{
    [Key]
    public string CropId{get; set;}

    public string? FarmerId{get; set;}

    public string CropType{get; set;}

    public string CropName{get; set;}

    public string FertilizerType{get; set;}
    
    public decimal Quantity{get; set;}

    public byte[] SoilpHCertificate{get; set;}

    public DateTime? createdate{get; set;}=DateTime.Now;

    public decimal? BasePrice{get; set;}

    public bool? Sold{get; set;}

    public decimal? SoldPrice{get; set;}

    public DateTime? SoldDate{get; set;}

    public decimal? MSP{get; set;}

    public string? RequestStatus{get; set;}

    //Navigation Property
    public Farmer Farmers{get; set;}

    
    
    //Navigating class
    public ICollection<BiddingCrops> BiddingCrops { get; set; } = new List<BiddingCrops>();
    
}