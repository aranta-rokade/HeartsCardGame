using Hearts.DAL;
using Hearts.ViewModel;
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
        public bool AddUser(string username, string email, string password)
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
                if (user != null)
                    return true;
                return false;
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

        public bool ValidateUser(string username, string password)
        {
            try
            {
                //TODO: Password Hashing
                UserDAL udal = new UserDAL();
                return udal.Validate(new User { Username = username, Password = password });
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

        public UserModel GetUserByUserName(string username)
        {
            try
            {
                Hashing hashing = new Hashing();
                UserDAL udal = new UserDAL();
                var user = udal.GetUserByUserName(username);
                return new UserModel
                {
                    UserId = hashing.Encrypt(user.UserId.ToString()),
                    UserName = user.Username,
                    EmailId = user.EmailId,
                    Wins = user.Wins,
                    Draws = user.Draws,
                    Losses = user.Losses,
                    ActiveGameURL = hashing.Encrypt(user.ActiveGameId.ToString()),
                    LastModifiedTime = user.LastModifiedTime,
                    Points = user.Points
                };
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

        public UserModel GetUserById(string hashedUserId)
        {
            try
            {
                Hashing hashing = new Hashing();
                UserDAL udal = new UserDAL();
                var userId = Convert.ToInt32(hashing.Decrypt(hashedUserId));
                var user = udal.GetUserById(userId);
                return new UserModel
                {
                    UserId = hashing.Encrypt(user.UserId.ToString()),
                    UserName = user.Username,
                    EmailId = user.EmailId,
                    Wins = user.Wins,
                    Draws = user.Draws,
                    Losses = user.Losses,
                    ActiveGameURL = hashing.Encrypt(user.ActiveGameId.ToString()),
                    LastModifiedTime = user.LastModifiedTime,
                    Points = user.Points
                };
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
