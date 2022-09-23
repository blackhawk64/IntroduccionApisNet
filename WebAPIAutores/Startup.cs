using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using WebAPIAutores.Filtros;
using WebAPIAutores.Middlewares;

namespace WebAPIAutores
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers(opciones =>
            {
                opciones.Filters.Add(typeof(FiltroDeExcepcion));
            })
                .AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
                .AddNewtonsoftJson();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("defaultConnection"))
            );

            services.AddResponseCaching();
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer();

            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddAutoMapper(typeof(Startup));
        }
        
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            // Configure the HTTP request pipeline.
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            //app.UseMiddleware<LoguearRespuestaHTTPMiddleware>();
            app.UseLoguearRespuestaHTTP();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
