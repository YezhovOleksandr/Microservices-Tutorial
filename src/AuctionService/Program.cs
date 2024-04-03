
using AuctionService.Data;
using Microsoft.EntityFrameworkCore;
using MassTransit;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AuctionService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<AuctionDbContext>(options =>
            {
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
            });
            builder.Services.AddControllers().AddJsonOptions(options => options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
            builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            builder.Services.AddMassTransit(x =>
             {
                 x.AddEntityFrameworkOutbox<AuctionDbContext>(o =>
                 {
                     o.QueryDelay = TimeSpan.FromSeconds(10);

                     o.UsePostgres();
                     o.UseBusOutbox();
                 });
                 x.AddConsumersFromNamespaceContaining<AuctionCreatedFaultConsumer>();
                 x.SetEndpointNameFormatter(new KebabCaseEndpointNameFormatter("auction", false));
                 x.UsingRabbitMq((context, cfg) =>
                 {  
                    cfg.Host(builder.Configuration["RabbitMq:Host"], "/", host => {
                        host.Username(builder.Configuration.GetValue("RabbitMq:Username", "guest"));
                        host.Password(builder.Configuration.GetValue("RabbitMq:Password", "guest"));
                    });
                     cfg.ConfigureEndpoints(context);
                 });
             });

            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
            {
                options.Authority = builder.Configuration["IdentityServiceUrl"];
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters.ValidateAudience = false;
                options.TokenValidationParameters.NameClaimType = "username";
            });

            var app = builder.Build();


            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            try
            {
                DbInitializer.InitDb(app);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            app.Run();
        }
    }
}
