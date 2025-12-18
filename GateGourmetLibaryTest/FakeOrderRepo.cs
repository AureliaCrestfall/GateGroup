using gategourmetLibrary.Models;
using gategourmetLibrary.Repo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GateGourmetLibaryTest
{
    internal class FakeOrderRepo : IOrderRepo
    {

        public Dictionary<int,Order> DictOrders = new Dictionary<int, Order>();
        public List<Order> ListOrders = new List<Order>();

        public void AddOrder(Order order)
        {
            DictOrders.Add(order.ID,order);
            ListOrders.Add(order);
        }

        public void CancelOrder(int orderId)
        {
            DictOrders[orderId].Status = OrderStatus.Cancelled;
        }

        public void DeleteOrder(int ID)
        {
            DictOrders.Remove(ID);
        }

        public List<Order> FilterByCompany(Customer customer)
        {
            throw new NotImplementedException();
        }

        public List<Order> FilterByDate(DateTime date)
        {
            throw new NotImplementedException();
        }

        public List<Order> FilterByDepartment(int departmentId)
        {
            throw new NotImplementedException();
        }

        public List<Order> FilterByEmployee(Employee employee)
        {
            throw new NotImplementedException();
        }

        public List<Order> FilterByStatus(OrderStatus status)
        {
            List<Order> FOrders = new List<Order>();
            foreach(Order o in ListOrders)
            {
                if(o.Status == status)
                {
                    FOrders.Add(o);
                }
            }
            return FOrders;
        }

        public List<Order> FilterByToday(DateTime today)
        {
            throw new NotImplementedException();
        }

        public Order Get(int orderID)
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, Order> GetAll()
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, string> GetAllAllergies()
        {
            throw new NotImplementedException();
        }

        public Dictionary<int, Ingredient> GetAllIngredients()
        {
            throw new NotImplementedException();
        }

        public List<Order> GetAllOrders()
        {
            throw new NotImplementedException();
        }

        public List<Order> GetAllOrdersFromid(int id)
        {
            throw new NotImplementedException();
        }

        public List<Warehouse> GetAllWarehouses()
        {
            throw new NotImplementedException();
        }

        public Warehouse GetRecipePartLocation(int recipePartId)
        {
            throw new NotImplementedException();
        }

        public List<RecipePart> GetRecipeParts(int orderID)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrder(int orderID, Order updatedOrder)
        {
            throw new NotImplementedException();
        }

        public void UpdateRecipePartLocation(int recipePartId, int warehouseId)
        {
            throw new NotImplementedException();
        }
    }
}
