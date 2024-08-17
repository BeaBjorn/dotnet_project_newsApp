using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NewsApp.Data;
using NewsApp.Models;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using System.IO;

namespace NewsApp.Controllers
{
    [Authorize]
    public class ArticleController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly string wwwRootPath;

        public ArticleController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            wwwRootPath = hostEnvironment.WebRootPath;
        }

        // GET: Article
        public async Task<IActionResult> Index()
        {
            if (_context.Articles == null)
            {
                return NotFound();
            }
            var applicationDbContext = _context.Articles.Include(a => a.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Article/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Check is _Context is null
            if (_context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Article/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "Name");
            return View();
        }

        // POST: Article/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,Title,Content,ImageFile,AttributionName,AttributionUrl,CreatedDate,CategoryId")] Article article)
        {
            if (ModelState.IsValid)
            {
                //check if image has been set
                if (article.ImageFile != null)
                {
                    //Generate unique file name for the image by combining the file name, a timestamp and the file type
                    string fileName = Path.GetFileNameWithoutExtension(article.ImageFile.FileName);
                    string extension = Path.GetExtension(article.ImageFile.FileName);

                    //Ensures there are no spaces and adds timestamp to file name
                    article.ImageName = fileName = fileName.Replace(" ", string.Empty) + DateTime.Now.ToString("yymmssfff") + extension;

                    //Setting the path to where the image is to be stored
                    string path = Path.Combine(wwwRootPath + "/images", fileName);

                    // Store image in filesystem
                    using (var image = Image.Load(article.ImageFile.OpenReadStream()))
                    {
                        // Resize the image to 1000px width, maintaining aspect ratio
                        image.Mutate(x => x.Resize(1000, 0));

                        // Crop the image to 1000x600px from the center
                        image.Mutate(x => x.Crop(new Rectangle((image.Width - 1000) / 2, (image.Height - 600) / 2, 1000, 600)));

                        // Save the processed image
                        await image.SaveAsync(path);
                    }
                }

                article.CreatedDate = DateTime.Now;
                //add logged in user as author to article
                article.CreatedBy = User.Identity?.Name ?? "Unknown";
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "Name", article.CategoryId);
            return View(article);
        }

        // GET: Article/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

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
            ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "Name", article.CategoryId);
            return View(article);
        }

        // POST: Article/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,Title,Content,ImageFile,AttributionName,AttributionUrl,CreatedDate,CategoryId")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            //Check is _Context is null
            if (_context.Articles == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing article from the database
                    var existingArticle = await _context.Articles.FindAsync(id);
                    if (existingArticle == null)
                    {
                        return NotFound();
                    }

                    // Update the properties
                    existingArticle.Title = article.Title;
                    existingArticle.Content = article.Content;
                    existingArticle.AttributionName = article.AttributionName;
                    existingArticle.AttributionUrl = article.AttributionUrl;
                    existingArticle.CreatedDate = article.CreatedDate;
                    existingArticle.CategoryId = article.CategoryId;

                    // Handle image file
                    if (article.ImageFile != null)
                    {
                        // Generate a unique file name for the new image
                        string fileName = Path.GetFileNameWithoutExtension(article.ImageFile.FileName);
                        string extension = Path.GetExtension(article.ImageFile.FileName);
                        string newFileName = fileName.Replace(" ", string.Empty) + DateTime.Now.ToString("yymmssfff") + extension;
                        string path = Path.Combine(wwwRootPath + "/images", newFileName);

                        // Save the new image
                        using (var image = Image.Load(article.ImageFile.OpenReadStream()))
                        {
                            image.Mutate(x => x.Resize(1000, 0));
                            image.Mutate(x => x.Crop(new Rectangle((image.Width - 1000) / 2, (image.Height - 600) / 2, 1000, 600)));
                            await image.SaveAsync(path);
                        }

                        // Delete the old image if it exists
                        if (!string.IsNullOrEmpty(existingArticle.ImageName))
                        {
                            string oldImagePath = Path.Combine(wwwRootPath + "/images", existingArticle.ImageName);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Update the image name in the database
                        existingArticle.ImageName = newFileName;
                    }
                    else
                    {
                        // If no new image is uploaded, keep the current image name
                        _context.Entry(existingArticle).Property(x => x.ImageName).IsModified = false;
                    }

                    // Update the article
                    _context.Update(existingArticle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "Name", article.CategoryId);
            return View(article);
        }





        // GET: Article/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Check is _Context is null
            if (_context.Articles == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Article/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            //Check is _Context is null
            if (_context.Articles == null)
            {
                return NotFound();
            }
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            //Check is _Context is null
            if (_context.Articles == null)
            {
                return false;
            }
            return _context.Articles.Any(e => e.ArticleId == id);
        }
    }
}
