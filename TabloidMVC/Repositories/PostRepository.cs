﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection.PortableExecutable;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class PostRepository : BaseRepository, IPostRepository
    {
        public PostRepository(IConfiguration config) : base(config) { }
        public List<Post> GetAllPublishedPosts()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT p.Id, p.Title, p.Content, 
                              p.ImageLocation AS HeaderImage,
                              p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                              p.CategoryId, p.UserProfileId,
                              c.[Name] AS CategoryName,
                              u.FirstName, u.LastName, u.DisplayName, 
                              u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                              u.UserTypeId, 
                              ut.[Name] AS UserTypeName
                         FROM Post p
                              LEFT JOIN Category c ON p.CategoryId = c.id
                              LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE IsApproved = 1 AND PublishDateTime < SYSDATETIME()
                        ORDER BY PublishDateTime DESC";
                    var reader = cmd.ExecuteReader();

                    var posts = new List<Post>();

                    while (reader.Read())
                    {
                        posts.Add(NewPostFromReader(reader));
                    }

                    reader.Close();

                    return posts;
                }
            }
        }
        public List<Post> GetPublishedPostsByTagId(int tagId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
               SELECT p.Id, p.Title, p.Content, 
                      p.ImageLocation AS HeaderImage,
                      p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                      p.CategoryId, p.UserProfileId,
                      c.[Name] AS CategoryName,
                      u.FirstName, u.LastName, u.DisplayName, 
                      u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                      u.UserTypeId, 
                      ut.[Name] AS UserTypeName,
                      t.Id AS TagId, t.Name AS TagName
                 FROM Post p
                      LEFT JOIN Category c ON p.CategoryId = c.id
                      LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                      LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                      LEFT JOIN PostTag pt ON pt.PostId = p.id
                      LEFT JOIN Tag t ON pt.TagId = t.id
                WHERE IsApproved = 1 AND PublishDateTime < SYSDATETIME()
                      AND pt.TagId = @tagId
                ORDER BY PublishDateTime DESC";

                    cmd.Parameters.AddWithValue("@tagId", tagId);

                    var reader = cmd.ExecuteReader();

                    var posts = new List<Post>();

                    while (reader.Read())
                    {
                        var postId = reader.GetInt32(reader.GetOrdinal("Id"));
                        var post = posts.FirstOrDefault(p => p.Id == postId);
                        if (post == null)
                        {
                            post = NewPostFromReader(reader);
                            posts.Add(post);
                        }

                        var tagIdFromReader = reader.GetInt32(reader.GetOrdinal("TagId"));
                        var tagNameFromReader = reader.GetString(reader.GetOrdinal("TagName"));
                        if (tagIdFromReader > 0 && !string.IsNullOrEmpty(tagNameFromReader))
                        {
                            if (post.Tags == null)
                            {
                                post.Tags = new List<Tag>();
                            }
                            post.Tags.Add(new Tag { Id = tagIdFromReader, Name = tagNameFromReader });
                        }

                    }

                    reader.Close();

                    return posts;
                }
            }
        }

        public List<Post> GetPublishedPostsByCategoryId(int categoryId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT p.Id, p.Title, p.Content, 
                    p.ImageLocation AS HeaderImage,
                    p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                    p.CategoryId, p.UserProfileId,
                    c.[Name] AS CategoryName,
                    u.FirstName, u.LastName, u.DisplayName, 
                    u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                    u.UserTypeId
                FROM Post p
                    LEFT JOIN Category c ON p.CategoryId = c.id
                    LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                WHERE IsApproved = 1 AND PublishDateTime < SYSDATETIME()
                    AND p.CategoryId = @categoryId
                ORDER BY PublishDateTime DESC";

                    cmd.Parameters.AddWithValue("@categoryId", categoryId);

                    var reader = cmd.ExecuteReader();

                    var posts = new List<Post>();

                    while (reader.Read())
                    {
                        var postId = reader.GetInt32(reader.GetOrdinal("Id"));
                        var post = posts.FirstOrDefault(p => p.Id == postId);
                        if (post == null)
                        {
                            post = new Post
                            {
                                Id = postId,
                                Title = reader.GetString(reader.GetOrdinal("Title")),
                                Content = reader.GetString(reader.GetOrdinal("Content")),
                                ImageLocation = reader.IsDBNull(reader.GetOrdinal("HeaderImage")) ? null : reader.GetString(reader.GetOrdinal("HeaderImage")),
                                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                                PublishDateTime = reader.IsDBNull(reader.GetOrdinal("PublishDateTime")) ? null : reader.GetDateTime(reader.GetOrdinal("PublishDateTime")),
                                IsApproved = reader.GetBoolean(reader.GetOrdinal("IsApproved")),
                                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                Category = new Category
                                {
                                    Id = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                                    Name = reader.GetString(reader.GetOrdinal("CategoryName"))
                                },
                                UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                                UserProfile = new UserProfile
                                {
                                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                    Email = reader.GetString(reader.GetOrdinal("Email")),
                                    CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                                    ImageLocation = reader.GetString(reader.GetOrdinal("AvatarImage")),
                                    UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId"))
                                }
                            };
                            posts.Add(post);
                        }
                    }

                    reader.Close();

                    return posts;
                }
            }
        }


        public List<Post> GetPublishedPostsByUserId(int userId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                SELECT p.Id, p.Title, p.Content, 
                       p.ImageLocation AS HeaderImage,
                       p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                       p.CategoryId, p.UserProfileId,
                       c.[Name] AS CategoryName,
                       u.FirstName, u.LastName, u.DisplayName, 
                       u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                       u.UserTypeId, 
                       ut.[Name] AS UserTypeName
                FROM Post p
                    LEFT JOIN Category c ON p.CategoryId = c.id
                    LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                    LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                WHERE IsApproved = 1 AND PublishDateTime < SYSDATETIME()
                    AND p.UserProfileId = @userId
                ORDER BY PublishDateTime DESC";

                    cmd.Parameters.AddWithValue("@userId", userId);

                    var reader = cmd.ExecuteReader();

                    var posts = new List<Post>();

                    while (reader.Read())
                    {
                        posts.Add(NewPostFromReader(reader));
                    }

                    reader.Close();

                    return posts;
                }
            }
        }


        public List<Post> GetPostsByUserId(int userId)
        {
            List<Post> userPosts = new();
            using (SqlConnection conn = Connection) 
            {
                conn.Open();
                {
                    using(SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = @"
                       SELECT p.Id, p.Title, p.Content, 
                              p.ImageLocation AS HeaderImage,
                              p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                              p.CategoryId, p.UserProfileId,
                              c.[Name] AS CategoryName,
                              u.FirstName, u.LastName, u.DisplayName, 
                              u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                              u.UserTypeId, 
                              ut.[Name] AS UserTypeName
                         FROM Post p
                              LEFT JOIN Category c ON p.CategoryId = c.id
                              LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE p.UserProfileId = @userId";

                        cmd.Parameters.AddWithValue("@userId", userId);

                        using(SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                Post _userPost = new Post();
                                _userPost.Title = reader.GetString(reader.GetOrdinal("Title"));
                                _userPost.Content = reader.GetString(reader.GetOrdinal("Content"));
                                _userPost.IsApproved = reader.GetBoolean(reader.GetOrdinal("IsApproved"));
                                _userPost.Id = reader.GetInt32(reader.GetOrdinal("Id"));
                                _userPost.CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime"));
                                _userPost.PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PublishDateTime"));
                                if(!reader.IsDBNull(reader.GetOrdinal("HeaderImage")))
                                {
                                    _userPost.ImageLocation = reader.GetString(reader.GetOrdinal("HeaderImage"));
                                }
                                _userPost.UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId"));
                                _userPost.CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId"));

                                Category _postCat = new();
                                _postCat.Name = reader.GetString(reader.GetOrdinal("CategoryName"));
                                _postCat.Id = reader.GetInt32(reader.GetOrdinal("CategoryId"));

                                _userPost.Category = _postCat;

                                UserProfile _postUser = new();
                                _postUser.FirstName = reader.GetString(reader.GetOrdinal("FirstName"));
                                _postUser.LastName = reader.GetString(reader.GetOrdinal("LastName"));
                                _postUser.DisplayName = reader.GetString(reader.GetOrdinal("DisplayName"));
                                _postUser.Email = reader.GetString(reader.GetOrdinal("Email"));
                                if(!reader.IsDBNull(reader.GetOrdinal("AvatarImage")))
                                {
                                    _postUser.ImageLocation = reader.GetString(reader.GetOrdinal("AvatarImage"));
                                }

                                _postUser.UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId"));

                                UserType _postUserType = new();
                                _postUserType.Name = reader.GetString(reader.GetOrdinal("UserTypeName"));
                                _postUserType.Id = _postUser.UserTypeId;

                                _postUser.UserType = _postUserType;

                                _userPost.UserProfile = _postUser;

                                userPosts.Add(_userPost);
                            }
                        }
                    }
                }
            }
            return userPosts;
        }

        public Post GetPublishedPostById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT p.Id, p.Title, p.Content, 
                              p.ImageLocation AS HeaderImage,
                              p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                              p.CategoryId, p.UserProfileId,
                              c.[Name] AS CategoryName,
                              u.FirstName, u.LastName, u.DisplayName, 
                              u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                              u.UserTypeId, 
                              ut.[Name] AS UserTypeName
                         FROM Post p
                              LEFT JOIN Category c ON p.CategoryId = c.id
                              LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE IsApproved = 1 AND PublishDateTime < SYSDATETIME()
                              AND p.id = @id";

                    cmd.Parameters.AddWithValue("@id", id);
                    var reader = cmd.ExecuteReader();

                    Post post = null;

                    if (reader.Read())
                    {
                        post = NewPostFromReader(reader);
                    }

                    reader.Close();

                    return post;
                }
            }
        }

        public Post GetPostByPostId(int postId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using(var cmd =  conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT p.Id, p.Title, p.Content, 
                              p.ImageLocation AS HeaderImage,
                              p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                              p.CategoryId, p.UserProfileId,
                              c.[Name] AS CategoryName,
                              u.FirstName, u.LastName, u.DisplayName, 
                              u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                              u.UserTypeId, 
                              ut.[Name] AS UserTypeName
                         FROM Post p
                              LEFT JOIN Category c ON p.CategoryId = c.id
                              LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE p.id = @id";

                    cmd.Parameters.AddWithValue("@id", postId);
                    var reader = cmd.ExecuteReader();
                    Post post = null;
                    if (reader.Read())
                    {
                        post = NewPostFromReader(reader);
                    }

                    reader.Close();
                    return post;
                }
            }
        }

        public Post GetUserPostById(int id, int userProfileId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                       SELECT p.Id, p.Title, p.Content, 
                              p.ImageLocation AS HeaderImage,
                              p.CreateDateTime, p.PublishDateTime, p.IsApproved,
                              p.CategoryId, p.UserProfileId,
                              c.[Name] AS CategoryName,
                              u.FirstName, u.LastName, u.DisplayName, 
                              u.Email, u.CreateDateTime, u.ImageLocation AS AvatarImage,
                              u.UserTypeId, 
                              ut.[Name] AS UserTypeName
                         FROM Post p
                              LEFT JOIN Category c ON p.CategoryId = c.id
                              LEFT JOIN UserProfile u ON p.UserProfileId = u.id
                              LEFT JOIN UserType ut ON u.UserTypeId = ut.id
                        WHERE p.id = @id AND p.UserProfileId = @userProfileId";

                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.Parameters.AddWithValue("@userProfileId", userProfileId);
                    var reader = cmd.ExecuteReader();

                    Post post = null;

                    if (reader.Read())
                    {
                        post = NewPostFromReader(reader);
                    }

                    reader.Close();

                    return post;
                }
            }
        }


        public void Add(Post post)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        INSERT INTO Post (
                            Title, Content, ImageLocation, CreateDateTime, PublishDateTime,
                            IsApproved, CategoryId, UserProfileId )
                        OUTPUT INSERTED.ID
                        VALUES (
                            @Title, @Content, @ImageLocation, @CreateDateTime, @PublishDateTime,
                            @IsApproved, @CategoryId, @UserProfileId )";
                    cmd.Parameters.AddWithValue("@Title", post.Title);
                    cmd.Parameters.AddWithValue("@Content", post.Content);
                    cmd.Parameters.AddWithValue("@ImageLocation", DbUtils.ValueOrDBNull(post.ImageLocation));
                    cmd.Parameters.AddWithValue("@CreateDateTime", post.CreateDateTime);
                    cmd.Parameters.AddWithValue("@PublishDateTime", DbUtils.ValueOrDBNull(post.PublishDateTime));
                    cmd.Parameters.AddWithValue("@IsApproved", post.IsApproved);
                    cmd.Parameters.AddWithValue("@CategoryId", post.CategoryId);
                    cmd.Parameters.AddWithValue("@UserProfileId", post.UserProfileId);

                    post.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        private Post NewPostFromReader(SqlDataReader reader)
        {
            return new Post()
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                Title = reader.GetString(reader.GetOrdinal("Title")),
                Content = reader.GetString(reader.GetOrdinal("Content")),
                ImageLocation = DbUtils.GetNullableString(reader, "HeaderImage"),
                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                PublishDateTime = DbUtils.GetNullableDateTime(reader, "PublishDateTime"),
                CategoryId = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                IsApproved = reader.GetBoolean(reader.GetOrdinal("IsApproved")),
                Category = new Category()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("CategoryId")),
                    Name = reader.GetString(reader.GetOrdinal("CategoryName"))
                },
                UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                UserProfile = new UserProfile()
                {
                    Id = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                    FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                    LastName = reader.GetString(reader.GetOrdinal("LastName")),
                    DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                    Email = reader.GetString(reader.GetOrdinal("Email")),
                    CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                    ImageLocation = DbUtils.GetNullableString(reader, "AvatarImage"),
                    UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                    UserType = new UserType()
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                        Name = reader.GetString(reader.GetOrdinal("UserTypeName"))
                    }
                }
            };
        }

        public void UpdatePost(Post post)
        {
            using (var conn = Connection)
            {
                conn.Open();

                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                    UPDATE Post
                    SET
                        Title = @title,
                        Content = @content,
                        ImageLocation = @imageLocation,
                        CreateDateTime = @createDateTime,
                        PublishDateTime = @publishDateTime,
                        IsApproved = @isApproved,
                        CategoryId = @categoryId,
                        UserProfileId = @userProfileId
                    WHERE Id = @Id";

                    cmd.Parameters.AddWithValue("@title", post.Title);
                    cmd.Parameters.AddWithValue("@content", post.Content);
                    cmd.Parameters.AddWithValue("@imageLocation", post.ImageLocation);
                    cmd.Parameters.AddWithValue("@createDateTime", post.CreateDateTime);
                    cmd.Parameters.AddWithValue("@publishDateTime", post.PublishDateTime);
                    cmd.Parameters.AddWithValue("@isApproved", post.IsApproved);
                    cmd.Parameters.AddWithValue("@categoryId", post.CategoryId);
                    cmd.Parameters.AddWithValue("@userProfileId", post.UserProfileId);
                    cmd.Parameters.AddWithValue("@Id", post.Id);

                    cmd.ExecuteNonQuery();


                }
            }
        }

        public void DeletePost(int postId)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        DELETE FROM Post
                        WHERE Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", postId);

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public List<Post> GetAllPosts()
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using(SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT p.Id, p.Title, p.Content, p.ImageLocation AS Image, p.CreateDateTime, 
                               p.PublishDateTime, p.IsApproved, p.UserProfileId, 
                               c.[Name] AS Category
                        FROM Post p
                        JOIN Category c ON p.CategoryId = c.Id;";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        List<Post> posts = new List<Post>();

                        while (reader.Read())
                        {
                            Post post = new Post
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? null : reader.GetString(reader.GetOrdinal("Title")),
                                Content = reader.IsDBNull(reader.GetOrdinal("Content")) ? null : reader.GetString(reader.GetOrdinal("Content")),
                                ImageLocation = reader.IsDBNull(reader.GetOrdinal("Image")) ? null : reader.GetString(reader.GetOrdinal("Image")),
                                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                                PublishDateTime = reader.GetDateTime(reader.GetOrdinal("PublishDateTime")),
                                IsApproved = reader.GetBoolean(reader.GetOrdinal("IsApproved")),
                                UserProfileId = reader.GetInt32(reader.GetOrdinal("UserProfileId")),
                                Category =  reader.IsDBNull(reader.GetOrdinal("Category")) ?  null : new Category {  Name = reader.GetString(reader.GetOrdinal("Category")) }


                            };

                            posts.Add(post);
                        }

                        return posts;
                    }
                }
            }
        }
    }
}