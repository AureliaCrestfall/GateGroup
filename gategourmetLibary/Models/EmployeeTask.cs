using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
// class for one task for one employee on one order, combines order, recipe part and warehouse info
namespace gategourmetLibrary.Models
{
    public class EmployeeTask
    {
        // id of order this tasks belongs to
        public int OrderId {  get; set; }

        //id of recipe part that employee works on
        public int RecipePartId {  get; set; }

        // name of the recipe part fx breakfast box
        public string TaskName {  get; set; }

        // warehouse location where specifik part is stored
        public string Location { get; set; }

        // true if employee has marked this task as completed
        public bool IsCompleted { get; set; }

       // status of the task 
       public OrderStatus Status { get; set; }

        public EmployeeTask() 
        {
            // default values when a nex object is created
            IsCompleted = false;
            Status = OrderStatus.Created;
        }

    }
}
