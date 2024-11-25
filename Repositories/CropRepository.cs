using Microsoft.EntityFrameworkCore;
using Schemes_for_Farmers.Data;
using Schemes_for_Farmers.Models;

public class CropRepository:ICropRepository
{
    private readonly SchemesDbContext _context;

    public CropRepository(SchemesDbContext context)
    {
        _context = context;
    }
    public async Task<Crop> GetCropAsync(string id)
    {
        return await _context.Crops.FindAsync(id);
    }
    public async Task<IEnumerable<Crop>> GetAllCropsAsync()
    {
        return await _context.Crops.ToListAsync();
    } 
    public async Task AddCropAsync(Crop crop)
    {
        
        _context.Crops.Add(crop);
        await _context.SaveChangesAsync();
    } 
    public async Task UpdateCropAsync(Crop crop)
    {
        _context.Crops.Update(crop);
        await _context.SaveChangesAsync();
    }
}