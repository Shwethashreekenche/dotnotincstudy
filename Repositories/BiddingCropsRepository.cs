using Microsoft.EntityFrameworkCore;
using Schemes_for_Farmers.Data;

public class BiddingCropsRepository:IBiddingCropsRepository
{
    private readonly SchemesDbContext _context;

    public BiddingCropsRepository(SchemesDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<BiddingCrops>> GetAuctionsAsync()
    {
        return await _context.BiddingCrops.ToListAsync();
    } 
    public async Task<BiddingCrops> GetAuctionAsync(string id)
    {
        return await _context.BiddingCrops.FindAsync(id);
    }

    public async Task AddAuctionAsync(BiddingCrops biddingcrops)
    {
        _context.BiddingCrops.Add(biddingcrops);
        await _context.SaveChangesAsync();
    }
    public async Task UpdateAuctionAsync(BiddingCrops biddingcrops)
    {
        _context.BiddingCrops.Update(biddingcrops);
        await _context.SaveChangesAsync();
    }
}