using Microsoft.EntityFrameworkCore;
using Schemes_for_Farmers.Data;
using Schemes_for_Farmers.Models;

public class BidderRepository:IBidderRepository
{
    private readonly SchemesDbContext _context;

    public BidderRepository(SchemesDbContext context)
    {
        _context = context;
    }
    public async Task<IEnumerable<Bidder>> GetBiddersAsync()
    {
        return await _context.Bidders.ToListAsync();
    } 

    public async Task<Bidder> GetBidderAsync(string id)
    {
        return await _context.Bidders.FirstOrDefaultAsync(c=>c.EmailId==id);
    }

    public async Task AddBidderAsync(Bidder bidder)
    {
        _context.Bidders.Add(bidder);
        await _context.SaveChangesAsync();     

    }
    public async Task UpdateBidderAsync(Bidder bidder)
    {
        _context.Bidders.Update(bidder);
        await _context.SaveChangesAsync();
    }
}