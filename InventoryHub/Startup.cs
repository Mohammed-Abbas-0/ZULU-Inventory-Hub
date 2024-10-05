using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryHub.Context;
using Microsoft.AspNetCore.Identity;
using InventoryHub.ServicesPatterns.IService;
using InventoryHub.Models;
using InventoryHub.ServicesPatterns.Implementation;
using InventoryHub.Helper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using InventoryHub.Helper.Authentication;
using InventoryHub.Helper.FilebaseStorage;
using AutoMapper;
using InventoryHub.Helper.Mapper;
using InventoryHub.Helper.HelperMethods;
using InventoryHub.Helper.ExternalLogin;
using InventoryHub.Helper.RedisCaching;

namespace InventoryHub
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
            // Firebase
            var storageBucket = Configuration["FirebaseSettings:StorageBucket"];

            // Sql
            services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            // Redis Caching
            services.AddStackExchangeRedisCache(idx =>
            {
                idx.Configuration = Configuration.GetConnectionString("RedisConnection");
                idx.InstanceName = "RedisDistributedCache";
            });


            // Implement Services
            services.AddScoped<IRepository<Product>, Repository<Product>>(); // تسجيل الـ Repository
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IStorageService, StorageService>();
            services.AddScoped<IProductQueryService, ProductQueryService>();
            services.AddScoped<ICategoryQueryService, CategoryQueryService>();
            services.AddScoped<ICustomerQueryService, CutomerQueryService>();
            services.AddScoped<IHelperMethod, HelperMethod>();
            services.AddScoped<IExternalLogin, ExternalLogin>();
            services.AddScoped(typeof(IRedisDistributedCache<>), typeof(RedisDistributedCache<>));

            // Factory Pattern
            services.AddScoped<PreviousSearchStrategy>();
            services.AddScoped<NextSearchStrategy>();
            services.AddScoped<ByCodeSearchStrategy>();
            // تسجيل مصنع الاستراتيجيات إذا لزم الأمر
            services.AddScoped<ISearchStrategyFactory, SearchStrategyFactory>();

            // Mapper
            services.AddAutoMapper(typeof(MapperService));


            services
                .AddIdentity<ApplicationUser, IdentityRole>()
                //يقوم بتحديد استخدام Entity Framework Core
                //كوسيط لتخزين بيانات المستخدمين والأدوار.
                //كما يحدد النوع (<Context>) الذي يمثل سياق قاعدة البيانات.
                .AddEntityFrameworkStores<AppDbContext>()
                //هذا السطر يقوم بإضافة مزودات الرموز الافتراضية لخدمة Identity.
                //تتولى هذه المزودات إنشاء وإدارة الرموز المستخدمة في
                //عمليات المصادقة مثل تأكيد البريد الإلكتروني وإعادة تعيين كلمة المرور.

                .AddDefaultTokenProviders();

            //عند بدء تشغيل التطبيق، يقوم ASP.NET Core
            //بقراءة إعدادات JWT من ملف appsettings.json.
            //يتم تحويل هذه الإعدادات إلى كائن من الفئة JWT،
            //بحيث يمكن استخدامها لاحقًا عند تكوين المصادقة(Authentication) وتوقيع الـ JWT.
            services.Configure<JWT>(Configuration.GetSection("JWT"));
          

            services
                .AddAuthentication(options =>
                {
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddCookie(options =>
                {
                    options.LoginPath = "/Account/Login"; // أو المسار الذي تستخدمه لتسجيل الدخول
                })
                .AddJwtBearer(options =>
                {
                    options.SaveToken = false; // Aviod Save Token in Memory
                    options.RequireHttpsMetadata = false; // Can use Http
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidateAudience = true,
                        ValidateIssuer = true,
                        ValidIssuer = Configuration["JWT:ValidIssuer"],
                        ValidAudience = Configuration["JWT:ValidAudience"],
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JWT:Key"])),
                        ClockSkew = TimeSpan.Zero
                    };
                })
                .AddGoogle(options =>
                {
                    options.ClientId = Configuration.GetSection("Google:ClientId").Value;
                    options.ClientSecret = Configuration.GetSection("Google:ClientSecret").Value;
                    options.CallbackPath = "/signin-google";
                });

            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "InventoryHub", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "InventoryHub v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();


            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
