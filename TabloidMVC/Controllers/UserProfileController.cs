using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TabloidMVC.Models;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserProfileController : Controller
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserProfileRepository _userProfileRepo;

        public UserProfileController(IUserProfileRepository userProfileRepo)
        {
            _userProfileRepository = userProfileRepo;
            _userProfileRepo = userProfileRepo;
        }

        // GET: UserProfileController
        public ActionResult Index()
        {
            var userProfiles = _userProfileRepository.GetAllUsersOrderedByDisplayName();
            return View(userProfiles);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ConfirmDeactivation(int id)
        {
            UserProfile userProfile = _userProfileRepo.GetUserProfileById(id);

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
            UserProfile userProfile = _userProfileRepo.GetUserProfileById(id);

            if (userProfile == null)
            {
                return NotFound();
            }

            // Activate the user profile
            userProfile.IsActive = true;
            _userProfileRepo.Update(userProfile);

            return RedirectToAction("Index");
        }

        // GET: UserProfileController/Details/5
        [Authorize]
        public ActionResult Details(int id)
        {
            var userProfile = _userProfileRepository.GetUserProfileById(id);
            if (userProfile == null)
            {
                return NotFound();
            }
            return View(userProfile);
        }

        // GET: UserProfileController/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: UserProfileController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserProfileController/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: UserProfileController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        // GET: UserProfileController/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: UserProfileController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, IFormCollection collection)
        {
            try
            {
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }
    }
}
