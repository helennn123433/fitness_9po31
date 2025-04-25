using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace fitness.Helpers
{
    internal class EmailSender
    {
        private static string smtpServer = "smtp.yandex.ru";
        private static int smtpPort = 587;

        private static string fromEmail = "Qusar2k19@yandex.ru";
        private static string fromPassword = "nkqtrpbcxpvuurcx";

        public static void SendConfirmChangePasswordEmail(string toEmail, string subject, string confirmCode)
        {
            string body = $@"
            <html>
            <head>
                <style>
                    body {{
                        font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
                        background-color: #f4f4f4;
                        padding: 20px;
                    }}
                    .container {{
                        max-width: 600px;
                        margin: auto;
                        background-color: #ffffff;
                        padding: 20px;
                        border-radius: 8px;
                        box-shadow: 0 2px 8px rgba(0, 0, 0, 0.1);
                    }}
                    .header {{
                        font-size: 24px;
                        color: #3f51b5;
                        margin-bottom: 16px;
                    }}
                    .code {{
                        font-size: 32px;
                        color: #000;
                        font-weight: bold;
                        background-color: #e3f2fd;
                        padding: 12px 20px;
                        border-radius: 6px;
                        display: inline-block;
                        margin: 20px 0;
                    }}
                    .footer {{
                        font-size: 14px;
                        color: #555;
                        margin-top: 20px;
                    }}
                </style>
            </head>
            <body>
                <div class='container'>
                    <div class='header'>Сброс пароля</div>
                    <p>Вы запросили смену пароля в системе <strong>Fitness</strong>.</p>
                    <p>Для подтверждения действия используйте следующий код:</p>
                    <div class='code'>{confirmCode}</div>
                    <p>Введите этот код в окне приложения, чтобы продолжить процедуру смены пароля.</p>
                    <div class='footer'>
                        Если вы не запрашивали смену пароля, просто проигнорируйте это сообщение.<br/>
                        Спасибо, что пользуетесь нашим сервисом!<br/>
                        — Команда Fitness
                    </div>
                </div>
            </body>
            </html>";

            try
            {
                MailAddress from = new MailAddress(fromEmail, "Fitness");
                MailAddress to = new MailAddress(toEmail);

                MailMessage mailMessage = new MailMessage(from, to)
                {
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true // важно для HTML-оформления
                };

                using (SmtpClient smtpClient = new SmtpClient(smtpServer, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(fromEmail, fromPassword);
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;

                    smtpClient.Send(mailMessage);
                }

            }
            catch (Exception ex)
            {
                // Обработка ошибок
                MessageBox.Show("Ошибка при отправке письма: " + ex.Message);
            }
        }
    }
}
