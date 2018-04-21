using Hearts.DAL;
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
            UserDAL u_dal = new Hearts.DAL.UserDAL();
            u_dal.Validate( new User { Username= "sampl", Password= "xxxxx" });
        }
    }
}
