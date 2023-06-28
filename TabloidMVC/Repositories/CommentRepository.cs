using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public class CommentRepository : BaseRepository, ICommentRepository
    {
        public CommentRepository(IConfiguration config) : base(config)
        {

        }
        public void AddComment(Comment comment)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();

                using(SqlCommand cmd =  conn.CreateCommand())
                {
                    cmd.CommandText = @"Insert Into Comment(Subject, Content, CreateDateTime, PostId, UserProfileId)
                                        values (@subject, @content, @createDateTime, @postId, @userProfileId)";

                    cmd.Parameters.AddWithValue("@subject", comment.Subject);
                    cmd.Parameters.AddWithValue("@content", comment.Content);
                    cmd.Parameters.AddWithValue("@createDateTime", comment.CreateDateTime);
                    cmd.Parameters.AddWithValue("@postId", comment.PostId);
                    cmd.Parameters.AddWithValue("@userProfileId", comment.UserProfileId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void DeleteComment(int commentId)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"delete from Comment where Id = @commentId";
                    cmd.Parameters.AddWithValue("@commentId", commentId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void EditComment(Comment comment)
        {
            using(SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"update Comment
                                            set Subject = @subject,
                                                Content = @content
                                            where Id = @commentId";
                    cmd.Parameters.AddWithValue("@subject", comment.Subject);
                    cmd.Parameters.AddWithValue("@content", comment.Content);
                    cmd.Parameters.AddWithValue("@commentId", comment.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Comment> GetPostComments(int postId)
        {
            List<Comment> _postComments = new();

            using(SqlConnection conn = Connection)
            {
                conn.Open();

                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"Select co.Id, co.Subject, co.Content, co.CreateDateTime, co.PostId, co.UserProfileId, u.DisplayName, u.ImageLocation
                                        from Comment co
                                        left join UserProfile u on
                                        u.Id = co.UserProfileId
                                        where co.PostId = @postId";
                    cmd.Parameters.AddWithValue("@postId", postId);

                    using(SqlDataReader reader  = cmd.ExecuteReader())
                    {
                        while(reader.Read())
                        {
                            string ImageLocation;
                            if (!reader.IsDBNull(reader.GetOrdinal("ImageLocation")))
                            {
                                ImageLocation = reader.GetString(reader.GetOrdinal("ImageLocation"));
                            }
                            else
                            {
                                ImageLocation = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png";
                            }
                            Comment comment = new Comment()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Subject = reader.GetString(reader.GetOrdinal("Subject")),
                                Content = reader.GetString(reader.GetOrdinal("Content")),
                                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                                PostId = reader.GetInt32(reader.GetOrdinal("PostId")),
                                UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                                UserProfile = new UserProfile()
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                                    ImageLocation = ImageLocation,
                                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName"))
                                }
                            };

                            _postComments.Add(comment);
                        }
                    }
                }
            }

            return _postComments;
        }
    }
}
