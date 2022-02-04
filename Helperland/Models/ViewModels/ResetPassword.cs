using System.ComponentModel.DataAnnotations;

namespace Helperland.Models.ViewModels
{
    public class ResetPassword
    {
        [Required]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&-+=()])(?=\\S+$).{6,14}$", ErrorMessage = "You must enter At least one upper case, one lower case, one digit and Minimum six in length")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = "The Password and Confirm Password fields do not match.")]
        public string ConfirmPassword { get; set; }
        
         
         
    };
}
