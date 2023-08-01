using System;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        /// Requests an ID to be used when requesting a short URL.
        /// </summary>
        /// <returns>object</returns>
        /// <response code="200">Generated Snowflake Id</response>
        [AllowAnonymous]
        [HttpGet("requestId")]
        [ProducesResponseType(typeof(Snowflake), 200, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GetId()
        {
            var generatedId = await _repository.GenerateSnowflake();
            var id = generatedId.ToString();
            var snowflake = new Snowflake(id);
            return Ok(snowflake);
        }

        /// <summary>
        /// Submits a build for sharing
        /// </summary>
        /// <param name="submission"></param>
        /// <returns>object</returns>
        /// <response code="200">Success Response</response>
        /// <response code="400">Failure Response</response>
        [AllowAnonymous]
        [HttpPost("submit")] 
        [Consumes("application/json")] 
        [ProducesResponseType(typeof(OperationResult), 200, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> SubmitBuild(Submission submission)
        {
            if (!string.IsNullOrWhiteSpace(submission.Id) && submission.Id.Length < 19)
            {
                return ValidationProblem(new ValidationProblemDetails
                    { Detail = "Invalid payload data" });
            }

            if (string.IsNullOrWhiteSpace(submission.Data))
            {
                return ValidationProblem(new ValidationProblemDetails
                    { Detail = "Invalid payload data" });
            }

            if (string.IsNullOrWhiteSpace(submission.Image))
            {
                return ValidationProblem(new ValidationProblemDetails
                    { Detail = "Invalid payload data" });
            }

            var recordResult = await _repository.CreateRecord(submission.Id, submission.Data, submission.Image);
            if (recordResult.Success)
            {
                return Ok(recordResult);
            }
            return BadRequest(recordResult);
        }

        /// <summary>
        /// Submits an html page update request for the specified build
        /// </summary>
        /// <param name="updateModel"></param>
        /// <returns>object</returns>
        /// <response code="200">Success Response</response>
        /// <response code="400">Failure Response</response>
        [AllowAnonymous]
        [HttpPost("update-page")]
        [Consumes("application/json")] 
        [ProducesResponseType(typeof(OperationResult), 200, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> UpdateBuildPage(UpdateModel updateModel)
        {
            var record = await _repository.RetrieveRecord(updateModel.Code);
            if (record == null)
            {
                return ValidationProblem(new ValidationProblemDetails
                    { Detail = "Invalid payload data" });
            }

            if (string.IsNullOrWhiteSpace(updateModel.PageData))
            {
                return ValidationProblem(new ValidationProblemDetails
                    { Detail = "Invalid payload data" });
            }

            var result = await _repository.UpdatePageData(updateModel.Code, updateModel.PageData);
            if (result.Success)
            {
                return Ok(result);
            }

            return BadRequest(result);
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
        [ProducesResponseType(typeof(DataFile), 200, MediaTypeNames.Application.Json)]
        public async Task<IActionResult> GenerateBuild(string code)
        {
            var record = await _repository.RetrieveRecord(code);
            if (record == null) return BadRequest("Record does not exist for given code.");
            var importData = new DataFile(record.BuildData);
            return Ok(importData);
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
            if (extension != "png") return NotFound();
            var record = await _repository.RetrieveRecord(code);
            if (record?.ImageData == null) return NotFound();
            var imageData = Compression.DecompressFromBase64(record.ImageData);
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
