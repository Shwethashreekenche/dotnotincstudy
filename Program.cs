using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Schemes_for_Farmers.Data;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddMemoryCache();
// Add services to the container.
builder.Services.AddAutoMapper(typeof(MappingProfile));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
// Register other services
builder.Services.AddScoped<IFarmerRepository, FarmerRepository>();
builder.Services.AddScoped<IBidderRepository, BidderRepository>();
builder.Services.AddScoped<ICropRepository, CropRepository>();
builder.Services.AddScoped<IBiddingCropsRepository, BiddingCropsRepository>();
builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();
builder.Services.AddScoped<IClaim_insuranceRepository, Claim_insuranceRepository>();
builder.Services.AddScoped<IEmailService, EmailService>();
builder.Services.AddScoped<IService,Service>();

// Configure JWT authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "YourIssuer",  // You should change this to a valid issuer
            ValidAudience = "YourAudience",  // You should change this to a valid audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your256bitlongrandomkeyforjwtsecurity1234567890"))  // Secure this key!
        };
    });
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
    options.AddPolicy("FarmerOnly", policy => policy.RequireRole("Farmer"));
    options.AddPolicy("BidderOnly", policy => policy.RequireRole("Bidder"));
});
builder.Services.AddControllers();

// Register Swagger and configure it to use JWT Bearer token

builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please enter your JWT token here",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddEndpointsApiExplorer();
// Configure the database context (ensure the connection string is correct)
builder.Services.AddDbContext<SchemesDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // This will add the JWT Bearer token input to Swagger UI
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Schemes_for_Farmers v1"); // Ensure the correct path
        c.RoutePrefix = string.Empty;  // Makes Swagger UI available at the root
    });
}

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseHttpsRedirection();

app.MapControllers();
app.Run();






// using System.Text;
// using Microsoft.AspNetCore.Authentication.JwtBearer;
// using Microsoft.EntityFrameworkCore;
// using Microsoft.IdentityModel.Tokens;
// using Schemes_for_Farmers.Data;

// var builder = WebApplication.CreateBuilder(args);


// // Add services to the container.
// // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
// builder.Services.AddAutoMapper(typeof(MappingProfile));

//     // Register other services
//     builder.Services.AddScoped<IFarmerRepository, FarmerRepository>();
//     builder.Services.AddScoped<IBidderRepository, BidderRepository>();
//     builder.Services.AddScoped<ICropRepository, CropRepository>();
//     builder.Services.AddScoped<IBiddingCropsRepository, BiddingCropsRepository>();
//     builder.Services.AddScoped<IInsuranceRepository, InsuranceRepository>();
//     builder.Services.AddScoped<IClaim_insuranceRepository, Claim_insuranceRepository>();
//     builder.Services.AddScoped<IService,Service>();
//     builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//             .AddJwtBearer(options =>
//             {
//                 options.TokenValidationParameters = new TokenValidationParameters
//                 {
//                     ValidateIssuer = true,
//                     ValidateAudience = true,
//                     ValidateLifetime = true,
//                     ValidateIssuerSigningKey = true,
//                     ValidIssuer = "YourIssuer",
//                     ValidAudience = "YourAudience",
//                     IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Your256bitlongrandomkeyforjwtsecurity1234567890"))
//                 };
//             });
//     builder.Services.AddControllers();

// builder.Services.AddEndpointsApiExplorer();
// builder.Services.AddSwaggerGen();
// builder.Services.AddDbContext<SchemesDbContext>(options =>
//     options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// var app = builder.Build();

// // Configure the HTTP request pipeline.
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI();
// }
// app.UseAuthentication(); // Enable authentication middleware
// app.UseAuthorization();  
// app.UseHttpsRedirection();
// app.MapControllers();

// // var summaries = new[]
// // {
// //     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
// // };

// // app.MapGet("/weatherforecast", () =>
// // {
// //     var forecast =  Enumerable.Range(1, 5).Select(index =>
// //         new WeatherForecast
// //         (
// //             DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
// //             Random.Shared.Next(-20, 55),
// //             summaries[Random.Shared.Next(summaries.Length)]
// //         ))
// //         .ToArray();
// //     return forecast;
// // })
// // .WithName("GetWeatherForecast")
// // .WithOpenApi();

// app.Run();

// // record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
// // {
// //     public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
// // }
