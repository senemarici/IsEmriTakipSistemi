using IsEmriTakip.API.Data;
using IsEmriTakip.API.DTOs;
using IsEmriTakip.API.Models;
using Microsoft.AspNetCore.Authorization; // <-- KORUMA İÇİN
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims; // <-- TOKEN'DAN VERİ OKUMAK İÇİN

namespace IsEmriTakip.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Yonetici")] // <-- !!! EN ÖNEMLİ KISIM !!!
    // Bu satır, bu controller'daki TÜM endpoint'lerin 
    // "Yonetici" rolüne sahip geçerli bir token gerektirdiğini söyler.
    public class IsEmirleriController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IsEmirleriController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: api/isemirleri
        [HttpPost]
        public async Task<IActionResult> Olustur(IsEmriCreateDto createDto)
        {
            // Token'dan giriş yapan yöneticinin ID'sini alıyoruz
            var yoneticiIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(yoneticiIdString))
            {
                return Unauthorized("Token'dan kullanıcı ID'si okunamadı.");
            }

            int yoneticiId = int.Parse(yoneticiIdString);

            var yeniIsEmri = new IsEmri
            {
                Baslik = createDto.Baslik,
                Aciklama = createDto.Aciklama,
                KategoriID = createDto.KategoriID,
                OncelikID = createDto.OncelikID,
                OlusturmaTarihi = DateTime.UtcNow.AddHours(3), // Türkiye saati için +3
                DurumID = 1, // 1 = "Atandı" (Bunu Adım 12'de garantilemiştik)
                OlusturanYoneticiID = yoneticiId,
                AtananTeknisyenID = null // Henüz atama yok
            };

            await _context.IsEmirleri.AddAsync(yeniIsEmri);
            await _context.SaveChangesAsync();

            return Ok(new { message = "İş emri başarıyla oluşturuldu.", id = yeniIsEmri.IsEmriID });
        }

        // GET: api/isemirleri
        [HttpGet]
        public async Task<IActionResult> Listele()
        {
            var isEmirleri = await _context.IsEmirleri
                .Include(ie => ie.Kategori) // Kategori bilgilerini çek
                .Include(ie => ie.Oncelik) // Oncelik bilgilerini çek
                .Include(ie => ie.Durum)   // Durum bilgilerini çek
                .Include(ie => ie.OlusturanYonetici) // Yönetici bilgilerini çek
                .Include(ie => ie.AtananTeknisyen)   // Teknisyen bilgilerini çek
                .OrderByDescending(ie => ie.OlusturmaTarihi)
                .Select(ie => new IsEmriViewDto // Veriyi DTO'ya dönüştür
                {
                    IsEmriID = ie.IsEmriID,
                    Baslik = ie.Baslik,
                    Aciklama = ie.Aciklama,
                    OlusturmaTarihi = ie.OlusturmaTarihi,
                    KategoriAdi = ie.Kategori.KategoriAdi,
                    OncelikAdi = ie.Oncelik.OncelikAdi,
                    DurumAdi = ie.Durum.DurumAdi,
                    OlusturanYonetici = ie.OlusturanYonetici.Ad + " " + ie.OlusturanYonetici.Soyad,
                    AtananTeknisyen = ie.AtananTeknisyen == null ? "Atanmadı" : (ie.AtananTeknisyen.Ad + " " + ie.AtananTeknisyen.Soyad)
                })
                .ToListAsync();

            return Ok(isEmirleri);
        }
    }
}