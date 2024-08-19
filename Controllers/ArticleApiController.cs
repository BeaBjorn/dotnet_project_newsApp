using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsApp.Data;
using NewsApp.Models;

namespace MyApp.Namespace
{
    //Sets the Route for the API endpoint to be api/article
    [Route("api/article")]
    [ApiController]
    public class ArticleApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ArticleApiController(ApplicationDbContext context)
        {
            _context = context;
        }
        //HttpGet to get all articles and belonging category from database
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles(int? categoryId = null)
        {
            //Check is _Context is null
            if (_context.Articles == null)
            {
                return NotFound();
            }
            IQueryable<Article> query = _context.Articles;

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            return Ok(await query.ToListAsync());
        }

        // GET: api/article/5
        //HttpGet to get specific article based on id from database
        [HttpGet("{id}")]
        public async Task<ActionResult> GetArticle(int id)
        {
            //Check is _Context is null
            if (_context.Articles == null)
            {
                return NotFound();
            }
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return Ok(article);
        }
    }
}
