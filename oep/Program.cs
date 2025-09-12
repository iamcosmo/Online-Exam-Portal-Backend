
using Domain.Data;
using Domain.Models;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

namespace oep
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Add services to the container.
                builder.Services.AddScoped<IExamRepository, ExamRepository>();
                builder.Services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
                builder.Services.AddControllers();

                var JwtKey = builder.Configuration.GetValue<string>("Jwt:Secret");

                //builder.Services.AddAuthentication(x =>
                //{
                //    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;

                //    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                //})
                //   .AddJwtBearer(x =>
                //   {
                //       x.RequireHttpsMetadata = true;
                //       x.SaveToken = true;
                //       x.TokenValidationParameters = new TokenValidationParameters
                //       {
                //           ValidateIssuerSigningKey = true,
                //           IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(JwtKey)),

                //           ValidateIssuer = true,
                //           ValidateAudience = true,
                //           ValidIssuer = builder.Configuration["Jwt:Issuer"],
                //           ValidAudience = builder.Configuration["Jwt:Audience"],
                //       };
                //   });


                builder.Services.AddEndpointsApiExplorer();
                builder.Services.AddSwaggerGen();

                var app = builder.Build();

                // Configure the HTTP request pipeline.
                //if (app.Environment.IsDevelopment())
                //{
                app.UseSwagger();
                app.UseSwaggerUI();
                //}


                app.UseHttpsRedirection();

                app.UseAuthorization();


                app.MapControllers();

                app.Run();
            }
            catch (Exception e) { Console.WriteLine(e); }
        }

    }
}
