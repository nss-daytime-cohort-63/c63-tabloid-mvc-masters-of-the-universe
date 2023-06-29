using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ICommentRepository
    {
        public List<Comment> GetPostComments(int postId);
        public void AddComment(Comment comment);
        public void EditComment(Comment comment);
        public void DeleteComment(int commentId);
        public Comment GetCommentById(int commentId);
    }
}
