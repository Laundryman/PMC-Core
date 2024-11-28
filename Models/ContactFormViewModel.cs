using System.ComponentModel.DataAnnotations;

namespace CoreSystem.Models
{
    public class ContactFormViewModel
    {
        public ContactFormViewModel()
        {
        }

        [Required]
        public string Name { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Comment { get; set; }

    }
}