using System.ComponentModel.DataAnnotations;
namespace NewsApp.Models
{
    public class Category
    {
        public int CategoryId { get; set; }
        [Required]
        public string? Name { get; set; }

        //Creating a has many relationship to the Book model
        public List<Article>? Articles { get; set; }
    }
}
