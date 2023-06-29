using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface ITagRepository
    {
        List<Tag> GetAll();
        void DeleteTag(int tagId);
        void UpdateTag(Tag tag);
        void AddTag(Tag tag);
        Tag GetTagById(int tagID);

        List<Tag> GetTagsByPostId(int postId);
    }
}