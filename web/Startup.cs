using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace web
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.Use((context, next) =>
            {
                var correlationId = context.Request.Headers["my-correlation-id"].FirstOrDefault()
                                       ?? Guid.NewGuid().ToString();
                // add to log context
                Console.WriteLine($"Correlation id: {correlationId}");
                return next();
            });

            app.Use(async (context, next) =>
            {
                var watch = new Stopwatch();
                watch.Start();
                await next().ConfigureAwait(false);
                watch.Stop();
                Console.WriteLine($"Pipeline time: {watch.ElapsedMilliseconds}ms");
            });

            app.UseMiddleware<MonitoringEndpointsMiddleware>();

            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }

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

