using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MidsApp.Models;
using MongoDB.Driver;

namespace MidsApp.Services
{
    /// <summary>
    /// Initializes the database by ensuring the creation of a Time-To-Live (TTL) index on the specified collection.
    /// </summary>
    public class TtlInitializer
    {
        private readonly IMongoDatabase _database;
        private readonly ILogger<TtlInitializer> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="TtlInitializer"/> class.
        /// </summary>
        /// <param name="database">The MongoDB database instance.</param>
        /// <param name="logger">The logger instance for logging.</param>
        public TtlInitializer(IMongoDatabase database, ILogger<TtlInitializer> logger)
        {
            _database = database;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves the collection name from custom attributes applied to a class.
        /// </summary>
        /// <param name="collectionType">The type of the collection which may have a CollectionAttribute defining its MongoDB collection name.</param>
        /// <returns>The name of the collection as specified by the CollectionAttribute, or null if the attribute is not found.</returns>
        private static string? GetCollectionName(ICustomAttributeProvider collectionType)
        {
            return ((CollectionAttribute)collectionType.GetCustomAttributes(typeof(CollectionAttribute), true).FirstOrDefault()!).CollectionName;
        }

        /// <summary>
        /// Initializes the database asynchronously by ensuring the TTL index.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task InitializeAsync()
        {
            await EnsureTtlIndexAsync();
        }

        /// <summary>
        /// Ensures the creation of a Time-To-Live (TTL) index on the collection asynchronously.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        private async Task EnsureTtlIndexAsync()
        {
            var collection = _database.GetCollection<BuildRecord>(GetCollectionName(typeof(BuildRecord)));
            var indexOptions = new CreateIndexOptions { ExpireAfter = TimeSpan.FromSeconds(0) };
            var indexKeys = Builders<BuildRecord>.IndexKeys.Ascending(record => record.ExpiresAt);

            try
            {
                await collection.Indexes.CreateOneAsync(new CreateIndexModel<BuildRecord>(indexKeys, indexOptions));
                _logger.LogInformation($"TTL index on 'ExpiresAt' ensured on {GetCollectionName(typeof(BuildRecord))} collection.");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to ensure TTL index on BuildRecords collection: {ex.Message}");
                throw;
            }
        }
    }
}
