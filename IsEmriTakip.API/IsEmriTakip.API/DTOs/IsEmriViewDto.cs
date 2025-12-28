using System.ComponentModel.DataAnnotations;

//İŞ EMRİ GÖRÜNTÜLEME KUTUSU
namespace IsEmriTakip.API.DTOs
{
    public class IsEmriViewDto
    {
        public int IsEmriID { get; set; }
        public string Baslik { get; set; }
        public string? Aciklama { get; set; }
        public DateTime OlusturmaTarihi { get; set; }

        
        public string KategoriAdi { get; set; }
        public string OncelikAdi { get; set; }
        public string DurumAdi { get; set; }

        public string OlusturanYonetici { get; set; }
        public string? AtananTeknisyen { get; set; }
        public int OlusturanYoneticiID { get; internal set; }
        public int? AtananTeknisyenID { get; internal set; }
    }
}