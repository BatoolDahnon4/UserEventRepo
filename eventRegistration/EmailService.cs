using eventRegistration;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Data.SqlClient;
using MimeKit;
using System.Net;

namespace MedcorSL.Services
{
    public interface IEmailService
    {
        Task SendEmailAsync(EmailViewModel emailViewModel);
       
        Task SendRegistrationEmailAsync(Guest guest, int count);
        Task SendRegistrationEmailAsync(Guest guest);
    }

    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailConfig;

        public EmailService(EmailConfig emailConfig)
        {
            _emailConfig = emailConfig;
        }

        public async Task SendEmailAsync(EmailViewModel emailViewModel)
        {
            var mimeMessage = new MimeMessage();


            mimeMessage.From.Add(new MailboxAddress("EXPOTECH", _emailConfig.Username));

            mimeMessage.To.Add(new MailboxAddress
            (emailViewModel.ToAdressTitle,
                emailViewModel.ToAddress
            ));
            mimeMessage.Subject = (string)emailViewModel.Subject;
            mimeMessage.Body = new TextPart("html")
            {
                Text = (string)emailViewModel.Body
            };
            using (var emailClient = new SmtpClient())
            {
                try
                {

                    emailClient.ServerCertificateValidationCallback = (a, b, c, d) => true;
                    await emailClient.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.SmtpPort,
                        SecureSocketOptions.StartTls);

                    emailClient.Authenticate(_emailConfig.Username, _emailConfig.Password);

                    await emailClient.SendAsync(mimeMessage);
                    await emailClient.DisconnectAsync(true);
                }
                catch (Exception e)
                {
                }

            }

        }

        public Task SendRegistrationEmailAsync(Guest guest, int count)
        {
          
            var body = "<html><body><p> Gala Dinner QR"
                + "</p><p>هذه دعوة خاصة وشخصية لحضور عشاء التشبيك وبناء العلاقات الذي سيعقد في الثاني عشر من ديسمبر في فندق الميلينيوم الساعة 6 مساءً ويجب إبرازها وقت الحضور</p>"+"<p>رقم الدعوة"+": "+count+"</p></body></html>";
            return SendEmailAsync(new EmailViewModel
            { ToAddress = guest.Email, Subject = "REGISTRATION CONFIRM", Body = body });
        }

        public Task SendRegistrationEmailAsync(Guest guest)
        {
            var body = "<html><body><p> "
                  + "</p><p> welcome </p>" + "</p></body></html>";
            return SendEmailAsync(new EmailViewModel
            { ToAddress = guest.Email, Subject = "REGISTRATION CONFIRM", Body = body });
        }
    }


}