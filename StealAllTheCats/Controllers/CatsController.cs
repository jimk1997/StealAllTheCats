using Hangfire;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StealAllTheCats.Data;
using StealAllTheCats.Dto;
using StealAllTheCats.Models;
using StealAllTheCats.Services;

namespace StealAllTheCats.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CatsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IBackgroundJobClient _backgroundJobClient;

        public CatsController(
            ApplicationDbContext context,
            IBackgroundJobClient backgroundJobClient)
        {
            _context = context;
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpPost("fetch")]
        public IActionResult FetchCats()
        {
            var jobId = _backgroundJobClient.Enqueue<CatFetchService>(service => service.FetchAndSaveCatsAsync());
            return Accepted(new { JobId = jobId });
        }

        // GET: api/cats/id
        [HttpGet("{id}")]
        public async Task<ActionResult<CatEntity>> GetCat(int id)
        {
            var cat = await _context.Cats.FindAsync(id);

            if (cat == null)
                return NotFound();

            return cat;
        }

        // GET: api/cats
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CatEntity>>> GetCats(
                        [FromQuery] string? tag = null,
                        [FromQuery] int page = 1,
                        [FromQuery] int pageSize = 10)
        {
            if (page <= 0) 
                page = 1;
            
            if (pageSize <= 0)
                pageSize = 10;

            IQueryable<CatEntity> query = _context.Cats.Include(c => c.CatTags).ThenInclude(ct => ct.Tag);

            if (!string.IsNullOrEmpty(tag))
                query = query.Where(c => c.CatTags.Any(ct => ct.Tag.Name == tag));

            var pagedCats = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var pagedCatsDto = pagedCats.Select(c => new CatDto
            {
                Id = c.Id,
                CatId = c.CatId,
                Width = c.Width,
                Height = c.Height,
                Created = c.Created,
                Tags = c.CatTags.Select(ct => ct.Tag.Name).ToList()
            }).ToList();

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
    }
}