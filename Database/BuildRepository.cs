using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using MidsApp.Models;
using MidsApp.Models.BuildFile;
using MidsApp.Services;
using MidsApp.Utils;
using MongoDB.Bson;
using Newtonsoft.Json;

namespace MidsApp.Database
{
    /// <summary>
    /// Provides a repository for managing BuildRecord objects within a MongoDB collection. This repository handles CRUD operations.
    /// </summary>
    public class BuildRepository : IBuildRepository
    {
        private readonly IMongoCollection<BuildRecord> _collection;
        private readonly IUrlBuilder _urlBuilder;

        /// <summary>
        /// Initializes a new instance of the BuildRepository class.
        /// </summary>
        /// <param name="database">The MongoDB database connection used to access the collection.</param>
        /// <param name="urlBuilder">The builder service used for generating URLs for various resources related to the build records.</param>
        public BuildRepository(IMongoDatabase database, IUrlBuilder urlBuilder)
        {
            _collection = database.GetCollection<BuildRecord>(GetCollectionName(typeof(BuildRecord)));
            _urlBuilder = urlBuilder;
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
        /// Asynchronously creates a new build record based on provided data and generates related resources like URLs.
        /// </summary>
        /// <param name="buildDto">The data transfer object containing build details used to create a new record.</param>
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
                
                var downloadUrl = _urlBuilder.BuildDownloadUrl(shortcode);
                var imageUrl = _urlBuilder.BuildImageUrl(shortcode);
                var schemaUrl = _urlBuilder.BuildSchemaUrl(shortcode);

                var creationResult = new TransactionResult(
                    shortcode: shortcode,
                    downloadUrl: downloadUrl,
                    imageUrl: imageUrl,
                    schemaUrl: schemaUrl,
                    expiresAt: buildRecord.ExpiresAt.ToString("o")  // ISO 8601 format
                );

                return OperationResult<TransactionResult>.Success("Build created successfully", creationResult);
            }
            catch (Exception e)
            {
                return OperationResult.Failure($"Could not create the build record: {e.Message}");
            }
        }

        /// <summary>
        /// Updates an existing build record identified by a shortcode with the provided new data.
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

                var updatedRecord = await _collection.Find(filter).FirstOrDefaultAsync();
                if (updatedRecord is null) return OperationResult.Failure("Failed to retrieve the updated build record.");

                var downloadUrl = _urlBuilder.BuildDownloadUrl(shortcode);
                var imageUrl = _urlBuilder.BuildImageUrl(shortcode);
                var schemaUrl = _urlBuilder.BuildSchemaUrl(shortcode);

                var updateResult = new TransactionResult(
                    shortcode: shortcode,
                    downloadUrl: downloadUrl,
                    imageUrl: imageUrl,
                    schemaUrl: schemaUrl,
                    expiresAt: updatedRecord.ExpiresAt.ToString("o")  // ISO 8601 format
                );
                return OperationResult<TransactionResult>.Success("Build updated successfully", updateResult);
            }
            catch (Exception e)
            {
                return OperationResult.Failure($"Could not update the build record: {e.Message}");
            }
        }

        /// <summary>
        /// Deletes a build record identified by a shortcode.
        /// </summary>
        /// <param name="shortcode">The unique shortcode used to identify the build record to be deleted.</param>
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
        /// Checks if a build record exists in the database identified by a shortcode.
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
        /// Generates a file from a build record identified by a shortcode, used for downloads or data transformations.
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

        /// <summary>
        /// Retrieves build records matching the specified criteria.
        /// </summary>
        /// <param name="value">The value to search for, which can be an archetype, primary, secondary, or a combination separated by commas.</param>
        /// <returns>An operation result containing the list of build records matching the criteria, or a failure result if an error occurs.</returns>
        public async Task<IOperationResult> FetchRecordsByValueAsync(string value)
        {
            try
            {
                var values = value.Split(',', StringSplitOptions.RemoveEmptyEntries);
                if (values.Length > 1)
                {
                    // If more than one parameter is specified, ensure they are in order
                    for (var i = 0; i < values.Length - 1; i++)
                    {
                        if (!IsParameterInOrder(values[i], values[i + 1]))
                        {
                            return OperationResult.Failure("Parameters must be listed in order when multiple parameters are provided.");
                        }
                    }
                }


                var filterBuilder = Builders<BuildRecord>.Filter;
                var filter = filterBuilder.Empty; // Start with an empty filter

                foreach (var val in values)
                {
                    // Dynamically construct the filter to search for records matching any of the provided values
                    filter |= filterBuilder.Where(record => record.Archetype == val ||
                                                            record.Primary == val ||
                                                            record.Secondary == val);
                }

                var result = await _collection.Find(filter).ToListAsync();
                return result.Count == 0 ? OperationResult.Failure("No build records found matching the criteria.") : OperationResult<IEnumerable<BuildRecord>>.Success("Build records retrieved successfully", result);
            }
            catch (Exception e)
            {
                return OperationResult.Failure($"Error retrieving build records: {e.Message}");
            }
        }

        private static bool IsParameterInOrder(string parameter1, string parameter2)
        {
            // Define the valid order of parameters (archetype, primary, secondary)
            var parameterOrder = new List<string> { "Archetype", "Primary", "Secondary" };

            // Get the index of each parameter in the order list
            var index1 = parameterOrder.IndexOf(parameter1);
            var index2 = parameterOrder.IndexOf(parameter2);

            // If the index of parameter2 is greater than or equal to parameter1, they are in order
            return index2 >= index1;
        }
    }
}
