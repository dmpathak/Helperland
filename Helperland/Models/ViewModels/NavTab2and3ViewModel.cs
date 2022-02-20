using System.ComponentModel.DataAnnotations;

namespace Helperland.Models.ViewModels
{
    public class NavTab2and3ViewModel
    {
        public int UserId { get; set; }

        public string pincode { get; set; }

        public DateTime DateTime { get; set; }

        public string basictime { get; set; }

        public string service_choosen   { get; set; }

        public string comment { get; set; }

        public bool is_pet { get; set; }    

        public string add_id { get; set; }

        public int id_of_fav_provider { get; set; }

        public string fav_provider { get; set; }


    }
}

