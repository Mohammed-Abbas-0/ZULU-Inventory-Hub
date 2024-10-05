using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryHub.Middleware
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
            if (context.User.Identity.IsAuthenticated &&
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
