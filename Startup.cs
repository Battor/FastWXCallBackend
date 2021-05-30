using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using System.IO;
using System.Reflection;
using log4net.Config;
using log4net;

using FastWXCallBackend.FilterAttributes;

namespace FastWXCallBackend
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            XmlConfigurator.Configure(
                LogManager.CreateRepository("logger"),
                new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.config")));
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(options => {
                    options.Filters.Add<GlobalExceptionFilter>();
                })
                .AddJsonOptions(options => {
                    options.JsonSerializerOptions.PropertyNamingPolicy = null;
                });

            services.AddDbContextPool<FastwxcallContext>(optionsAction => 
            { 
                optionsAction.UseMySql(
                    Configuration.GetConnectionString("DefaultConnection"), 
                    Microsoft.EntityFrameworkCore.ServerVersion.Parse("5.7.29-mysql")); 
            });

            services.AddSwaggerGen(option =>
            {
                option.SwaggerDoc("v1", new OpenApiInfo { Title = "FastWXCallApi", Version = "v1" });

                // 使用反射获取xml文件。并构造出文件的路径
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                // 启用xml注释. 该方法第二个参数启用控制器的注释，默认为false.
                option.IncludeXmlComments(xmlPath, true);
            });

            services.AddScoped<JwtHelper>();

            IConfigurationSection jwtSection = Configuration.GetSection("JWT");

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ClockSkew = TimeSpan.FromSeconds(30),

                        ValidIssuer = jwtSection.GetValue<string>("Issuer"),
                        ValidAudience = jwtSection.GetValue<string>("Audience"),
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSection.GetValue<string>("SecurityKey")))
                    };
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

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "FastWXCallApi");
            });

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
