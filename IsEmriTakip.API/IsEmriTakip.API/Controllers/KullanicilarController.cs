using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using IsEmriTakip.API.Data;
using IsEmriTakip.API.Models; 

namespace IsEmriTakip.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KullanicilarController : ControllerBase
    {
        
        private readonly ApplicationDbContext _context;

        
        public KullanicilarController(ApplicationDbContext context)
        {
            _context = context;
        }

        
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Kullanici>>> GetKullanicilar()
        {
            return await _context.Kullanicilar.ToListAsync();
        }

        
        [HttpGet("teknisyenler")]
        public async Task<ActionResult<IEnumerable<object>>> GetTeknisyenler()
        {
            var teknisyenler = await _context.Kullanicilar
                .Where(x => x.RolID == 2)
                .Select(x => new
                {
                    x.KullaniciID,
                    AdSoyad = x.Ad + " " + x.Soyad,
                    x.Email,
                    
                    Unvan = "Teknisyen"
                })
                .ToListAsync();

            return Ok(teknisyenler);
        }
    }
}