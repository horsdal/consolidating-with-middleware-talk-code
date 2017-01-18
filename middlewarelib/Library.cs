using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace middlewarelib
{
    public class MonitoringEndpointsMiddleware
    {
        private readonly RequestDelegate next;

        public MonitoringEndpointsMiddleware(RequestDelegate next)
        {
            this.next = next;
        }

        public Task Invoke(HttpContext context)
        {
            if (context.Request.Path.Equals("/_monitor"))
                return context.Response.WriteAsync("Version 1.0");
            return this.next(context);
        }
    }
}