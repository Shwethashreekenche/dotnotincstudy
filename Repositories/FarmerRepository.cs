using Microsoft.EntityFrameworkCore;
using Schemes_for_Farmers.Data;
using Schemes_for_Farmers.Models;

public class FarmerRepository :IFarmerRepository
{
    private readonly SchemesDbContext _context;

    public FarmerRepository(SchemesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Farmer>> GetFarmersAsync()
    {
        return await _context.Farmers.ToListAsync();
    } 

    public async Task<Farmer> GetFarmerAsync(string id)
    {
        return await _context.Farmers.FirstOrDefaultAsync(c=>c.EmailId==id);
    }    public async Task AddFarmerAsync(Farmer farmers)
    {
        _context.Farmers.Add(farmers);
        await _context.SaveChangesAsync();
        
    }

    public async Task UpdateFarmerAsync(Farmer farmers)
    {        
        _context.Farmers.Update(farmers);
        await _context.SaveChangesAsync();
    }
}