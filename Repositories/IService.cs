using Schemes_for_Farmers.Models;

public interface IService
{
    
    Task<Admin> AunthenticateAdmin(string email,string password);
    Task<Farmer> AunthenticateFarmer(string email,string password);
    Task<Bidder> AunthenticateBidder(string email,string password);
}