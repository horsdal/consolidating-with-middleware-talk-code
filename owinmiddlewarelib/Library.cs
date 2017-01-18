using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LibOwin;

namespace OwinMiddleware
{
    using AppFunc = Func<IDictionary<string, object>, Task>;

    public class CorrelationIdMiddleware
    {
        private readonly AppFunc next;

        public CorrelationIdMiddleware(AppFunc next)
        {
            this.next = next;
        }

        public Task Invoke(IDictionary<string, object> ctx)
        {
            var context = new OwinContext(ctx);

            var correlationId = context.Request.Headers["my-correlationid"] ?? Guid.NewGuid().ToString();
            // add to log context
            Console.WriteLine($"Correlation id: {correlationId}");
            return this.next(ctx);
        }
    }
}