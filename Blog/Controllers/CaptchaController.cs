using Microsoft.AspNetCore.Mvc;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

public class CaptchaController : Controller
{
    [HttpGet]
    public IActionResult GetCaptcha()
    {
        string code = GenerateRandomCode(4); // 產生 4 碼隨機數字/字母
        HttpContext.Session.SetString("CaptchaCode", code); // 存入 Session

        using (var bitmap = new Bitmap(100, 40))
        using (var g = Graphics.FromImage(bitmap))
        {
            g.Clear(Color.White);

            // 畫干擾線
            Random rand = new Random();
            for (int i = 0; i < 5; i++)
            {
                g.DrawLine(Pens.LightGray, rand.Next(100), rand.Next(40), rand.Next(100), rand.Next(40));
            }

            // 畫驗證碼
            Font font = new Font("Arial", 20, FontStyle.Bold | FontStyle.Italic);
            LinearGradientBrush brush = new LinearGradientBrush(new Rectangle(0, 0, bitmap.Width, bitmap.Height), Color.Blue, Color.DarkRed, 1.2f, true);
            g.DrawString(code, font, brush, 10, 5);

            // 畫噪點
            for (int i = 0; i < 50; i++)
            {
                bitmap.SetPixel(rand.Next(bitmap.Width), rand.Next(bitmap.Height), Color.FromArgb(rand.Next()));
            }

            using (var stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                return File(stream.ToArray(), "image/png");
            }
        }
    }

    private string GenerateRandomCode(int length)
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789"; // 去掉容易混淆的 I, 1, O, 0
        var random = new Random();
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }
}