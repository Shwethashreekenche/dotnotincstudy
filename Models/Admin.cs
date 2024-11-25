using System.ComponentModel.DataAnnotations;

namespace Schemes_for_Farmers.Models;
public class Admin
{
    [Key]
    public string AdminId{get; set;}

    public string EmailId{get; set;}

    public string Password{get; set;}

    public string FullName{get; set;}
}