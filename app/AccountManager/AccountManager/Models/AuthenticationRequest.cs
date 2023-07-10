namespace AccountManager.Models
{
    public class AuthenticationRequest
    {
        public string UserName { get; set; }
        public string Password { get; set; }

        public void GetRequestFromCollection(IFormCollection collection) {
            // put form into variables
            UserName = collection["username"];
            Password = collection["password"];
        }

    }
}
