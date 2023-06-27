using Microsoft.AspNetCore.Mvc;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    public class UserProfileController : Controller
    {
        // Add the UserProfile repository dependency
        private readonly IUserProfileRepository _userProfileRepository;

        public UserProfileController(IUserProfileRepository userProfileRepository)
        {
            _userProfileRepository = userProfileRepository;
        }
        public IActionResult Index()
        {
            var userProfiles = _userProfileRepository.GetAllUsersOrderedByDisplayName();
            return View(userProfiles);
        }

    }
}
