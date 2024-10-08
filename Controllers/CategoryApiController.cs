using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NewsApp.Data;

namespace MyApp.Namespace
{
    //Sets the Route for the API endpoint to be api/category
    [Route("api/category")]
    [ApiController]
    public class CategoryApiController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CategoryApiController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetCategories()
        {
            if (_context.categories == null)
            {
                return NotFound();
            }

            return Ok(await _context.categories.ToListAsync());
        }

        // GET: api/category/5
        [HttpGet("{id}")]
        public async Task<ActionResult> GetCategory(int id)
        {
            //Check is _Context is null
            if (_context.categories == null)
            {
                return NotFound();
            }
            var category = await _context.categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return Ok(category);
        }
    }
}
