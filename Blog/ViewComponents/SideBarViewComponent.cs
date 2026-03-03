using Blog.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myBlog.Data;

namespace Blog.ViewComponents
{
    public class SideBarViewComponent : ViewComponent
    {
        private readonly BlogDbContext _context;
        public SideBarViewComponent(BlogDbContext context) => _context = context;

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var categories = await _context.Categories
                .Where(c => c.Posts.Any()) // 只顯示有文章的分類
                .Select(c => new CategoryDto
                {
                    Id = c.Id,
                    Name = c.Name,
                    Slug = c.Slug, // 記得這裡要有 Slug，不然 View 做不出連結
                    PostCount = c.Posts.Count() // EF Core 會自動幫你算數量
                })
                .ToListAsync();
            return View(categories);
        }
    }
}
