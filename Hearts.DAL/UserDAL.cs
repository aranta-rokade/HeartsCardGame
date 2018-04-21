using System;
using System.Linq;

namespace Hearts.DAL
{
    public class UserDAL
    {
        private static NLog.Logger logger = NLog.LogManager.GetCurrentClassLogger();
        public User AddUser(User user)
        {
            using (var db = new HeartsEntities())
            {
                if (db.Users.FirstOrDefault(x => x.Username.Equals(user.Username)) != null)
                    throw new Exception("User name already taken.");
                if (db.Users.FirstOrDefault(x => x.EmailId.Equals(user.EmailId)) != null)
                    throw new Exception("Email Id already exists.");

                db.Users.Add(user);
                db.SaveChanges();
                logger.Info("UserId: {0} - Registered.", user.UserId);
                return user;
            }
        }

        public User Validate(User user)
        {
            using (var db = new HeartsEntities())
            {
                var u = db.Users.FirstOrDefault(x =>
                        x.Username.Equals(user.Username) && x.Password.Equals(user.Password));
                if (u != null)
                {
                    logger.Info("UserId: {0} - Logged in.", user.UserId);
                    return u;
                }
                   

                logger.Info("Username: {0} - Invalid Credentials.", user.Username);
                throw new Exception("Invalid Credentials.");
            }
        }
    }
}
