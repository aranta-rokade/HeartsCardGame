using Hearts.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hearts.BAL
{
    public class UserBAL
    {
        public User AddUser(string username, string emailId, string password)
        {
            UserDAL udal = new UserDAL();
            return udal.AddUser(new User
            {
                Username = username,
                EmailId = emailId,
                Password = password,
                LastModifiedTime = DateTime.Now
            });
        }

        public User ValidateUser(string username, string password)
        {
            UserDAL udal = new UserDAL();
            return udal.Validate(new User { Username = username, Password = password });
        }
    }
}
