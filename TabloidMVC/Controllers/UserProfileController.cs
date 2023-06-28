using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TabloidMVC.Models;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileRepository _userProfileRepo;

        public UserProfileController(IUserProfileRepository userProfileRepo)
        {
            _userProfileRepo = userProfileRepo;
        }

        public IActionResult Index()
        {
            var users = _userProfileRepo.GetAllUsersOrderedByDisplayName();
            return View(users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDeactivation(int id)
        {
            UserProfile userProfile = _userProfileRepo.GetUserById(id);

            if (userProfile == null)
            {
                return NotFound();
            }

            // Deactivate the user profile
            userProfile.IsActive = false;
            _userProfileRepo.Update(userProfile);

            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmActivation(int id)
        {
            UserProfile userProfile = _userProfileRepo.GetUserById(id);

            if (userProfile == null)
            {
                return NotFound();
            }

            // Activate the user profile
            userProfile.IsActive = true;
            _userProfileRepo.Update(userProfile);

            return RedirectToAction("Index");
        }
    }
}

