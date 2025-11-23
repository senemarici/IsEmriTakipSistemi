using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsEmriTakip.API.Models
{
    public class Kullanici
    {
        [Key] // Bu özelliğin Primary Key olduğunu belirtir
        public int KullaniciID { get; set; }

        [Required] // Bu alanın zorunlu olduğunu belirtir
        [StringLength(100)]
        public string Ad { get; set; }

        [Required]
        [StringLength(100)]
        public string Soyad { get; set; }

        [Required]
        [StringLength(100)]
        public string Email { get; set; } // Bunu Unique (benzersiz) yapacağız

        [Required]
        public string SifreHash { get; set; } // Asla düz metin şifre tutmuyoruz!

        // --- İlişkiler (Foreign Key) ---

        public int RolID { get; set; }

        [ForeignKey("RolID")] // Bu özelliğin RolID'ye bağlı olduğunu belirtir
        public virtual Rol Rol { get; set; }
    }
}