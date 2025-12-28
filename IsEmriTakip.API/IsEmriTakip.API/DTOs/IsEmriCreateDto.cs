using System.ComponentModel.DataAnnotations;

//İŞ EMRİ OLUŞTURMA FORMU
namespace IsEmriTakip.API.DTOs
{
    public class IsEmriCreateDto
    {
        [Required]
        public string Baslik { get; set; }

        public string? Aciklama { get; set; }

        [Required]
        public int KategoriID { get; set; }

        [Required]
        public int OncelikID { get; set; }

        public int? AtananTeknisyenID { get; set; }

        
    }
}