namespace AccountManager.Models
{
    public class CreateCharacterRequest
    {
        public string Name { get; set; }
        public string SaveData { get; set; }

        public CreateCharacterRequest()
        {
            Name = string.Empty;
            SaveData = string.Empty;
        }
    }
}
