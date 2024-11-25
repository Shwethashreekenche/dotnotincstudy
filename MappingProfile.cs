using AutoMapper;
using Schemes_for_Farmers.Models;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Admin, AdminLoginDTO>();  
        CreateMap<AdminLoginDTO,Admin>();
        
        CreateMap<Farmer, LoginDTO>();  
        CreateMap<LoginDTO,Farmer>();

        CreateMap<Bidder, LoginDTO>();  
        CreateMap<LoginDTO,Bidder>();
        //Farmer
        CreateMap<Farmer, FarmerDTO>();  
        CreateMap<FarmerDTO,Farmer>();  
        //Bidder
        CreateMap<Bidder, BidderDTO>();  
        CreateMap<BidderDTO,Bidder>();  
        //Crop
        CreateMap<Crop, CropDTO>();  
        CreateMap<CropDTO,Crop>();  
        //Insurance
        CreateMap<Insurance, InsuranceDTO>();  
        CreateMap<InsuranceDTO,Insurance>();  
        //ClaimInsurance
        CreateMap<Claim_insurance, Claim_insuranceDTO>();  
        CreateMap<Claim_insuranceDTO,Claim_insurance>();  
        //BiddingCrop
        CreateMap<BiddingCrops, BiddingCropsDTO>();  
        CreateMap<BiddingCropsDTO,BiddingCrops>();  
    }
}
