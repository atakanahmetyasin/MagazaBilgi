using System;
using IadeFormu.Data; // DbContext'in bulunduğu namespace
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace IadeFormu
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
    
        public IConfiguration Configuration { get; }
    
        public void ConfigureServices(IServiceCollection services)
        {
            // MVC (Controllers ve Views) desteğini ekle
            services.AddControllersWithViews();

            // SQL Server kullanarak DbContext kaydını ekle
            services.AddDbContext<MagazaBilgiDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("MagazaBilgiDb")));

            // Dağıtılmış bellek önbelleğini (memory cache) ekle (Session için gereklidir)
            services.AddDistributedMemoryCache();

            // Session ayarlarını yapılandır (örneğin, 30 dakika boşta kalınca oturum sonlandırma)
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
            });

            // HttpContextAccessor'ü ekle (bazı durumlarda HttpContext'e erişmek için gereklidir)
            services.AddHttpContextAccessor();
        }
    
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
    
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Session middleware'ini ekle (endpoint tanımlamadan önce eklenmeli)
            app.UseSession();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Account}/{action=Login}/{id?}");
            });
        }
    }
}
