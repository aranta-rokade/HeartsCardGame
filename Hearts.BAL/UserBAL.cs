using Hearts.DAL;
using Hearts.Enitity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility;

namespace Hearts.BAL
{
    public class UserBAL
    {
        public LoginModel AddUser(string username, string email, string password)
        {
            try
            {
                UserDAL udal = new UserDAL();

                var user = udal.AddUser(new User
                {
                    Username = username,
                    EmailId = email,
                    Password = password,
                    LastModifiedTime = DateTime.Now
                });
                return new LoginModel{ UserName = user.Username};
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                //logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }
            
        }

        public LoginModel ValidateUser(string username, string password)
        {
            try
            {
                //TODO: Password Hashing
                UserDAL udal = new UserDAL();
                var user = udal.Validate(new User { Username = username, Password = password });
                return new LoginModel { UserName = user.Username };
            }
            catch (CustomException e)
            {
                throw new CustomException(e.Message);
            }
            catch (Exception e)
            {
                //TODO: logger.Error(e);
                throw new Exception("Oops! Some error occured.");
            }
        }
    }
}
