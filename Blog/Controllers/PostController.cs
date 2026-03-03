using Blog.Dtos;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using myBlog.Data; // 或你的 Data 命名空間

namespace Blog.Controllers
{
    public class PostController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public PostController(BlogDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // ==========================================
        // 顯示所有文章
        // ==========================================
        // GET: Post/Index
        public async Task<IActionResult> Index()
        {
            var posts = await _context.Posts
                .Select(p => new PostListDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    Slug = p.Slug,
                    CategoryName = p.Category != null ? p.Category.Name : string.Empty,
                    PublishDate = p.PublishDate,
                    ViewCount = p.ViewCount,
                    Author = p.Author ?? string.Empty,
                    Excerpt = p.Excerpt
                })
                .OrderByDescending(p => p.PublishDate) // 讓最新的文章排在最前面
                .ToListAsync();

            return View(posts);
        }

        // 1. 確保 Attribute 裡的參數是 {slug}
        [HttpGet("Post/Details/{slug}")]
        public async Task<IActionResult> Details(string slug) // 2. 方法名稱改成 Details (要有 s)
        {
            // 3. 檢查 slug 是否為空
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            // 4. 使用 Slug 搜尋
            var post = await _context.Posts
                .Include(p => p.Category)
                .Include(p => p.Images) // 如果你有加圖片表
                .Include(p => p.Comments)    // 第一層：留言
                        .ThenInclude(c => c.User) // 第二層：留言的使用者
                .FirstOrDefaultAsync(m => m.Slug == slug);

            if (post == null)
            {
                return NotFound();
            }

            //增加瀏覽量
            post.ViewCount++;
            _context.Update(post);
            await _context.SaveChangesAsync();

            return View("Details", post); // 明確指定回傳 Details View，以防萬一
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int postId, string content)
        {
            // 1. 先把這篇文章抓出來 (為了拿它的 Slug)
            // 變數 post 在這裡第一次宣告
            var post = await _context.Posts.FindAsync(postId);

            if (post == null)
            {
                return NotFound();
            }

            // 防呆：如果內容是空的，直接跳回去
            if (string.IsNullOrWhiteSpace(content))
            {
                return RedirectToAction("Details", new { slug = post.Slug });
            }

            // 2. 取得當前使用者 (因為上面改了 UserManager<ApplicationUser>，這裡會自動抓到新版使用者)
            var user = await _userManager.GetUserAsync(User);

            // 3. 檢查是否被禁言
            // 現在 IDE 應該認得 IsMuted 了
            if (user != null && user.IsMuted)
            {
                TempData["Error"] = "抱歉，您的帳號目前處於禁言狀態，無法發表評論。";

                // 【修正重點 3】直接使用最上面宣告的 post，不要再 var post = ...
                return RedirectToAction("Details", new { slug = post.Slug });
            }

            // 4. 建立留言
            var comment = new Comment
            {
                PostId = postId,
                Content = content,
                UserId = user.Id, // 直接用 user 物件拿 Id 即可
                CreatedDate = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { slug = post.Slug });
        }
    }
}