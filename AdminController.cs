using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Schemes_for_Farmers.Models;

namespace Schemes_for_Farmers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AdminController:ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IService _service;
    private readonly IFarmerRepository _farmerrepo;
    private readonly IBidderRepository _bidderrepo;
    private readonly ICropRepository _croprepo;
    private readonly IBiddingCropsRepository _biddingcropsrepo;
    private readonly IInsuranceRepository _insurancerepo;
    private readonly IClaim_insuranceRepository _claim_insurancerepo;
    private readonly IEmailService _emailrepo;
    public AdminController(IMapper mapper,IService service,IFarmerRepository farmerrepo,IBidderRepository bidderrepo,ICropRepository croprepo,IBiddingCropsRepository biddingcropsrepo,IInsuranceRepository insurancerepo,IClaim_insuranceRepository claim_Insurancerepo,IEmailService emailrepo)
    {
        _mapper=mapper;
        _service=service;
        _farmerrepo = farmerrepo;
        _bidderrepo=bidderrepo;
        _croprepo=croprepo;
        _biddingcropsrepo=biddingcropsrepo;
        _insurancerepo=insurancerepo;
        _claim_insurancerepo=claim_Insurancerepo;
        _emailrepo=emailrepo;
    }
    
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] AdminLoginDTO login)
    {
        var admin = await _service.AunthenticateAdmin(login.EmailId, login.Password);

        if (admin == null)
        {
            return BadRequest("Invalid credentials");
        }

        var token = GenerateJwtToken(admin,"Admin");  // Generate JWT token after authentication
        return Ok(new { Token = token });
    }
        private string GenerateJwtToken(Admin admin,string role)
    {
        // Logic to generate the JWT token goes here
        var claims = new[]
        {
            new Claim(ClaimTypes.Email, admin.EmailId),
            new Claim(ClaimTypes.Role, role),
            // Add other claims as needed
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your256bitlongrandomkeyforjwtsecurity1234567890"));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: "YourIssuer",
            audience: "YourAudience",
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // GET: api/Admin/farmers
    [HttpGet("farmers")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<FarmerDTO>>> GetFarmers()
    {
        var farmers= await _farmerrepo.GetFarmersAsync();
        var farmersDtos = _mapper.Map<List<FarmerDTO>>(farmers);
        return Ok(farmersDtos);
    }

    // GET: api/Admin/bidders
    [HttpGet("bidders")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<BidderDTO>>> GetBidders()
    {
        var bidders=await _bidderrepo.GetBiddersAsync();
        var bidderDtos = _mapper.Map<List<BidderDTO>>(bidders);
        return Ok(bidderDtos);
    }

    // GET: api/Admin/Crops
    [HttpGet("Crops")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<CropDTO>>> GetCrops()
    {
        var crops=await _croprepo.GetAllCropsAsync();
        var farmersDtos = _mapper.Map<List<CropDTO>>(crops);
        return Ok(farmersDtos);

    }

    // GET: api/Admin/Auctions
    [HttpGet("Auctions")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<IEnumerable<BiddingCropsDTO>>> GetAuctions()
    {
        var auctions=await _biddingcropsrepo.GetAuctionsAsync();
        var biddingcropsDtos = _mapper.Map<List<BiddingCropsDTO>>(auctions);
        return Ok(biddingcropsDtos);

    }
    //grants user access ,change status of bidding final bid amount, validate insurance and claim(4 functions--)

    //PUT :api/Admin/grantaccessf
    [HttpPut("farmeraccess")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<FarmerDTO>> GrantAccessFarmer([FromBody]FarmerDTO farmersOne)
    {
            if (string.IsNullOrEmpty(farmersOne.EmailId))
        {
            return BadRequest("EmailId is required.");
        }

        // Retrieve the Farmer from the database based on the EmailId
        var farmer = await _farmerrepo.GetFarmerAsync(farmersOne.EmailId);
        
        if (farmer == null)
        {
            return NotFound("Farmer not found with the provided EmailId.");
        }

        // Map the data from the DTO to the existing Farmer entity
        _mapper.Map(farmersOne, farmer);

        // Now update the entity in the database
        await _farmerrepo.UpdateFarmerAsync(farmer);

        return Ok("Farmer updated successfully.");
    }

    [HttpPut("bidderaccess")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<BidderDTO>> GrantAccessBidder([FromBody]BidderDTO bidderOne)
    {
        
        var bidders=await _bidderrepo.GetBidderAsync(bidderOne.EmailId);
        _mapper.Map(bidderOne, bidders);
        //update only grant access col--
        await _bidderrepo.UpdateBidderAsync(bidders);
        var bidderDtos=_mapper.Map<BidderDTO>(bidders);
        return Ok("Bidder updated successfully.") ;
    }

    // [HttpPut("cropaccess")]
    // [Authorize(Policy = "AdminOnly")]
    // public async Task<ActionResult<CropDTO>> GrantAccesscrop([FromBody]CropDTO cropOne)
    // {
    //     var crop=await _croprepo.GetCropAsync((DateTime)cropOne.createdate);
    //     _mapper.Map(cropOne, crop);
    //     await _croprepo.UpdateCropAsync(crop);
    //     var CropDto=_mapper.Map<CropDTO>(crop);
    //     return Ok(CropDto); 
    // }

    [HttpPut("claim-insurance")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<Claim_insuranceDTO>> ValidateClaim([FromBody]Claim_insuranceDTO claimOne)
    {
        var claim=await _claim_insurancerepo.GetClaimInsuranceAsync((DateTime)claimOne.DateofLoss);
        var ins=await _insurancerepo.GetInsuranceAsync(claim.InsuranceId);
        var farmer=await _farmerrepo.GetFarmersAsync();
        var f=from t in farmer where t.FarmerId==ins.FarmerId select (t.FullName,t.FarmerId,t.EmailId);
        var list=f.ToList();
        string email="";
        foreach(var i in list){
            email=i.EmailId;
        }
        _mapper.Map(claimOne, claim);
        await _claim_insurancerepo.UpdateClaimStatusAsync(claim);
        var CropDto=_mapper.Map<Claim_insuranceDTO>(claim);
        if(CropDto.ClaimStatus=="Yes"){
            var emailSubject = "Claim Request Status on Schemes_for_Farmers";
            var emailBody = $"{list[0]} , Your insurance has been claimed,thank You!!";

        try
        {
            
            await _emailrepo.SendEmailAsync(email, emailSubject, emailBody);
        }
        catch (Exception ex)
        {
            // Log the exception (You can use a logger)
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }
        }
        
        return Ok(CropDto); 
    }

    [HttpPost("AddBid")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<BiddingCropsDTO>> AddBid()
    {
        var crops=await _croprepo.GetAllCropsAsync();
        var rescropstoauctions=from t in crops where t.RequestStatus=="Yes" select t;
        Random random=new Random();
        if(rescropstoauctions!=null){
        foreach(var crop in rescropstoauctions)
        {
            BiddingCropsDTO bids=new BiddingCropsDTO{
                CropId=crop.CropId,
                BidderId=null,
                BidAmount= (decimal)crop.BasePrice
            };
            var bid=_mapper.Map<BiddingCrops>(bids); 
            bid.BidId=$"AUCS{random.Next()}";
            //condition--cexits
            await _biddingcropsrepo.AddAuctionAsync(bid); 
            var BiddingCropsDto = _mapper.Map<BiddingCropsDTO>(bids);
            return Ok("New Auctions added.");
        }return Ok("New Auctions added.");}
        
        
        return NotFound("No data found for the user.");
        
    }

    [HttpPut("finalBid")]
    [Authorize(Policy = "AdminOnly")]
    public async Task<ActionResult<BiddingCropsDTO>> FinalBid(decimal amount,string CropId,string biddermail)
    {
        var auctions=await _biddingcropsrepo.GetAuctionsAsync();
        var crop=await _croprepo.GetCropAsync(CropId);
        var bidder=await _bidderrepo.GetBidderAsync(biddermail);

        
        var get=from i in auctions where i.BidAmount==amount && i.CropId==crop.CropId && i.BidderId==bidder.BidderId select i;
        
        var list=get.ToList();
        var auction=await _biddingcropsrepo.GetAuctionAsync(list[0]);
        if(get!=null){
            var result=new BiddingCropsDTO{
                CropId=(string)list[1],
                BidderId=(string)list[2],
                BidAmount=amount,
                bidStatus="Yes"

            };
            var getCrop=await _croprepo.GetCropAsync(CropId);
            getCrop.Sold=true;
            getCrop.SoldDate=DateTime.Now;
            getCrop.SoldPrice=amount;
            getCrop.MSP=getCrop.BasePrice;
            _mapper.Map(result, auction);
            await _biddingcropsrepo.UpdateAuctionAsync(auction);
            await _croprepo.UpdateCropAsync(getCrop);
            var BiddingCropsDto=_mapper.Map<BiddingCropsDTO>(auction);
            return Ok(BiddingCropsDto); 
        }
        
        return NotFound("Your input data does not exist");

        
    }

    private ActionResult<BiddingCropsDTO> Ok(BiddingCropsDTO biddingCropsDto, Crop getCrop)
    {
        throw new NotImplementedException();
    }
}