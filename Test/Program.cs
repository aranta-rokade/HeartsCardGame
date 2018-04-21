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
            //JOIN GAME
            GameBAL g_bal = new GameBAL();
            try
            {
                var gameURL = g_bal.JoinGame(11, 4);
                Console.WriteLine("game url: {0}", gameURL);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

            ////NEW GAME
            //GameBAL g_bal = new GameBAL();
            //try
            //{
            //    var gameURL = g_bal.NewGame(4);
            //    Console.WriteLine("game url: {0}", gameURL);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}

            ////LOGIN
            //UserBAL u_bal = new UserBAL();
            //try
            //{
            //    var user = u_bal.ValidateUser("Alice", "alice");
            //    if (user == null)
            //        Console.WriteLine("Invalid Credentials");
            //    else
            //        Console.WriteLine("Welcome, {0}!", user.UserName);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}

            ////REGISTER
            //UserBAL u_bal = new UserBAL();
            //try
            //{
            //    var user = u_bal.AddUser("Eve", "eve@host.com", "eve");
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
