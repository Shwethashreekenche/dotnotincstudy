using Schemes_for_Farmers.Models;

public interface ICropRepository
{
    Task<IEnumerable<Crop>> GetAllCropsAsync(); //Admin : access to all sell request Crops
    Task<Crop> GetCropAsync(string id);
    Task AddCropAsync(Crop crop); //Farmer : Sell Request
    Task UpdateCropAsync(Crop crop);//Admin : Updates RequestStatus to Approved/rejected/Pending
    
}