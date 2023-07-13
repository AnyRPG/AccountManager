namespace AccountManager.Models
{
    public class AccountManagerSettings
    {
        public AccountManagerSettings() {
            BearerKey = string.Empty;
        }
        public string BearerKey { get; set; }
    }
}
