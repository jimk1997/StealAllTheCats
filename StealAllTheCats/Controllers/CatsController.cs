using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StealAllTheCats.Data;
using StealAllTheCats.Dto;
using StealAllTheCats.Models;
using StealAllTheCats.Repositories;
using StealAllTheCats.Services;

namespace StealAllTheCats.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatsController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ICatService _catService;

        public CatsController(
            ICatService catService,
            IBackgroundJobClient backgroundJobClient)
        {
            _catService = catService;
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpPost("fetch")]
        public IActionResult FetchCats()
        {
            var jobId = _backgroundJobClient.Enqueue<ICatService>(service => service.FetchAndSaveCatsAsync());
            return Accepted(new { JobId = jobId });
        }

        // GET: api/cats/id
        [HttpGet("{id}")]
        public async Task<ActionResult<CatDto>> GetCat(int id)
        {
            var cat = await _catService.GetCatWithTagsAsync(id);

            if (cat == null)
                return NotFound();

            var catDto = CatEntityToDto(cat);

            return Ok(catDto);
        }

        // GET: api/cats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatDto>>> GetCats(
                        [FromQuery] string? tag = null,
                        [FromQuery] int page = 1,
                        [FromQuery] int pageSize = 10)
        {
            if (page <= 0) 
                page = 1;
            
            if (pageSize <= 0)
                pageSize = 10;

            IQueryable<CatEntity> query = _catService.GetCatsWithTags();

            if (!string.IsNullOrEmpty(tag))
                query = query.Where(c => c.CatTags.Any(ct => ct.Tag.Name == tag));

            var pagedCats = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedCatsDto = pagedCats.Select(c => CatEntityToDto(c)).ToList();

            return Ok(pagedCatsDto);
        }

        [HttpGet("jobs/{id}")]
        public IActionResult GetJobStatus(string id)
        {
            var monitoringApi = JobStorage.Current.GetMonitoringApi();
            var jobDetails = monitoringApi.JobDetails(id);

            if (jobDetails == null)
                return NotFound(new { Status = "NotFound", Message = "Job ID not found" });

            var state = jobDetails.History.FirstOrDefault()?.StateName;

            return Ok(new { JobId = id, Status = state });
        }

        private CatDto CatEntityToDto(CatEntity cat)
        {
            return new CatDto
            {
                Id = cat.Id,
                CatId = cat.CatId,
                Width = cat.Width,
                Height = cat.Height,
                Created = cat.Created,
                Tags = cat.CatTags.Select(ct => ct.Tag.Name).ToList()
            };
        }
    }
}