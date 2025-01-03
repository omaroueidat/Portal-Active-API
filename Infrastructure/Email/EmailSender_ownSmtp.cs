using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace Infrastructure.Email
{
    public class EmailSender_ownSmtp
    {
        private readonly string _smtpHost = "smtp.gmail.com";
        private readonly int _smtpPort = 587; // Port for TLS
        private readonly IConfiguration _configuration;

        public EmailSender_ownSmtp(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(string recipientEmail, string user_name, string url, string subject = "Verify Your Email")
        {
            var _smtpUser = _configuration["Smtp2:User"];
            var _smtpPass = _configuration["Smtp2:Pass"];

            var body = "<html>\r\n    <head>\r\n<style>\r\n    .responsive-container { \r\n        display: flex; \r\n        flex-direction: column; \r\n        align-items: center; \r\n        justify-content: center; \r\n        width: 100%; \r\n        max-width: 1200px; \r\n        margin: 0 auto; \r\n        padding: 15px; \r\n        box-sizing: border-box; \r\n        overflow: visible; \r\n    }\r\n    p, h1, h2, h3, span, div { \r\n        text-align: center; \r\n        word-wrap: break-word; \r\n        overflow: visible; \r\n        max-width: 100%; \r\n    }\r\n    button { \r\n        margin: 10px auto; \r\n        padding: 10px 20px; \r\n        font-size: 16px; \r\n        border: none; \r\n        background-color: #007bff; \r\n        color: white; \r\n        cursor: pointer; \r\n        border-radius: 5px; \r\n    }\r\n    button:hover { \r\n        background-color: #0056b3; \r\n    }\r\n    @media (max-width: 768px) { \r\n        .responsive-container { \r\n            padding: 10px; \r\n        } \r\n        button { \r\n            width: 80%; \r\n        } \r\n    }\r\n    @media (max-width: 480px) { \r\n        .responsive-container { \r\n            padding: 5px; \r\n        } \r\n        button { \r\n            width: 100%; \r\n        } \r\n    }\r\n    </style>\r\n    </head>\r\n    <body>\r\n<div class=\"responsive-container\">\r\n<div style=\"text-align:center; direction: ltr; color: rgb(80, 0, 80); font-family: Arial, Helvetica, sans-serif; background-color: rgb(255, 255, 255); font-size: small;\"><h1 style=\"direction: ltr; font-family: Helvetica, sans-serif; color: rgb(18, 18, 18); font-size: 45px; line-height: 56.25px; font-weight: 400; margin: 0px 0px 20px;\"><strong style=\"direction: ltr;\">Welcome, your account has been<span> </span><span style=\"direction: ltr; line-height: 56.25px; color: rgb(25, 102, 255);\">created!</span></strong></h1>\r\n\r\n    <h1 style=\"direction: ltr; font-family: Helvetica, sans-serif; color: rgb(18, 18, 18); font-size: 45px; line-height: 56.25px; font-weight: 400; margin: 0px 0px 20px;\"><div style=\"direction: ltr; color: rgb(80, 0, 80); font-family: Arial, Helvetica, sans-serif; font-size: small;\"></div><div style=\"direction: ltr; color: rgb(80, 0, 80); font-family: Arial, Helvetica, sans-serif; font-size: small;\"><p style=\"direction: ltr; font-family: Helvetica, sans-serif; color: rgb(18, 18, 18); font-size: 14px; line-height: 17.5px; margin: 0px 0px 20px;\"></p>\r\n    \r\n    </div><table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" align=\"center\" width=\"100%\" style=\"direction: ltr; border-spacing: 0px; border-collapse: collapse; color: rgb(80, 0, 80); font-family: Arial, Helvetica, sans-serif; font-size: small;\"><tbody style=\"direction: ltr;\"><tr style=\"direction: ltr;\"><td align=\"center\" style=\"margin: 0px; direction: ltr; border-collapse: collapse; padding: 0px 0px 20px;\"><table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" align=\"center\" width=\"100%\" style=\"direction: ltr; border-spacing: 0px; border-collapse: initial; border-top: 1px solid rgb(234, 236, 237); width: 640px;\"><tbody style=\"direction: ltr;\"><tr style=\"direction: ltr;\"><td height=\"1\" style=\"margin: 0px; direction: ltr; border-collapse: collapse;\"></td>\r\n    </tr>\r\n    \r\n    </tbody>\r\n    </table>\r\n    \r\n    </td>\r\n    </tr>\r\n    \r\n    </tbody>\r\n    </table>\r\n    \r\n    </h1>\r\n    \r\n    <h2 style=\"direction: ltr; font-family: Helvetica, sans-serif; color: rgb(18, 18, 18); font-size: 22px; line-height: 27.5px; font-weight: 400; margin: 0px 0px 20px; text-align: center;\"><strong style=\"direction: ltr;\"><span style=\"direction: ltr; line-height: 27.5px; color: rgb(25, 102, 255);\"> " + user_name + ", <span style=\"direction: ltr; line-height: 27.5px; color: rgb(0, 0, 0);\">your account has been created and waiting your</span> verification!</span></strong></h2>\r\n    \r\n    <h1 style=\"direction: ltr; font-family: Helvetica, sans-serif; color: rgb(18, 18, 18); font-size: 45px; line-height: 56.25px; font-weight: 400; margin: 0px 0px 20px;\"><div style=\"direction: ltr; color: rgb(80, 0, 80); font-family: Arial, Helvetica, sans-serif; font-size: small;\"></div><div style=\"direction: ltr; color: rgb(80, 0, 80); font-family: Arial, Helvetica, sans-serif; font-size: small;\"><p style=\"direction: ltr; font-family: Helvetica, sans-serif; color: rgb(18, 18, 18); font-size: 14px; line-height: 17.5px; margin: 0px 0px 20px; text-align: center;\">Press the below button to confrim your email, it will automatically verify your email and redirect you to the website.</p>\r\n    \r\n        <td align=\"center\" style=\"margin: 0px; direction: ltr; border-collapse: collapse; padding: 0px 0px 20px; font-family: Helvetica, sans-serif; text-align: center; width: 100%; display: table;\">\r\n            <a href=\"" + url +"\" target=\"_blank\" style=\"direction: ltr; color: rgb(255, 255, 255); background-color: rgb(0, 123, 255); border-radius: 3px; display: inline-block; font-size: 14px; line-height: 20px; padding: 15px 0px; width: 200px; font-weight: 700; text-decoration-line: none; text-align: center; margin-bottom:20px;\">\r\n                Verify\r\n            </a>\r\n        </td>\r\n    </tr>\r\n    \r\n    </tbody>\r\n    </table>\r\n    \r\n    <div style=\"direction: ltr; color: rgb(80, 0, 80); font-family: Arial, Helvetica, sans-serif; font-size: small;\"><p style=\"direction: ltr; font-family: Helvetica, sans-serif; color: rgb(18, 18, 18); font-size: 14px; line-height: 17.5px; margin: 0px 0px 20px;\"><strong style=\"direction: ltr;\"><span style=\"direction: ltr; line-height: 20px; font-size: 16px;\"><span style=\"direction: ltr; line-height: 20px;\">If you did not request this m</span><span style=\"direction: ltr; line-height: 20px;\">essage, Please simply Ignore it.</span></span></strong></p>\r\n    \r\n    </div><table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" align=\"center\" width=\"100%\" style=\"direction: ltr; border-spacing: 0px; border-collapse: collapse; color: rgb(80, 0, 80); font-family: Arial, Helvetica, sans-serif; font-size: small;\"><tbody style=\"direction: ltr;\"><tr style=\"direction: ltr;\"><td align=\"center\" style=\"margin: 0px; direction: ltr; border-collapse: collapse; padding: 0px 0px 20px;\"><table role=\"presentation\" cellpadding=\"0\" cellspacing=\"0\" border=\"0\" align=\"center\" width=\"100%\" style=\"direction: ltr; border-spacing: 0px; border-collapse: initial; border-top: 1px solid rgb(234, 236, 237); width: 640px;\"><tbody style=\"direction: ltr;\"><tr style=\"direction: ltr;\"><td height=\"1\" style=\"margin: 0px; direction: ltr; border-collapse: collapse;\"></td>\r\n    </tr>\r\n    \r\n    </tbody>\r\n    </table>\r\n    \r\n    </td>\r\n    </tr>\r\n    \r\n    </tbody>\r\n    </table>\r\n    \r\n    <div style=\"direction: ltr; color: rgb(80, 0, 80); font-family: Arial, Helvetica, sans-serif; font-size: small;\"><p style=\"direction: ltr; font-family: Helvetica, sans-serif; color: rgb(18, 18, 18); font-size: 14px; line-height: 17.5px; margin: 0px 0px 20px;\"><strong style=\"direction: ltr;\"><span style=\"direction: ltr; line-height: 15px; font-size: 12px;\"> © 2024 <span style=\"direction: ltr; line-height: 15px; color: rgb(25, 102, 255);\">Portal Active</span>. All rights reserved.</span></strong></p>\r\n    \r\n    </div></h1>\r\n    \r\n    <h1 style=\"direction: ltr; font-family: Helvetica, sans-serif; color: rgb(18, 18, 18); font-size: 45px; line-height: 56.25px; font-weight: 400; margin: 0px 0px 20px;\"><div style=\"direction: ltr; color: rgb(34, 34, 34); font-family: Arial, Helvetica, sans-serif; font-size: small;\"> </div></h1>\r\n    \r\n    <h1 style=\"direction: ltr; font-family: Helvetica, sans-serif; color: rgb(18, 18, 18); font-size: 45px; line-height: 56.25px; font-weight: 400; margin: 0px 0px 20px;\"> </h1>\r\n    \r\n    </div>\r\n</div>\r\n</body>\r\n</html>";

            
            var client = new SmtpClient(_smtpHost, _smtpPort)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_smtpUser, _smtpPass),
            };

            var mailMessage = new MailMessage
            {
                From = new MailAddress(_configuration["Smtp2:User"]),
                Subject = "Verify Your Email!",
                Body = body,
                IsBodyHtml = true, // Enable HTML formatting
            };

            mailMessage.To.Add(recipientEmail);

            try
            {
                await client.SendMailAsync(mailMessage);
                Console.WriteLine("Email sent successfully.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending email: {ex.Message}");
            };
        }
        }
    }

