﻿using DataLayers.SQLServer;
using Microsoft.AspNetCore.Authentication.Cookies;
using SV21T1020599.BusinessLayers;
using SV21T1020599.DataLayers;
using SV21T1020599.DomainModels;

namespace SV21T1020599.Shop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Lấy connection string và kiểm tra null
            string connectionString = builder.Configuration.GetConnectionString("SV21T1020599")
                ?? throw new InvalidOperationException("Connection string 'SV21T1020599' not found.");

            // Khởi tạo Business Layer configuration
            Configuration.Initialize(connectionString);

            // Đăng ký các DAL
            builder.Services.AddScoped<ICommonDAL<Customer>>(provider => new CustomerDAL(connectionString));

            // Add services to the container
            builder.Services.AddControllersWithViews();
            builder.Services.AddHttpContextAccessor();
            builder.Services.AddSession(option =>
            {
                option.IdleTimeout = TimeSpan.FromMinutes(60);
                option.Cookie.HttpOnly = true;
                option.Cookie.IsEssential = true;
            });
            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                            .AddCookie(option =>
                            {
                                option.Cookie.Name = "AuthenticationCookie";
                                option.LoginPath = "/Account/Login";
                                option.ExpireTimeSpan = TimeSpan.FromDays(360);
                            });

            var app = builder.Build();

            // Cấu hình ApplicationContext
            ApplicationContext.Configure(
                app.Services.GetRequiredService<IHttpContextAccessor>(),
                app.Environment
            );

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();

            // Quan trọng: Session phải được khai báo TRƯỚC Authentication
            app.UseSession();

            app.UseAuthentication(); // Phải đặt trước UseAuthorization
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
