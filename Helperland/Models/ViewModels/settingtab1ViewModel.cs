using System.ComponentModel.DataAnnotations;

namespace Helperland.Models.ViewModels
{
    public class settingtab1ViewModel
    {
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

        public string language { get; set; }

    }
}
