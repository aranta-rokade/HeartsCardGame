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

                string activeGameId = null;
                if(user.ActiveGameId!=null)
                    activeGameId = hashing.Encrypt(user.ActiveGameId.ToString());
                return new UserModel
                {
                    UserId = hashing.Encrypt(user.UserId.ToString()),
                    UserName = user.Username,
                    EmailId = user.EmailId,
                    Wins = user.Wins,
                    Draws = user.Draws,
                    Losses = user.Losses,
                    ActiveGameId = activeGameId,
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

                string activeGameId = null;
                if (user.ActiveGameId != null)
                    activeGameId = hashing.Encrypt(user.ActiveGameId.ToString());

                return new UserModel
                {
                    UserId = hashing.Encrypt(user.UserId.ToString()),
                    UserName = user.Username,
                    EmailId = user.EmailId,
                    Wins = user.Wins,
                    Draws = user.Draws,
                    Losses = user.Losses,
                    ActiveGameId = activeGameId,
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

        public List<UserModel> GetAllUsers() {
            try
            {
                Hashing hashing = new Hashing();
                UserDAL u_dal = new UserDAL();
                var users = u_dal.GetAllUsers();
                List<UserModel> userModel = new List<UserModel>();
                foreach (var u in users)
                {
                    userModel.Add(new UserModel
                    {
                        UserName = u.Username,
                        EmailId = u.EmailId,
                        Wins = u.Wins,
                        Draws = u.Draws,
                        Losses = u.Losses,
                        Points = u.Points
                    });
                }
                return userModel;
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
