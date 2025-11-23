namespace IsEmriTakip.API.Models
{
    public class Oncelik
    {
        public int OncelikID { get; set; }   //Öncelik kimlik numarası (Primary Key)
        public string OncelikAdi { get; set; } //Örnek: (ID: 1, Ad: "Düşük"), (ID: 2, Ad: "Orta"), (ID: 3, Ad: "Yüksek")
    }
}