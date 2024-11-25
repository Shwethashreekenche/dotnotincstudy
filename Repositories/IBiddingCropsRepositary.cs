public interface IBiddingCropsRepository
{
    Task<IEnumerable<BiddingCrops>> GetAuctionsAsync(); 

    Task<BiddingCrops> GetAuctionAsync(string id);
    
    Task AddAuctionAsync(BiddingCrops biddingcrops); 
    Task UpdateAuctionAsync(BiddingCrops biddingcrops);
}