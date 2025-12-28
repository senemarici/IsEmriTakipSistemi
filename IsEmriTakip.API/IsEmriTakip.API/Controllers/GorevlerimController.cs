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
    [Authorize(Roles = "Teknisyen")] 
    public class GorevlerimController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public GorevlerimController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<IActionResult> GetBenimGorevlerim()
        {
            
            var teknisyenIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(teknisyenIdString)) return Unauthorized();
            int teknisyenId = int.Parse(teknisyenIdString);

            var gorevler = await _context.IsEmirleri
                .Include(x => x.Kategori)
                .Include(x => x.Oncelik)
                .Include(x => x.Durum)
                .Include(x => x.OlusturanYonetici)
                
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
                    AtananTeknisyen = "Ben" 
                })
                .ToListAsync();

            return Ok(gorevler);
        }

        
        [HttpPut("{id}/durum")]
        public async Task<IActionResult> DurumGuncelle(int id, [FromBody] GorevDurumUpdateDto updateDto)
        {
            
            var teknisyenIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int teknisyenId = int.Parse(teknisyenIdString);

            
            var isEmri = await _context.IsEmirleri.FindAsync(id);

            if (isEmri == null)
                return NotFound("İş emri bulunamadı.");

            
            if (isEmri.AtananTeknisyenID != teknisyenId)
            {
                return BadRequest("Bu işlem size atanmamış, durumunu değiştiremezsiniz.");
            }

            
            isEmri.DurumID = updateDto.YeniDurumID;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Görev durumu güncellendi." });
        }
    }
}