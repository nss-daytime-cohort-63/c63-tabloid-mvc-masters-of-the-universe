using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
    public class FilterPostByAuthorViewModel
    {
        public List<Post> Posts { get; set; }
        public List<UserProfile> AllUserProfiles { get; set; }
    }
}
