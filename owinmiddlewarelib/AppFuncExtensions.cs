using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace OwinMiddleware
{
    using BuildFunc = Action<Func<Func<IDictionary<string, object>, Task>, Func<IDictionary<string, object>, Task>>>;

    public static class AppFuncExtensions
    {
        public static BuildFunc UseOwinPlatform(this BuildFunc app)
        {
            app(next => new CorrelationIdMiddleware(next).Invoke);
            return app;
        }
    }
}