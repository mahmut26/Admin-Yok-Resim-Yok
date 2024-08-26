using DataLayer.Model_Blog;
using DataLayer.Model_DBContext;
using DataLayer.Model_Kullanicilar;
using DataLayer.Model_Login;
using DataLayer.Model_VM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Blog_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    //[Authorize(Policy ="Admin")]
    public class AdminController : ControllerBase
    {
        private readonly Identity_DB _DB;
        private readonly Blog_DB _ctxt;
        public AdminController(Identity_DB dB,Blog_DB b_db)
        {
            _DB = dB;
            _ctxt = b_db;
        }
        
        [HttpGet("KullaniciMi")]
        public async Task<IActionResult> KullaniciMi(string a)
        {

            var aa = _DB.Users.FirstOrDefault(x => x.Email == a);

            var b = aa.IsKullanici;

            string msg = $"{aa} nın durumu {b}";

            return Ok(msg);
        }

        [HttpGet("KullaniciYap")]
        public async Task<IActionResult> KullaniciYap(string a,bool b)
        {

            var aa = _DB.Users.FirstOrDefault(x => x.Email == a);
            aa.IsKullanici=b;
            _DB.SaveChangesAsync();

            var bb = aa.Email;

            var kontrol = _ctxt.kullanicis.FirstOrDefault(x=>x.Name==bb);

            if (kontrol == null)
            {
                Kullanici kullanici = new Kullanici()
                {
                    Name = bb
                };

                _ctxt.kullanicis.Add(kullanici);

                _ctxt.SaveChangesAsync();

                return Ok(aa);
            }
            else
            {
                return BadRequest("Var zaten sadece kullanilik claimi değişti !! - ");
            }
           
        }

        [HttpGet("MailOrderOnay")]
        public async Task<IActionResult> MailOrderOnay(string a, bool b)
        {

            var aa = _DB.Users.FirstOrDefault(x => x.Email == a);
            aa.EmailConfirmed = b;
            _DB.SaveChangesAsync();

            return Ok(aa);
        }
        [HttpGet("YazarMi")]
        public async Task<IActionResult> YazarMi(string a)
        {

            var aa = _DB.Users.FirstOrDefault(x => x.Email == a);

            var b = aa.IsYazar;

            string msg = $"{aa} nın durumu {b}";

            return Ok(msg);
        }

        [HttpGet("YazarYap")]
        public async Task<IActionResult> YazarYap(string a, bool b)
        {

            var aa = _DB.Users.FirstOrDefault(x => x.Email == a);
            aa.IsYazar = b;
            _DB.SaveChangesAsync();

            var bb = aa.Email; var kontrol = _ctxt.kullanicis.FirstOrDefault(x => x.Name == bb);

            if (kontrol == null)
            {
                Yazar yazar = new Yazar()
                {
                    Name = bb
                };

                _ctxt.yazars.Add(yazar);

                _ctxt.SaveChangesAsync();

                return Ok(aa);
            }
            else
            {
                return BadRequest("Var zaten sadece yazarlik claimi değişti !! - ");
            }

            //Yazar yazar = new Yazar()
            //{
            //    Name = bb
            //};

            //_ctxt.yazars.Add(yazar);

            //_ctxt.SaveChangesAsync();

            //return Ok(aa);
        }

        [HttpGet("MakaleOnay")] //!!! Onay Yok Şu anda !!
        public async Task<IActionResult> MakaleOnay(string a, bool b)
        {

            var aa = _ctxt.makales.FirstOrDefault(x => x.Baslik == a);
            //aa.onay = true;
            _DB.SaveChangesAsync();

            return Ok(aa);
        }
        [HttpGet("Durum")] //!!! Onay Yok Şu anda !!
        public async Task<IActionResult> Durum()
        {

            var aa = _DB.Users.Select(x => new
            {
                x.UserName,
                x.EmailConfirmed,
                x.IsAdmin,
                x.IsYazar,
                x.IsKullanici
            }).ToList();
            //aa.onay = true;

            List<KullanicilarinDurumlari> kullanicilarinDurumlari = new List<KullanicilarinDurumlari>();

            foreach (var user in aa) 
            {

                kullanicilarinDurumlari.Add(new KullanicilarinDurumlari
                {
                    UserName = user.UserName,
                    EmailConfirmed = user.EmailConfirmed,
                    IsAdmin=user.IsAdmin,
                    IsKullanici=user.IsKullanici,
                    IsYazar=user.IsYazar
                });
            }

            return Ok(kullanicilarinDurumlari);
        }

    }
}
