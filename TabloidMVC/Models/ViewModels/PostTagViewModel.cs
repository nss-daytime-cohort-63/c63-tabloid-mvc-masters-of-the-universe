using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
    public class PostTagViewModel
    {
        public List<Tag> Tags { get; set; }
        public Post Post { get; set; }

    }
}
