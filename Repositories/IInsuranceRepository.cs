using Schemes_for_Farmers.Models;

public interface IInsuranceRepository
{
    Task<Insurance> GetInsuranceAsync(string id); 
    Task<IEnumerable<Insurance>> GetAllInsuranceAsync(); 
    Task AddInsuranceAsync(Insurance i); 
    
    
}