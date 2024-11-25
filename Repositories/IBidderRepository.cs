using Schemes_for_Farmers.Models;

public interface IBidderRepository
{
    Task<IEnumerable<Bidder>> GetBiddersAsync();//Admin : access to all registered bidders
    Task<Bidder> GetBidderAsync(string id);
    Task AddBidderAsync(Bidder bidder); //Bidder : bidder registers itself
    Task UpdateBidderAsync(Bidder bidder);//Admin : Updates (registration)Approval to Approved/Cancelled/Pending
    
}