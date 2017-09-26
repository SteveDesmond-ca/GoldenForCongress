using System;
using System.Net.Http;
using GoldenForCongress.Data;
using GoldenForCongress.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Rest.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace GoldenForCongress
{
    public class Startup
    {
        private static readonly ReadOnlyJsonContractResolver SnakeCaseResolver = new ReadOnlyJsonContractResolver { NamingStrategy = new SnakeCaseNamingStrategy() };
        public static readonly JsonSerializer SnakeCase = JsonSerializer.CreateDefault(new JsonSerializerSettings { ContractResolver = SnakeCaseResolver });

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var dbFactory = new DBFactory(Configuration);
            services.AddScoped<IDesignTimeDbContextFactory<DB>>(s => dbFactory);
            services.AddDbContext<DB>(o => o.UseSqlServer(Configuration.GetConnectionString("DB")));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<DB>();

            services.AddMvc()
                .AddJsonOptions(o => o.SerializerSettings.ContractResolver = SnakeCaseResolver);

            services.AddSingleton<Func<DB>>(s => () => dbFactory.CreateDbContext(new string[] { }));
            services.AddSingleton<HttpClient>();
            services.AddSingleton<SPOTAPI>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseDeveloperExceptionPage();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes => routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}"));
        }
    }
}
