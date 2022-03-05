using System.ComponentModel.DataAnnotations;

namespace Helperland.Models.ViewModels
{
    #nullable disable
    public class CustomerSettingsViewModel
    {
        [Required]
        public settingtab1ViewModel setting1viewmodel { get; set; }

        [Required]
        public settingtab2ViewModel setting2viewmodel { get; set; }

        [Required]
        public settingtab3ViewModel setting3viewmodel { get; set; }

        


    }
}


