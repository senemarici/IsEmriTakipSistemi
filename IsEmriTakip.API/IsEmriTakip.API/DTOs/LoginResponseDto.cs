using System.ComponentModel.DataAnnotations;

//GİRİŞ BAŞARILI-BAŞARISIZ CEVABI
namespace IsEmriTakip.API.DTOs
{
    public class LoginResponseDto
    {
        public string Email { get; set; }
        public string Rol { get; set; }
        public string Token { get; set; } // En önemlisi: JWT Jetonu
    }
}