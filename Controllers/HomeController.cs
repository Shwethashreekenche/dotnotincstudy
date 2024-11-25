using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Transactions;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Schemes_for_Farmers.Models;
namespace Schemes_for_Farmers.Controllers;
[Route("api/[controller]")]
[ApiController]
public class HomeController: ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IFarmerRepository _farmerrepo;
    private readonly IBidderRepository _bidderrepo;
    private readonly IService _service;
    private readonly IEmailService _emailrepo;

    public HomeController(IMapper mapper,IFarmerRepository farmerrepo,IBidderRepository bidderrepo,IService service, IEmailService emailrepo)
    {
        _mapper=mapper;
        _farmerrepo = farmerrepo;
        _bidderrepo = bidderrepo;
        _service=service;
        _emailrepo=emailrepo;
    }

    // [HttpGet("AboutUs")]
    // public ActionResult AboutUs()
    // {

    //     // Return success response
    //     return Ok("Schemes for Farmer");
    // }

    [HttpPost("register-farmer")]
    public async Task<ActionResult<FarmerDTO>> RegisterFarmer([FromBody] FarmerDTO farmers)
    {
        
        Random random=new Random();
        
        var farmer = _mapper.Map<Farmer>(farmers);
        var emailexists=await _farmerrepo.GetFarmerAsync(farmer.EmailId);
        
        if(emailexists!=null){
            return BadRequest("Email already exists");
        }
        
        else if(emailexists==null)
        {
        
            farmers.GrantAccess = "NO";
            farmer.FarmerId=$"FAR{random.Next()}";
            await _farmerrepo.AddFarmerAsync(farmer); 
            var farmerDto = _mapper.Map<FarmerDTO>(farmers);

            // Send the confirmation email
            var emailSubject = "Welcome to Our Website!";
            var emailBody = $"Hello {farmer.FullName},\n\n" +
                            "Thank you for registering on our platform.\n\n" +
                            "Best regards,\n" +
                            "Schemes_for_Farmer Team";

            try
            {
                await _emailrepo.SendEmailAsync(farmer.EmailId, emailSubject, emailBody);
            }
            catch (Exception ex)
            {
                // Log the exception (You can use a logger)
                return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
            }

        // Return success response
        return Ok("User registered successfully!");
        }
        return NotFound("Farmer not registered.");
        
    }

    [HttpPost("register-bidder")]
    public async Task<ActionResult<BidderDTO>> RegisterFarmer([FromBody] BidderDTO bidders)
    {
        
        Random random=new Random();
        var emailexists=await _bidderrepo.GetBidderAsync(bidders.EmailId);
        if(emailexists!=null){
            return Ok("email already In Use, Enter New email.");
        }
        bidders.GrantAccess = "NO";  // Hide this property for non-admin users
        var bidder = _mapper.Map<Bidder>(bidders); 
        bidder.BidderId=$"BID{random.Next()}";
        await _bidderrepo.AddBidderAsync(bidder); 
        var bidderDto = _mapper.Map<BidderDTO>(bidders);

         var emailSubject = "Welcome to Our Website!";
        var emailBody = $"Hello {bidder.FullName},\n\n" +
                        "Thank you for registering on our platform.\n\n" +
                        "Best regards,\n" +
                        "Schemes_for_Farmer Team";

        try
        {
            await _emailrepo.SendEmailAsync(bidder.EmailId, emailSubject, emailBody);
        }
        catch (Exception ex)
        {
            // Log the exception (You can use a logger)
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }

        // Return success response
        return Ok("User registered successfully!");
    }

    [HttpGet("forgot-password")]
    public async Task<ActionResult> Forgot_Password(string email)
    {   var user=await _farmerrepo.GetFarmerAsync(email);
    
        
        var anotheruser=await _bidderrepo.GetBidderAsync(email);
        var emailSubject = "";
        var emailBody = "";
        //validation
        // Send the confirmation email
        if(user!=null){
            emailSubject = "Your credentials of Schemes_for_Farmers";
            emailBody = $"{user.Password}";
        }
        else if(anotheruser!=null){
            emailSubject = "Your credentials of Schemes_for_Farmers";
            emailBody = $"{anotheruser.Password}";
        }

        try
        {
            await _emailrepo.SendEmailAsync(email, emailSubject, emailBody);
        }
        catch (Exception ex)
        {
            // Log the exception (You can use a logger)
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }

        // Return success response
        return Ok("User registered successfully!");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDTO login)
    {
        var farmer = await _service.AunthenticateFarmer(login.EmailId, login.Password);
        var bidder= await _service.AunthenticateBidder(login.EmailId, login.Password);
        if (farmer == null)
        {
            var token = GenerateJwtToken(bidder,"Bidder");  // Generate JWT token after authentication
            return Ok(new { Token = token });
        }
        else if(bidder==null)
        {
            var token = GenerateJwtToken(farmer,"Farmer");  // Generate JWT token after authentication
            return Ok(new { Token = token });
        }
        return BadRequest("Invalid Credentials");
        
    }
        private string GenerateJwtToken<TUser>(TUser user,string role)
        {
            var email = user.GetType().GetProperty("EmailId")?.GetValue(user, null)?.ToString();
    
            if (string.IsNullOrEmpty(email))
            {
                throw new ArgumentException("User must have a valid EmailId.");
            }
            // Logic to generate the JWT token goes here
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email,email),
                new Claim(ClaimTypes.Role, role),
                // Add other claims as needed
            };

             if (user is Farmer farmer)
            {
                // Add Farmer-specific claims
                claims.Add(new Claim("FarmerId", farmer.FarmerId.ToString())); // Example: Add FarmerId claim
            }
            else if (user is Bidder bidder)
            {
                // Add Bidder-specific claims
                claims.Add(new Claim("BidderId", bidder.BidderId.ToString()));  // Example: Add UserId claim
            }

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

    [HttpGet("ContactUs")]
    public async Task<ActionResult> ContactUs(string email,string message)
    {
        //validation
        // Send the confirmation email
        var emailSubject = "New Contact for Schemes_for_Farmer";
        var emailBody = $"{message}";

        try
        {
            await _emailrepo.SendEmailAsync(email, emailSubject, emailBody);
        }
        catch (Exception ex)
        {
            // Log the exception (You can use a logger)
            return StatusCode((int)HttpStatusCode.InternalServerError, ex.Message);
        }

        // Return success response
        return Ok("Your Message is Send.they will contact you in few days, thank you.");
    }
}