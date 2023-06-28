using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public class SubscriptionRepository : BaseRepository, ISubscriptionRepository
    {
        public SubscriptionRepository(IConfiguration config) : base(config) { }


        //need lookup by subscriberId and authorId
        //will return list of ACTIVE subscriptions
        public Subscription GetActiveSubByAuthAndSubscriber(int authorId, int subscriberId)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"SELECT Id, SubscriberUserProfileId, ProviderUserProfileId,	BeginDateTime,	EndDateTime
                                        FROM Subscription
                                        WHERE SubscriberUserProfileId = @subId
                                        AND ProviderUserProfileId = @authorId
                                        AND EndDateTime IS NULL;";
                    cmd.Parameters.AddWithValue("@subId", subscriberId);
                    cmd.Parameters.AddWithValue("@authorId", authorId);

                    SqlDataReader reader = cmd.ExecuteReader();
                    if (reader.Read())
                    {
                        Subscription sub = new Subscription()
                        {
                            Id = reader.GetInt32(reader.GetOrdinal("Id")),
                            SubscriberUserProfileId = reader.GetInt32(reader.GetOrdinal("SubscriberUserProfileId")),
                            ProviderUserProfileId = reader.GetInt32(reader.GetOrdinal("ProviderUserProfileId")),
                            BeginDateTime = reader.GetDateTime(reader.GetOrdinal("BeginDateTime")),
                            EndDateTime = null
                        };
                    return sub;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public void Add(Subscription subscription)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"INSERT INTO Subscription
                                        (SubscriberUserProfileId, ProviderUserProfileId, BeginDateTime, EndDateTime)
                                        OUTPUT INSERTED.ID
                                        VALUES (@subscriber, @provider, @beginDate, NULL)";
                    cmd.Parameters.AddWithValue("@subscriber", subscription.SubscriberUserProfileId);
                    cmd.Parameters.AddWithValue("@provider", subscription.ProviderUserProfileId);
                    cmd.Parameters.AddWithValue("@beginDate", subscription.BeginDateTime);
                    subscription.Id = (int)cmd.ExecuteScalar();
                }
            }
        }

        public void Delete(int Id)
        {
            using (SqlConnection conn = Connection)
            {
                conn.Open();
                using (SqlCommand cmd = conn.CreateCommand())
                {
                    cmd.CommandText = @"DELETE Subscription
                                        WHERE Id = @id";
                    cmd.Parameters.AddWithValue("@id", Id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
