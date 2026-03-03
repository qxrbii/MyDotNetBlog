using Blog.Dtos;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using myBlog.Data;

namespace Blog.Controllers
{
    [Authorize(Roles = "Admin")] // 確保只有管理員能進來
    public class AdminController : Controller
    {
        private readonly BlogDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public AdminController(BlogDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }


        // 文章列表 (R)
        public async Task<IActionResult> Posts()
        {
            var isAdmin = User.IsInRole("Admin");
            var currentUser = User.Identity.Name; // 這是目前登入者的 Email

            // 投影到 DTO：效能好且安全
            var query = _context.Posts.AsQueryable();

            // 如果不是管理者，就只能看自己的
            if (!isAdmin)
            {
                query = query.Where(p => p.Author == currentUser);
            }

            var posts = await query
                .Select(p => new AdminPostListDto
                {
                    Id = p.Id,
                    Title = p.Title,
                    CategoryName = p.Category != null ? p.Category.Name : "未分類",
                    PublishDate = p.PublishDate,
                    Author = string.IsNullOrEmpty(p.Author) ? "匿名" : p.Author,
                    Slug = p.Slug,
                    ViewCount = p.ViewCount
                }).ToListAsync();

            return View(posts);
        }

        [HttpGet]
        public IActionResult CreatePost()
        {
            // 這裡可以傳遞分類選單給前端
            ViewBag.Categories = _context.Categories.ToList();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePost(Posts post)
        {
            // 檢查點 手動移除不需要使用者填寫的驗證（因為我們會在後端補齊）
            ModelState.Remove("Author");
            ModelState.Remove("Slug");
            ModelState.Remove("Category"); // 這是導航屬性，不該被驗證

            if (ModelState.IsValid)
            {
                // 自動設定作者為當前登入者
                post.Author = User.Identity.Name;
                post.PublishDate = DateTime.Now;

                // 檢查點 補上必填的 Slug (假設用標題當 Slug)
                if (string.IsNullOrEmpty(post.Slug))
                {
                    post.Slug = Guid.NewGuid().ToString().Substring(0, 8); // 暫時用亂碼代替，確保不重複
                }

                try
                {
                    _context.Add(post);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Posts)); // 成功後跳轉
                }
                catch (Exception ex)
                {
                    // 如果資料庫報錯，可以在這裡下斷點看原因
                    ModelState.AddModelError("", "資料庫儲存失敗：" + ex.Message);
                }
            }
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(post);
        }

        // GET: /Admin/EditPost/my-slug-title
        [HttpGet]
        public async Task<IActionResult> EditPost(string slug)
        {
            // 防止空值
            if (string.IsNullOrEmpty(slug)) return NotFound();

            // 1. 用 Slug 找到文章
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);

            // 2. 找不到就回傳 404
            if (post == null) return NotFound();

            // 3. 載入分類選單 (ViewBag)
            ViewBag.Categories = await _context.Categories.ToListAsync();

            return View(post);
        }

        // POST: /Admin/EditPost/5 (表單送出是用 ID)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPost(int id, Posts post)
        {
            // 1. 安全檢查：網址 ID 與表單 ID 必須一致
            if (id != post.Id) return NotFound();

            // 2. 移除驗證：這些欄位我們不修改，所以移除驗證避免報錯
            ModelState.Remove("Author");
            ModelState.Remove("Category");

            if (ModelState.IsValid)
            {
                try
                {
                    // 3. 從資料庫撈出「原始資料」
                    var existingPost = await _context.Posts.FindAsync(id);
                    if (existingPost == null) return NotFound();

                    // 4. 只更新這些欄位 (手動映射)
                    existingPost.Title = post.Title;
                    existingPost.Author = post.Author; 
                    existingPost.CategoryId = post.CategoryId;
                    existingPost.Excerpt = post.Excerpt;
                    existingPost.Content = post.Content;

                    _context.Update(existingPost);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Posts.Any(e => e.Id == id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Posts));
            }

            // 失敗時重新載入分類
            ViewBag.Categories = await _context.Categories.ToListAsync();
            return View(post);
        }
        // POST: Admin/DeletePost/my-slug-title
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeletePost(string slug)
        {
            // 1. 防止空值
            if (string.IsNullOrEmpty(slug))
            {
                return NotFound();
            }

            // 2. 用 Slug 找到要刪除的那筆資料
            // 注意：因為 Slug 不是主鍵(PK)，所以不能用 FindAsync，要用 FirstOrDefaultAsync
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Slug == slug);

            // 3. 如果找到了，就執行刪除
            if (post != null)
            {
                _context.Posts.Remove(post);
                await _context.SaveChangesAsync();
            }
            else
            {
                // 找不到這篇文章 (可能已經被別人刪了)
                return NotFound();
            }

            // 4. 刪除完成，跳轉回列表
            return RedirectToAction(nameof(Posts));
        }
    }
}
