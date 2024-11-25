using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace Schemes_for_Farmers.Controllers;


[Route("api/[controller]")]
[ApiController]
public class BidderController:ControllerBase
{
    private readonly IMapper _mapper;
    
    private readonly IService _service;
    private readonly ICropRepository _croprepo;
    private readonly IBidderRepository _bidderrepo;
    
    private readonly IBiddingCropsRepository _biddingcropsrepo;
    
    private readonly IEmailService _emailrepo;
    public BidderController(IMapper mapper,IService service,ICropRepository croprepo,IBidderRepository bidderrepo,IBiddingCropsRepository biddingcropsrepo,IEmailService emailrepo)
    {
        _mapper=mapper;
        _service=service;
        _croprepo=croprepo;
        _bidderrepo = bidderrepo;
        _biddingcropsrepo=biddingcropsrepo;
        _emailrepo=emailrepo;
        
    }
    
    [HttpGet("currentAuctions")]
    [Authorize(Policy = "BidderOnly")]
    public async Task<ActionResult<IEnumerable<BiddingCropsDTO>>> GetCurrentAuctions()
    {
        var auctions=await _biddingcropsrepo.GetAuctionsAsync();
        var crops=await _croprepo.GetAllCropsAsync();
        var result = from t in auctions where t.bidStatus!="Yes"
             group t by t.CropId into cropGroup
             join c in crops on cropGroup.Key equals c.CropId 
             select new
             {
                CropName = c.CropName,
                CropType = c.CropType,
                CurrentAmount = cropGroup.Max(t => t.BidAmount)
                 
             };

        var resultList = result.ToList();
        return Ok(resultList);

    }

    [HttpPost("AddBid")]
    [Authorize(Policy = "BidderOnly")]
    public async Task<ActionResult<BiddingCropsDTO>> AddBid([FromBody] BiddingCropsDTO bids)
    {
        Random random=new Random();
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("User not authenticated.");
        }

        // Fetch all entities associated with the current user
        var entities = await _bidderrepo.GetBidderAsync(email);
        if(entities!=null && entities.GrantAccess=="Yes")
        {
            var bid=_mapper.Map<BiddingCrops>(bids); 
            bid.BidId=$"AUCS{random.Next()}";
            //condition--cexits
            await _biddingcropsrepo.AddAuctionAsync(bid); 
            var BiddingCropsDto = _mapper.Map<BiddingCropsDTO>(bids);
            return Ok(BiddingCropsDto);
        }
        return NotFound("No data found for the user.");
        
    }

    [HttpPut("updateBid")]
    [Authorize(Policy = "BidderOnly")]
    public async Task<ActionResult<BiddingCropsDTO>> UpdateBid([FromBody] BiddingCropsDTO bids)
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("User not authenticated.");
        }
        // Fetch all entities associated with the current user
        var entities = await _bidderrepo.GetBidderAsync(email);
        if(entities!=null && entities.GrantAccess=="Yes")
        {
        
        var bid=await _biddingcropsrepo.GetAuctionAsync(bids.BidderId);
        _mapper.Map(bids, bid);
        await _biddingcropsrepo.UpdateAuctionAsync(bid);
        var BiddingCropsDto=_mapper.Map<BiddingCropsDTO>(bid);
        return Ok(BiddingCropsDto); 
        }
        return NotFound("No data found for the user.");
    }



}