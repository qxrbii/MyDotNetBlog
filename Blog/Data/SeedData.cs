using Blog.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using myBlog.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new BlogDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<BlogDbContext>>()))
        {
            // 檢查是否有任何分類，如果已經有了，就跳過新增
            if (!context.Categories.Any())
            {
                context.Categories.AddRange(
                    new Category { Name = "技術分享" }, // 不要手動寫 Id = 1，讓資料庫自動跳號
                    new Category { Name = "生活點滴" },
                    new Category { Name = "正能量" }
                );
                await context.SaveChangesAsync();
            }

            // 取得設定檔服務 (IConfiguration)
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();

            // 讀取 appsettings.json 裡的數值
            // 如果讀不到，就用 "Admin123!" 當作備用預設值
            var adminUserVal = configuration["AdminSettings:UserName"] ?? "Admin";
            var adminEmailVal = configuration["AdminSettings:Email"] ?? "admin@blog.com";
            var adminPwdVal = configuration["AdminSettings:Password"] ?? "Admin123!";

            // 處理角色
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // 處理管理員
            var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            // 用讀取到的 adminUserVal 去找人
            var adminUser = await userManager.FindByNameAsync(adminUserVal);

            if (adminUser == null)
            {
                adminUser = new ApplicationUser
                {
                    UserName = adminUserVal,  // 使用設定檔的值
                    Email = adminEmailVal,    // 使用設定檔的值
                    EmailConfirmed = true
                };

                // 建立帳號時，使用讀取到的密碼
                var result = await userManager.CreateAsync(adminUser, adminPwdVal);

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}