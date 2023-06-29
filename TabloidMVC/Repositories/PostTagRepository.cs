using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public class PostTagRepository : BaseRepository, IPostTagRepository
    {
        public PostTagRepository(IConfiguration config) : base(config) { }

        public void CreatePostTag(PostTag postTag)
        {
            using (SqlConnection conn = Connection)
            { 
                conn.Open(); 
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO PostTag
                        (PostId, TagId)
                        OUTPUT INSERTED.ID
                        VALUES
                        (@PostId, @TagId)";

                    cmd.Parameters.AddWithValue("@PostId", postTag.PostId);
                    cmd.Parameters.AddWithValue("@TagId", postTag.TagId);

                    postTag.Id = (int)cmd.ExecuteScalar();
                }
            
            }
        }
    }
}
