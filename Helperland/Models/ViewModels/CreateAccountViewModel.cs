using System.ComponentModel.DataAnnotations;

namespace Helperland.Models.ViewModels
{
    public class CreateAccountViewModel
    {
        public int UserId { get; set; }


        [Required(ErrorMessage = "please enter FirstName")]
        [MaxLength(100)]
        public string FirstName { get; set; }


        [Required]
        [MaxLength(100)]
        public string LastName { get; set; }


        [Required]
        [EmailAddress]
        [MaxLength(100)]
        public string Email { get; set; }


        [Required]
        [MaxLength(20)]
        public string Mobile { get; set; }


        [Required]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&-+=()])(?=\\S+$).{6,14}$", ErrorMessage = "You must enter At least one upper case, one lower case, one digit and Minimum six in length")]
        public string Password { get; set; }


        [Required]
        [Compare("Password", ErrorMessage = "The Password and Confirm Password fields do not match.")]
        public string ConfirmPassword { get; set; }

    }
}
