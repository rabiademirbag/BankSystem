using System.ComponentModel.DataAnnotations;

namespace BankSystem.WebApi.Models
{
    public class RegisterRequest
    {
        [MaxLength(100)]
        [Required]

        public string FirstName { get; set; }

        [MaxLength(100)]
        [Required]

        public string LastName { get; set; }

        [EmailAddress]
        [Required]

        public string Email { get; set; }

        [Phone]
        [Required]

        public string PhoneNumber { get; set; }

        [RegularExpression(@"^\d{11}$", ErrorMessage = "TC Kimlik Numarası 11 haneli rakamlardan oluşmalıdır.")]
        [Required]

        public string NationalId { get; set; }

        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).+$",
        ErrorMessage = "Şifre en az bir büyük harf ve bir özel karakter içermelidir.")]
        [Required]

        public string Password { get; set; }

        [Compare(nameof(Password))]
        [Required]

        public string ConfirmPassword { get; set; }
    }
}
