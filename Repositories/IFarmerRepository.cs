using Schemes_for_Farmers.Models;

public interface IFarmerRepository
{
    Task<IEnumerable<Farmer>> GetFarmersAsync(); //Admin : access to all registered farmers
    Task<Farmer> GetFarmerAsync(string id); // If you want to get a single crop
    Task AddFarmerAsync(Farmer farmer); //Farmer : farmer registers itself
    Task UpdateFarmerAsync(Farmer farmer);//Admin : Updates (registration)Approval to Approved/Cancelled/Pending
    
}