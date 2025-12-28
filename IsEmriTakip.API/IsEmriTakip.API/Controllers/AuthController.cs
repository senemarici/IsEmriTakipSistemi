using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using IsEmriTakip.API.Data;
using IsEmriTakip.API.Models;

using BCrypt.Net;

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

        // --- MODELLER (DTO) ---
        public class LoginDto
        {
            public string Email { get; set; }
            public string Sifre { get; set; }
        }

        public class RegisterDto
        {
            public string Ad { get; set; }
            public string Soyad { get; set; }
            public string Email { get; set; }
            public string Sifre { get; set; }
            public int RolID { get; set; } // 1: Yönetici, 2: Teknisyen
        }

        // --- 1. KAYIT OL (REGISTER) ---
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            
            if (await _context.Kullanicilar.AnyAsync(x => x.Email == model.Email))
            {
                return BadRequest(new { message = "Bu e-posta adresi zaten kullanımda." });
            }

            
            string hashliSifre = BCrypt.Net.BCrypt.HashPassword(model.Sifre);

           
            var yeniKullanici = new Kullanici
            {
                Ad = model.Ad,
                Soyad = model.Soyad,
                Email = model.Email,
                SifreHash = hashliSifre, 
                RolID = model.RolID
            };

            _context.Kullanicilar.Add(yeniKullanici);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Kullanıcı başarıyla oluşturuldu." });
        }

        // --- 2. GİRİŞ YAP (LOGIN) ---
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
        {
            
            var kullanici = await _context.Kullanicilar
                .Include(k => k.Rol)
                .FirstOrDefaultAsync(k => k.Email == loginDto.Email);

            if (kullanici == null)
            {
                return Unauthorized(new { message = "Kullanıcı bulunamadı." });
            }

            
            bool sifreDogruMu = BCrypt.Net.BCrypt.Verify(loginDto.Sifre, kullanici.SifreHash);

            if (!sifreDogruMu)
            {
                return Unauthorized(new { message = "Şifre hatalı!" });
            }

            
            var token = CreateJwtToken(kullanici);

            return Ok(new
            {
                token = token,
                email = kullanici.Email,
                adSoyad = kullanici.Ad + " " + kullanici.Soyad,
                rol = kullanici.Rol != null ? kullanici.Rol.RolAdi : "Teknisyen",
                rolId = kullanici.RolID
            });
        }

        // --- TOKEN ÜRETİCİ ---
        private string CreateJwtToken(Kullanici kullanici)
        {
            var keyString = _configuration["Jwt:Key"] ?? "BuBenimCokGizliVeUzunAnahtarim123456789";
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, kullanici.KullaniciID.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, kullanici.Email),
                new Claim(ClaimTypes.Role, kullanici.Rol != null ? kullanici.Rol.RolAdi : "Teknisyen"),
                new Claim("AdSoyad", kullanici.Ad + " " + kullanici.Soyad)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"] ?? "IsEmriTakipAPI",
                audience: _configuration["Jwt:Audience"] ?? "IsEmriTakipFrontend",
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}