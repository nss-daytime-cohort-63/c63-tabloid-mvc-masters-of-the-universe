using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
    public class FilterPostByCategoryViewModel
    {
        public List<Post> Posts { get; set; }
        public List<Category> AllCategories { get; set; }
    }
}
