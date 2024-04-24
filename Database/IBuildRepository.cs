using System.Threading.Tasks;
using MidsApp.Models;

namespace MidsApp.Database;

/// <summary>
/// Provides an interface for repository operations on build records.
/// </summary>
public interface IBuildRepository
{
    /// <summary>
    /// Creates a new build record asynchronously.
    /// </summary>
    /// <param name="buildDto">The data transfer object containing build details.</param>
    /// <returns>The operation result indicating success or failure.</returns>
    Task<IOperationResult> CreateAsync(BuildRecordDto buildDto);

    /// <summary>
    /// Updates an existing build record identified by a shortcode.
    /// </summary>
    /// <param name="shortcode">The shortcode that identifies the build record.</param>
    /// <param name="buildDto">The updated build data.</param>
    /// <returns>The operation result indicating success or failure.</returns>
    Task<IOperationResult> UpdateByShortcodeAsync(string shortcode, BuildRecordDto buildDto);

    /// <summary>
    /// Deletes a build record identified by a shortcode.
    /// </summary>
    /// <param name="shortcode">The shortcode that identifies the build record.</param>
    /// <returns>The operation result indicating success or failure.</returns>
    Task<IOperationResult> DeleteByShortcodeAsync(string shortcode);

    /// <summary>
    /// Retrieves a build record by shortcode.
    /// </summary>
    /// <param name="shortcode">The shortcode that identifies the build record.</param>
    /// <returns>The operation result indicating success or failure.</returns>
    Task<IOperationResult> RetrieveByShortcodeAsync(string shortcode);

    /// <summary>
    /// Looks up a build record by shortcode to verify its existence.
    /// </summary>
    /// <param name="shortcode">The shortcode used to find the build record.</param>
    /// <returns>The operation result indicating success or failure.</returns>
    Task<IOperationResult> LookupRecordByShortcodeAsync(string shortcode);

    /// <summary>
    /// Generates a file from a build record identified by shortcode.
    /// </summary>
    /// <param name="shortcode">The shortcode that identifies the build record.</param>
    /// <returns>The operation result including the file data on success.</returns>
    Task<IOperationResult> GenerateBuildFileFromRecordAsync(string shortcode);

}