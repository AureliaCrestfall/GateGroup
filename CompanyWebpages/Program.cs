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

            // Razor Pages
            builder.Services.AddRazorPages();

            // Repositories
            builder.Services.AddSingleton<IOrderRepo>(sp => new OrderRepo(connection));
            builder.Services.AddSingleton<IEmpolyeeRepo>(sp => new EmployeeRepo(connection));
            builder.Services.AddSingleton<IDepartmentRepo>(sp => new DepartmentRepo(connection));
            builder.Services.AddSingleton<ICustomerRepo>(sp => new CustomerRepo(connection));

            // Services (de får deres repo automatisk via DI)
            builder.Services.AddSingleton<OrderService>();
            builder.Services.AddSingleton<EmployeeService>();
            builder.Services.AddSingleton<DepartmentService>();
            builder.Services.AddSingleton<CustomerService>();

            // Hvis EmployeeTask også er en service, der skal bruge connection:
            // builder.Services.AddSingleton<EmployeeTask>(sp => new EmployeeTask(connection));

            // Session
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();
            builder.Services.AddRazorPages();
           

            var app = builder.Build();

            // Pipeline
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseSession();      // session skal være før endpoints
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
