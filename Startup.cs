using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace WDXWebApiDespachoJuridico
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            string server = "localhost";
            string database = "WDXDespachoMora";
            string user = "usr_despacho";
            string password = "Jerry200346602";
            string connection = $"Server={server};Database={database};user={user};password={password}";
            services.AddDbContext<IdentityDbContext>(options => options.UseSqlServer(connection, optionsbuilder =>
             optionsbuilder.MigrationsAssembly("WDXWebApiDespachoJuridico.dll")));
            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<IdentityDbContext>().AddDefaultTokenProviders();
            services.Configure<IdentityOptions>(o => { o.SignIn.RequireConfirmedEmail = true; });
            services.AddCors(options => options.AddPolicy("CorsPolicy",
                builder =>
                builder.WithOrigins("https://localhost:5001/")
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader()));
            services.AddTransient<IMessageService, FileMessageService>();
            services.AddControllers();
            services.AddSingleton<IAuthorizationHandler, PermisoRequirement>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Permiso", policyBuilder =>
                {
                    policyBuilder.Requirements.Add(new PermissionAuthorizationRequirement());
                });
            });            
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(options=> {
                options.Cookie.Name = "despacho-cookie-juridico";
                options.Cookie.SecurePolicy = CookieSecurePolicy.SameAsRequest;
                options.Cookie.SameSite = SameSiteMode.None;
                options.ExpireTimeSpan = TimeSpan.FromDays(14);
                options.SlidingExpiration = true;
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            // este es el orden de la autorizaciòn autenticación
            app.UseAuthentication();
            app.UseAuthorization();
            
            
            app.UseCors("CorsPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            
        }
    }
}
