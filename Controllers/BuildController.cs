using System;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MidsApp.Database;
using MidsApp.Models;
using MidsApp.Utils;

namespace MidsApp.Controllers
{
    /// <inheritdoc />
    [ApiController]
    [Route("[controller]")]
    public class BuildController : ControllerBase
    {
        private readonly BuildRepository<BuildRecord> _repository;

        /// <inheritdoc />
        public BuildController(BuildRepository<BuildRecord> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Submits a new build record to the database.
        /// </summary>
        /// <param name="buildDto">The build data transfer object containing information necessary to create a new build record.</param>
        /// <returns>An IActionResult that contains the result of the submission operation.</returns>
        /// <remarks>
        /// This endpoint is anonymous and accepts JSON data. It checks the model state for validation errors before proceeding
        /// to create a new build record. Depending on the success of the operation, it returns either an OK (200) response with the operation result
        /// or a BadRequest (400) with error details.
        /// </remarks>
        [AllowAnonymous]
        [HttpPost("submit")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(OperationResult<string>), 200)]
        [ProducesResponseType(typeof(OperationResult<string>), 400)]
        public async Task<IActionResult> SubmitBuild([FromBody] BuildRecordDto buildDto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var recordResult = await _repository.CreateAsync(buildDto);

            if (recordResult.IsSuccessful)
            {
                return Ok(recordResult);
            }

            return BadRequest(recordResult);
        }

        /// <summary>
        /// Updates a build record by its shortcode.
        /// </summary>
        /// <param name="shortcode">The shortcode of the build record to update.</param>
        /// <param name="buildDto">The DTO containing the build data to update.</param>
        /// <returns>An <see cref="IActionResult"/> encapsulating the success or failure of the update operation.</returns>
        [AllowAnonymous]
        [HttpPatch("update/{shortcode}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(typeof(OperationResult<string>), 200)]
        [ProducesResponseType(typeof(OperationResult<string>), 400)]
        public async Task<IActionResult> UpdateBuildPage(string shortcode, [FromBody] BuildRecordDto buildDto)
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

            if (result is IOperationResult<string> updateResult)
            {
                return Ok(updateResult.Data);
            }

            return BadRequest("Expected response data but unable to process.");
        }

        [AllowAnonymous]
        [HttpGet("download/{code}")]
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
        /// Retrieves a build from the database matching the provided short URL code.
        /// </summary>
        /// <param name="code"></param>
        /// <returns>JSON</returns>
        /// <response code="200">Success Response</response>
        /// <response code="400">Failure Response</response>
        [AllowAnonymous]
        [HttpGet("{code}")]
        [ProducesResponseType(typeof(SchemaData), 200, MediaTypeNames.Application.Json)]
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
        /// Requests a schema redirect for a Build 
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Redirect to schema</returns>
        [AllowAnonymous]
        [HttpGet("request/{code}")]
        public IActionResult RequestBuild(string code)
        {
            var url = $"mrb://{code}";
            return new RedirectResult(url, true);
        }

        /// <summary>
        /// Retrieves a build image from the database matching the specified id and extension.
        /// </summary>
        /// <param name="code"></param>
        /// <param name="extension"></param>
        /// <returns>Image</returns>
        [AllowAnonymous]
        [HttpGet("image/{code}.{extension}")]
        public async Task<IActionResult> GetDynamicImage(string code, string extension)
        {
            if (extension != "png") return NotFound("The requested resource could not be found.");
            var result = await _repository.RetrieveByShortcodeAsync(code);
            if (!result.IsSuccessful)
            {
                return NotFound("The requested resource could not be found.");
            }

            if (result is not IOperationResult<BuildRecord> buildRecord)
                return BadRequest("Expect file data but was unable to process.");
            var imageData = Compression.DecompressFromBase64(buildRecord.Data.ImageData);
            return File(imageData, "image/png");

        }

        /// <summary>
        /// Displays a build to a user
        /// </summary>
        /// <param name="code"></param>
        /// <param name="extension"></param>
        /// <returns>Page</returns>
        [AllowAnonymous]
        [HttpGet("preview/{code}.{extension}")]
        public async Task<IActionResult> ViewBuild(string code, string extension)
        {
            var record = await _repository.RetrieveRecord(code);
            if (record == null || extension != "htm") return NotFound();
            var pageData = Compression.DecompressFromBase64(record.PageData);
            var html = Encoding.UTF8.GetString(pageData);
            Console.WriteLine(html);
            return base.Content(html, "text/html");
        }
    }
}
