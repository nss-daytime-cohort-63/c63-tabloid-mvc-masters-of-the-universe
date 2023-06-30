using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TabloidMVC.Models;
using TabloidMVC.Repositories;

namespace TabloidMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly IPostRepository _postRepository;

        public HomeController(ILogger<HomeController> logger, ISubscriptionRepository subscriptionRepository, IPostRepository postRepository)
        {
            _logger = logger;
            _subscriptionRepository = subscriptionRepository;
            _postRepository = postRepository;
        }

        public IActionResult Index()
        {
            List<Post> posts = new List<Post>();
            //list of subscriptions from subscriber id based on current user Id
            List<Subscription> subscriptions = _subscriptionRepository.GetActiveSubscriptionsBySubscriberId(GetCurrentUserProfileId());

            //convert subscriptions to list of author Ids
            List<int> authorIds = subscriptions.Select(s => s.ProviderUserProfileId).Distinct().ToList();

            //iterate throught list of author Id's and search (published?) posts by authorId
            //add all posts to list
            foreach (int author in authorIds)
            {
                List<Post> authorsPosts = _postRepository.GetPublishedPostsByUserId(author);
                posts.AddRange(authorsPosts);
            }

            return View(posts);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private int GetCurrentUserProfileId()
        {
            string id = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(id);
        }
    }
}
