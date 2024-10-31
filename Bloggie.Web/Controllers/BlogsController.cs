using Microsoft.AspNetCore.Mvc;
using Bloggie.Web.Repositories;
using Bloggie.Web.Models.ViewModels;
using Microsoft.AspNetCore.Components.Web;
using Bloggie.Web.Models.Domain;
using Microsoft.AspNetCore.Identity;

namespace Bloggie.Web.Controllers
{
    public class BlogsController : Controller
    {
        private readonly IBlogPostsRepository blogPostsRepository;
        private readonly IBlogPostLikeRepository blogPostLikeRepository;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IBlogPostCommentRepository blogPostCommentRepository;

        public BlogsController(IBlogPostsRepository blogPostsRepository,
            IBlogPostLikeRepository blogPostLikeRepository,
            SignInManager<IdentityUser> signInManager,
            UserManager<IdentityUser> userManager,
            IBlogPostCommentRepository blogPostCommentRepository) 
        {
            this.blogPostsRepository = blogPostsRepository;
            this.blogPostLikeRepository = blogPostLikeRepository;
            this.signInManager = signInManager;
            this.userManager = userManager;
            this.blogPostCommentRepository = blogPostCommentRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Index(string urlHandle)
        {
            var blogPost = await blogPostsRepository.GetByUrlHandleAsync(urlHandle);
            var blogDetailsViewModel = new BlogDetailsViewModel();
            var liked = false;

            if (blogPost != null)
            {
                var totalLikes = await blogPostLikeRepository.GetTotalLikes(blogPost.Id);

                if (signInManager.IsSignedIn(User))
                {
                    // Get the like for this blog for this user
                    var likesForBlog = await blogPostLikeRepository.GetLikesForBlog(blogPost.Id);

                    var userId = userManager.GetUserId(User);

                    if(!string.IsNullOrEmpty(userId)) 
                    {
                        var likeFromUser = likesForBlog.FirstOrDefault(x => x.UserId == Guid.Parse(userId));
                        liked = likeFromUser != null;
                    }
                }

                // Get comments for Blog post
                var blogCommentsDomainModel = await blogPostCommentRepository.GetCommentsByBlogPostIdAsync(blogPost.Id);

                var blogCommentsForView = new List<BlogComment>();

                foreach (var blogComment in blogCommentsDomainModel)
                {
                    var userName = "Anonymous";  // Default value if UserId is null or user lookup fails

                    if (blogComment.UserId != null)
                    {
                        var user = await userManager.FindByIdAsync(blogComment.UserId.ToString());
                        userName = user?.UserName ?? "Unknown";  // If user lookup fails, set to "Unknown"
                    }

                    blogCommentsForView.Add(new BlogComment()
                    {
                        Description = blogComment.Description,
                        DateAdded = blogComment.DateAdded,
                        UserName = userName
                    });
                }


                blogDetailsViewModel = new BlogDetailsViewModel()
                {
                    Id = blogPost.Id,
                    Heading = blogPost.Heading,
                    PageTitle = blogPost.PageTitle,
                    Content = blogPost.Content,
                    ShortDescription = blogPost.ShortDescription,
                    FeaturedImageUrl = blogPost.FeaturedImageUrl,
                    UrlHandle = blogPost.UrlHandle,
                    PublishedDate = blogPost.PublishedDate,
                    Author = blogPost.Author,
                    Visible = blogPost.Visible,
                    Tags = blogPost.Tags,
                    TotalLikes = totalLikes,
                    Liked = liked,  
                    Comments = blogCommentsForView
                };
            }

            return View(blogDetailsViewModel);
        }


        [HttpPost]
        public async Task<IActionResult> Index(BlogDetailsViewModel blogDetailsViewModel)
        {
            if(signInManager.IsSignedIn(User))
            {
                var userId = userManager.GetUserId(User);

                if (!string.IsNullOrEmpty(userId))
                {
                    var domainModel = new BlogPostComment()
                    {
                        BlogPostId = blogDetailsViewModel.Id,
                        Description = blogDetailsViewModel.CommentDescription,
                        UserId = Guid.Parse(userId),
                        DateAdded = DateTime.Now
                    };

                    await blogPostCommentRepository.AddAsync(domainModel);

                    return RedirectToAction("Index", "Blogs", new { urlHandle = blogDetailsViewModel.UrlHandle });
                }
                else
                {
                    // Handle the case where userId is null 
                    return RedirectToAction("Index", "Blogs");
                }
            }

            return View();        
        }
    }
}
