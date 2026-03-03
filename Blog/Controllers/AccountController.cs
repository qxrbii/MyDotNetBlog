using Blog.Dtos;       // 你的 LoginDto
using Blog.Models;      // ApplicationUser
using Blog.ViewComponents;
using Blog.ViewModel;  // 剛剛建立的 RegisterViewModel
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;

namespace Blog.Controllers
{
    public class AccountController : Controller
    {
        // 1. 改用 Identity 的兩大管理員，取代 DbContext 和 PasswordHasher
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IEmailSender _emailSender; // 新增這行

        public AccountController(UserManager<ApplicationUser> userManager,
                                 SignInManager<ApplicationUser> signInManager,
                                 IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
        }

        // -----------------------------------------------------------
        // 註冊功能 (Register)
        // -----------------------------------------------------------
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("RegisterPolicy")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // 檢查 Session 是否有驗證碼
            var serverCaptcha = HttpContext.Session.GetString("CaptchaCode");

            if (string.IsNullOrEmpty(serverCaptcha) || string.IsNullOrEmpty(model.CaptchaCode) ||
                !serverCaptcha.Equals(model.CaptchaCode, StringComparison.OrdinalIgnoreCase))
            {
                ModelState.AddModelError("CaptchaCode", "驗證碼錯誤或已過期");
            }

            // 驗證過後，建議清除 Session 防止重複使用
            HttpContext.Session.Remove("CaptchaCode");

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (ModelState.IsValid)
            {
                // 建立 ApplicationUser 物件
                // 我們把 Email 當作帳號 (UserName)
                var user = new ApplicationUser { UserName = model.UserName, Email = model.Email };

                // 建立帳號 (這會自動處理密碼雜湊、寫入資料庫)
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    // 註冊成功後，直接幫使用者登入
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // 處理錯誤 (例如密碼太簡單、Email 已被註冊)
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }

        // -----------------------------------------------------------
        // 登入功能 (Login) - 這部分也要重寫！
        // -----------------------------------------------------------
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [EnableRateLimiting("RegisterPolicy")]
        public async Task<IActionResult> Login(LoginDto model) // 假設你的 LoginDto 有 Email 和 Password
        {
            if (ModelState.IsValid)
            {
                // 使用 SignInManager 進行登入驗證
                // 參數：帳號(Email), 密碼, 記住我(false), 失敗鎖定(false)
                // 這裡我們假設 LoginDto 的 Username 欄位其實是讓使用者填 Email
                var result = await _signInManager.PasswordSignInAsync(
                    model.Username,
                    model.Password,
                    false,
                    false);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "帳號或密碼錯誤");
            }

            return View(model);
        }

        // -----------------------------------------------------------
        // 登出功能 (Logout)
        // -----------------------------------------------------------
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync(); // 清除 Cookie
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult Forgot()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotAccount(ForgotAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user != null)
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(user);

                var resetLink = Url.Action("ResetPassword", "Account",
                    new { token = token, email = user.Email }, Request.Scheme);

                string subject = "[正能量部落格] 帳號找回與密碼重設通知";
                string message = $@"<p>您好，我們收到您的帳號找回請求：</p>
                                    <p><b>您的登入帳號為：{user.UserName}</b></p>
                                    <p>若您也忘記了密碼，請點擊下方連結進行重設：</p>
                                    <p><a href='{resetLink}'>點此重設密碼</a></p>
                                    <p>此連結將於 24 小時後失效。如果您沒有要求此操作，請忽略此信件。</p>";
                await _emailSender.SendEmailAsync(user.Email, subject, message);
            }
            return RedirectToAction("ForgotAccountConfirmation");
        }

        [HttpGet]
        public IActionResult ResetPassword(string token = null, string email = null)
        {
            if (token == null || email == null)
            {
                return RedirectToAction("Index", "Home");
            }
            var model = new ResetPasswordViewModel { Token = token, Email = email };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                // 為了安全性，即使找不到 Email 也顯示成功畫面，避免被掃描帳號
                return RedirectToAction("ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, model.Token, model.Password);

            if (result.Succeeded)
            {
                return RedirectToAction("ResetPasswordConfirmation");
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }

            // ★ 修改這裡：把 model 傳回去，保留 Hidden Field 的 Email 與 Token
            return View(model);
        }

        public IActionResult ForgotAccountConfirmation() => View();
        public IActionResult ResetPasswordConfirmation() => View();
    }
}