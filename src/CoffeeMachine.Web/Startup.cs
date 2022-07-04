using System;
using System.IO;
using System.Reflection;

using CoffeeMachine.Application.Services;
using CoffeeMachine.Application.Services.Interfaces;
using CoffeeMachine.Infrastructure;
using CoffeeMachine.Infrastructure.Repositories;
using CoffeeMachine.Web.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;

namespace CoffeeMachine.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMiddleware<GlobalExceptionHandler>();
            if (env.IsDevelopment())
            {
                app.UseMiddleware<GlobalExceptionHandler>();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "CoffeeMachine v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "CoffeeMachine", Version = "v1" });

                var xmlFileNameWeb = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlFileNameDomain = "CoffeeMachine.Domain.xml";
                opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileNameWeb));
                opt.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFileNameDomain));
            });
            services.AddDbContext<DataContext>(opt =>
            {
                opt.UseNpgsql(Configuration.GetConnectionString("DockerPgsql"));
            });
            services.AddTransient<GlobalExceptionHandler>()
                .AddScoped<CoffeeRepository>()
                .AddScoped<BalanceRepository>()
                .AddScoped<BanknoteCashboxRepository>()
                .AddScoped<PaymentRepository>()
                .AddScoped<IncomeRepository>()
                .AddScoped<UnitOfWork>()
                .AddScoped<ICoffeeService, CoffeeService>()
                .AddScoped<IBalanceService, BalanceService>()
                .AddScoped<IBanknoteCashboxService, BanknoteCashboxService>()
                .AddScoped<IIncomeService, IncomeService>()
                .AddScoped<IPaymentService, PaymentService>();
        }
    }
}