using System.ComponentModel.DataAnnotations;

namespace Helperland.Models.ViewModels
{
    public class settingtab3ViewModel
    {
        [Required]
        public string oldpassword { get; set; }

        [Required]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&-+=()])(?=\\S+$).{6,14}$", ErrorMessage = "You must enter At least one upper case, one lower case, one digit and Minimum six in length")]
        public string newpassword { get; set; }

        [Required]
        [Compare("newpassword", ErrorMessage = "The Password and Confirm Password fields do not match.")]
        public string confirmpassword { get; set; }
    }
}
