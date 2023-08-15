using AccountManager.Database;
using AccountManager.Models;
using Microsoft.EntityFrameworkCore;

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

        public bool AddPlayerCharacter(PlayerCharacter playerCharacter)
        {
            //dbContext.Add(user);
            dbContext.PlayerCharacters.Add(playerCharacter);
            dbContext.SaveChanges();

            logger.LogInformation($"Added Player Character {playerCharacter.Name} with Id {playerCharacter.Id}");

            return true;
        }

        public bool AddPlayerCharacter(int userId, CreateCharacterRequest createCharacterRequest)
        {

            // add player character
            PlayerCharacter playerCharacter = new PlayerCharacter()
            {
                AccountId = userId,
                Name = createCharacterRequest.Name,
                SaveData = createCharacterRequest.SaveData
            };

            return AddPlayerCharacter(playerCharacter);
        }

        public List<PlayerCharacter> GetPlayerCharacters(int userId)
        {
            return dbContext.PlayerCharacters.Where(u => u.AccountId == userId).ToList();
        }
    }
}
