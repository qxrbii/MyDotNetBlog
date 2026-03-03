using Blog.Models;
using Blog.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using myBlog.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// 設定資料庫連線
builder.Services.AddDbContext<BlogDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("BlogDatabase")));

// 1. 加入限流服務
builder.Services.AddRateLimiter(options =>
{
    // 定義一個名為 "RegisterPolicy" 的策略
    options.AddFixedWindowLimiter("RegisterPolicy", opt =>
    {
        opt.PermitLimit = 5; // 每個視窗限制 5 次請求
        opt.Window = TimeSpan.FromMinutes(1); // 視窗時間為 1 分鐘
        opt.QueueLimit = 0; // 超過限制後不排隊，直接拒絕
        // 429 Too Many Requests
    });
});

// 註冊 Identity 服務
// 將 <ApplicationUser, IdentityRole> 改為 <ApplicationUser, IdentityRole>
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    // 這裡可以設定密碼強度，方便測試我們先設寬鬆一點
    options.Password.RequireDigit = true;              //是否包含數字(0 - 9)
    options.Password.RequiredLength = 8;               //密碼最小長度 (預設為 6)
    options.Password.RequireNonAlphanumeric = true;    //是否包含特殊符號 (如 !, @, #)
    options.Password.RequireUppercase = true;          //是否包含大寫字母 (A-Z)
    options.Password.RequireLowercase = true;          //是否包含小寫字母 (a-z)
    options.User.RequireUniqueEmail = true;            //Email 是否必須唯一（不能重複註冊）
})
    .AddEntityFrameworkStores<BlogDbContext>() // 告訴系統：使用者資料存在 BlogDbContext
    .AddDefaultTokenProviders() // 啟用 Token (例如忘記密碼功能需要)
    .AddErrorDescriber<CustomIdentityErrorDescriber>();

// 註冊 Cookie 驗證 
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";     // 沒登入被擋下時導向這
        options.AccessDeniedPath = "/Home/Index"; // 權限不足時導向這
    });

// 加入 Session 服務
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // 10分鐘過期
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();  // 強制加密轉Https
app.UseStaticFiles();       // 讀取 wwwroot 裡的 CSS/JS/圖片

app.UseRouting();           // 路由規劃 (決定這封信要寄給哪個 Controller)

app.UseSession();           // 啟用 Session 功能
app.UseAuthentication();    // 身份識別
app.UseAuthorization();     // 權限控管

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        // 呼叫我們剛剛寫的 Initialize 方法
        // 因為是 async 方法，所以要用 Wait() 等它做完
        SeedData.Initialize(services).Wait();
    }
    catch (Exception ex)
    {
        var logger = services.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "建立種子資料時發生錯誤");
    }
}

app.Run();
