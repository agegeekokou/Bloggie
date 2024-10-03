using Microsoft.AspNetCore.Mvc;
using Bloggie.Web.Repositories;

namespace Bloggie.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostsRepository blogPostsRepository;

        public BlogsController(IBlogPostsRepository blogPostsRepository) 
        {
            this.blogPostsRepository = blogPostsRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var blogPost = await blogPostsRepository.GetByUrlHandleAsync(urlHandle);

            return View(blogPost);
        }
    }
}
