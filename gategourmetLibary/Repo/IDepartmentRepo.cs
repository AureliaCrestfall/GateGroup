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
        // gets the managers of a specific department
        List<Manager> GetDepartmentManagers(int departmentId);
        // gets the employees of a specific department
        List<Employee> GetDepartmentEmployees(int departmentId);
        // adds a new manager to a department
        void AddNewDepartmentManager(int departmentId, Manager newManager);
        // adds a new employee to a department
        void AddNewDepartmentEmployee(int departmentId, Employee newEmployee);
        // removes stock from a department's warehouse
        void RemoveStock(Ingredient ingredient, int amount, int departmentID, int warehouseID);



        //    void Add(int department);
        //    void Delete(int department);
        //    void GetAll();
        //    void Update(int DepartmentID, Department UpdateDepartment);
        //    void NewWarehouse(Warehouse newWarehouse);
        //    void StokIngredient(Ingredient stockIngredient);
        //    void GetWarehouseStock(int warehouse);
        //    void GetDepartmentManagers(int department);
        //    void GetDepartmentEmployees(int department);
        //    int Get(int DepartmentID);
        //    void AddNewDepartmentManager(int DepartmentID, manager newManager);
        //    void AddnewDepartmentEmpolyee(int DepartmentID, Employee newEmployee);
        //    void RemoveStock(Ingredient ingredient, int amount, Department departmentID, Warehouse warehouseID);
        //void Add(int department);
        //void Delete(int department);
        //void GetAll();
        //void Update(int DepartmentID, Department UpdateDepartment);
        //void NewWarehouse(WereHous newwerehous);
        //void Stoklngredient(Ingredient stocklngredient);
        //void GetWerehousStock(int werehous);
        //void GetDepartmentManagers(int department);
        //void GetDepartmentEmployees(int department);
        //int Get(int DepartmentID);
        //void AddNewDepartmentManager(int DepartmentID, manager newManager);
        //void AddnewDepartmentEmpolyee(int DepartmentID, Employee newEmployee);
        //void RemoveStock(Ingredient Ingredient, int amount, Department departmentID, Warehouse warehouseID);

    }
}

