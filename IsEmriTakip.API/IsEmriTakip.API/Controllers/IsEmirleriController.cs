using System.Security.Claims;
using IsEmriTakip.API.Data;
using IsEmriTakip.API.DTOs;
using IsEmriTakip.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskLink.DTOs;

namespace IsEmriTakip.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    [Authorize]
    public class IsEmirleriController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public IsEmirleriController(ApplicationDbContext context)
        {
            _context = context;
        }

       
        [HttpPost]
        
        [Authorize(Roles = "Yonetici")]
        public async Task<IActionResult> Olustur(IsEmriCreateDto createDto)
        {
            var yoneticiIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(yoneticiIdString)) return Unauthorized();

            int yoneticiId = int.Parse(yoneticiIdString);

            var yeniIsEmri = new IsEmri
            {
                Baslik = createDto.Baslik,
                Aciklama = createDto.Aciklama,
                KategoriID = createDto.KategoriID,
                OncelikID = createDto.OncelikID,
                OlusturmaTarihi = DateTime.UtcNow.AddHours(3),
                OlusturanYoneticiID = yoneticiId,
                AtananTeknisyenID = (createDto.AtananTeknisyenID.HasValue && createDto.AtananTeknisyenID > 0) ? createDto.AtananTeknisyenID : null,
                DurumID = (createDto.AtananTeknisyenID.HasValue && createDto.AtananTeknisyenID > 0) ? 2 : 1
            };

            await _context.IsEmirleri.AddAsync(yeniIsEmri);
            await _context.SaveChangesAsync();

            return Ok(new { message = "İş emri oluşturuldu.", id = yeniIsEmri.IsEmriID });
        }

        
        [HttpPut("{id}")]
        [Authorize(Roles = "Yonetici")] 
        public async Task<IActionResult> Guncelle(int id, [FromBody] IsEmriGuncelleDto dto)
        {
            var mevcut = await _context.IsEmirleri.FindAsync(id);
            if (mevcut == null) return NotFound();

            mevcut.Baslik = dto.Baslik;
            mevcut.Aciklama = dto.Aciklama;
            mevcut.KategoriID = dto.KategoriID;
            mevcut.OncelikID = dto.OncelikID;
            mevcut.DurumID = dto.DurumID;
            if (dto.AtananTeknisyenID.HasValue) mevcut.AtananTeknisyenID = dto.AtananTeknisyenID;

            await _context.SaveChangesAsync();
            return Ok(new { mesaj = "Güncellendi." });
        }

        
        [HttpPut("{id}/durum")]
        [AllowAnonymous]
         
        
        public async Task<IActionResult> DurumGuncelle(int id, [FromQuery] int yeniDurumId)
        {
            var isEmri = await _context.IsEmirleri.FindAsync(id);
            if (isEmri == null) return NotFound("İş emri bulunamadı.");

            

            isEmri.DurumID = yeniDurumId;
            await _context.SaveChangesAsync();

            return Ok(new { mesaj = "Durum başarıyla güncellendi." });
        }

        
        [HttpDelete("{id}")]
        [Authorize(Roles = "Yonetici")] 
        public async Task<IActionResult> Sil(int id)
        {
            var isEmri = await _context.IsEmirleri.FindAsync(id);
            if (isEmri == null) return NotFound();

            _context.IsEmirleri.Remove(isEmri);
            await _context.SaveChangesAsync();
            return Ok(new { mesaj = "Silindi." });
        }

       
        [HttpGet]
        
        public async Task<IActionResult> Listele()
        {
            
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            var sorgu = _context.IsEmirleri
                .Include(ie => ie.Kategori)
                .Include(ie => ie.Oncelik)
                .Include(ie => ie.Durum)
                .Include(ie => ie.OlusturanYonetici)
                .Include(ie => ie.AtananTeknisyen)
                .AsQueryable();

           

            var liste = await sorgu
                .OrderByDescending(ie => ie.OlusturmaTarihi)
                .Select(ie => new IsEmriViewDto
                {
                    IsEmriID = ie.IsEmriID,
                    Baslik = ie.Baslik,
                    Aciklama = ie.Aciklama,
                    OlusturmaTarihi = ie.OlusturmaTarihi,
                    KategoriAdi = ie.Kategori != null ? ie.Kategori.KategoriAdi : "-",
                    OncelikAdi = ie.Oncelik != null ? ie.Oncelik.OncelikAdi : "-",
                    DurumAdi = ie.Durum != null ? ie.Durum.DurumAdi : "-",
                    AtananTeknisyen = ie.AtananTeknisyen != null ? ie.AtananTeknisyen.Ad + " " + ie.AtananTeknisyen.Soyad : "Atanmadı"
                })
                .ToListAsync();

            return Ok(liste);
        }
    }
}