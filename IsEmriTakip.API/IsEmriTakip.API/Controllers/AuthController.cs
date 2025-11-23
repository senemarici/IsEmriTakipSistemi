using IsEmriTakip.API.Data;
using IsEmriTakip.API.DTOs;
using IsEmriTakip.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace IsEmriTakip.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        // POST: api/auth/register
        [HttpPost("register")]  //Metodun tam adresini belirtir. bu adresin "yeni veri kaydetmek" için kullanıldığını belirtir.
        public async Task<IActionResult> Register(RegisterDto registerDto)
        {
            // 1. Bu email adresi zaten var mı?
            if (await _context.Kullanicilar.AnyAsync(k => k.Email == registerDto.Email))  // eposta kullanımda mı kontrol eder.
            {
                return BadRequest("Bu email adresi zaten kullanılıyor.");
            }

            // 2. Şifreyi Hash'le (BCrypt)
            string sifreHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Sifre);

            // 3. Yeni Kullanıcı Nesnesi Oluştur
            var yeniKullanici = new Kullanici //verileri DTOdan alıp hash'lenmiş şifre olarak veritabanına doldurur.
            {
                Ad = registerDto.Ad,
                Soyad = registerDto.Soyad,
                Email = registerDto.Email,
                SifreHash = sifreHash,
                RolID = registerDto.RolID
            };

            // 4. Veritabanına kaydet
            await _context.Kullanicilar.AddAsync(yeniKullanici);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kullanıcı başarıyla oluşturuldu." });
        }

        // POST: api/auth/login => adresi bu şekilde belirler.
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto loginDto) //LoginDto formatında bir veri girişi bekler.
        {
            // 1. Kullanıcıyı bul
            var kullanici = await _context.Kullanicilar
                                    .Include(k => k.Rol) // Rol bilgisini de çek
                                    .FirstOrDefaultAsync(k => k.Email == loginDto.Email); //E-postası LoginDtodan gelen e postayla eşleşen ilk kullanıcıyı bul.

            if (kullanici == null)
            {
                return Unauthorized("Geçersiz email veya şifre.");
            }

            // 2. Şifreyi Doğrula
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Sifre, kullanici.SifreHash))
            {
                return Unauthorized("Geçersiz email veya şifre.");
            }

            // 3. Şifre doğruysa -> JWT Token Oluştur
            var token = CreateJwtToken(kullanici);

            // 4. Cevabı (LoginResponseDto) hazırla
            var response = new LoginResponseDto
            {
                Email = kullanici.Email,
                Rol = kullanici.Rol.RolAdi,
                Token = token
            };

            return Ok(response);
        }

        // --- Helper Metot --- JWT Üretici
        private string CreateJwtToken(Kullanici kullanici)
        {
            // 1. "Gizli Anahtarı" (Key) Al
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            // 2. "İmza" (Credentials) Oluştur
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            // Token'ın içine hangi bilgileri (claims) koyacağımızı belirliyoruz. Herkes okuyabilir ama değiştiremez.
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, kullanici.KullaniciID.ToString()), // Benzersiz ID
                new Claim(JwtRegisteredClaimNames.Email, kullanici.Email),
                new Claim(ClaimTypes.Role, kullanici.Rol.RolAdi) // Kullanıcının Rolü
            };

            //4. Kartın Kendisini (Token) Oluştur
            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"], //Kim yayınladı?
                audience: _configuration["Jwt:Audience"], //Kimin için?
                claims: claims, 
                expires: DateTime.Now.AddDays(1), // Token 1 gün geçerli olsun
                signingCredentials: credentials);

            // 5. Kartı Metne Çevir ve Gönder
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}