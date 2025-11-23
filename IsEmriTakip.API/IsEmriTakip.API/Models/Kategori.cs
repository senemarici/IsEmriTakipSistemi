namespace IsEmriTakip.API.Models
{
    public class Kategori
    {
        public int KategoriID { get; set; } //(Kimlik Numarası)
        public string KategoriAdi { get; set; } //Örnek: (ID: 1, Ad: "Bakım"), (ID: 2, Ad: "Arıza"), (ID: 3, Ad: "Montaj")
    }
}