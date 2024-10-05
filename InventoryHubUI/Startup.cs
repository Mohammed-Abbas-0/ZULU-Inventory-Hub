using InventoryHubUI.Middleware;
using InventoryHubUI.Services.APITokens;
using InventoryHubUI.Services.HelperMethods;
using InventoryHubUI.Services.TokenHeader;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHubUI
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
            services.AddHttpClient<ApiService>(); // تسجيل ApiService كخدمة
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>(); // تسجيل IHttpContextAccessor

            services.AddDistributedMemoryCache();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";
                    //options.Cookie.Name = "ZuluInventoryHubAuthCookie"; // تأكد من أن اسم الكوكيز مناسب
                    //options.Cookie.HttpOnly = true;
                    //options.Cookie.SameSite = SameSiteMode.Lax; // قد تحتاج لتعديل هذا الإعداد حسب احتياجاتك
                });
            services.AddScoped<ICategoryService,CategoryService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IHeaderBearerToken, HeaderBearerToken>();
            services.AddControllersWithViews()
                .AddRazorRuntimeCompilation();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            // إضافة Middleware التحقق من حالة المصادقة
            app.UseMiddleware<RedirectIfAuthenticatedMiddleware>();
            

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
