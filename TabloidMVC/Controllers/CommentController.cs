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
            Comment comment = _commentRepo.GetCommentById(id); 
            if (User.IsInRole("Admin") || comment.UserProfileId == GetCurrentUserProfileId())
            {
                if (!(comment == null))
                {
                    CommentCreateVM ccvm = new()
                    {
                        Comment = comment,
                        PostId = comment.PostId
                    };
                    return View(ccvm);
                }
                else
                {
                    return NotFound();
                }
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
            if (User.IsInRole("Admin") || ccvm.Comment.UserProfileId == GetCurrentUserProfileId())
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
            else
            {
                return NotFound();
            }
        }

        // GET: CommentController/Delete/5
        public ActionResult Delete(int id)
        {
            Comment comment = _commentRepo.GetCommentById(id);
            if (User.IsInRole("Admin") || comment.UserProfileId == GetCurrentUserProfileId())
            {
                return View(comment);
            }
            else
            {
                return NotFound();
            }
        }

        // POST: CommentController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Comment comment)
        {
            Comment _comment = _commentRepo.GetCommentById(id);
            if (User.IsInRole("Admin") || _comment.UserProfileId == GetCurrentUserProfileId())
            {
                try
                {
                    _commentRepo.DeleteComment(comment.Id);
                    return RedirectToAction("Details", "Post", new { id = _comment.PostId });
                }
                catch
                {
                    return View(comment);
                }
            }
            else
            {
                return NotFound();
            }
        }

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
