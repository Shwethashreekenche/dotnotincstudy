public interface IClaim_insuranceRepository
{
    Task<Claim_insurance> GetClaimInsuranceAsync(DateTime dateTime); 
    Task<IEnumerable<Claim_insurance>> GetAllCInsuranceAsync(); 
    Task AddCInsuranceAsync(Claim_insurance i); 
    Task UpdateClaimStatusAsync(Claim_insurance i);
    
    
}