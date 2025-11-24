using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// class for enums
namespace gategourmetLibrary.Models
{
    // describes the current status of an order 
    public enum OrderStatus
    {
        Created,
        InProgress,
        Completed,
        Cancelled
    }

    //describes the type of warehouse 

    public enum WarehouseType
    {
        Freezer,
        Fridge, 
        DryStorage
    }
}
