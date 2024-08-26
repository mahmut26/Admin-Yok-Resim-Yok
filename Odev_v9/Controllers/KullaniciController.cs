using DataLayer.Model_VM;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Net.Http;
using DataLayer.Model_Parcala;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataLayer.Model_DBContext;
using Microsoft.EntityFrameworkCore;
using DataLayer.Model_Blog;

namespace Odev_v9.Controllers
{
    public class KullaniciController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly Blog_DB context;

        public KullaniciController(IHttpClientFactory httpClientFactory,Blog_DB _DB)
        {
            _httpClientFactory = httpClientFactory;
            context = _DB;
        }
        [HttpGet]
        public IActionResult KategoriEkle()
        {
            List<string> categories = context.kategoris.Select(x => x.Name).ToList();
            ViewBag.Categories = new SelectList(categories, "Name");
            return View();
            //return View();
        }
        [HttpPost]
        public async Task<IActionResult> Ekle(Link link)
        {
            string token = HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string baseUrl = "https://localhost:7052/api/";
            string metot = "Kullanici/kategori-ekle";

            

            
            string url = string.Concat(baseUrl, metot);
            if (token == null)
            {
                ViewBag.Mesaj = "Token yok olm nereye gidiyorsun";
                return View();
            }
            string jsonPayload = Base64UrlHelper.Base64UrlDecode(token.Split('.')[1]);
            var payload = JsonConvert.DeserializeObject<JwtPayload>(jsonPayload);
            string yazarid = (payload.Name); //name'i aldı

            link.sorgu1= yazarid;

            var content = new StringContent(JsonConvert.SerializeObject(link), Encoding.UTF8, "application/json");


            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    ViewBag.Mesaj = responseContent;
                    return View();
                }
                else
                {
                    // API hatalarını işleme
                     //StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                    ViewBag.Mesaj = $"{response.StatusCode}";
                    return View();
                }
            }
            catch (HttpRequestException ex)
            {
                // İstek hatalarını işleme
                return StatusCode(StatusCodes.Status500InternalServerError, $"Request error: {ex.Message}");
            }

        }
        //[HttpGet]
        //public IActionResult Baslik()
        //{

        //    return View();
        //}
        [HttpGet]
        public async Task<IActionResult> Baslik()
        {
            string token = HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string baseUrl = "https://localhost:7052/api/";
            string metot = "Kullanici/baslik-goster";




            string url = string.Concat(baseUrl, metot);
            if (token == null)
            {
                ViewBag.Mesaj = "Token yok olm nereye gidiyorsun";
                return View("Ekle");
            }
            string jsonPayload = Base64UrlHelper.Base64UrlDecode(token.Split('.')[1]);
            var payload = JsonConvert.DeserializeObject<JwtPayload>(jsonPayload);
            string yazarid = (payload.Name); //name'i aldı

            Sorgu sor = new Sorgu()
            {
                Name = yazarid
            };

            var content = new StringContent(JsonConvert.SerializeObject(sor), Encoding.UTF8, "application/json");


            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();
                    

                    var linkInt2List = JsonConvert.DeserializeObject<List<Link_int2>>(responseContent);


                    return View(linkInt2List);
                }
                else
                {
                    // API hatalarını işleme
                    ViewBag.Mesaj = $"{response.StatusCode}";
                    return View("Ekle");
                    //return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }
            }
            catch (HttpRequestException ex)
            {
                // İstek hatalarını işleme
                return StatusCode(StatusCodes.Status500InternalServerError, $"Request error: {ex.Message}");
            }

        }
        [HttpGet]
        public async Task<IActionResult> Icerik(string sorgu) //Burada MVC döndürecek. MVC YOK !!!
        {
            string token = HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string baseUrl = "https://localhost:7052/api/";
            string metot = "Kullanici/oku";


            string url = string.Concat(baseUrl, metot);
            if (token == null)
            {
                return BadRequest("Token yok olm nereye gidiyorsun");
            }
            Sorgu gonder = new Sorgu()
            {
                Name = sorgu
            };

            var content = new StringContent(JsonConvert.SerializeObject(gonder), Encoding.UTF8, "application/json");



            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();

                    var linkInt2List = JsonConvert.DeserializeObject<MakaleVM_v2>(responseContent);

                    var yazar = context.yazars.Find(linkInt2List.YazarId);
                    var kateg = context.kategoris.Find(linkInt2List.KategoriId);

                    MakaleViewModel doncek = new MakaleViewModel
                    {
                        Baslik = linkInt2List.Baslik,
                        Icerik = linkInt2List.Icerik,
                        Cat = kateg.Name,
                        Yaz = yazar.Name
                    };

                    return View(doncek);
                }
                else
                {
                    // API hatalarını işleme

                    ViewBag.Mesaj = $"{response.StatusCode}";
                    return View("Ekle");
                    //return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }
            }
            catch (HttpRequestException ex)
            {
                // İstek hatalarını işleme
                return StatusCode(StatusCodes.Status500InternalServerError, $"Request error: {ex.Message}");
            }

        }
    }
}
