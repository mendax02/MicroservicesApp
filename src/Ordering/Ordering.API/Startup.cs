using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using Ordering.Application.Handlers;
using Ordering.Core.Repositories;
using Ordering.Core.Repositories.Base;
using Ordering.Infrastructure.Data;
using Ordering.Infrastructure.Repositories;
using Ordering.Infrastructure.Repositories.Base;

namespace Ordering.API
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
            services.AddDbContext<OrderContext>(x =>
            x.UseSqlServer(Configuration.GetConnectionString("OrderConnection")), ServiceLifetime.Singleton);

            // Add AutoMapper
            services.AddAutoMapper(typeof(Startup));

            // Add MediatR
            services.AddMediatR(typeof(CheckOutorderHandler).GetTypeInfo().Assembly);

            // Add Infrastructure Layer
            // we made transient this in order to resolve in mediatR when consuming Rabbit
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));

            // Inject swagger
            services.AddSwaggerGen(x =>
            {
                x.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Order Api",
                    Version = "v1"
                });
            });


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });


            // Use swagger middleware
            app.UseSwagger();
            app.UseSwaggerUI(x => x.SwaggerEndpoint("/swagger/v1/swagger.json", "Order API V1"));
        }
    }
}
