using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Collections.Generic;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    [Authorize]
    public class ReactionController : Controller
    {
            private readonly IReactionRepository _reactionRepository;
        public ReactionController(IReactionRepository reactionRepository)
        {
            _reactionRepository = reactionRepository;
        }
        
        // GET: Reaction
        public ActionResult Index()
        {
            List<Reaction> reactions = _reactionRepository.GetAllReactions();
            return View(reactions);
        }

        // GET: Reaction/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET
        public ActionResult AddReactionToPost(int postId)
        {
            PostReaction postReaction = new PostReaction();
            postReaction.PostId = postId;
            postReaction.UserProfileId = GetCurrentUserProfileId();
            List<Reaction> reactions = _reactionRepository.GetAllReactions();
            PostReactionViewModel prvm = new PostReactionViewModel();
            prvm.PostReaction = postReaction;
            prvm.Reactions = reactions;
            

            return View(prvm);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddReactionToPost(PostReactionViewModel prvm)
        {
            try
            {
                _reactionRepository.AddReactionToPost(prvm.PostReaction);
                return RedirectToAction("Details", "Post", new {id=prvm.PostReaction.PostId});
            }
            catch
            {
                return View(prvm);
            }
        }

        // GET: Reaction/Create - create a new reaction
        public ActionResult Create()
        {
            return View();
        }

        // POST: Reaction/Create
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

        // GET: Reaction/Edit/5
        public ActionResult Edit(int id)
        {
            return View();
        }

        // POST: Reaction/Edit/5
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

        // GET: Reaction/Delete/5
        public ActionResult Delete(int id)
        {
            return View();
        }

        // POST: Reaction/Delete/5
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
