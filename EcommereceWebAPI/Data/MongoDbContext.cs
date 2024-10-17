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
            // Retrieve the MongoDB connection string from the configuration
            var connectionString = configuration.GetSection("MongoDB:ConnectionString").Value;
            // Retrieve the MongoDB database name from the configuration
            var databaseName = configuration.GetSection("MongoDB:DatabaseName").Value;

            // Create a new instance of the MongoClient using the connection string
            var client = new MongoClient(connectionString);

            _logger = logger;

            try
            {
                // Get the MongoDB database using the database name
                _database = client.GetDatabase(databaseName);
                // Optionally run a command to check connectivity
                var command = new BsonDocument { { "ping", 1 } };
                _database.RunCommand<BsonDocument>(command);
                // Log a success message indicating successful connection to MongoDB
                _logger.LogInformation("Successfully connected to MongoDB.");
            }
            catch (Exception ex)
            {
                // Log an error message indicating failed connection to MongoDB
                _logger.LogError($"Failed to connect to MongoDB: {ex.Message}");
                throw; // Optionally rethrow or handle as needed
            }
        }

        // Get a collection from the MongoDB database
        public IMongoCollection<T> GetCollection<T>(string collectionName)
        {
            return _database.GetCollection<T>(collectionName);
        }
    }
}
