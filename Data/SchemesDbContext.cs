using Microsoft.EntityFrameworkCore;
using Schemes_for_Farmers.Models;
namespace Schemes_for_Farmers.Data;
public class SchemesDbContext : DbContext
{
    public SchemesDbContext(DbContextOptions<SchemesDbContext>options) :base(options)
    {
        
    }
    public DbSet<Admin> Admins{get; set;}
    public DbSet<Bidder> Bidders{get; set;}
    public DbSet<Farmer> Farmers{get; set;}
    public DbSet<Crop> Crops{get; set;}
    public DbSet<BiddingCrops> BiddingCrops{get; set;}

    public DbSet<Insurance> Insurances{get; set;}

    public DbSet<Claim_insurance> Claim_Insurances{get; set;}

}