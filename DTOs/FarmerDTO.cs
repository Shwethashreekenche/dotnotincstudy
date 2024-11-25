using System.ComponentModel.DataAnnotations;

public class FarmerDTO
{
    [StringLength(100, MinimumLength = 3, ErrorMessage = "The product name must be between 3 and 100 characters.")]
    [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "The product name can only contain letters and spaces.")]
    public string FullName{get; set;}

    [Required(ErrorMessage = "Phone number is required ok na.")]
    [Phone(ErrorMessage = "Invalid phone number format.")]
    public string ContactNo{get; set;}

    [EmailAddress(ErrorMessage = "The email format is invalid.")]
    public string EmailId{get; set;}
    public string Address{get; set;}

    public double LandArea{get; set;}
    public string LandAddress{get; set;}

    [Required(ErrorMessage = "Account number is required.")]
    [RegularExpression(@"^\d{9,18}$", ErrorMessage = "Invalid account number. It must be between 9 and 18 digits.")]
    public string AccountNo{get; set;}

    [Required(ErrorMessage = "IFSC code is required.")]
    [RegularExpression(@"^[A-Z]{4}0[A-Z0-9]{6}$", ErrorMessage = "Invalid IFSC code. It must be in the format 'AAAA0BBBBBB'.")]
    public string IFSCCode{get; set;}

    public byte[] Aadhaar{get; set;}

    public byte[] Pan{get; set;}

    public byte[] Certificate{get; set;}
    [Required(ErrorMessage = "Password is required.")]
    [StringLength(12, MinimumLength = 4, ErrorMessage = "Password must be at least 8 characters long.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[@$!%*?&#_.])[A-Za-z\d@$!%*?&#_.]{4,8}$",
        ErrorMessage = "Password must contain at least one uppercase letter, one lowercase letter, one number, and one special character.")]
    public string Password{get; set;}  
    
    public string? GrantAccess{get; set;}=null;
}