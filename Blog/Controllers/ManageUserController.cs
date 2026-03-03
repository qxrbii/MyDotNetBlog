using Blog.Models;
using Blog.ViewModel; // 確保有引用 ViewModel
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Blog.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ManageUserController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ManageUserController(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        // 原本叫 ManageUsers，建議改名為 Index，這樣網址就是 /ManageUser/Index (或簡稱 /ManageUser)
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var model = new List<UserRoleViewModel>();

            foreach (var user in users)
            {
                model.Add(new UserRoleViewModel
                {
                    UserId = user.Id,
                    Email = user.Email,
                    IsAdmin = await _userManager.IsInRoleAsync(user, "Admin"),
                    IsMuted = user.IsMuted // ★ 記得去 ViewModel 補上這個屬性
                });
            }

            return View(model);
        }

        // 功能 1：升降管理員權限 (從您原本的程式碼搬過來的)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdmin(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // --- 防呆機制 ---
            if (user.Email == "admin@blog.com")
            {
                TempData["Error"] = "無法變更最高管理員的權限！";
                return RedirectToAction(nameof(Index));
            }

            if (user.UserName == User.Identity.Name)
            {
                TempData["Error"] = "你不能取消自己的管理員權限！";
                return RedirectToAction(nameof(Index));
            }
            // ------------------

            var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");

            if (isAdmin)
            {
                await _userManager.RemoveFromRoleAsync(user, "Admin");
                TempData["Message"] = $"已取消 {user.Email} 的管理員權限";
            }
            else
            {
                await _userManager.AddToRoleAsync(user, "Admin");
                TempData["Message"] = $"已將 {user.Email} 升級為管理員";
            }

            return RedirectToAction(nameof(Index));
        }

        // 功能 2：禁言/解禁 (這是新加入的功能)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleMute(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // 最高權限保護區 ---

            // 1. 保護「超級管理員」 (依照你的 SeedData 設定的 Email)
            if (user.Email == "admin@blog.com")
            {
                TempData["Error"] = "錯誤：無法禁言最高權限管理員！";
                return RedirectToAction(nameof(Index));
            }

            // 2. 保護「自己」 (避免管理員不小心禁言自己)
            if (user.UserName == User.Identity.Name)
            {
                TempData["Error"] = "錯誤：您不能禁言自己！";
                return RedirectToAction(nameof(Index));
            }
            // ----------------------------

            user.IsMuted = !user.IsMuted;
            await _userManager.UpdateAsync(user);

            TempData["Message"] = user.IsMuted ? $"已禁言 {user.Email}" : $"已解除 {user.Email} 的禁言";

            return RedirectToAction(nameof(Index));
        }
    }
}