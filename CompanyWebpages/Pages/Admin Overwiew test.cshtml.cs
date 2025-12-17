using gategourmetLibary.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using gategourmetLibrary.Models;
using System.Collections.Generic;



namespace CompanyWebpages.Pages
{
    public class Admin_Overwiew_testModel : PageModel
    {
        public List<Admin> Admins { get; set; }

        public void OnGet()
        {
            Admins = new List<Admin>();
        }

    }
}
