using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsApp.Data;

namespace MyApp.Namespace
{
    [Route("api/article")]
    [ApiController]
    public class ArticleApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArticleApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetArticles()
        {
            if (_context.Articles == null)
            {
                return NotFound();
            }

            return Ok(await _context.Articles.ToListAsync());
        }
    }
}
