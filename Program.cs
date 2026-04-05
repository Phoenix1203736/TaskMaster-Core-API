// ... (Tus usings) ...

using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagerPro.TaskManagerPro.Interfaces;
using TaskManagerPro.TaskMasterPro.Application.Services;
using TaskManagerPro.TaskMasterPro.Infrastructure;
using TaskManagerPro.TaskMasterPro.Infrastructure.Repositories;
using TaskManagerPro.TaskMasterPro.Infrastructure.Services; // Agregado

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. INDISPENSABLE: Registrar los Controladores
        builder.Services.AddControllers();

        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
        var serverVersion = new MariaDbServerVersion(new Version(12, 2, 2));
        // Dile al contenedor de dependencias que sepa crear tu servicio
        builder.Services.AddScoped<GenerateTokenService>();
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, serverVersion));

        builder.Services.AddAuthorization();

        // 2. REGISTRAR LOS REPOS (Ya lo tenías)
        builder.Services.AddScoped<ITaskRepository, TaskRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IPasswordHasher, BCryptHasher>();
        // 3. INDISPENSABLE: Registrar el Servicio (El que inyectaste en el Controller)
        builder.Services.AddScoped<TaskServices>();

//4. builder service for jwt 
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true, // Esto verifica que no haya expirado
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = builder.Configuration["Jwt:Issuer"],
                    ValidAudience = builder.Configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
                };
            });
        builder.Services.AddOpenApi();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();

        // 4. INDISPENSABLE: Mapear los Controladores a las rutas
        app.MapControllers();

        app.Run();
    }
}