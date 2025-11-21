using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace gategourmetLibrary.Repo
{
    interface IEmpolyeeRepo
    {
        void Add(int empolyee);
        int Get();
        void Delete(int empolyee);
        void GetAll();
        void Update(int empolyee);
        void Filter(string empolyee);
    }
}
