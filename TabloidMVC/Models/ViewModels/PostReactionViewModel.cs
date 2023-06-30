using System.Collections.Generic;

namespace TabloidMVC.Models.ViewModels
{
    public class PostReactionViewModel
    {
        public List<Reaction> Reactions { get; set; }
        public PostReaction PostReaction { get; set; }
    }
}
