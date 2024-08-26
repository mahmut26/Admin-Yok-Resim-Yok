using DataLayer.Model_Parcala;
using DataLayer.Model_VM;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace Odev_v9.Controllers
{
    public class AdminController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public AdminController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }
        [HttpGet]
        public async Task<IActionResult> Durumlar()
        {
            string token = HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string baseUrl = "https://localhost:7052/api/";
            string metot = "Admin/Durum";

            string url = string.Concat(baseUrl, metot);
            if (token == null)
            {
                ViewBag.Mesaj = "Token yok olm nereye gidiyorsun";
                return View("Ekle");
                //return BadRequest("Token yok olm nereye gidiyorsun");
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync();


                    var linkInt2List = JsonConvert.DeserializeObject<List<KullanicilarinDurumlari>>(responseContent);


                    return View(linkInt2List);
                    //return View()
                }
                else
                {
                    // API hatalarını işleme
                    return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
                }
            }
            catch (HttpRequestException ex)
            {
                // İstek hatalarını işleme
                return StatusCode(StatusCodes.Status500InternalServerError, $"Request error: {ex.Message}");
            }

        }




        //[HttpPost]
        //public async Task<IActionResult> Ekle(Sorgu sorgu)
        //{
        //    string token = HttpContext.Session.GetString("Token");
        //    var client = _httpClientFactory.CreateClient();
        //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        //    string baseUrl = "https://localhost:7052/api/";
        //    string metot = "Kullanici/kategori-ekle";




        //    string url = string.Concat(baseUrl, metot);
        //    if (token == null)
        //    {
        //        return BadRequest("Token yok olm nereye gidiyorsun");
        //    }
        //    string jsonPayload = Base64UrlHelper.Base64UrlDecode(token.Split('.')[1]);
        //    var payload = JsonConvert.DeserializeObject<JwtPayload>(jsonPayload);
        //    string yazarid = (payload.Name); //name'i aldı

        //    sorgu.Name = yazarid;

        //    var content = new StringContent(JsonConvert.SerializeObject(sorgu), Encoding.UTF8, "application/json");


        //    try
        //    {
        //        HttpResponseMessage response = await client.PostAsync(url, content);

        //        if (response.IsSuccessStatusCode)
        //        {
        //            string responseContent = await response.Content.ReadAsStringAsync();
        //            return Ok(responseContent);
        //        }
        //        else
        //        {
        //            // API hatalarını işleme
        //            return StatusCode((int)response.StatusCode, await response.Content.ReadAsStringAsync());
        //        }
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        // İstek hatalarını işleme
        //        return StatusCode(StatusCodes.Status500InternalServerError, $"Request error: {ex.Message}");
        //    }

        //}



    }
    }

