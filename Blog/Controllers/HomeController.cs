using Blog.Dtos;
using Blog.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using myBlog.Data;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Blog.Controllers
{
    public class HomeController : Controller
    {
        private readonly BlogDbContext _context;

        public HomeController(BlogDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(string category, int page = 1)
        {
            int pageSize = 10;  // 每頁顯示的文章數量

            var query = _context.Posts.Include(p => p.Category).AsQueryable();  // 包含分類資料

            if (!string.IsNullOrEmpty(category))    // 如果有提供分類參數，則過濾文章
            {
                query = query.Where(p => p.Category!.Slug == category);     // 過濾指定分類的文章
            }

            var totalPosts = await query.CountAsync();  // 計算總文章數量
            var totalPages = (int)Math.Ceiling(totalPosts / (double)pageSize);  // 計算總頁數

            var posts = await query
                   .OrderByDescending(p => p.PublishDate)
                   .Skip((page - 1) * pageSize)
                   .Take(pageSize)
                   .AsNoTracking()
                   .Select(p => new PostListDto
                   {
                       Title = p.Title,
                       CategoryName = p.Category != null ? p.Category.Name : "未分類",
                       PublishDate = p.PublishDate,
                       Author = p.Author ?? "匿名",
                       Excerpt = p.Excerpt
                   })
                   .ToListAsync();

            ViewBag.CurrentPage = page;             // 當前頁碼
            ViewBag.TotalPages = totalPages;        // 總頁數
            ViewBag.CurrentCategory = category;     // 當前分類

            // 取得所有分類供右側導覽使用
            ViewBag.Categories = await _context.Categories
                .Include(c => c.Posts)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(posts);
        }

        public async Task<IActionResult> Posts(string slug)
        {
            var post = await _context.Posts
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Slug == slug);  // 根據 slug 查找文章

            if (post == null)
            {
                return NotFound();
            }

            post.ViewCount++;                       // 增加瀏覽次數
            await _context.SaveChangesAsync();      // 儲存變更

            // 取得所有分類供右側導覽使用
            ViewBag.Categories = await _context.Categories
                .Include(c => c.Posts)
                .OrderBy(c => c.Name)
                .ToListAsync();

            return View(post);
        }

        //文章列表頁
        public async Task<IActionResult> Posts(int? seriesId)
        {
            var query = _context.Posts.Include(p => p.Category).AsQueryable();    // 包含系列資料

            if (seriesId.HasValue)  // 如果有提供系列 ID，則過濾文章
            {
                query = query.Where(p => p.CategoryId == seriesId.Value);  // 過濾指定系列的文章
            }

            var posts = await query
                 .OrderByDescending(p => p.PublishDate)
                 .Select(p => new PostListDto
                 {
                     Id = p.Id,
                     Title = p.Title,
                     CategoryName = p.Category != null ? p.Category.Name : "未分類",
                     PublishDate = p.PublishDate,
                     Author = p.Author ?? "匿名",
                     Excerpt = p.Excerpt
                 })
                 .ToListAsync();

            return View(posts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
