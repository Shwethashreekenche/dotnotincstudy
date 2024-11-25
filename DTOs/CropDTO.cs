public class CropDTO
{
    public string? FarmerId{get; set;}

    public string CropType{get; set;}

    public string CropName{get; set;}

    public string FertilizerType{get; set;}
    
    public decimal Quantity{get; set;}

    public byte[] SoilpHCertificate{get; set;}

    public DateTime? createdate{get; set;}=DateTime.Now;

    public decimal? BasePrice{get; set;}

    public bool? Sold{get; set;}=false;

    public decimal? SoldPrice{get; set;}

    public DateTime? SoldDate{get; set;}

    public decimal? MSP{get; set;}

    public string? RequestStatus{get; set;}
}