using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using gategourmetLibary.Models;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;


namespace CompanyWebpages.Pages
{
    public class Create_AdminModel : PageModel
    {
        readonly EmployeeService _employee;

        public Create_AdminModel(EmployeeService employee)
        {
            _employee = employee;
        }


        [BindProperty]
        [Required(ErrorMessage = "Please select a position")]
        public int? PositionId { get; set; }


        [BindProperty]
        public Admin Admin { get; set; } = new Admin();


        public List<SelectListItem> Positions { get; set; }
       
        public IActionResult OnGet()
        {
            // Tjek om brugeren er logget ind før den giver adgang til siden 
            if (HttpContext.Session.GetString("IsLoggedIn") != "true")
            {
                // Hvis IKKE logget ind - send til login siden
                return RedirectToPage("/EmployeeLogin");
            }
            else
            {
                // Simuler data - i praksis vil du hente dette fra en database/service
                Positions = new List<SelectListItem>
            {
                new SelectListItem { Value = "1", Text = "System Administrator" },
                new SelectListItem { Value = "2", Text = "HR Manager" },
                new SelectListItem { Value = "3", Text = "Deparment Manager" }
            };
                // Hvis logget ind - vis siden som normalt
                return Page();
            }
           
            

        }
        

        public IActionResult OnPost()
        {
            //if (!ModelState.IsValid)
            //{
            //    // Genindlæs positions hvis validering fejler
            //    OnGet();
            //    Debug.WriteLine("is not valid test");
            //    return Page();

            //}
            Admin.MyPosition.Id = Convert.ToInt32(PositionId);
                _employee.AddNewAdmin(Admin);



            Debug.WriteLine("admin is being added");

            
            

            // Efter oprettelse - redirect til en bekræftelsesside eller liste
            return RedirectToPage("/Employees");
        }
    }
        
        


    
}
