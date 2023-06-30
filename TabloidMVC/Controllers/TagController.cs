using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    public class TagController : Controller
    {
        private readonly ITagRepository _tagRepository;
        private readonly IPostRepository _postRepository;
        private readonly IPostTagRepository _postTagRepository;

        public TagController(ITagRepository tagRepository, IPostRepository postRepository, IPostTagRepository postTagRepository)
        {

            _tagRepository = tagRepository;
            _postRepository = postRepository;
            _postTagRepository = postTagRepository;
        }


        // GET: TagController
        [Authorize(Roles = "Admin")]
        public ActionResult Index(int id)
        {
            PostTagViewModel ptvm = new PostTagViewModel();

            ptvm.Tags = _tagRepository.GetAll();
            //May not need this line
            ptvm.Post = _postRepository.GetPublishedPostById(id);

            return View(ptvm);
        }

        // GET: TagController/Details/5
        public ActionResult Details(int id)
        {
            var tag = _tagRepository.GetTagById(id);

            if (tag == null)
            {
                return NotFound();
            }
            return View(tag);
        }

        // GET: TagController/Create
        [Authorize(Roles = "Admin")]
        public ActionResult Create()
        {
            

            return View();
        }

        // POST: TagController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Tag tag)
        {
            try
            {
                _tagRepository.AddTag(tag);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(tag);
            }
        }

        // GET: TagController/Edit/5
        [Authorize]
        public ActionResult Edit(int id)
        {
            Tag tag = _tagRepository.GetTagById(id);

            if (tag == null)
            {
                return NotFound();
            }

            return View(tag);
        }

        // POST: TagController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(int id, Tag tag)
        {
            try
            {
                _tagRepository.UpdateTag(tag);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(tag);
            }
        }

        // GET: TagController/Delete/5
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id)
        {
            Tag _tag = _tagRepository.GetTagById(id);
            return View(_tag);
        }

        // POST: TagController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public ActionResult Delete(int id, Tag tag)
        {
            try
            {
                _tagRepository.DeleteTag(tag.Id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        public ActionResult AddToPost(int tagId, int postId)
        {
            try
            {
                PostTag postTag = new PostTag();
                
                postTag.PostId = postId;
                postTag.TagId = tagId;

                _postTagRepository.CreatePostTag(postTag);
                return RedirectToAction("Details", "Post", new {id = postId});
            }
            catch
            {
                return Content("That didn't work for some reason or another.");
            }
        }


        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
