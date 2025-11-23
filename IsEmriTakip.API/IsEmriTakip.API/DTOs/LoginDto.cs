using System.ComponentModel.DataAnnotations;

//GİRİŞ FORMU
namespace IsEmriTakip.API.DTOs
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Sifre { get; set; }
    }
}
