using AutoMapper;
using eAuction.Business.Helpers;
using eAuction.Business.SellerBusiness;
using eAuction.DataAccess.DBContext;
using eAuction.DataAccess.Repositories.BaseRepositories;
using eAuction.DataAccess.Repositories.BuyerRepositories;
using eAuction.DataAccess.Repositories.ProductRepositories;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using SellerService.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace SellerService
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

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SellerService", Version = "v1" });
            });
            
            services.Configure<Mongosettings>(
             Configuration.GetSection("eAuctionDatabase"));
            services.AddScoped<IMongoDBContext, MongoDBContext>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IBuyerRepository, BuyerRepository>();
            services.AddScoped<ISellerBusinessManager, SellerBusinessManager>();
            services.AddAutoMapper(typeof(Startup));
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new MappingProfile());
            });
            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);

            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", 
                    options => options.WithOrigins("http://localhost:4200","http://54.159.203.144"
                    , "http://eauction.abhagam.com"));
            });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "SellerService v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors(options => 
            options.WithOrigins("http://localhost:4200", "http://54.159.203.144"
            ,"http://eauction.abhagam.com"));

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}
