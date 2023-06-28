using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using TabloidMVC.Models;
using TabloidMVC.Utils;

namespace TabloidMVC.Repositories
{
    public class UserProfileRepository : BaseRepository, IUserProfileRepository
    {
        public UserProfileRepository(IConfiguration config) : base(config) { }

        public UserProfile GetByEmail(string email)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT u.Id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                                               u.CreateDateTime, u.ImageLocation, u.UserTypeId, u.IsActive,
                                               ut.[Name] AS UserTypeName
                                        FROM UserProfile u
                                        LEFT JOIN UserType ut ON u.UserTypeId = ut.Id
                                        WHERE email = @email";


                    UserProfile userProfile = null;
                    cmd.Parameters.AddWithValue("email", email);
                    var reader = cmd.ExecuteReader();


                    if (reader.Read())
                    {
                        userProfile = new UserProfile()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                            ImageLocation = DbUtils.GetNullableString(reader, "ImageLocation"),
                            UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            UserType = new UserType()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("UserTypeName"))
                            },
                        };
                    }

                    reader.Close();

                    return userProfile;
                }
            }
        }

        public void AddUserProfile(UserProfile user)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        INSERT INTO UserProfile
                                        (DisplayName, FirstName, LastName, Email, CreateDateTime, ImageLocation, UserTypeId, IsActive)
                                        OUTPUT INSERTED.ID
                                        VALUES (@displayName, @firstName, @lastName, @email,
                                                @createDateTime, @imageLocation, @userTypeId, @isActive)";
                    
                    cmd.Parameters.AddWithValue("@isActive", user.IsActive);
                    cmd.Parameters.AddWithValue("@displayName", user.DisplayName);
                    cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", user.LastName);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@userTypeId", user.UserTypeId);
                    cmd.Parameters.AddWithValue("@createDateTime", DateTime.Now);
                    if (user.ImageLocation != null)
                    {
                        cmd.Parameters.AddWithValue("@imageLocation", user.ImageLocation);
                    }
                    else
                    {
                        cmd.Parameters.AddWithValue("@imageLocation", DBNull.Value);
                    }
                    int id = (int)cmd.ExecuteScalar();
                    user.Id = id;
                }
            }
        }

        public List<UserProfile> GetAllUsersOrderedByDisplayName()
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT u.Id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                                               u.CreateDateTime, u.ImageLocation, u.UserTypeId, u.IsActive,
                                               ut.[Name] AS UserTypeName
                                        FROM UserProfile u
                                        LEFT JOIN UserType ut ON u.UserTypeId = ut.Id
                                        ORDER BY u.DisplayName ASC";

                    var reader = cmd.ExecuteReader();

                    var users = new List<UserProfile>();

                    while (reader.Read())
                    {
                        var userProfile = new UserProfile()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                            ImageLocation = DbUtils.GetNullableString(reader, "ImageLocation"),
                            UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            UserType = new UserType()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("UserTypeName"))
                            },
                        };

                        users.Add(userProfile);
                    }

                    reader.Close();

                    return users;
                }
            }
        }


        public UserProfile GetUserProfileById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        SELECT u.Id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                               u.CreateDateTime, u.ImageLocation, u.UserTypeId,
                               ut.[Name] AS UserTypeName
                        FROM UserProfile u
                        LEFT JOIN UserType ut ON u.UserTypeId = ut.Id
                        WHERE u.Id = @id
                        ";

                    cmd.Parameters.AddWithValue("@id", id);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            UserProfile userProfile = new UserProfile
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                                LastName = reader.GetString(reader.GetOrdinal("LastName")),
                                DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                                Email = reader.GetString(reader.GetOrdinal("Email")),
                                CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                                // ImageLocation = reader.GetString(reader.GetOrdinal("ImageLocation")),
                                UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                UserType = new UserType
                                {
                                    Name = reader.GetString(reader.GetOrdinal("UserTypeName")),
                                    Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                }
                            };

                            if (!reader.IsDBNull(reader.GetOrdinal("ImageLocation")))
                            {
                                userProfile.ImageLocation = reader.GetString(reader.GetOrdinal("ImageLocation"));
                            }
                            else
                            {
                                userProfile.ImageLocation = "https://cdn.pixabay.com/photo/2015/10/05/22/37/blank-profile-picture-973460_1280.png";
                            }

                            return userProfile;
                        }
                        else
                        {
                            return null;
                        }
                    }


                }


            }
        }

        public UserProfile GetUserById(int id)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                                        SELECT u.Id, u.FirstName, u.LastName, u.DisplayName, u.Email,
                                               u.CreateDateTime, u.ImageLocation, u.UserTypeId, u.IsActive,
                                               ut.[Name] AS UserTypeName
                                        FROM UserProfile u
                                        LEFT JOIN UserType ut ON u.UserTypeId = ut.Id
                                        WHERE u.Id = @id";


                    cmd.Parameters.AddWithValue("@id", id);

                    var reader = cmd.ExecuteReader();

                    UserProfile userProfile = null;

                    if (reader.Read())
                    {
                        userProfile = new UserProfile()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            Email = reader.GetString(reader.GetOrdinal("Email")),
                            FirstName = reader.GetString(reader.GetOrdinal("FirstName")),
                            LastName = reader.GetString(reader.GetOrdinal("LastName")),
                            DisplayName = reader.GetString(reader.GetOrdinal("DisplayName")),
                            CreateDateTime = reader.GetDateTime(reader.GetOrdinal("CreateDateTime")),
                            ImageLocation = DbUtils.GetNullableString(reader, "ImageLocation"),
                            UserTypeId = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                            IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive")),
                            UserType = new UserType()
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("UserTypeId")),
                                Name = reader.GetString(reader.GetOrdinal("UserTypeName"))
                            },
                        };
                    }

                    reader.Close();

                    return userProfile;
                }
            }
        }
        public void UpdateUserProfile(UserProfile user)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"UPDATE UserProfile
                                SET DisplayName = @displayName,
                                    FirstName = @firstName,
                                    LastName = @lastName,
                                    Email = @email,
                                    UserTypeId = @userTypeId
                                WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@id", user.Id);
                    cmd.Parameters.AddWithValue("@displayName", user.DisplayName);
                    cmd.Parameters.AddWithValue("@firstName", user.FirstName);
                    cmd.Parameters.AddWithValue("@lastName", user.LastName);
                    cmd.Parameters.AddWithValue("@email", user.Email);
                    cmd.Parameters.AddWithValue("@userTypeId", user.UserTypeId);

                    cmd.ExecuteNonQuery();
                }
            }
        }
        public void Update(UserProfile userProfile)
        {
            using (var conn = Connection)
            {
                conn.Open();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"
                        UPDATE UserProfile
                        SET IsActive = @isActive
                        WHERE Id = @id";

                    cmd.Parameters.AddWithValue("@isActive", userProfile.IsActive);
                    cmd.Parameters.AddWithValue("@id", userProfile.Id);

                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
