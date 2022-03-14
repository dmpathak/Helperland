using System.ComponentModel.DataAnnotations;


namespace Helperland.Models.ViewModels
{
    public class ProviderDashboardViewModel
    {
        public int UserId { get; set; }
        public int ServiceId { get; set; }

        public string comment { get; set; }


        //1

        [Required(ErrorMessage = "please enter FirstName")]
        [MaxLength(100)]
        public string first_name { get; set; }

        [Required(ErrorMessage = "please enter LastName")]
        [MaxLength(100)]
        public string last_name { get; set; }

        public string email { get; set; }

        [Required(ErrorMessage = "please enter Mobile number")]
        [MaxLength(20)]
        public string mobile { get; set; }

        [Required(ErrorMessage = "please enter Date of Birth")]
        public string date { get; set; }

        [Required(ErrorMessage = "please enter Date of Birth")]
        public string month { get; set; }

        [Required(ErrorMessage = "please enter Date of Birth")]
        public string year { get; set; }

        [Required(ErrorMessage = "please Selecct Nationality")]
        public int nationality { get; set; }

        [Required(ErrorMessage = "please Select Gender")]
        public int gender { get; set; }

        [Required(ErrorMessage = "please select Avtar")]
        public string avtar { get; set; }


        //2
        [Required(ErrorMessage = "please enter street name")]
        public string street { get; set; }

        [Required(ErrorMessage = "please enter House number")]
        public string house { get; set; }

        [Required(ErrorMessage = "please enter Postalcode")]
        public string postcode { get; set; }

        [Required(ErrorMessage = "please enter City")]
        public string city { get; set; }



        //3
        [Required(ErrorMessage = "please enter oldpassword")]
        public string oldpassword { get; set; }

        [Required(ErrorMessage = "please enter newpassword")]
        [RegularExpression("^(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&-+=()])(?=\\S+$).{6,14}$", ErrorMessage = "You must enter At least one upper case, one lower case, one digit and Minimum six in length")]
        public string newpassword { get; set; }

        [Required(ErrorMessage = "please enter confirmpassword")]
        [Compare("newpassword", ErrorMessage = "The Password and Confirm Password fields do not match.")]
        public string confirmpassword { get; set; }






    }
}
