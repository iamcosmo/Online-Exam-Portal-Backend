
using Domain.Models;
using Infrastructure.Repositories.Implementations;
using Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace oep
{
    public class Program
    {
        public static void Main(string[] args)
        {
            try{var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddScoped<IExamRepository,ExamRepository>();
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
            builder.Services.AddControllers();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
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
            catch (Exception e){ Console.WriteLine(e); }
        }

    }
}
