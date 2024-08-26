using DataLayer.Model_Blog;
using DataLayer.Model_DBContext;
using DataLayer.Model_Kullanicilar;
using DataLayer.Model_VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class YazarController : ControllerBase
    {
        private readonly Blog_DB _context;

        public YazarController(Blog_DB context)
        {
            _context = context;
        }

        [HttpPost("basliklar")]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "Yazar")]
        public async Task<IActionResult> yazilanlar(Sorgu yazar)
        {
            //int id = Convert.ToInt32(yazar); 
            if(yazar == null)
            {
                return BadRequest();
            }
            var yazarid= await _context.yazars.Where(y => y.Name == yazar.Name).Select(x => x.Id).ToListAsync();
            int sorgu = yazarid[0];
            var makaleler = await _context.makales.Where(m => m.YazarId == sorgu).ToListAsync();
            var basliklar = _context.makales
                             .Where(m => m.YazarId == sorgu)
                             .Select(m => m.Baslik)
                             .ToList();

            List<Sorgu> aa = new List<Sorgu>();
            foreach (var item in basliklar)
            {
                aa.Add(new Sorgu
                {
                    Name=item
                });
            }
            return Ok(aa);

           
        }

        [HttpPost("makale-ekle")]
        [Authorize(AuthenticationSchemes = "Bearer", Policy = "Yazar")]
        public async Task<IActionResult> MakaleYaz(MakaleViewModel model)
        {
            if (model.Baslik == null)
            {
                return BadRequest();
            }

            var catid = await _context.kategoris.FirstOrDefaultAsync(k => k.Name == model.Cat);

            if (catid == null)
            {
                catid = new Kategori
                {
                    Name = model.Cat,
                };

                _context.kategoris.Add(catid);
                await _context.SaveChangesAsync();
            }

            var yazid = await _context.yazars.FirstOrDefaultAsync(k => k.Name == model.Yaz);

            if (yazid == null)
            {
                yazid = new Yazar
                {
                    Name = model.Yaz,
                };

                _context.yazars.Add(yazid);

                await _context.SaveChangesAsync();
            }

            var kadi = await _context.kategoris.Where(y => y.Name == model.Cat).Select(x => x.Id).ToListAsync();

            var yadi = await _context.yazars.Where(y => y.Name == model.Yaz).Select(x => x.Id).ToListAsync();

            Makale donus = new Makale()
            {
                Baslik=model.Baslik,
                Icerik = model.Icerik,
                KategoriId= kadi[0],
                YazarId = yadi[0],
            };

            Kategori a = _context.kategoris.FirstOrDefault(x=>x.Name==model.Cat);

            donus.kategori=a;

            Yazar b = _context.yazars.FirstOrDefault(x => x.Name == model.Yaz);

            donus.yazar = b;


            _context.makales.Add(donus);

            await _context.SaveChangesAsync();

            return Ok("Kaydoldu");
             

        }
    }
}
