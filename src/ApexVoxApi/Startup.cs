using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ApexVoxApi.Infrastructure;
using ApexVoxApi.Providers;
using Microsoft.EntityFrameworkCore;
using ApexVoxApi.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ApexVoxApi.TenantProviders;
using ApexVoxApi.ElasticDatabaseClient;
using NSwag.Generation.Processors.Security;
using NSwag;
using System.Linq;

namespace ApexVoxApi
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
            // configure DI for application services
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IProductService, ProductService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITenantProvider, UserClaimsTenantProvider>();
            services.AddScoped<IConnectionProvider, ConnectionProvider>();
            services.AddScoped<IUnitOfWork, ApexVoxContext>();
            services.AddScoped<ITenantContext, TenantContext>();
            services.AddScoped<ITenantsService, TenantsService>();
            services.AddScoped<IShardingManager, ShardingManager>();
            services.AddScoped<ICurrentUser, HttpContextCurrentUser>();

            services.Configure<ShardingManagerConnectionStringOptions>(Configuration.GetSection("ConnectionStrings"));

            //configure db context
            services.AddDbContext<ApexVoxContext>((serviceProvider, options) =>
            {
                var connectionProvider = serviceProvider.GetService<IConnectionProvider>();
                var connectionString = connectionProvider.OpenDDRConnection();
                options.UseSqlServer(connectionString);
            });

            services.AddDbContext<TenantContext>((options) =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("ShardMap"));
            });

            services.AddCors();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.IncludeErrorDetails = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = false,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
            });

            services.AddAuthorization();

            services.AddOpenApiDocument(document =>
            {
                document.AddSecurity("JWT", Enumerable.Empty<string>(), new OpenApiSecurityScheme
                {
                    Type = OpenApiSecuritySchemeType.ApiKey,
                    Name = "authorization",
                    In = OpenApiSecurityApiKeyLocation.Header,
                    Description = "Type into the textbox: Bearer {your JWT token}."
                });

                document.OperationProcessors.Add(
                    new AspNetCoreOperationSecurityScopeProcessor("JWT"));
                
            });
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStaticFiles();


            app.UseOpenApi();
            app.UseSwaggerUi3();


            app.UseCors(x => x
                 .AllowAnyOrigin()
                 .AllowAnyMethod()
                 .AllowAnyHeader());

            app.UseAuthentication();

            app.UseMvc();
        }
    }
}
