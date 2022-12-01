﻿using Azure;
using eventRegistration;
using eventRegistration.Controllers;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Data.SqlClient;
using MimeKit;
using MimeKit.Utils;
using Newtonsoft.Json;
using QRCoder;
using System.Data;
using System.Drawing;
using System.Net;
using System.Xml.Linq;

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


            var builder = new BodyBuilder();
            
            var image = builder.LinkedResources.Add("qr.png", emailViewModel.Image);

            image.ContentId = MimeUtils.GenerateMessageId();

            builder.HtmlBody = string.Format(emailViewModel.Body, image.ContentId);

            mimeMessage.Body = builder.ToMessageBody();


            //mimeMessage.Body = new TextPart("html")
            //{
            //    Text = (string)emailViewModel.Body
            //};

            
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
            var qrData = CreateQRCode(guest);


            return SendEmailAsync(new EmailViewModel
            { ToAddress = guest.Email, Subject = "REGISTRATION CONFIRM", Body = @"<p>هذه دعوة خاصة وشخصية لحضور عشاء التشبيك وبناء العلاقات الذي سيعقد في الثاني عشر من ديسمبر في فندق الميلينيوم الساعة 6 مساءً ويجب إبرازها وقت الحضور</p>
              <br> ID Number:" +count, Image = qrData });
            //var qrData = CreateQRCode(guest);
            ////var body = @$"<html><body><p>هذه دعوة خاصة وشخصية لحضور عشاء التشبيك وبناء العلاقات الذي سيعقد في الثاني عشر من ديسمبر في فندق الميلينيوم الساعة 6 مساءً ويجب إبرازها وقت الحضور</p>"+"<p>رقم الدعوة"+":"+count+
            ////   "</p><br><img style='width:500px;' src='{qrData}'></body></html>";
            ////var body = "<html><body><p> Gala Dinner QR"+"</p>" +
            ////    "<p>هذه دعوة خاصة وشخصية لحضور عشاء التشبيك وبناء العلاقات الذي سيعقد في الثاني عشر من ديسمبر في فندق الميلينيوم الساعة 6 مساءً ويجب إبرازها وقت الحضور</p>"+"<p>رقم الدعوة"+": "+count+
            ////    "</p> <br><img style='width:500px; src='{qrData}'></body></html>";
            //return SendEmailAsync(new EmailViewModel
            //{ ToAddress = guest.Email, Subject = "REGISTRATION CONFIRM", Body = body });
        }

        public Task SendRegistrationEmailAsync(Guest guest)
        {

            var qrData = CreateQRCode(guest);
            
           
            return SendEmailAsync(new EmailViewModel
            { ToAddress = guest.Email, Subject = "REGISTRATION CONFIRM", Body= @"Successfully registered ", Image = qrData });
        }


        private byte[] CreateQRCode(Guest guest)
        {
            QRCodeGenerator QrGenerator = new QRCodeGenerator();

            var data = JsonConvert.SerializeObject(guest);
            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.M);
            QRCode QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(10);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
           
            return BitmapArray;
        }
    }


}