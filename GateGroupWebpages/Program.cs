

using gategourmetLibrary.Models;
using gategourmetLibrary.Repo;
using gategourmetLibrary.Service;

namespace GateGroupWebpages
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            builder.Services.AddSingleton<IOrderRepo, OrderRepo>();
            builder.Services.AddSingleton<OrderService>(); 
            builder.Services.AddSingleton<IEmpolyeeRepo, EmployeeRepo>();
            builder.Services.AddSingleton<EmployeeService>();
            builder.Services.AddSingleton<IDepartmentRepo, DepartmentRepo>();
            builder.Services.AddSingleton<DepartmentService>();
            builder.Services.AddSingleton<ICustomerRepo, CustomerRepo>();
            builder.Services.AddSingleton<CustomerService>();




            var app = builder.Build();


            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.MapRazorPages();

            app.Run();
        }
    }
}
