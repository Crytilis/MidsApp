using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MidsApp.Models;
using MidsApp.Models.BuildFile;
using MidsApp.Utils;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace MidsApp.Database
{
    /// <summary>
    /// Provides a repository for managing <see cref="BuildRecord"/> objects within a MongoDB collection.
    /// </summary>
    /// <typeparam name="T">The type of data managed by the repository.</typeparam>
    public class BuildRepository<T> : IBuildRepository
    {
        private readonly IMongoCollection<BuildRecord> _collection;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildRepository{T}"/> class.
        /// </summary>
        /// <param name="database">The MongoDB database connection.</param>
        public BuildRepository(IMongoDatabase database)
        {
            _collection = database.GetCollection<BuildRecord>(GetCollectionName(typeof(BuildRecord)));
        }

        /// <summary>
        /// Retrieves the collection name from custom attributes.
        /// </summary>
        /// <param name="collectionType">The type of the collection.</param>
        /// <returns>The name of the collection.</returns>
        private static string? GetCollectionName(ICustomAttributeProvider collectionType)
        {
            return ((CollectionAttribute)collectionType.GetCustomAttributes(typeof(CollectionAttribute), true).FirstOrDefault()!).CollectionName;
        }

        /// <summary>
        /// Creates a new build record asynchronously based on the provided data.
        /// </summary>
        /// <param name="buildDto">The data transfer object containing build details.</param>
        /// <returns>An operation result indicating success or failure with an optional error message.</returns>
        public async Task<IOperationResult> CreateAsync(BuildRecordDto buildDto)
        {
            try
            {
                if (string.IsNullOrEmpty(buildDto.Primary) || string.IsNullOrEmpty(buildDto.Secondary))
                {
                    return OperationResult.Failure("Primary and Secondary powersets are required.");
                }

                if (string.IsNullOrEmpty(buildDto.Archetype))
                {
                    return OperationResult.Failure("Archetype is required.");
                }

                if (string.IsNullOrEmpty(buildDto.BuildData) || string.IsNullOrEmpty(buildDto.ImageData))
                {
                    return OperationResult.Failure("Build and image data are required.");
                }

                var buildRecord = new BuildRecord
                {
                    Name = buildDto.Name,
                    Archetype = buildDto.Archetype,
                    Description = buildDto.Description,
                    Primary = buildDto.Primary,
                    Secondary = buildDto.Secondary,
                    BuildData = buildDto.BuildData,
                    ImageData = buildDto.ImageData,
                    ExpiresAt = DateTime.UtcNow.AddDays(30)
                };

                await _collection.InsertOneAsync(buildRecord);
                var idBytes = new ObjectId(buildRecord.Id).ToByteArray();
                var shortcode = Transform.ToBase62(idBytes);

                var filter = Builders<BuildRecord>.Filter.Eq(record => record.Id, buildRecord.Id);
                var update = Builders<BuildRecord>.Update.Set(record => record.Code, shortcode);

                await _collection.UpdateOneAsync(filter, update);
                return OperationResult<string>.Success("Build created successfully", shortcode);
            }
            catch (Exception e)
            {
                return OperationResult.Failure($"Could not create the build record: {e.Message}");
            }
        }

        /// <summary>
        /// Updates an existing build record identified by a shortcode.
        /// </summary>
        /// <param name="shortcode">The unique shortcode identifying the build record.</param>
        /// <param name="buildDto">The updated data for the build record.</param>
        /// <returns>An operation result indicating success or failure with an optional error message.</returns>
        public async Task<IOperationResult> UpdateByShortcodeAsync(string shortcode, BuildRecordDto buildDto)
        {
            try
            {
                var filter = Builders<BuildRecord>.Filter.Eq(record => record.Code, shortcode);
                var existingBuild = await _collection.Find(filter).FirstOrDefaultAsync();

                if (existingBuild == null)
                {
                    return OperationResult.Failure("Build record not found.");
                }

                var updates = new List<UpdateDefinition<BuildRecord>>();

                // Optional fields - Only update if provided
                if (buildDto.Name != null)
                    updates.Add(Builders<BuildRecord>.Update.Set(r => r.Name, buildDto.Name));
        
                if (buildDto.Description != null)
                    updates.Add(Builders<BuildRecord>.Update.Set(r => r.Description, buildDto.Description));
        
                if (buildDto.Primary != null)
                    updates.Add(Builders<BuildRecord>.Update.Set(r => r.Primary, buildDto.Primary));
        
                if (buildDto.Secondary != null)
                    updates.Add(Builders<BuildRecord>.Update.Set(r => r.Secondary, buildDto.Secondary));
        
                // Required fields
                updates.Add(Builders<BuildRecord>.Update.Set(r => r.BuildData, buildDto.BuildData));
                updates.Add(Builders<BuildRecord>.Update.Set(r => r.ImageData, buildDto.ImageData));

                // Optionally reset the ExpiresAt to extend the expiration
                updates.Add(Builders<BuildRecord>.Update.Set(r => r.ExpiresAt, DateTime.UtcNow.AddDays(30)));

                if (updates.Count == 0)
                {
                    return OperationResult.Success("No changes detected, nothing to update.");
                }
        
                var combinedUpdates = Builders<BuildRecord>.Update.Combine(updates);
                await _collection.UpdateOneAsync(filter, combinedUpdates);

                return OperationResult<string>.Success("Build updated successfully", shortcode);
            }
            catch (Exception e)
            {
                return OperationResult.Failure($"Could not update the build record: {e.Message}");
            }
        }

        /// <summary>
        /// Deletes a build record identified by a shortcode.
        /// </summary>
        /// <param name="shortcode">The unique shortcode identifying the build record to be deleted.</param>
        /// <returns>An operation result indicating success or failure with an optional error message.</returns>
        public async Task<IOperationResult> DeleteByShortcodeAsync(string shortcode)
        {
            try
            {
                var filter = Builders<BuildRecord>.Filter.Eq(record => record.Code, shortcode);
                var result = await _collection.DeleteOneAsync(filter);

                return result.DeletedCount == 0 ? OperationResult.Failure("No record found with the given shortcode to delete.") : OperationResult.Success("Build successfully deleted.");
            }
            catch (Exception e)
            {
                return OperationResult.Failure($"Could not delete the build record: {e.Message}");
            }
        }

        /// <summary>
        /// Retrieves a build record identified by a shortcode.
        /// </summary>
        /// <param name="shortcode">The unique shortcode identifying the build record.</param>
        /// <returns>An operation result containing the build record if found, otherwise a failure result.</returns>
        public async Task<IOperationResult> RetrieveByShortcodeAsync(string shortcode)
        {
            try
            {
                var filter = Builders<BuildRecord>.Filter.Eq(record => record.Code, shortcode);
                var result = await _collection.Find(filter).FirstOrDefaultAsync();
                return result != null ? OperationResult<BuildRecord>.Success("Build retrieved successfully", result) : OperationResult.Failure("No record found.");
            }
            catch (Exception e)
            {
                return OperationResult.Failure($"Could not retrieve the requested build record: {e.Message}");
            }
        }

        /// <summary>
        /// Locates a build record by its shortcode to determine if it exists.
        /// </summary>
        /// <param name="shortcode">The unique shortcode identifying the build record.</param>
        /// <returns>An operation result indicating whether the record was found or not.</returns>
        public async Task<IOperationResult> LookupRecordByShortcodeAsync(string shortcode)
        {
            try
            {
                var filter = Builders<BuildRecord>.Filter.Eq(record => record.Code, shortcode);
                var result = await _collection.Find(filter).FirstOrDefaultAsync();
                return result != null ? OperationResult.Success("Build located successfully") : OperationResult.Failure("No record found.");
            }
            catch (Exception e)
            {
                return OperationResult.Failure($"Could not locate the requested build record: {e.Message}");
            }
        }

        /// <summary>
        /// Generates a file from a build record identified by a shortcode.
        /// </summary>
        /// <param name="shortcode">The unique shortcode identifying the build record.</param>
        /// <returns>An operation result containing the file data if successful, otherwise a failure result.</returns>
        public async Task<IOperationResult> GenerateBuildFileFromRecordAsync(string shortcode)
        {
            try
            {
                var filter = Builders<BuildRecord>.Filter.Eq(record => record.Code, shortcode);
                var result = await _collection.Find(filter).FirstOrDefaultAsync();
                if (result == null)
                {
                    return OperationResult.Failure("No record found.");
                }

                var decompressedData = Compression.DecompressFromBase64(result.BuildData);
                var decodedJson = Encoding.UTF8.GetString(decompressedData);
                var data = JsonConvert.DeserializeObject<BuildFile>(decodedJson);

                if (data == null)
                {
                    return OperationResult.Failure("Deserialization failed.");
                }

                var serializedData = JsonConvert.SerializeObject(data, Formatting.Indented);
                var bytes = Encoding.UTF8.GetBytes(serializedData);

                var returnData = new FileData
                {
                    CharacterName = result.Name,
                    Archetype = result.Archetype,
                    Primary = result.Primary,
                    Secondary = result.Secondary,
                    DataBytes = bytes
                };
                return OperationResult<FileData>.Success("Successfully reconstructed the build file from the record.", returnData);
            }
            catch (Exception e)
            {
                return OperationResult.Failure($"Error processing the build record: {e.Message}");
            }
        }
    }
}
