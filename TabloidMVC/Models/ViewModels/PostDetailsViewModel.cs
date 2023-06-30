using System.Collections.Generic;
namespace TabloidMVC.Models.ViewModels
{
    public class PostDetailsViewModel
    {
        public Post Post { get; set; }
        public Subscription ActiveSubscription { get; set; }
        public List<Comment> Comments { get; set; }
        public List<Tag> TagsOnPost { get; set; }
        public List<Reaction> Reactions { get; set; }
        public List<PostReaction> PostReactions { get; set; }
    }
}
