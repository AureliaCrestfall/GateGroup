using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using gategourmetLibrary.Models;
using gategourmetLibrary.Service;

namespace gategourmetLibrary.Repo
{
    public interface IDepartmentRepo
    {
        // returns all departments
        List<Department> GetAllDepartments();
        // adds a new department
        void AddDepartment(Department newDepartment);
        // deletes a department by ID
        void DeleteDepartment(int departmentId);
        // updates department info by ID
        void UpdateDepartment(int departmentId, Department updatedDepartment);
        // returns a specific department by ID
        Department GetDepartment(int departmentId);
        // assigns a new warehouse to a department
        void NewWarehouse(Warehouse newWarehouse);
        // stocks an ingredient in the department's warehouse
        void StockIngredient(Ingredient stockIngredient);
        // gets the stock of a specific warehouse
        List<Ingredient> GetWarehouseStock(int warehouseId);

        // gets the positions of a specific department
        List<Position> GetDepartmentPositions(int departmentId);

        // gets the employees of a specific department
        List<Employee> GetDepartmentEmployees(int departmentId);
    
        // removes stock from a department's warehouse
        void RemoveStock(Ingredient ingredient, int amount, int departmentID, int warehouseID);

        // adds a new position to an employee in a specific department
        void AddNewDepartmentPosition(int departmentId, Employee employee, Position newPosition);

        // adds a new employee in department
        void AddNewDepartmentEmployee(int departmentId, Employee newEmployee);

    }
}

