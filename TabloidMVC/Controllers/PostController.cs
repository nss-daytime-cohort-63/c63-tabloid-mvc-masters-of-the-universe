using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;



namespace TabloidMVC.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;

        public PostController(IPostRepository postRepository, ICategoryRepository categoryRepository, ISubscriptionRepository subscriptionRepository)
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _subscriptionRepository = subscriptionRepository;
        }

        public IActionResult Index()
        {
            var posts = _postRepository.GetAllPublishedPosts();
            return View(posts);
        }

        public IActionResult PostsByTag(int tagId)
        {
            var posts = _postRepository.GetPublishedPostsByTagId(tagId);
            return View("FilteredPosts", posts);
        }


        public IActionResult Details(int id)
        {
            PostDetailsViewModel pdvm = new PostDetailsViewModel();

            var post = _postRepository.GetPublishedPostById(id);
            int userId = GetCurrentUserProfileId();

            if (post == null)
            {
                post = _postRepository.GetUserPostById(id, userId);
                if (post == null)
                {
                    return NotFound();
                }
            }
            pdvm.Post = post;
            pdvm.ActiveSubscription = _subscriptionRepository.GetActiveSubByAuthAndSubscriber(post.UserProfileId, userId);
            //pass a view model with the post and any active subscriptions
            return View(pdvm);
        }

        public IActionResult Create()
        {
            var vm = new PostCreateViewModel();
            vm.CategoryOptions = _categoryRepository.GetAll();
            return View(vm);
        }

        [HttpPost]
        public IActionResult Create(PostCreateViewModel vm)
        {
            try
            {
                vm.Post.CreateDateTime = DateAndTime.Now;
                vm.Post.IsApproved = true;
                vm.Post.UserProfileId = GetCurrentUserProfileId();

                _postRepository.Add(vm.Post);

                return RedirectToAction("Details", new { id = vm.Post.Id });
            }
            catch
            {
                vm.CategoryOptions = _categoryRepository.GetAll();
                return View(vm);
            }
        }
        [Authorize]
        public IActionResult MyPosts()
        {
            int currentUserId = GetCurrentUserProfileId();
            List<Post> myPosts = _postRepository.GetPostsByUserId(currentUserId);

            return View(myPosts);
        }

        // GET: PostController/Delete
        [Authorize]
        public ActionResult Delete(int id)
        {
            int userId = GetCurrentUserProfileId();
            Post post = _postRepository.GetPublishedPostById(id);

            if (post == null || post.UserProfileId != userId)
            {
                return NotFound();
            }

            return View(post);
        }

        // POST: PostController/Delete
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Post post)
        {
            try
            {
                _postRepository.DeletePost(id);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
                return View(post);
            }
        }



        //GET Edit
        [Authorize]
        public ActionResult Edit(int id)
        {
            int profileId = GetCurrentUserProfileId();

            Post post = _postRepository.GetUserPostById(id, profileId);

            if (post == null)
            {
                return NotFound();
            }

            return View(post);
        }

        //POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Post post)
        {
            try
            {
                //post.UserProfileId = GetCurrentUserProfileId();

                _postRepository.UpdatePost(post);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                return View(post);
            }
        }

        [Authorize]
        public ActionResult CreateSubscription(int providerId)
        {
            try
            {
                // Create a new Subscription object
                Subscription sub = new Subscription();

                // Set the properties of the Subscription object
                sub.ProviderUserProfileId = providerId;
                sub.SubscriberUserProfileId = GetCurrentUserProfileId();
                sub.BeginDateTime = DateTime.Now;

                // Create the subscription in the database
                _subscriptionRepository.Add(sub);

                // Redirect the user to the home page or any other desired action
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                // Handle errors, if any
                return Content("Error occurred while creating the subscription.");
            }
        }

        //write a DeleteSubscription Action like the above
        [Authorize]
        public ActionResult DeleteSubscription(int subToEndId)
        {
            try
            {

                _subscriptionRepository.AddEndDate(subToEndId);
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                // Handle errors, if any
                return Content("Error occurred while ending the subscription.");
            }
            
        }

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}



