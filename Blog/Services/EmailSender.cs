using Microsoft.AspNetCore.Identity.UI.Services;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace Blog.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var emailMessage = new MimeMessage();

            // 1. 設定發件人資訊 (從 appsettings.json 讀取或直接寫死)
            emailMessage.From.Add(new MailboxAddress("正能量部落格管理員", "qxrbii@gmail.com"));

            // 2. 設定收件人
            emailMessage.To.Add(new MailboxAddress("", email));

            // 3. 設定主旨與內容
            emailMessage.Subject = subject;
            var bodyBuilder = new BodyBuilder { HtmlBody = htmlMessage };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using (var client = new SmtpClient())
            {
                // 4. 連線到 Gmail SMTP 伺服器
                // 注意：587 埠號配上 StartTls 是標準做法
                await client.ConnectAsync("smtp.gmail.com", 587, SecureSocketOptions.StartTls);

                // 5. 驗證 (請填入您的 Gmail 與 16 位應用程式密碼)
                await client.AuthenticateAsync("qxrbii@gmail.com", "rbvm gazx bged xdrs");

                // 6. 傳送信件
                await client.SendAsync(emailMessage);
                await client.DisconnectAsync(true);
            }
        }
    }
}