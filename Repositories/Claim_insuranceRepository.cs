using Microsoft.EntityFrameworkCore;
using Schemes_for_Farmers.Data;

public class Claim_insuranceRepository:IClaim_insuranceRepository
{
   private readonly SchemesDbContext _context;

    public Claim_insuranceRepository(SchemesDbContext context)
    {
        _context = context;
    } 

    public async Task<Claim_insurance> GetClaimInsuranceAsync(DateTime dateTime){
        return await _context.Claim_Insurances.FirstOrDefaultAsync(a=>a.DateofLoss==dateTime);
    }
    public async Task<IEnumerable<Claim_insurance>> GetAllCInsuranceAsync()
    {
        return await _context.Claim_Insurances.ToListAsync();
    } 
    public async Task AddCInsuranceAsync(Claim_insurance ci)
    {
        _context.Claim_Insurances.Add(ci);
        await _context.SaveChangesAsync();
    } 
    
    public async Task UpdateClaimStatusAsync(Claim_insurance i)
    {

        _context.Claim_Insurances.Update(i);
        await _context.SaveChangesAsync();
    }
}