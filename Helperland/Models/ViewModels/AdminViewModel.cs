using System.ComponentModel.DataAnnotations;

namespace Helperland.Models.ViewModels
{
    public class AdminViewModel
    {
        public int id_for_user { get; set; }


        public int id_for_service { get; set; }

        [Required]
        public string date { get; set; }

        [Required]
        public string time { get; set; }

        [Required]
        public string street { get; set; }

        [Required]
        public string house { get; set; }

        [Required]
        public string postal { get; set; }

        [Required]
        public string city { get; set; }

        public string comment { get; set; }

        [Required]
        public decimal refund_amount { get; set; }
  

    }
}