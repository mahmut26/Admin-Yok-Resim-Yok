using DataLayer.Model_DBContext;
using DataLayer.Model_Login;
using DataLayer.Model_Servis;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Blog_Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddControllersWithViews();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddDbContext<Blog_DB>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("connStr"));
            });

            builder.Services.AddDbContext<Identity_DB>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDbContext"));
            });

            builder.Services.AddAuthentication(X => {
                X.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                X.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                {
                     options.TokenValidationParameters = new TokenValidationParameters
                        {
                              ValidateIssuer = true,
                              ValidateAudience = true,
                              ValidateLifetime = true,
                              ValidateIssuerSigningKey = true,
                              ValidIssuer = builder.Configuration["Jwt:Issuer"],
                              ValidAudience = builder.Configuration["Jwt:Issuer"],
                              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                        };
                });

            builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<Identity_DB>().AddDefaultTokenProviders();

            builder.Services.AddScoped<TokenService>();

            builder.Services.AddScoped<EmailService>();
 
            builder.Services.AddSingleton<EmailService>();




            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.MaxDepth = 1024; 
            });
            builder.Services.AddMvc(options =>
            {
                options.SuppressAsyncSuffixInActionNames = false;
            });
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                    policy.RequireClaim("IsAdmin", "true")); 

                options.AddPolicy("Yazar", policy =>
                    policy.RequireClaim("IsYazar","true")); 

                options.AddPolicy("Kullanici", policy =>
                    policy.RequireClaim("IsUser", "true")); 
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.UseAuthentication();
            


            app.MapControllers();

            app.Run();
        }
    }
}
