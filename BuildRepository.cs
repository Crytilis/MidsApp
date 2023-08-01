using System;
using System.Linq;
using MongoDB.Driver;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using IdGen;
using Microsoft.Extensions.DependencyInjection;
using MidsApp.Models;
using MongoDB.Driver.Linq;

namespace MidsApp
{
    /// <summary>
    /// MongoDB crud repository
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class BuildRepository<T> where T : BuildRecord, new()
    {
        private readonly IMongoCollection<T> _collection;
        private readonly IdGenerator _idGenerator;
        private readonly IMongoQueryable<T> _query;

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="database"></param>
        /// <param name="provider"></param>
        public BuildRepository(IMongoDatabase database, IServiceProvider provider)
        {
            _collection = database.GetCollection<T>(GetCollectionName(typeof(T)));
            _idGenerator = provider.GetRequiredService<IdGenerator>();
            _query = _collection.AsQueryable();
        }

        private static string? GetCollectionName(ICustomAttributeProvider collectionType)
        {
            return ((CollectionAttribute) collectionType.GetCustomAttributes(typeof(CollectionAttribute), true).FirstOrDefault()!).CollectionName;
        }

        private async Task<bool> Exists(string value) => await _query.AnyAsync(s => s != null && s.Id.Equals(value));

        public bool BuildExists(string code) => _query.Any(b => b.Code.Equals(code));

        private static string CodeFromSnowflake(long snowflake)
        {
            var map = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789".ToCharArray();
            var url = string.Empty;
            while (snowflake > 0)
            {
                url += (map[snowflake % 62]);
                snowflake /= 62;
            }

            return Reverse(url);
        }
        private static string Reverse(string input)
        {
            var a = input.ToCharArray();
            int l, r = a.Length - 1;
            for (l = 0; l < r; l++, r--)
            {
                (a[l], a[r]) = (a[r], a[l]);
            }

            return string.Join("", a);
        }

        private static long SnowflakeFromCode(string code)
        {
            var id = code.Aggregate<char, long>(0, (current, c) => c switch
            {
                >= 'a' and <= 'z' => current * 62 + c - 'a',
                >= 'A' and <= 'Z' => current * 62 + c - 'A' + 26,
                >= '0' and <= '9' => current * 62 + c - '0' + 52,
                _ => current
            });
            return id;
        }

        /// <summary>
        /// Generates snowflake based ID
        /// </summary>
        /// <returns>long</returns>
        public async Task<long> GenerateSnowflake()
        {
            var snowflake = _idGenerator.CreateId();
            while (await Exists(snowflake.ToString()))
            {
                snowflake = _idGenerator.CreateId();
            }

            return snowflake;
        }

        public async Task<long?> GetSnowflakeFromCode(string code)
        {
            var snowflake = SnowflakeFromCode(code);
            var exists = await Exists(snowflake.ToString());
            if (exists) return snowflake;
            return null;
        }

        /// <summary>
        /// Creates a record with the given snowflake, buildData, and imageData
        /// </summary>
        /// <param name="snowflake"></param>
        /// <param name="buildData"></param>
        /// <param name="imageData"></param>
        /// <param name="cancel"></param>
        /// <returns>object</returns>
        public async Task<OperationResult> CreateRecord(string snowflake, string buildData, string imageData, CancellationToken cancel = default)
        {
            var result = new OperationResult();
            T record;
            try
            {
                record = new T
                {
                    Id = snowflake,
                    BuildData = buildData,
                    ImageData = imageData,
                    Code = CodeFromSnowflake(Convert.ToInt64(snowflake))
                };

                await _collection.InsertOneAsync(record, cancellationToken: cancel);
            }
            catch (MongoWriteException)
            {
                result.Success = false;
                result.ErrorMessage = "Record already exists for this Id";
                return result;
            }

            result.Success = true;
            result.Id = snowflake;
            result.BuildUrl = $"https://mids.app/build/retrieve/{record.Code}";
            result.ImageUrl = $"https://mids.app/build/image/{record.Code}.png";
            result.Code = record.Code;
            return result;
        }

        /// <summary>
        /// Updates the htmlData for a record matching the provided code
        /// </summary>
        /// <param name="code"></param>
        /// <param name="pageData"></param>
        /// <returns>object</returns>
        public async Task<OperationResult> UpdatePageData(string code, string pageData)
        {
            var result = new OperationResult();
            T record;
            var idFilter = Builders<T>.Filter.Eq(r => r.Code, code);
            var updateFilter = Builders<T>.Update.Set(buildRecord => buildRecord.PageData, pageData);
            try
            {
                record = await _collection.FindOneAndUpdateAsync(idFilter, updateFilter);
            }
            catch (MongoWriteException)
            {
                result.Success = false;
                result.ErrorMessage = $"Failed to find a record matching {code}";
                return result;
            }

            result.Success = true;
            result.PageUrl = $"https://mids.app/build/preview/{record.Code}.htm";
            return result;
        }

        /// <summary>
        /// Retrieves a build from the database for the specified short code
        /// </summary>
        /// <param name="code"></param>
        /// <returns>object</returns>
        public async Task<T?> RetrieveRecord(string code)
        {
            return await _query.FirstOrDefaultAsync(record => record != null && record.Code.Equals(code));
        }

        private async Task<OperationResult> Clean()
        {
            var filter = Builders<T>.Filter.Empty;
            var update = Builders<T>.Update.Unset("Extension");
            var uResult = await _collection.UpdateManyAsync(filter, update);
            return new OperationResult { Success = true };
        }
    }
}
