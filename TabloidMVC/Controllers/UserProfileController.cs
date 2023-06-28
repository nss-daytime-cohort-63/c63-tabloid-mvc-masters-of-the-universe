using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using TabloidMVC.Models;
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

        // GET: UserProfileController
        [Authorize(Roles = "Admin")]
        public ActionResult Index()
        {
            var userProfiles = _userProfileRepository.GetAllUsersOrderedByDisplayName();
            return View(userProfiles);        
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
        [Authorize]
        public ActionResult Edit(int id)
        {
            int profileId = GetCurrentUserProfileId();

            UserProfile userProfile = _userProfileRepository.GetUserProfileById(id);

            if (userProfile == null || userProfile.Id != profileId)
            {
                return NotFound();
            }

            return View(userProfile);
        }

        // POST: UserProfileController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, UserProfile userProfile)
        {
            try
            {
                _userProfileRepository.UpdateUserProfile(userProfile);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(userProfile);
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

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
