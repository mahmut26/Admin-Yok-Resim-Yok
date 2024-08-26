﻿using DataLayer.Model_DBContext;
using DataLayer.Model_Parcala;
using DataLayer.Model_VM;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        public async Task<IActionResult> Durumlar() //api durumları getiriyoru bu hepsinde aynı kodda değişiklik yapıp çalışıyor. mantık aramaya gerek yok 
        {
            string token = HttpContext.Session.GetString("Token"); //tokeni aldım 
            var client = _httpClientFactory.CreateClient(); 
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token); //tokeni jwt (jwt nasıl bearer olabilir b yok jwt de)

            string baseUrl = "https://localhost:7052/api/";
            string metot = "Admin/Durum";

            string url = string.Concat(baseUrl, metot);
            if (token == null) //token yoksa durdur burda olur ya autorize durmaz falan dermişim neden yaptım ki bunu acaba
            {
                ViewBag.Mesaj = "Token yok olm nereye gidiyorsun";
                return View("Ekle"); //bu ekle de aslında sharede konulabilirmişti
                
            }

            try
            {
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseContent = await response.Content.ReadAsStringAsync(); //geleni aldım


                    var linkInt2List = JsonConvert.DeserializeObject<List<KullanicilarinDurumlari>>(responseContent); //parçaladım 


                    return View(linkInt2List); //gösterdim
                    
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
        [HttpGet] //yapamadım bunu bi
        public IActionResult UserVer()
        {
            return View(); // Değer giriş sayfasını gösterir
        }

        [HttpPost]//yapamadım bunu bi
        public async Task<IActionResult> UserVer(DurumDeg durumDeg)
        {
            string token = HttpContext.Session.GetString("Token");
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            string baseUrl = "https://localhost:7052/api/";
            string metot = "Admin/KullaniciYap";

            string url = string.Concat(baseUrl, metot);
            if (token == null)
            {
                ViewBag.Mesaj = "Token yok olm nereye gidiyorsun";
                return View("Ekle");
                //return BadRequest("Token yok olm nereye gidiyorsun");
            }
            var content = new StringContent(JsonConvert.SerializeObject(durumDeg), Encoding.UTF8, "application/json");
            try
            {
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (response.IsSuccessStatusCode)
                {
                    //string responseContent = await response.Content.ReadAsStringAsync();


                    string responseContent = await response.Content.ReadAsStringAsync();
                    ViewBag.Mesaj = responseContent;
                    return View();


                    
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





    }
}

