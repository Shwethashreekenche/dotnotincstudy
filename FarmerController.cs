using System.Net;
using System.Security.Claims;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Schemes_for_Farmers.Models;

namespace Schemes_for_Farmers.Controllers;

[Route("api/[controller]")]
[ApiController]
public class FarmerController:ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IFarmerRepository _farmerrepo;
    private readonly ICropRepository _croprepo;
    private readonly IBiddingCropsRepository _biddingcropsrepo;
    private readonly IInsuranceRepository _insurancerepo;
    private readonly IClaim_insuranceRepository _claiminsurancerepo;
    
    private readonly IMemoryCache _storecal;

    private readonly IEmailService _emailrepo;

    public FarmerController(IMapper mapper,IFarmerRepository farmerrepo,ICropRepository croprepo,IBiddingCropsRepository biddingcropsrepo,IInsuranceRepository insurancerepo,IClaim_insuranceRepository claiminsurancerepo,IMemoryCache storecal,IEmailService emailrepo)
    {
        _mapper=mapper;
        _farmerrepo = farmerrepo;
        _croprepo=croprepo;
        _biddingcropsrepo=biddingcropsrepo;
        _insurancerepo=insurancerepo;
        _claiminsurancerepo=claiminsurancerepo;
        _storecal=storecal;
        _emailrepo=emailrepo;
    }

    //View marketplace
    [HttpGet("viewmarketplace")]
    [Authorize(Policy = "FarmerOnly")]
    public async Task<ActionResult<CropDTO>> getMarketplace()
    {
        
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("User not authenticated.");
        }
        var entities = await _farmerrepo.GetFarmerAsync(email);
        var crops = await _croprepo.GetAllCropsAsync();
        var auctions=await _biddingcropsrepo.GetAuctionsAsync();
        var getfarmerscrops = from t in crops
                       join c in auctions on t.CropId equals c.CropId
                       where entities.FarmerId == t.FarmerId
                       group c by new { t.CropId, t.CropType, t.CropName, t.BasePrice } into auctionGroup
                       select new
                       {
                           CropType = auctionGroup.Key.CropType,
                           CropName = auctionGroup.Key.CropName,
                           BasePrice = auctionGroup.Key.BasePrice,
                           CurrentBid = auctionGroup.Max(a => a.BidAmount) // Get max BidAmount for each CropId
                       };
        var getfarmerscropsList = getfarmerscrops.ToList();
        return Ok(getfarmerscropsList);
    }   
    //View Sold History
    [HttpGet("soldhistory")]
    [Authorize(Policy = "FarmerOnly")]
    public async Task<ActionResult<CropDTO>> CropsoldHistory()
    {
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("User not authenticated.");
        }
        // Fetch all entities associated with the current user
        
        var crops = await _croprepo.GetAllCropsAsync();
        var tell=crops.Where(c=>c.Sold==true);
        if (tell== null)
        {
            return Ok("No sold History");
        }
        return Ok(tell);
    }
    //Insurance Calculator
    
    [HttpGet("calculate")]
    [Authorize(Policy = "FarmerOnly")]
    public async Task<ActionResult<InsuranceDTO>> CalculateInsurance(string Season,DateTime year,decimal suminsurance,string Crop,decimal Area)
    {
        if(year<DateTime.Now)
        {
            return BadRequest("year must be greater than or equal to current Date");
        }
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        var entities = await _farmerrepo.GetFarmerAsync(email);
        string? insCompany="";
        decimal? SumInsurancePerHec=40000;
        decimal? farmerPA=0;
        decimal? premium_share=0;
        if(Season=="Kharif")
        {
            insCompany="Bajaj Finace Insurance";
            premium_share=2;
            farmerPA=suminsurance-(suminsurance*2/100);
        }
        else if(Season=="Rabi")
        {
            insCompany="Oriental Insurance";
            premium_share=1.5m;//1.5%
            farmerPA=suminsurance-(suminsurance *1.5m/100);
        }
        else if(Season=="others")
        {
            insCompany="LIC Max Insurance";
            premium_share=5;//5%
            farmerPA=suminsurance-(suminsurance*5/100);
        }
        var calculatejson=new {
            
            InsuranceCompany=insCompany,
            SharePremium=premium_share,
            PermiumAmount=farmerPA,
            SumInsured=suminsurance,
            Crop=Crop,
            year=year,
            Area=Area,
            Season=Season
        };
        var Msg=new{insCompany,SumInsurancePerHec,premium_share,farmerPA,Crop,Area,suminsurance};
        _storecal.Set("getonlycalculate",Msg);
        _storecal.Set("calculate",calculatejson);
        var e=_storecal.Get<dynamic>("getonlycalculate");
        return Ok(e);

    }

     //Request/Apply Insurance
    [HttpPost("applyinsurance")]
    [Authorize(Policy = "FarmerOnly")]
    public async Task<ActionResult<InsuranceDTO>> ApplyInsurance()
    {
        //calculate premium,share,etc here,
        var storecal=_storecal.Get<dynamic>("calculate");
        
        Random random=new Random();
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("User not authenticated.");
        }
        
        // Fetch all entities associated with the current user
        var entities = await _farmerrepo.GetFarmerAsync(email);
        
        if(entities!=null)
        {
            var adddata=new InsuranceDTO{
                FarmerId=entities.FarmerId,
                InsuranceCompany=storecal.InsuranceCompany,
                SharePremium=storecal.SharePremium,
                PermiumAmount=storecal.PermiumAmount,
                SumInsured=storecal.SumInsured,
                Crop=storecal.Crop,
                year=storecal.year,
                Area=storecal.Area,
                Season=storecal.Season
         };
         var validata=await _insurancerepo.GetAllInsuranceAsync();
         var result=from t in validata where t.FarmerId==adddata.FarmerId && t.InsuranceCompany==adddata.InsuranceCompany && t.SharePremium==adddata.SharePremium && t.PermiumAmount==adddata.PermiumAmount && t. SumInsured==adddata.SumInsured && t.Crop==adddata.Crop && t.year==adddata.year && t.Area==adddata.Area && t.Season==adddata.Season select t;
         if(result.Any())
         {
            return Ok("You have Already have Applied given details for Insurance");
         }

            var insurance=_mapper.Map<Insurance>(adddata);
            insurance.InsuranceId=$"IN{random.Next()}";
            await _insurancerepo.AddInsuranceAsync(insurance); 
            var inDto = _mapper.Map<InsuranceDTO>(insurance);
             var emailSubject = "Successfully Insurance Applied";
            var emailBody = $" Dear {entities.FullName}, Your Insurance Details:{insurance.InsuranceId}  is your policy number for Crop {insurance.Crop} from {insurance.InsuranceCompany} Company of Rupees {insurance.SumInsured} thank You!!";

            try
            {
                
                await _emailrepo.SendEmailAsync(email, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                // Log the exception (You can use a logger)
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }
        
            return Ok(inDto);
        }
        return Ok("No access Provided by Admin");
    }

    // Sell Request
    [HttpPost("sellcrop")]
    [Authorize(Policy = "FarmerOnly")]
    public async Task<ActionResult<CropDTO>> CreateCrop([FromBody] CropDTO crops)
    {
        Random random=new Random();
        var email = User.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
        {
            return Unauthorized("User not authenticated.");
        }

        // Fetch all entities associated with the current user
        var entities = await _farmerrepo.GetFarmerAsync(email);
        if(entities!=null && entities.GrantAccess!=null)

        {
            crops.RequestStatus="NO";
            crops.FarmerId=entities.FarmerId;
            var crop = _mapper.Map<Crop>(crops); 
            crop.CropId=$"2024{random.Next()}";
            await _croprepo.AddCropAsync(crop); 
            var cropDto = _mapper.Map<CropDTO>(crops);
            return Ok("Successfully Added");
        }
        return Ok("No access Provided by Admin");
    }
   
    //Claim Insurance
    [HttpPost("claim_insurance")]
    [Authorize(Policy = "FarmerOnly")]
    public async Task<ActionResult<Claim_insuranceDTO>> Claim_Insurance(string policyNo,string InsuranceCompany,string NameofInsuree,decimal sumInsured,string causeofLoss,DateTime dateofLoss)
    {
        
        var claimins=new Claim_insuranceDTO{
            InsuranceId=policyNo,
            DateofLoss=dateofLoss,
            ReasonofLoss=causeofLoss,
            ClaimStatus="NO"
        };
        var ins=await _insurancerepo.GetInsuranceAsync(policyNo);
        if(ins!=null && claimins.DateofLoss>=ins.year){

        Random random=new Random();
        var cinsurance=_mapper.Map<Claim_insurance>(claimins);
        cinsurance.ClaimId=$"CIN{random.Next()}";
        await _claiminsurancerepo.AddCInsuranceAsync(cinsurance); 
        var cinDto = _mapper.Map<InsuranceDTO>(claimins);
        return Ok(cinDto); 
        }

        return BadRequest("Insurance is not applied for the concerned claim");

    }
 
}



