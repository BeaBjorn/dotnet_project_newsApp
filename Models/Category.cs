//dataannotations for validation
using System.ComponentModel.DataAnnotations;

//Namespace
namespace NewsApp.Models
{
    //public class for Category
    public class Category
    {
        //Get and Set of type integer as Id
        public int CategoryId { get; set; }

        //Get and Set of type string as for Name - required field
        [Required]
        public string? Name { get; set; }

        //Creating a has many relationship to the Article model
        public List<Article>? Articles { get; set; }
    }
}
