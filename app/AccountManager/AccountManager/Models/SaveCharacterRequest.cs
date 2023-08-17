namespace AccountManager.Models
{
    public class SaveCharacterRequest
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string SaveData { get; set; }

        public SaveCharacterRequest()
        {
            Name = string.Empty;
            SaveData = string.Empty;
        }
    }
}
