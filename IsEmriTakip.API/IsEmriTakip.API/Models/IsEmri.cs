using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace IsEmriTakip.API.Models
{
    public class IsEmri
    {
        [Key]
        public int IsEmriID { get; set; }

        [Required]
        [StringLength(250)]
        public string Baslik { get; set; }

        public string Aciklama { get; set; }

        public DateTime OlusturmaTarihi { get; set; }

        // --- İlişkiler (Foreign Keys) ---

        // Kategori ile ilişki
        public int KategoriID { get; set; }
        [ForeignKey("KategoriID")]
        public virtual Kategori? Kategori { get; set; }

        // Öncelik ile ilişki
        public int OncelikID { get; set; }
        [ForeignKey("OncelikID")]
        public virtual Oncelik? Oncelik { get; set; }

        // Durum ile ilişki
        public int DurumID { get; set; }
        [ForeignKey("DurumID")]
        public virtual Durum? Durum { get; set; }

        // İş emrini oluşturan Yönetici ile ilişki
        public int OlusturanYoneticiID { get; set; }
        [ForeignKey("OlusturanYoneticiID")]
        public virtual Kullanici? OlusturanYonetici { get; set; }

        // İş emrinin atandığı Teknisyen ile ilişki
        // Null olabilir (atanmamış görevler için) ? işareti koyuyoruz
        public int? AtananTeknisyenID { get; set; }
        [ForeignKey("AtananTeknisyenID")]
        public virtual Kullanici? AtananTeknisyen { get; set; }
    }
}
