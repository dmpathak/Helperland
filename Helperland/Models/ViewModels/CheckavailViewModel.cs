using System.ComponentModel.DataAnnotations;

namespace Helperland.Models.ViewModels
{
    public class CheckavailViewModel
    {
        public string MyCheckavail { get; set; }

        // NavTab 2 and 3rd
        public int UserId { get; set; }

        public string pincode { get; set; }

        public string date { get; set; }

        public string time { get; set; }

        public string basic_time { get; set; }

        public IEnumerable<bool> extra_services { get; set; }

        public string comments { get; set; }

        public bool have_pets { get; set; }

        public int address_id { get; set; }

        [Required]
        public string street_name { get; set; }

        [Required]
        public string house_number { get; set; }

        public string? city  { get; set; }

        public string? phone_number { get; set; }

        public string total_time { get; set; }

        public string? id_of_fav_provider { get; set; }

        public string? fav_provider { get; set; }

        public int selected_service_pro_id { get; set; }
    }
}
