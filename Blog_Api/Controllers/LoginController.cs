using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using DataLayer.Model_Login;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using System.Text;
using DataLayer.Model_Kullanicilar;
using System;
using DataLayer.Model_DBContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity.UI.Services;
using DataLayer.Model_Blog;
using DataLayer.Model_VM;
using DataLayer.Model_Servis;

namespace Blog_Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginController : ControllerBase
    {

        //mvc den alsın, db ye baksın ok ise user rolünü dönsün. Role göre de mvc kontrolcülere gidebilir.
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailSender;
        private readonly Blog_DB _ctxt;

        public LoginController(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager, TokenService tokenService, EmailService emailSender,Blog_DB blog)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _tokenService = tokenService;
            _emailSender = emailSender;
            _ctxt = blog;
        }

        [HttpPost("giris")]
        public async Task<IActionResult> giris([FromBody] LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: true);

            if (result.Succeeded)
            {
                var user = await _userManager.FindByEmailAsync(model.Email);
                if (user == null || !await _userManager.IsEmailConfirmedAsync(user))
                    return BadRequest("Please confirm your email before logging in");

                var cat = await _ctxt.kullanicis.FirstOrDefaultAsync(k => k.Name == model.Email);

                if (cat == null)
                {
                    // Kullanıcı mevcut değilse yeni bir kullanıcı oluştur
                    cat = new Kullanici
                    {
                        Name = model.Email,
                    };

                    _ctxt.kullanicis.Add(cat);
                    await _ctxt.SaveChangesAsync();
                }
                var kadi = await _ctxt.kullanicis.Where(y => y.Name == model.Email).Select(x => x.Id).ToListAsync();

                var token = await _tokenService.GenerateTokenAsync(user, kadi[0]);
                return Ok(new { Token = token });
            }

            if (result.IsLockedOut)
                return StatusCode(StatusCodes.Status423Locked, "Account is locked out");

            return Unauthorized("Invalid login attempt");
        }

        [HttpPost] //modeli json a serialize etmek gerekli ki mvcde çalışsın!!
        [Route("olustur")]
        public async Task<IActionResult> olustur(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (model.Password != model.ConfirmPassword)
                return BadRequest("Passwords do not match");

            var user = new AppUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                string conf_link = $"/Login/confirmemail/{user.Id},{token}";

                await _emailSender.SendEmailAsync(model.Email, "Confirm your email", $"Please confirm your account by <a href='{conf_link}'> link </a>."); //düzenlenecek burası !!!

                return Ok("Registration successful. Please check your email to confirm your account.");
            }

            return BadRequest(result.Errors);
        }
        [HttpGet("confirmemail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
                return BadRequest("User ID and code must be provided");

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return BadRequest("User not found");

            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return Ok("Email confirmed successfully");

            return BadRequest("Email confirmation failed");
        }
       
        

    }
}

