using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProductComponent.Data;
using System.Net.Http;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace ProductComponent
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
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.RoleClaimType = JwtClaimTypes.Role;
            });

            services.AddControllers();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddCors(c =>
            {
                c.AddPolicy("AllowOrigin", options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
                c.AddPolicy("AllowExposedXTotalCount", options => options.AllowAnyHeader().WithExposedHeaders("Access-Control-Expose-Headers"));
            });
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            string domain = $"https://{Configuration["Auth0:Domain"]}/";
            services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = domain;
                    options.Audience = Configuration["Auth0:Audience"];
            // If the access token does not have a `sub` claim, `User.Identity.Name` will be `null`. Map it to a different claim by setting the NameClaimType below.
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        NameClaimType = ClaimTypes.NameIdentifier
                    };
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("create:product", policy => policy.Requirements.Add(new HasScopeRequirement("create:product", domain)));
                options.AddPolicy("edit:product", policy => policy.Requirements.Add(new HasScopeRequirement("edit:product", domain)));
                options.AddPolicy("delete:product", policy => policy.Requirements.Add(new HasScopeRequirement("delete:product", domain)));
            });

            services.AddSingleton<IAuthorizationHandler, HasScopeHandler>();
            services.AddHttpContextAccessor();

            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Bearer Authenthication for API",
                });
                c.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = "bearer"
                                },
                            },
                            Array.Empty<string>()
                        }
                    }
                );
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "ProductComponent", Version = "v1"});
            });

            services.AddDbContext<ProductComponentContext>(options =>
                    options.UseSqlServer(Environment.GetEnvironmentVariable("ConnectionString") ?? Configuration.GetConnectionString("ProductComponentContext")));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment() || env.IsEnvironment("Local"))
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();

                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductComponent v1"));

                using (var serviceScope = app.ApplicationServices.CreateScope())
                {
                    ProductComponentContext context = serviceScope.ServiceProvider.GetService<ProductComponentContext>();
                    context.Database.Migrate();
                }
            }else
            {
                // Disabled for development because auto ssl redirect from gateway!
                app.UseHttpsRedirection();
            }

      

            app.UseRouting();
            app.UseCors(builder => builder.AllowAnyMethod().AllowAnyOrigin().AllowAnyHeader());
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
