using AccountManager.Database;
using AccountManager.Models;

namespace AccountManager.Services
{
    public class UserAccountService
    {
        private GameDbContext dbContext;

        public UserAccountService(GameDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public bool AddUser(User user)
        {
            dbContext.Add(user);
            dbContext.Users.Add(user);
            dbContext.SaveChanges();

            Console.WriteLine("Added user");

            return true;
        }

        public void AddUserFromForm(IFormCollection collection)
        {

            // put form into variables
            string userName = collection["UserName"];
            string password = collection["Password"];
            string email = collection["Email"];
            string phone = collection["Phone"];

            // check if username is not taken

            // do password stuff

            // add user

            User user = new User()
            {
                UserName = userName,
                Email = email,
                Phone = phone,
                PasswordHash = password,
                Salt = "sdfsdfsd"
            };

            AddUser(user);
        }
    }
}
