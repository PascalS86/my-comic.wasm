using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;
using Radzen;
using Csandra.Comics.App.wasm.Data;

namespace Csandra.Comics.App.wasm
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<ComicDataService>();
            services.AddSingleton<SessionService>();
            services.AddScoped<DialogService>();
        }

        
        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
