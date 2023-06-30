using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class ReactionRepository : BaseRepository, IReactionRepository
    {
        public ReactionRepository(IConfiguration config) : base(config) { }

        public List<Reaction> GetAllReactions()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT Id, Name, ImageLocation
                        FROM Reaction
                    ";
                    var reader = cmd.ExecuteReader();

                    var reactions = new List<Reaction>();

                    while (reader.Read())
                    {
                        Reaction reaction = new Reaction()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Name = reader.GetString(reader.GetOrdinal("Name")),
                            ImageLocation = reader.GetString(reader.GetOrdinal("ImageLocation")),
                        };

                        reactions.Add(reaction);
                    }

                    reader.Close();

                    return reactions;
                }
            }
        }

        public List<PostReaction> GetPostReaction(int postId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT pr.Id AS PostReactionId, pr.PostId AS PostId, pr.ReactionId AS ReactionId, pr.UserProfileId AS UserProfileId, r.Name AS ReactionName, r.ImageLocation AS ReactionImage
                        FROM PostReaction pr
                        JOIN Reaction r ON pr.ReactionId = r.Id
                        WHERE pr.PostId = @id                                               
                    ";

                    cmd.Parameters.AddWithValue("@id", postId);

                    var reader = cmd.ExecuteReader();

                    var postReactions = new List<PostReaction>();

                    while (reader.Read())
                    {
                        PostReaction postReaction = new PostReaction()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("PostReactionId")),
                            PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                            ReactionId = reader.GetInt32(reader.GetOrdinal("ReactionId")),
                            UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),

                            Reaction = new Reaction
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("ReactionId")),
                                Name = reader.GetString(reader.GetOrdinal("ReactionName")),
                                ImageLocation = reader.GetString(reader.GetOrdinal("ReactionImage"))
                            }
                        };

                        postReactions.Add(postReaction);
                    }

                    reader.Close();

                    return postReactions;
                }
            }

        }

        public void AddReactionToPost(PostReaction postReaction)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())

                {
                    cmd.CommandText = @"
                        INSERT INTO PostReaction ( PostId, ReactionId, UserProfileId )
                        OUTPUT INSERTED.ID
                        VALUES ( @postId, @reactionId, @userProfileId );
                ";

                    cmd.Parameters.AddWithValue("@postId", postReaction.PostId);
                    cmd.Parameters.AddWithValue("@reactionId", postReaction.ReactionId);
                    cmd.Parameters.AddWithValue("@userProfileId", postReaction.UserProfileId);

                    int id = (int)cmd.ExecuteScalar();

                    postReaction.Id = id;
                }
            }
        }

        public void AddReaction(Reaction reaction)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())

                {
                    cmd.CommandText = @"
                        INSERT INTO Reaction (Name, ImageLocation)
                        OUTPUT INSERTED.ID
                        VALUES (@name, @imageLocation)
                    ";

                    cmd.Parameters.AddWithValue("@name", reaction.Name);
                    cmd.Parameters.AddWithValue("@imageLocation", reaction.ImageLocation);

                    int id = (int)cmd.ExecuteScalar();

                    reaction.Id = id;
                }
            }
        }
        
    }
}
