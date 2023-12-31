﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System.Collections.Generic;
using System;
using System.Security.Claims;
using TabloidMVC.Models;
using TabloidMVC.Models.ViewModels;
using TabloidMVC.Repositories;
using System.Security.Policy;

namespace TabloidMVC.Controllers
{
    [Authorize]
    public class PostController : Controller
    {
        private readonly IPostRepository _postRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ICommentRepository _commentRepository;
        private readonly ITagRepository _tagRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IReactionRepository _reactionRepository;


        public PostController(IPostRepository postRepository, ICategoryRepository categoryRepository, ISubscriptionRepository subscriptionRepository, ICommentRepository commentRepository, ITagRepository tagRepository, IUserProfileRepository userProfileRepository, IReactionRepository reactionRepository
            )
        {
            _postRepository = postRepository;
            _categoryRepository = categoryRepository;
            _subscriptionRepository = subscriptionRepository;
            _commentRepository = commentRepository;
            _tagRepository = tagRepository;
            _userProfileRepository = userProfileRepository;
            _reactionRepository = reactionRepository;
        }

        public IActionResult Index()
        {
            var posts = _postRepository.GetAllPublishedPosts();
            PostIndexViewModel pivm = new PostIndexViewModel();
            pivm.Posts = posts;
            return View(pivm);
        }

        public IActionResult PostsByTag(int? tagId)
        {
            var tagOptions = _tagRepository.GetAll();
            var posts = (tagId != null)
            ? _postRepository.GetPublishedPostsByTagId(tagId.Value)
            : _postRepository.GetAllPublishedPosts();
            FilterPostByTagViewModel vm = new FilterPostByTagViewModel();
            vm.Posts = posts;
            vm.AllTags = tagOptions;
            return View("FilteredPostsByTag", vm);
        }

        public IActionResult PostsByCategory(int? categoryId)
        {
            var categoryOptions = _categoryRepository.GetAll();
            var posts = (categoryId != null)
                ? _postRepository.GetPublishedPostsByCategoryId(categoryId.Value)
                : _postRepository.GetAllPublishedPosts();
            FilterPostByCategoryViewModel vm = new FilterPostByCategoryViewModel();
            vm.Posts = posts;
            vm.AllCategories = categoryOptions;
            return View("FilterPostsByCategory", vm);
        }

        public IActionResult PostsByAuthor(int? authorId)
        {
            var authorOptions = _userProfileRepository.GetAllUsersOrderedByDisplayName();
            var posts = (authorId != null)
                ? _postRepository.GetPublishedPostsByUserId(authorId.Value)
                : _postRepository.GetAllPublishedPosts();
            FilterPostByAuthorViewModel vm = new FilterPostByAuthorViewModel();
            vm.Posts = posts;
            vm.AllUserProfiles = authorOptions;
            return View("FilterPostsByAuthor", vm );
        }

        //GET
        public IActionResult PostApproval()
        {
            var posts = _postRepository.GetAllPosts();
            return View(posts);
        }

        public IActionResult Details(int id)
        {
            PostDetailsViewModel pdvm = new PostDetailsViewModel();
            pdvm.TagsOnPost = _tagRepository.GetTagsByPostId(id);
            pdvm.Reactions = _reactionRepository.GetAllReactions();
            pdvm.PostReactions = _reactionRepository.GetPostReaction(id);

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
            pdvm.Comments = _commentRepository.GetPostComments(post.Id);
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
                vm.Post.IsApproved = false;
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
            UserProfile user = _userProfileRepository.GetUserProfileById(profileId);
            Post post = _postRepository.GetPostByPostId(id);
            PostEditViewModel pevm = new PostEditViewModel();
            pevm.Post = post;
            pevm.CurrentUser = user;

            if (post == null)
            {
                return NotFound();
            }

            return View(pevm);
        }

        //POST Edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(Post post)
        {
            try
            {
                //post.UserProfileId = GetCurrentUserProfileId();

                _postRepository.UpdatePost(post);
                int profileId = GetCurrentUserProfileId();
                UserProfile user = _userProfileRepository.GetUserProfileById(profileId);

                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("PostApproval");
                }
                else
                {
                    return RedirectToAction("Index");
                }
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



