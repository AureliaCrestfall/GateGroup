using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gategourmetLibrary.Repo
{
    interface IOrderRepo
    {
        void Add(int order);
        int Get();
        void Delete(int order);
        void GetAll();
        void Update(int order);
        void Filter(string order);
    }
}
