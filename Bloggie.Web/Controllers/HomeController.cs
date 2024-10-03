using Bloggie.Web.Models;
using Bloggie.Web.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace Bloggie.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> logger;
        private readonly IBlogPostsRepository blogPostsRepository;

        public HomeController(ILogger<HomeController> logger, IBlogPostsRepository blogPostsRepository)
        {
            this.logger = logger;
            this.blogPostsRepository = blogPostsRepository;
        }

        public async Task<IActionResult> Index()
        {
            var blogPosts = await blogPostsRepository.GetAllAsync();

            return View(blogPosts);
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
    }
}
