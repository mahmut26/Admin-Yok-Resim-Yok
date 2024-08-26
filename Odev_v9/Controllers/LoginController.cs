using DataLayer.Model_Login;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using DataLayer.Model_VM;
using DataLayer.Model_Hatalar;
using Azure;

namespace Odev_v9.Controllers
{
    public class LoginController : Controller
    {
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            string baseUrl = "https://localhost:7052/api/";
            string loginMethod = "Login/olustur";

            if (ModelState.IsValid)
            {
                var serializeUser = JsonConvert.SerializeObject(register);

                StringContent stringContent = new StringContent(serializeUser, Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();

                string url = string.Concat(baseUrl, loginMethod);

                var result = client.PostAsync(url, stringContent).Result;

                string json = await result.Content.ReadAsStringAsync();

                //var mesaj = result.Content.ReadFromJsonAsync<string>();
                if (result.Content.Headers.ContentType.MediaType == "application/json")
                {
                    //string json = await result.Content.ReadAsStringAsync();
                    List<JsonHata> requirements = JsonConvert.DeserializeObject<List<JsonHata>>(json);
                    TempData["Requirements"] = JsonConvert.SerializeObject(requirements);

                    return RedirectToAction("DisplayRequirementsL");
                }
                else
                {
                    TempData["Requirements"] = json;

                    return RedirectToAction("DisplayRequirements");
                }
            }

            return View();
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel login)
        {
            string baseUrl = "https://localhost:7052/api/";
            string loginMethod = "Login/giris";

            if (ModelState.IsValid)
            {
                var serializeUser = JsonConvert.SerializeObject(login);

                StringContent stringContent = new StringContent(serializeUser, Encoding.UTF8, "application/json");

                HttpClient client = new HttpClient();

                string url = string.Concat(baseUrl, loginMethod);

                var result = client.PostAsync(url, stringContent).Result;

                string json = await result.Content.ReadAsStringAsync();

                try
                {
                    Token token = JsonConvert.DeserializeObject<Token>(json);
                    
                    if (token != null)
                    {
                        HttpContext.Session.SetString("Token", token.token);
                        return RedirectToAction("Success"); // Başarı durumunda yönlendirme
                    }
                }
                catch (JsonException ex)
                {
                    // Token deserialization hatası
                    if (result.Content.Headers.ContentType.MediaType == "application/json")
                    {
                        try
                        {
                            List<JsonHata> requirements = JsonConvert.DeserializeObject<List<JsonHata>>(json);
                            TempData["Requirements"] = JsonConvert.SerializeObject(requirements);
                            return RedirectToAction("DisplayRequirementsL");
                        }
                        catch (JsonException exe)
                        {
                            // JSON deserialization hatası işleme
                            TempData["Error"] = $"JSON deserialization error: {exe.Message}";
                            return RedirectToAction("Error"); // Hata sayfasına yönlendirme
                        }
                    }
                    else
                    {
                        TempData["Requirements"] = json;

                        return RedirectToAction("DisplayRequirements");
                    }
                    //TempData["Error"] = $"Token deserialization error: {ex.Message}";
                    //return RedirectToAction("Error");
                }


                //var mesaj = result.Content.ReadFromJsonAsync<string>();
                
            }

            return View();
        }

        public IActionResult DisplayRequirementsL()
        {
            // TempData'dan veriyi alma
            if (TempData["Requirements"] is string requirementsJson)
            {
                List<JsonHata> requirements = JsonConvert.DeserializeObject<List<JsonHata>>(requirementsJson);
                return View(requirements);
            }

            return View(new List<JsonHata>());
        }
        public IActionResult DisplayRequirements()
        {
            // TempData'dan veriyi alma
            if (TempData["Requirements"] is string requirementsJson)
            {
                //List<JsonHata> requirements = JsonConvert.DeserializeObject<List<JsonHata>>(requirementsJson);
                ViewBag.ResponseContent = requirementsJson;
                //return View();
            }

            return View();
        }
        public IActionResult Error()
        {
            // Hata sayfasına yönlendirme
            if (TempData["Error"] is string errorMessage)
            {
                ViewBag.ErrorMessage = errorMessage;
            }

            return View();
        }
        public IActionResult Success()
        {
            // Başarı durumunu gösteren bir View
            return View();
        }
    }
}
