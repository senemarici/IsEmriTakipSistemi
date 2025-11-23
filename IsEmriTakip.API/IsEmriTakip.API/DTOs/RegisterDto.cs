using System.ComponentModel.DataAnnotations;

//KAYIT FORMU
namespace IsEmriTakip.API.DTOs
{
    public class RegisterDto
    {
        [Required]
        public string Ad { get; set; }

        [Required]
        public string Soyad { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Sifre { get; set; }

        [Required]
        public int RolID { get; set; } // 1 = Yonetici, 2 = Teknisyen
    }
}