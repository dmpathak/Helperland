using System.ComponentModel.DataAnnotations;

namespace Helperland.Models.ViewModels
{
    public class CustomerDashboardViewModel
    {
        public string service_provide_name { get; set; }


        //rating of history page
        public string? Comments { get; set; }
        public decimal OnTimeArrival { get; set; }
        public decimal Friendly { get; set; }
        public decimal QualityOfService { get; set; }


        //1st modal 
        public int ServiceId { get; set; }

        public string date { get; set; }

        public string start_time { get; set; }

        public string end_time { get; set; }

        public string service_duration { get; set; }

        public IEnumerable<bool> extras { get; set; }

        public decimal total_cost { get; set; }

        [Required]
        public string street_name { get; set; }

        [Required]
        public string house_number { get; set; }

        [Required]
        public string? city { get; set; }

        [Required]
        public string? phone_number { get; set; }

        public string email { get; set; }

        public bool have_pets { get; set; }

        public string why_cancel { get; set; }





    }

}
