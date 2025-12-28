namespace TaskLink.DTOs
{
    public class IsEmriGuncelleDto
    {
        public string Baslik { get; set; }
        public string Aciklama { get; set; }
        public int KategoriID { get; set; }
        public int OncelikID { get; set; }
        public int DurumID { get; set; }

 
        public int? AtananTeknisyenID { get; set; }
    }
}