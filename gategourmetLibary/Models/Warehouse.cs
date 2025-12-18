using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// class representing a physical warehouse, where ingredients are stored
namespace gategourmetLibrary.Models
{
    public class Warehouse
    {
        // physical location of warehouse
        public string Location {  get; set; }

        // using enum and type of warehouse
        public WarehouseType Type { get; set; }
        
        // name of warehouse
        public string Name { get; set; }

        // Unique identifier (id)
        public int ID { get; set; }

        // id for the department this warehouse belongs to 
        public int DepartmentId { get; set; }

        // list for stored ingredients in warehouse 
        public List<Ingredient> ingredientsInWarehouse { get; set; }
         
        public Warehouse()
        {
            ingredientsInWarehouse = new List<Ingredient>();
        }
    }
}
