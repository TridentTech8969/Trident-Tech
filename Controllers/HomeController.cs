using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using TridentTechSolutions.Models;

namespace TridentTechSolutions.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private const string SmtpHost = "smtp.gmail.com";
        private const int SmtpPort = 587;              // STARTTLS
        private const bool UseSsl = true;
        private const string SmtpUsername = "eternalvision2025@gmail.com"; // your Gmail address
        private const string SmtpAppPassword = "gvvy enkz fjjo iccp"; // 16-char App Password
        private const string AdminEmail = "eternalvision2025@gmail.com";     // where you receive messages
        private const string FromDisplayName = "Contact from Trident Tech Website";      // display name shown to admin
        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SubmitContact(ContactFormModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                );
                return Json(new { success = false, errors = errors, message = "Please correct the validation errors." });
            }

            try
            {
                bool emailSent = SendEmail(model);

                if (emailSent)
                {
                    return Json(new
                    {
                        success = true,
                        message = "Thank you for contacting us! We'll get back to you within 24 hours."
                    });
                }
                else
                {
                    return Json(new
                    {
                        success = false,
                        message = "Failed to send email. Please try again later or contact us directly."
                    });
                }
            }
            catch (Exception ex)
            {
                // Log the exception (use your logging framework)
                // _logger.LogError(ex, "Error sending contact form email");

                return Json(new
                {
                    success = false,
                    message = "An error occurred while sending your message. Please try again later."
                });
            }
        }

        private bool SendEmail(ContactFormModel model)
        {
            try
            {
                // Get email settings from appsettings.json
                var smtpHost = "smtp.gmail.com";
                var smtpPort = 587;
                var smtpUser = "eternalvision2025@gmail.com";
                var smtpPass = "gvvy enkz fjjo iccp";
                var fromEmail = model.Email;
                var toEmail = "eternalvision2025@gmail.com";

                using (var client = new SmtpClient(smtpHost, smtpPort))
                {
                    client.Credentials = new NetworkCredential(smtpUser, smtpPass);
                    client.EnableSsl = true;

                    // Email to admin
                    var mailMessage = new MailMessage
                    {
                        From = new MailAddress(fromEmail, "Trident Tech Solutions - Contact Form"),
                        Subject = $"New Contact: {model.ProjectType} - {model.Name}",
                        Body = $@"
                            <html>
                            <head>
                                <style>
                                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                                    .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 20px; border-radius: 5px 5px 0 0; }}
                                    .content {{ background: #f9f9f9; padding: 20px; border: 1px solid #ddd; }}
                                    .info-row {{ margin: 10px 0; padding: 10px; background: white; border-radius: 3px; }}
                                    .label {{ font-weight: bold; color: #667eea; }}
                                    .footer {{ text-align: center; margin-top: 20px; color: #666; font-size: 12px; }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <div class='header'>
                                        <h2 style='margin: 0;'>New Contact Form Submission</h2>
                                    </div>
                                    <div class='content'>
                                        <div class='info-row'>
                                            <span class='label'>Name:</span> {model.Name}
                                        </div>
                                        <div class='info-row'>
                                            <span class='label'>Email:</span> <a href='mailto:{model.Email}'>{model.Email}</a>
                                        </div>
                                        <div class='info-row'>
                                            <span class='label'>Phone:</span> {(string.IsNullOrEmpty(model.Phone) ? "Not provided" : model.Phone)}
                                        </div>
                                        <div class='info-row'>
                                            <span class='label'>Project Type:</span> {model.ProjectType}
                                        </div>
                                        <div class='info-row'>
                                            <span class='label'>Message:</span><br>
                                            <p style='margin: 10px 0; white-space: pre-wrap;'>{model.Message}</p>
                                        </div>
                                    </div>
                                    <div class='footer'>
                                        <p>This email was sent from the Trident Tech Solutions contact form</p>
                                    </div>
                                </div>
                            </body>
                            </html>
                        ",
                        IsBodyHtml = true
                    };

                    mailMessage.To.Add(toEmail);

                    // Auto-reply to customer
                    var autoReply = new MailMessage
                    {
                        From = new MailAddress(fromEmail, "Trident Tech Solutions"),
                        Subject = "We received your message - Trident Tech Solutions",
                        Body = $@"
                            <html>
                            <head>
                                <style>
                                    body {{ font-family: Arial, sans-serif; line-height: 1.6; color: #333; }}
                                    .container {{ max-width: 600px; margin: 0 auto; padding: 20px; }}
                                    .header {{ background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); color: white; padding: 30px; text-align: center; border-radius: 5px 5px 0 0; }}
                                    .content {{ background: #fff; padding: 30px; border: 1px solid #ddd; }}
                                    .message-box {{ background: #f9f9f9; padding: 15px; border-left: 4px solid #667eea; margin: 20px 0; }}
                                    .footer {{ text-align: center; margin-top: 20px; padding: 20px; color: #666; font-size: 12px; }}
                                </style>
                            </head>
                            <body>
                                <div class='container'>
                                    <div class='header'>
                                        <h2 style='margin: 0;'>Thank You for Contacting Us!</h2>
                                    </div>
                                    <div class='content'>
                                        <p>Dear {model.Name},</p>
                                        <p>Thank you for reaching out to Trident Tech Solutions. We have received your message regarding <strong>{model.ProjectType}</strong> and will get back to you within 24 hours.</p>
                                        
                                        <div class='message-box'>
                                            <strong>Your Message:</strong><br>
                                            <p style='margin: 10px 0; white-space: pre-wrap;'>{model.Message}</p>
                                        </div>
                                        
                                        <p>If you have any urgent questions, feel free to reply to this email or call us directly.</p>
                                        
                                        <p>Best regards,<br>
                                        <strong>Trident Tech Solutions Team</strong></p>
                                    </div>
                                    <div class='footer'>
                                        <p>Trident Tech Solutions | Professional Web Development & Design</p>
                                    </div>
                                </div>
                            </body>
                            </html>
                        ",
                        IsBodyHtml = true
                    };
                    autoReply.To.Add(model.Email);

                    client.Send(mailMessage);
                    client.Send(autoReply);

                    return true;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
