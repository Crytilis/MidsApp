using System.Collections.Generic;
using System.Net.Mime;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MidsApp.Database;
using MidsApp.Models;
using MidsApp.Utils;

namespace MidsApp.Controllers
{
    /// <summary>
    /// Controller responsible for handling build operations.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BuildController : ControllerBase
    {
        private readonly BuildRepository _repository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BuildController"/>.
        /// </summary>
        /// <param name="repository">Repository for handling build data operations.</param>
        public BuildController(BuildRepository repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Submits a new build record to the database.
        /// </summary>
        /// <param name="buildDto">DTO containing the build data.</param>
        /// <returns>The result of the creation operation.</returns>
        [AllowAnonymous]
        [HttpPost("submit")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(OperationResult<TransactionResult>), 200)]
        [ProducesResponseType(typeof(OperationResult), 400)]
        public async Task<IActionResult> SubmitBuild([FromBody] BuildRecordDto buildDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var result = await _repository.CreateAsync(buildDto);

            if (!result.IsSuccessful)
            {
                return BadRequest(result);
            }

            if (result is IOperationResult<TransactionResult> submissionResult)
            {
                return Ok(submissionResult);
            }

            return BadRequest("Expected data but unable to process");
        }

        /// <summary>
        /// Updates an existing build record identified by its shortcode.
        /// </summary>
        /// <param name="shortcode">The shortcode identifying the build.</param>
        /// <param name="buildDto">DTO containing updated build data.</param>
        /// <returns>The result of the update operation.</returns>
        [AllowAnonymous]
        [HttpPatch("update/{shortcode}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(OperationResult<TransactionResult>), 200)]
        [ProducesResponseType(typeof(OperationResult), 400)]
        public async Task<IActionResult> UpdateBuild(string shortcode, [FromBody] BuildRecordDto buildDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(OperationResult.Failure("Invalid data provided."));
            }

            // Update the build record using the repository method, passing the shortcode and build data.
            var result = await _repository.UpdateByShortcodeAsync(shortcode, buildDto);

            // Determine the response based on the outcome of the update operation.
            if (!result.IsSuccessful)
            {
                return BadRequest(result.Message);
            }

            if (result is IOperationResult<TransactionResult> updateResult)
            {
                return Ok(updateResult);
            }

            return BadRequest("Expected response data but unable to process.");
        }

        /// <summary>
        /// Downloads a build file using a unique code.
        /// </summary>
        /// <param name="code">The unique code to identify the build.</param>
        /// <returns>A file stream containing the build data.</returns>
        [AllowAnonymous]
        [HttpGet("download/{code}")]
        [Produces("application/octet-stream")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(typeof(OperationResult), 404)]
        public async Task<IActionResult> DownloadBuild(string code)
        {
            var result = await _repository.GenerateBuildFileFromRecordAsync(code);
            if (!result.IsSuccessful)
            {
                return NotFound(result.Message);
            }

            if (result is not IOperationResult<FileData> fileResult)
            {
                return BadRequest("Expected file data but was unable to process the request.");
            }

            var fileData = fileResult.Data;
            return File(fileData.DataBytes, "application/octet-stream", fileData.FileName);

        }

        /// <summary>
        /// Retrieves build data for a specific schema identified by a code.
        /// </summary>
        /// <param name="code">The code identifying the build.</param>
        /// <returns>Build data formatted as per schema requirements.</returns>
        [AllowAnonymous]
        [HttpGet("schema/{code}")]
        [Produces(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(SchemaData), 200)]
        [ProducesResponseType(typeof(OperationResult), 404)]
        public async Task<IActionResult> GetBuildDataForSchema(string code)
        {
            var result = await _repository.RetrieveByShortcodeAsync(code);
            if (!result.IsSuccessful)
            {
                NotFound(result.Message);
            }

            if (result is not IOperationResult<BuildRecord> buildResult)
            {
                return BadRequest("Expected to produce data but Unable to process.");
            }

            var schemaData = new SchemaData(buildResult.Data.BuildData);
            return Ok(schemaData);

        }

        /// <summary>
        /// Redirects to a build-specific schema based on a unique code.
        /// </summary>
        /// <param name="code">The unique code used for redirection.</param>
        /// <returns>A redirect result to the schema URL.</returns>
        [AllowAnonymous]
        [HttpGet("redirect-to-schema/{code}")]
        public IActionResult RedirectToSchema(string code)
        {
            var schemaUrl = Url.Action(nameof(GetBuildDataForSchema), new { code });
            if (schemaUrl is null) return NotFound($"Unable to generate a URL for the given code: {code}");
            return new RedirectResult(schemaUrl);
        }

        /// <summary>
        /// Retrieves an image associated with a build, identified by code and file extension.
        /// </summary>
        /// <param name="code">The build code.</param>
        /// <param name="extension">The file extension, currently only 'png' supported.</param>
        /// <returns>The image file associated with the build.</returns>
        [AllowAnonymous]
        [HttpGet("image/{code}.{extension}")]
        [Produces("image/png")]
        [ProducesResponseType(typeof(FileContentResult), 200)]
        [ProducesResponseType(typeof(OperationResult), 404)]
        public async Task<IActionResult> GetDynamicImage(string code, string extension)
        {
            if (extension != "png") return NotFound("The requested resource could not be found.");
            var result = await _repository.RetrieveByShortcodeAsync(code);
            if (!result.IsSuccessful)
            {
                return NotFound("The requested resource could not be found.");
            }

            if (result is not IOperationResult<BuildRecord> buildRecord)
                return BadRequest("Expected file data but was unable to process.");
            var imageData = Compression.DecompressFromBase64(buildRecord.Data.ImageData);
            return File(imageData, "image/png");
        }

        /// <summary>
        /// Retrieves a list of build records based on search criteria.
        /// </summary>
        /// <param name="searchString">The search string representing the criteria for searching build records. Multiple values should be separated by a space.</param>
        /// <returns>Returns a list of build records matching the search criteria.</returns>
        /// <response code="200">Returns the list of build records matching the search criteria.</response>
        /// <response code="400">If an unexpected result type is received from the build repository.</response>
        /// <response code="404">If no build records are found matching the search criteria.</response>
        [AllowAnonymous]
        [HttpGet("list")]
        [Produces("application/json")]
        [Consumes("application/json")]
        public async Task<IActionResult> GetBuilds([FromBody] string searchString)
        {
            var searchResult = await _repository.FetchRecordsByValueAsync(searchString);
            if (!searchResult.IsSuccessful)
            {
                return NotFound(searchResult.Message);
            }

            if (searchResult is not IOperationResult<IEnumerable<BuildRecord>> buildRecords)
            {
                return BadRequest("Unexpected result type received from build repository.");
            }

            return Ok(buildRecords);
        }
     }
}
