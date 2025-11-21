
using simply_database_test.Secret;

namespace simply_database_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            connect on = new connect();
            testrepo test = new testrepo(on.cstring);

            List<testallergi> allergiw = new List<testallergi>();
            allergiw = test.GetAll();

            foreach (testallergi a in allergiw)
            {
                Console.WriteLine($" it has an id of {a.ID} and name  of {a.Name}");

            }
        }
    }
}
