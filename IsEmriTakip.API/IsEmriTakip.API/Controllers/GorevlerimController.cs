using IsEmriTakip.API.Data;
using IsEmriTakip.API.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace IsEmriTakip.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teknisyen")] // <-- DİKKAT: Sadece Teknisyenler Girebilir!
    public class GorevlerimController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GorevlerimController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. BANA ATANAN GÖREVLERİ LİSTELE (GET api/gorevlerim)
        [HttpGet]
        public async Task<IActionResult> GetBenimGorevlerim()
        {
            // Giriş yapan teknisyenin ID'sini Token'dan al
            var teknisyenIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(teknisyenIdString)) return Unauthorized();
            int teknisyenId = int.Parse(teknisyenIdString);

            var gorevler = await _context.IsEmirleri
                .Include(x => x.Kategori)
                .Include(x => x.Oncelik)
                .Include(x => x.Durum)
                .Include(x => x.OlusturanYonetici)
                // FİLTRE: Sadece "AtananTeknisyenID"si benim ID'm olanları getir!
                .Where(x => x.AtananTeknisyenID == teknisyenId)
                .OrderByDescending(x => x.OlusturmaTarihi)
                .Select(x => new IsEmriViewDto
                {
                    IsEmriID = x.IsEmriID,
                    Baslik = x.Baslik,
                    Aciklama = x.Aciklama,
                    OlusturmaTarihi = x.OlusturmaTarihi,
                    KategoriAdi = x.Kategori.KategoriAdi,
                    OncelikAdi = x.Oncelik.OncelikAdi,
                    DurumAdi = x.Durum.DurumAdi,
                    OlusturanYonetici = x.OlusturanYonetici.Ad + " " + x.OlusturanYonetici.Soyad,
                    AtananTeknisyen = "Ben" // Zaten kendisine atananları görüyor
                })
                .ToListAsync();

            return Ok(gorevler);
        }

        // 2. GÖREV DURUMUNU GÜNCELLE (PUT api/gorevlerim/{id}/durum)
        // Örn: api/gorevlerim/5/durum
        [HttpPut("{id}/durum")]
        public async Task<IActionResult> DurumGuncelle(int id, [FromBody] GorevDurumUpdateDto updateDto)
        {
            // Giriş yapan teknisyenin ID'sini al
            var teknisyenIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int teknisyenId = int.Parse(teknisyenIdString);

            // 1. İş emrini bul
            var isEmri = await _context.IsEmirleri.FindAsync(id);

            if (isEmri == null)
                return NotFound("İş emri bulunamadı.");

            // 2. GÜVENLİK KONTROLÜ: Bu iş gerçekten bu teknisyene mi ait?
            // Başkasının işini tamamlayamasın!
            if (isEmri.AtananTeknisyenID != teknisyenId)
            {
                return BadRequest("Bu işlem size atanmamış, durumunu değiştiremezsiniz.");
            }

            // 3. Durumu güncelle
            isEmri.DurumID = updateDto.YeniDurumID;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Görev durumu güncellendi." });
        }
    }
}