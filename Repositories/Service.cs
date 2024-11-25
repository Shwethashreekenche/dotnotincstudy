using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Schemes_for_Farmers.Data;
using Schemes_for_Farmers.Models;

public class Service :IService
{
    private readonly SchemesDbContext _context;

    public string UseId => throw new NotImplementedException();

    public Service(SchemesDbContext context)
    {
        _context = context;
    }
    public async Task<Admin> AunthenticateAdmin(string email,string password)
    {
        return await _context.Admins.SingleOrDefaultAsync(s=>s.EmailId==email && s.Password==password);
    }

    public async Task<Farmer> AunthenticateFarmer(string email,string password)
    {
        return await _context.Farmers.SingleOrDefaultAsync(s=>s.EmailId==email && s.Password==password);
    }
    public async Task<Bidder> AunthenticateBidder(string email,string password)
    {
        return await _context.Bidders.SingleOrDefaultAsync(s=>s.EmailId==email && s.Password==password);
    }

    
}
