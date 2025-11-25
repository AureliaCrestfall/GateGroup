using simply_database_test.Secret;
using System;
using System.Collections.Generic;
using System.Text;

namespace simply_database_test
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // existing DB/test code
            connect on = new connect();
            testrepo test = new testrepo(on.cstring);

            List<testallergi> allergi = test.GetAll();

            foreach (testallergi a in allergi)
            {
                Console.WriteLine($" it has an id of {a.ID} and name  of {a.Name}");
            }

            // Simple in-memory user store for demo (replace with real user store)
            var users = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                ["alice"] = "password1",
                ["bob"]   = "secret"
            };







            Console.WriteLine("Log In");
            Console.WriteLine("Insert UserName");
            string? usernameInput = Console.ReadLine();
            string username = usernameInput ?? string.Empty; // avoid nullability warnings

            Console.Write("Insert Password: ");
            string password = ReadPassword();

            if (Authenticate(username, password, users))
            {
                Console.WriteLine("Log in successful: " + username);
            }
            else
            {
                Console.WriteLine("forkert Brugernavn eller password ");
            }

            // Local function: authenticate against dictionary
            static bool Authenticate(string username, string password, Dictionary<string, string> users)
            {
                if (string.IsNullOrEmpty(username))
                    return false;

                return users.TryGetValue(username, out var stored) && stored == password;
            }

            // Local function: secure console password reader (masks input with '*')
            static string ReadPassword()
            {
                var pass = new StringBuilder();
                ConsoleKeyInfo key;

                while (true)
                {
                    key = Console.ReadKey(true);

                    if (key.Key == ConsoleKey.Enter)
                    {
                        Console.WriteLine();
                        break;
                    }

                    if (key.Key == ConsoleKey.Backspace)
                    {
                        if (pass.Length > 0)
                        {
                            pass.Length--;
                            Console.Write("\b \b");
                        }
                    }
                    else
                    {
                        pass.Append(key.KeyChar);
                        Console.Write("*");
                    }
                }

                return pass.ToString();
            }
        }
    }
}
