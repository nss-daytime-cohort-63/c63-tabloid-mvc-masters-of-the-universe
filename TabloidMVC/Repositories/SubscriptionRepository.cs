using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public class SubscriptionRepository : BaseRepository, ISubscriptionRepository
    {
        public SubscriptionRepository(IConfiguration config) : base(config) { }

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
    }
}
