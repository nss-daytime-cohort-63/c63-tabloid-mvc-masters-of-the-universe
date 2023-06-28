using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public class TagRepository : BaseRepository, ITagRepository
    {


        public TagRepository(IConfiguration config) : base(config) { }
        

        public List<Tag> GetAll()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT id, name
                                        FROM Tag
                                        ORDER BY name";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Tag> tags = new List<Tag>();

                        while (reader.Read())
                        {
                            Tag tag = new Tag
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Name = reader.GetString(reader.GetOrdinal("Name"))
                            };

                            tags.Add(tag);
                        }

                        return tags;
                    }
                    
                }
            }
        }
        public void AddTag(Tag tag)
        {
            throw new System.NotImplementedException();
        }

        public void DeleteTag(int tagId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Delete from Tag
                                        where Id = @tagId";

                    cmd.Parameters.AddWithValue("@tagId", tagId);

                    cmd.ExecuteNonQuery();
                }
            }
        }


        public void UpdateTag(Tag tag)
        {
            throw new System.NotImplementedException();
        }
    }
}
