//dataannotations for validation
using System.ComponentModel.DataAnnotations;
//dataannotations for mapping to database
using System.ComponentModel.DataAnnotations.Schema;

//Namespace
namespace NewsApp.Models
{
    //public class for Article
    public class Article
    {
        //Get and Set of type integer as Id
        public int ArticleId { get; set; }

        //Get and Set of type string as for Title - required field
        [Required]
        public string? Title { get; set; }

        //Get and Set of type string as for Content - required field
        [Required]
        public string? Content { get; set; }

        //Get and Set of type string as for ImageName
        public string? ImageName { get; set; }

        //Using "NotMapped" to prevent data from being stored in database
        //Setting deisplayName to be "Image" instead of "ImageFile"
        //Get and Set of type IFromFile for ImageFile
        [NotMapped]
        [Display(Name = "Image")]
        public IFormFile? ImageFile { get; set; }

        //Setting deisplayName to be "Attribution name" instead of "AttributionName"
        //Get and Set of type string for AtttributionName
        [Display(Name = "Attribution name")]
        public string? AttributionName { get; set; }

        //Setting deisplayName to be "Attribution url" instead of "AttributionUrl"
        //åURL¨ is used to ensure a valid URL has been entered 
        //Get and Set of type string for AtttributionUrl
        [Display(Name = "Attribution url")]
        [Url]
        public string? AttributionUrl { get; set; }

        //Setting deisplayName to be "Created by" instead of "CreatedBy"
        //Get and Set of type string for CreatedBy
        [Display(Name = "Created by")]
        public string? CreatedBy { get; set; }

        //Setting deisplayName to be "Published" instead of "CreatedDate"
        //Get and Set of type string for CreatedDate and setting this to rodays date and time by default
        [Display(Name = "Published")]
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        //Creating a has one relationship to the Category model
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

    }
}
