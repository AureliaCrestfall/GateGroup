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

            // get connection string from Connect class
            string connection = new Connect().cstring;

            // Razor Pages
            builder.Services.AddRazorPages();

            // Repositories that use the connection string
            builder.Services.AddSingleton<IOrderRepo>(sp => new OrderRepo(connection));
            builder.Services.AddSingleton<IEmpolyeeRepo>(sp => new EmployeeRepo(connection));
            builder.Services.AddSingleton<IDepartmentRepo>(sp => new DepartmentRepo(connection));
            builder.Services.AddSingleton<ICustomerRepo>(sp => new CustomerRepo(connection));

            // Services (DI injects the right repo automatically)
            builder.Services.AddSingleton<OrderService>();
            builder.Services.AddSingleton<EmployeeService>();
            builder.Services.AddSingleton<DepartmentService>();
            builder.Services.AddSingleton<CustomerService>();

            // Session setup
            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession();

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

            app.UseSession();      // session must be before endpoints
            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
