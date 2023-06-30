using System.Collections.Generic;
using TabloidMVC.Models;

namespace TabloidMVC.Repositories
{
    public interface IUserProfileRepository
    {
        UserProfile GetByEmail(string email);
        void AddUserProfile(UserProfile user);
        List<UserProfile> GetAllUsersOrderedByDisplayName();
        UserProfile GetUserProfileById(int id);
        void UpdateUserProfile(UserProfile user);
        void Update(UserProfile userProfile);

        List<UserType> GetAllUserTypes();

    }
}