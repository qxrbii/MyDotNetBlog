using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Hosting; // 用於 IWebHostEnvironment
using Microsoft.AspNetCore.Http;    // 用於 IFormFile
using System;
using System.IO;                    // 用於 Path, FileStream, Directory
using System.Threading.Tasks;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UploadController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        public UploadController(IWebHostEnvironment env)
        {
            _env = env;
        }

        [HttpPost]
        public async Task<IActionResult> upload(IFormFile upload)
        {
            if (upload == null || upload.Length == 0)
            {
                return BadRequest(new { error = new { message = "上傳失敗，檔案為空。" } });
            }

            // 1. 定義儲存路徑 (wwwroot/uploads)
            var uploadPath = Path.Combine(_env.WebRootPath, "uploads");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            // 2. 產生唯一檔名 (避免檔名重複覆蓋)
            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(upload.FileName);
            var filePath = Path.Combine(uploadPath, fileName);

            // 3. 存檔
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await upload.CopyToAsync(stream);
            }

            // 4. 回傳 CKEditor 需要的 JSON 格式
            // CKEditor 預設接收格式：{ "url": "你的圖片網址" }
            var url = $"/uploads/{fileName}";

            return Ok(new { url = url });

        }
    }
}
