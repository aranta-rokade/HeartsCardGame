using Hearts.BAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            UserBAL u_bal = new UserBAL();
            try
            {
                var user = u_bal.ValidateUser("sampl", "xxxx");
                if (user == null)
                    Console.WriteLine("Invalid Credentials");
                else
                    Console.WriteLine("Welcome, {0}!", user.UserName);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            //UserBAL u_bal = new UserBAL();
            //try
            //{
            //    var user = u_bal.AddUser("sampl", "sample@host.com", "xxxxx");
            //    if (user != null)
            //        Console.WriteLine("User Created");
            //    else
            //        Console.WriteLine("not created.");
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}
        }
    }
}
