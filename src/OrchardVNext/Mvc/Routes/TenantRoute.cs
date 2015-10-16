﻿using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Routing;
using System;
using System.Threading.Tasks;

namespace OrchardVNext.Mvc.Routes {
    public class TenantRoute : IRouter {
        private readonly IRouter _target;
        private readonly string _urlHost;
        private readonly RequestDelegate _pipeline;

        public TenantRoute(IRouter target, string urlHost, RequestDelegate pipeline) {
            _target = target;
            _urlHost = urlHost;
            _pipeline = pipeline;
        }

        public async Task RouteAsync(RouteContext context) {
            if (context.HttpContext.Request.Host.Value == _urlHost) {
                context.HttpContext.Items["orchard.Handler"] = new Func<Task>(async () => {
                    await _target.RouteAsync(context);
                });

                await _pipeline.Invoke(context.HttpContext);
            }
        }

        public string GetVirtualPath(VirtualPathContext context) {
            return null;
        }
    }
}