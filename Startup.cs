using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ArchimydeschallengeAPI.Data;
using ArchimydesWeb.Data;
using ArchimydesWeb.Models;
using ArchimydesWeb.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArchimydeschallengeAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen();
            
            var connection = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ArchimydesWebContext>(options => options.UseSqlServer("connection"));


            services.AddIdentity<ApplicationUser, ApplicationRole>()
           .AddEntityFrameworkStores<ArchimydesWebContext>()
            .AddDefaultTokenProviders();

            services.AddScoped<UnitOfWork>();

            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder
                        .AllowAnyMethod()
                        .AllowCredentials()
                        .SetIsOriginAllowed((host) => true)
                        .AllowAnyHeader());
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            //app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            //app.UseSwagger(c =>
            //{
            //    c.SerializeAsV2 = true;
            //});
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ArchimydeschallengeAPI");
                c.RoutePrefix = string.Empty;
            });

            


        }
    }
}
