using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gategourmetLibrary.Repo
{
    public interface ICustomerRepo
    {
        void Add(int customer);
        int Get();
        void Delete(int customer);
        void GetAll();
        void MyOrder(int customer);
        void Update(int customer);
        void Filter(string customer);
    }
}
