using gategourmetLibrary.Models;
using gategourmetLibrary.Repo;
using gategourmetLibrary.Secret;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CompanyWebpages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Henter connection string fra Connect klassen 
            string connection = new Connect().cstring;
            Debug.WriteLine(connection);
            // Add services to the container
            builder.Services.AddRazorPages();

            builder.Services.AddSingleton<IOrderRepo>
            (sp => new OrderRepo(connection));

            builder.Services.AddSingleton<OrderService>();

            builder.Services.AddSingleton<IEmpolyeeRepo>
                (sp => new EmployeeRepo(connection));

            builder.Services.AddSingleton<EmployeeService>();

            builder.Services.AddSingleton<IDepartmentRepo>
                (sp => new DepartmentRepo(connection));

            builder.Services.AddSingleton<DepartmentService>();

            builder.Services.AddSingleton<ICustomerRepo>
                (sp => new CustomerRepo(connection));

            builder.Services.AddSingleton<CustomerService>();

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            // Til logout knap mm
            app.UseSession();

            app.MapRazorPages();

            app.Run();
        }
    }
}
