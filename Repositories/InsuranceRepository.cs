using Microsoft.EntityFrameworkCore;
using Schemes_for_Farmers.Data;
using Schemes_for_Farmers.Models;

public class InsuranceRepository:IInsuranceRepository
{
    private readonly SchemesDbContext _context;

    public InsuranceRepository(SchemesDbContext context)
    {
        _context = context;
    }
    public async Task<Insurance> GetInsuranceAsync(string id)
    {
        return await _context.Insurances.FindAsync(id);
    }
    public async Task<IEnumerable<Insurance>> GetAllInsuranceAsync()
    {
       return await _context.Insurances.ToListAsync();
    }
    public async Task AddInsuranceAsync(Insurance i)
    {
        _context.Insurances.Add(i);
        await _context.SaveChangesAsync();
    } 
    
}