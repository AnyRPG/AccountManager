namespace AccountManager.Models
{
    public class PlayerData
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public string Name { get; set; }
        public string SaveData { get; set; }

        public PlayerData() {
            Name = string.Empty;
            SaveData = string.Empty;
        }
    }
}
