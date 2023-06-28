﻿using Microsoft.AspNetCore.Razor.Language.Intermediate;
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
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Tag (Name)
                        OUTPUT INSERTED.ID
                        VALUES (@name);";

                    cmd.Parameters.AddWithValue("@name", tag.Name);
                    cmd.Parameters.AddWithValue("@id", tag.Id);

                    int id = (int)cmd.ExecuteScalar();

                    tag.Id = id;
                }
            }
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

        public Tag GetTagById(int tagId)
        {
            Tag tag = null;
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select [Name], Id 
                                        From Tag 
                                        where Id = @id";
                    cmd.Parameters.AddWithValue("@id", tagId);

                    using(SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if(reader.Read())
                        {

                        int _id = reader.GetInt32(reader.GetOrdinal("Id"));
                        string _name = reader.GetString(reader.GetOrdinal("Name"));
                        
                        tag = new() { Id = _id, Name = _name };
                        return tag;
                        }

                    }
                }
            }
            return tag;
        }
    }
}
