using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IReactionRepository
    {
        public List<Reaction> GetAllReactions();
        public List<PostReaction> GetPostReaction(int postId);
        public void AddReactionToPost(PostReaction postReaction);
        public void AddReaction(Reaction reaction);
    }
}
