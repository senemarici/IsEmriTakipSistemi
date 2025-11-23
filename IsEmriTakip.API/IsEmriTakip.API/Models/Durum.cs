namespace IsEmriTakip.API.Models
{
    public class Durum //Durum planı oluşturuluyor
    {
        public int DurumID { get; set; }  // kimlik numarası (Primary Key)
        public string DurumAdi { get; set; } // Metin bilgisi ("Atandı", "Devam Ediyor" , "Tamamlandı" gibi)
    }
}