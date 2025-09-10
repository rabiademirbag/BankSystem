using System.ComponentModel.DataAnnotations;

namespace BankSystem.WebApi.Models
{
    public class ChangePasswordRequest
    {
        [Required]
        public string Password { get; set; }

        [MinLength(6, ErrorMessage = "Şifre en az 6 karakter olmalıdır.")]
        [RegularExpression(@"^(?=.*[A-Z])(?=.*[\W_]).+$",
   ErrorMessage = "Şifre en az bir büyük harf ve bir özel karakter içermelidir.")]
        [Required]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword))]
        [Required]
        public string NewPasswordConfirm { get; set; }

    }
}
