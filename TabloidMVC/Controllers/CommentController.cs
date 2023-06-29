using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    [Authorize]
    public class CommentController : Controller
    {
        private readonly ICommentRepository _commentRepo;

        public CommentController(ICommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }



        // GET: CommentController
        public ActionResult Index()
        {
            return View();
        }

        // GET: CommentController/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: CommentController/Create
        public ActionResult Create(int postId)
        {
            CommentCreateVM ccvm = new()
            {
                PostId = postId,
                Comment = new()
                {
                    PostId = postId
                }
            };
            ccvm.Comment.UserProfileId = GetCurrentUserProfileId();
            return View(ccvm);
        }

        // POST: CommentController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CommentCreateVM ccvm)
        {
            try
            {
                int postId = ccvm.PostId;
                _commentRepo.AddComment(ccvm.Comment);
                return RedirectToAction("Details", "Post", new {id = postId});
            }
            catch
            {
                return View();
            }
        }

        // GET: CommentController/Edit/5
        public ActionResult Edit(int id)
        {
            if(!(_commentRepo.GetCommentById(id) == null))
            {
                Comment _comment = _commentRepo.GetCommentById(id);
                CommentCreateVM ccvm = new()
                {
                    Comment = _comment,
                    PostId = _comment.PostId
                };
                return View(ccvm);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: CommentController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, CommentCreateVM ccvm)
        {
            try
            {
                _commentRepo.EditComment(ccvm.Comment);
                return RedirectToAction("Details", "Post", new {id = ccvm.Comment.PostId});
            }
            catch
            {
                return View(ccvm);
            }
        }

        // GET: CommentController/Delete/5
        public ActionResult Delete(int id)
        {
            Comment _comment = _commentRepo.GetCommentById(id);
            return View(_comment);
        }

        // POST: CommentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Comment comment)
        {
            try
            {
                Comment _comment = _commentRepo.GetCommentById(comment.Id);
                _commentRepo.DeleteComment(comment.Id);
                return RedirectToAction("Details", "Post", new {id = _comment.PostId});
            }
            catch
            {
                return View(comment);
            }
        }
        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
