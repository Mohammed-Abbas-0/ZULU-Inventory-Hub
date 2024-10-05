using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHubUI.Middleware
{
    public class RedirectIfAuthenticatedMiddleware
    {
        private readonly RequestDelegate _next;

        public RedirectIfAuthenticatedMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string token = context.Session.GetString("RefreshToken");
            // تحقق مما إذا كان المستخدم قد سجل الدخول
            bool userAuthenticated = (context.User.Identity.IsAuthenticated && !string.IsNullOrEmpty(token));
                if (userAuthenticated &&
                        (context.Request.Path.StartsWithSegments("/Account/Login") || context.Request.Path.StartsWithSegments("/Account/Register")))
                {
                    // إعادة توجيه المستخدم إلى الصفحة الرئيسية أو لوحة التحكم
                    context.Response.Redirect("/Home/Index");
                    return;
                }

            await _next(context);
        }
    }

}
