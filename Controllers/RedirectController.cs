using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MidsApp.Models;

namespace MidsApp.Controllers
{
    /// <inheritdoc />
    [Route("[controller]")]
    [ApiController]
    public class RedirectController : ControllerBase
    {
        private readonly BuildRepository<BuildRecord> _repository;

        /// <inheritdoc />
        public RedirectController(BuildRepository<BuildRecord> repository)
        {
            _repository = repository;
        }

        /// <summary>
        /// Requests a schema redirect for a Build 
        /// </summary>
        /// <param name="code"></param>
        /// <returns>Redirect to schema</returns>
        [AllowAnonymous]
        [HttpGet("{code}")]
        public IActionResult RequestBuild(string code)
        {
            var url = $"mrb://{code}";
            return new RedirectResult(url, true);

            //return RedirectPermanent($"mrb://{code}.mbd");
        }
    }
}
