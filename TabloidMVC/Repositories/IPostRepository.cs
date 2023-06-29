using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IPostRepository
    {
        void Add(Post post);
        List<Post> GetAllPublishedPosts();
        List<Post> GetPublishedPostsByTagId(int tagId);
        Post GetPublishedPostById(int id);
        Post GetUserPostById(int id, int userProfileId);
        List<Post> GetPostsByUserId(int userId);
        void DeletePost(int postId);
        void UpdatePost(Post post);
        
    }
}