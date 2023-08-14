using AccountManager.Database;
using AccountManager.Models;

namespace AccountManager.Services
{
    public class PlayerCharacterService
    {
        private GameDbContext dbContext;
        private ILogger logger;

        public PlayerCharacterService(GameDbContext dbContext, ILogger logger)
        {
            this.dbContext = dbContext;
            this.logger = logger;
        }

        public bool AddPlayerCharacter(PlayerData playerData)
        {
            //dbContext.Add(user);
            dbContext.PlayerCharacters.Add(playerData);
            dbContext.SaveChanges();

            logger.LogInformation($"Added Player Character {playerData.Name} with Id {playerData.Name}");

            return true;
        }

        public bool AddPlayerCharacter(int userId, CreateCharacterRequest createCharacterRequest)
        {

            // add player character
            PlayerData playerData = new PlayerData()
            {
                AccountId = userId,
                Name = createCharacterRequest.Name,
                SaveData = createCharacterRequest.SaveData
            };

            return AddPlayerCharacter(playerData);
        }
    }
}
