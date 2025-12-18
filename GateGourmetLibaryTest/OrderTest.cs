using gategourmetLibrary.Models;
using gategourmetLibrary.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using System.Diagnostics;

namespace GateGourmetLibaryTest
{
    public class OrderTest
    {
        [Theory]
        [InlineData(1)]
        [InlineData(4)]
        public async Task AddTest(int id)
        {
            FakeOrderRepo orderRepo = new FakeOrderRepo();
            OrderService orderService = new OrderService(orderRepo);
            Order order = new Order();
            order.ID = id;
            order.OrderMade = DateTime.Now;
            order.OrderDoneBy = DateTime.Now.AddDays(7);
            order.Recipe.Add(1,new RecipePart());
            order.Recipe[1].partName = "test";


            orderService.AddOrder(order);


            Assert.Equal(id, orderRepo.DictOrders[id].ID);
        }

        [Fact]
        public async Task FilterByStatus()
        {
            FakeOrderRepo orderRepo = new FakeOrderRepo();
            OrderService orderService = new OrderService(orderRepo);
            List<Order> testList = new List<Order>();

            for(int i = 0; i < 10; i++)
            {
                testList.Add(new Order
                {
                    ID = i,

                    Status = OrderStatus.Created
                });

            }



            foreach(Order o in testList)
            {
              Order or = new Order
                {
                    ID = o.ID,
                    OrderMade = DateTime.Now,
                    OrderDoneBy = DateTime.Now.AddDays(7),


                } ;
                or.Recipe.Add(1, new RecipePart());
                 or.Recipe[1].partName = "test";
                orderService.AddOrder(or);
            }
           List<Order> FilteredList = orderService.FilterOrdersByStatus(OrderStatus.Cancelled);

           Assert.Equal(testList.Count, orderRepo.ListOrders.Count);
           Assert.NotEqual(testList.Count, FilteredList.Count);
           Assert.Equal(0, FilteredList.Count);
           



        }

    }
}
