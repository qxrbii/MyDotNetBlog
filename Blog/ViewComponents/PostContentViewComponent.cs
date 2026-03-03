using Blog.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myBlog.Data;

namespace Blog.ViewComponents
{
    public class PostContentViewComponent : ViewComponent
    {
        private readonly BlogDbContext _context; // 注入資料庫
        public PostContentViewComponent(BlogDbContext context) => _context = context;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var posts = await _context.Posts.Select(p => new PostListDto
            {
                Id = p.Id,
                Title = p.Title,
                Slug = p.Slug,
                CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                PublishDate = p.PublishDate,
                ViewCount = p.ViewCount,
                Author = p.Author ?? string.Empty,
                Excerpt = p.Excerpt
            }).ToListAsync();
            return View(posts);
        }
    }
}
