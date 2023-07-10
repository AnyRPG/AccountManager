namespace AccountManager.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string Salt { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }

        public User ()
        {
            UserName = string.Empty;
            PasswordHash = string.Empty;
            Salt = string.Empty;
            Email = string.Empty;
            Phone = string.Empty;
        }
    }
}
