using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace NewsApp.Models
{
    public class Article
    {
        public int ArticleId { get; set; }
        [Required]
        public string? Title { get; set; }
        [Required]
        public string? Content { get; set; }
        public string? ImageName { get; set; }

        //Using "NotMapped" so that this field is not stored in the database
        //Setting the deisplayName for this field to be "Image"
        //Get and Set for type IFromFile (not required to be an IFormFile) for ImageFile
        [NotMapped]
        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }
        public string? AttributionName { get; set; }
        [Url]
        public string? AttributionUrl { get; set; }
        [Display(Name = "Author")]
        public string? CreatedBy { get; set; }
        [Display(Name = "Published")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //Creating a has one relationship to the Author model
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

    }
}
