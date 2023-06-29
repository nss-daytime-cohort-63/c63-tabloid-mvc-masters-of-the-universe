using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
    public class PostIndexViewModel
    {
        public List<Post> Posts { get; set; }

        public List<Tag> AllTags { get; set; }

        //public List<Subscription> ActiveSubscriptions {get; set:} This will come in handy when showing posts based on subscriptions
        //must be "filled in" inside the Index Action found in the post controller
    }
}
