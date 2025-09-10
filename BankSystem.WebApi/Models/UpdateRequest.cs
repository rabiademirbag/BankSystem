using System.ComponentModel.DataAnnotations;

namespace BankSystem.WebApi.Models
{
    public class UpdateRequest
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [MaxLength(100)]
        [Required]
        public string FirstName { get; set; }

        [MaxLength(100)]
        [Required]
        public string LastName { get; set; }

        [Phone]
        [Required]
        public string PhoneNumber { get; set; }

    }
}
