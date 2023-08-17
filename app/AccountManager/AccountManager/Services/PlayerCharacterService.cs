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

        public bool SavePlayerCharacter(int userId, SaveCharacterRequest saveCharacterRequest)
        {
            var playerCharacter = dbContext.PlayerCharacters.First(u => u.AccountId == userId && u.Id == saveCharacterRequest.Id);
            if (playerCharacter.Name != saveCharacterRequest.Name)
            {
                playerCharacter.Name = saveCharacterRequest.Name;
            }
            playerCharacter.SaveData = saveCharacterRequest.SaveData;
            dbContext.SaveChanges();

            return true;
        }

        public bool DeletePlayerCharacter(int userId, DeleteCharacterRequest deleteCharacterRequest)
        {
            var playerCharacter = dbContext.PlayerCharacters.First(u => u.AccountId == userId && u.Id == deleteCharacterRequest.Id);
            dbContext.PlayerCharacters.Remove(playerCharacter);
            dbContext.SaveChanges();

            return true;
        }

        public PlayerCharacterListResponse GetPlayerCharacters(int userId)
        {
            PlayerCharacterListResponse playerCharacterListResponse = new PlayerCharacterListResponse()
            {
                PlayerCharacters = dbContext.PlayerCharacters.Where(u => u.AccountId == userId).ToList()
            };

            return playerCharacterListResponse;
        }

    }

    
}
