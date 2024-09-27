using MongoDB.Driver;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;
using MongoDB.Bson;
namespace EcommereceWebAPI.Data
{
    public class MongoDbContext
    {

        private readonly IMongoDatabase _database;
        private readonly ILogger<MongoDbContext> _logger;

        public MongoDbContext(IConfiguration configuration, ILogger<MongoDbContext> logger)
        {
            var connectionString = configuration.GetSection("MongoDB:ConnectionString").Value;
            var databaseName = configuration.GetSection("MongoDB:DatabaseName").Value;

            var client = new MongoClient(connectionString);
         
            _logger = logger;

            try
            {
                _database = client.GetDatabase(databaseName);
                // Optionally run a command to check connectivity
                var command = new BsonDocument { { "ping", 1 } };
                _database.RunCommand<BsonDocument>(command);
                _logger.LogInformation("Successfully connected to MongoDB.");
            }
            catch (Exception ex)
            {

                _logger.LogError($"Failed to connect to MongoDB: {ex.Message}");
                throw; // Optionally rethrow or handle as needed
            }
        }

        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
