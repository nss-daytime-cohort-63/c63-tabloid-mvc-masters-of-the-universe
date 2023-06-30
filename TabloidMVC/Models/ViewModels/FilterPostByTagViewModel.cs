using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
    public class FilterPostByTagViewModel
    {
        public List<Post> Posts { get; set; }
        public List<Tag> AllTags { get; set; }
    }
}
