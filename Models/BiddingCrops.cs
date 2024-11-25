using System.ComponentModel.DataAnnotations;
using Schemes_for_Farmers.Models;

public class BiddingCrops
{
    [Key]
    public string BidId{get; set;}

    public string CropId{get; set;}

    public string? BidderId{get; set;}

    public decimal BidAmount{get; set;}

    public string? bidStatus{get; set;}

    //Navigation property
    public Bidder Bidders { get; set; }
    public Crop Crops { get; set; }

    public static implicit operator string(BiddingCrops v)
    {
        throw new NotImplementedException();
    }

    public static explicit operator decimal(BiddingCrops v)
    {
        throw new NotImplementedException();
    }
}