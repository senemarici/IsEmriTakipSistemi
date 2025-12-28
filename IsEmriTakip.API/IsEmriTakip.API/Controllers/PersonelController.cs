using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[HttpGet("teknisyenler")]
public async Task<ActionResult<IEnumerable<object>>> GetTeknisyenler()
{
    // Veritabanından sadece RolID'si 2 olanları (Teknisyenleri) çekiyoruz
    var teknisyenler = await _context.Kullanicilar
        .Where(x => x.RolID == 2) // Filtreleme: Sadece Teknisyenler
        .Select(x => new
        {
            // Frontend'e sadece lazım olan bilgileri gönderiyoruz (Şifreyi gizliyoruz!)
            x.KullaniciID,
            x.Ad,
            x.Soyad,
            x.Email,
            Unvan = "Teknisyen" // Sabit bir ünvan da ekleyebiliriz
        })
        .ToListAsync();

    return Ok(teknisyenler);
}