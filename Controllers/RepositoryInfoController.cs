using Microsoft.AspNetCore.Mvc;
using ecommerce_api.Services;

namespace ecommerce_api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RepositoryInfoController : ControllerBase
    {
        private readonly RepositoryInfoService _repositoryInfoService;

        public RepositoryInfoController(RepositoryInfoService repositoryInfoService)
        {
            _repositoryInfoService = repositoryInfoService;
        }

        [HttpGet("branches")]
        public async Task<ActionResult<IEnumerable<string>>> GetBranches([FromQuery] string owner, [FromQuery] string repo)
        {
            if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(repo))
            {
                return BadRequest("Owner and repo parameters are required.");
            }

            try
            {
                var branches = await _repositoryInfoService.GetBranchesAsync(owner, repo);
                return Ok(branches);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving repository branches.");
            }
        }

        [HttpGet("commits/count")]
        public async Task<ActionResult<int>> GetCommitCount([FromQuery] string owner, [FromQuery] string repo)
        {
            if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(repo))
            {
                return BadRequest("Owner and repo parameters are required.");
            }

            try
            {
                var commitCount = await _repositoryInfoService.GetCommitCountAsync(owner, repo);
                return Ok(commitCount);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving commit count.");
            }
        }

        [HttpGet("collaborators")]
        public async Task<ActionResult<IEnumerable<string>>> GetCollaborators([FromQuery] string owner, [FromQuery] string repo)
        {
            if (string.IsNullOrEmpty(owner) || string.IsNullOrEmpty(repo))
            {
                return BadRequest("Owner and repo parameters are required.");
            }

            try
            {
                var collaborators = await _repositoryInfoService.GetCollaboratorsAsync(owner, repo);
                return Ok(collaborators);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while retrieving repository collaborators.");
            }
        }
    }
}