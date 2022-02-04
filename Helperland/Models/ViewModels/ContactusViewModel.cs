using System.ComponentModel.DataAnnotations;

namespace Helperland.Models.ViewModels
{
    public class ContactusViewModel
    {   
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        [Required]
        [MaxLength(20)]
        public string PhoneNumber { get; set; } = null!;
        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = null!;
        [Required]
        [MaxLength(500)]
        public string Subject { get; set; }
        [Required]
        public string Message { get; set; } = null!;

        public IFormFile FileUpload { get; set; }

    }
}
