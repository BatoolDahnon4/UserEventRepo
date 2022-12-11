using MailKit.Net.Smtp;
using MailKit.Security;
using MedcorSL.Services;
using MimeKit;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using System.Net.Mail;
using System.Net.Mime;
using XAct;

namespace eventRegistration.Jobs
{
    public class EmailJob
    {
        public static void SendInvitationQR(Guest guest, EmailConfig emailConfig)
        {
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            MimeMessage message = new MimeMessage();
            BodyBuilder bodyBuilder = new BodyBuilder();
            List<MemoryStream> list = new List<MemoryStream>();

            message.From.Add(new MailboxAddress("PITA", emailConfig.Username));
            message.To.Add(new MailboxAddress(guest.Name, guest.Email) );
            message.Bcc.Add(new MailboxAddress("Hani Araj", "hani.araj@gmail.com"));
            message.ReplyTo.Add(new MailboxAddress("noreply", "noreply@pita.ps"));

            message.Subject = "Gala Invitation - Expotech 2022";
            
            string QrUri;
            string p1;
            string p2;
            string tableP1 = "";
            string tableP2 = "";
            string table;
            switch (guest.Source.ToLower())
            {
                case "wb":
                    p1 = "فندق ميلينيوم رام الله";
                    p2 = "in Millennium Hotel";
                    break;
                case "gaza":
                    p1 = "فندق الروتس - غزة";
                    p2 = "in Roots Hotel - Gaza";
                    break;
                default:
                    p1 = "";
                    p2 = "";
                    throw new ArgumentException("Source value is not valid");
            }

            table = guest.Table.ToString();
            if(guest.Source == "wb")
            {
                if (!table.IsNullOrEmpty())
                {
                    tableP1 = "طاولة رقم (" + table + ")";
                    tableP2 = "Table# (" + table + ")";
                }
                else
                {
                    tableP1 = "طاولة رقم (N/A)";
                    tableP2 = "Table# (N/A)";
                }
            }
            

            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(getQRText(guest), QRCodeGenerator.ECCLevel.H);
            QRCode QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(10);

            using (MemoryStream ms = new MemoryStream())
            {
                QrBitmap.Save(ms, format: ImageFormat.Png);
                byte[] BitmapArray = ms.ToArray();
                bodyBuilder.Attachments.Add("Invitation-QR.png", BitmapArray);
                QrUri = Convert.ToBase64String(BitmapArray, Base64FormattingOptions.None);
            }

            bodyBuilder.TextBody = getEmailTextTemplate();
            bodyBuilder.HtmlBody = getEmailHTMLTemplate()
                .Replace("{{QRString}}", QrUri)
                .Replace("{{p1}}", p1)
                .Replace("{{p2}}", p2)
                .Replace("{{tableP1}}", tableP1)
                .Replace("{{tableP2}}", tableP2);
            message.Body = bodyBuilder.ToMessageBody();

            using (var emailClient = new MailKit.Net.Smtp.SmtpClient())
            {
                try
                {
                    emailClient.CheckCertificateRevocation = false;
                    emailClient.ServerCertificateValidationCallback = (a, b, c, d) => true;
                    emailClient.Connect(
                        emailConfig.SmtpServer,
                        emailConfig.SmtpPort,
                        SecureSocketOptions.StartTls);

                    emailClient.Authenticate(emailConfig.Username, emailConfig.Password);
                    emailClient.Send(message);
                }
                catch (Exception e)
                {
                    throw;
                }

            }
        }
        public static string getQRText(Guest guest)
        {
            /*return string.Format(
                @"Name: {0}; 
                Id: {1}; 
                Table#: {2};", guest.Name, guest.Id, guest.Table);*/
            return guest.Id.ToString();
        }

        public static string getEmailTextTemplate()
        {
            return @"
هذا الإيميل لتأكيد تسجيل حضوركم لحفل عشاء التشبيك وبناء العلاقات ضمن فعاليات أسبوع فلسطين التكنولوجي - إكسبوتك 2022

يوم الاثنين 12/12/2022 الساعة 6 مساءً، فندق ميلينيوم رام الله.

تجدون مرفقاً رمز ال QR من أجل معرفة مكان الجلوس، لذا يرجى إشهار الرمز لدى وصولكم القاعة.

This email is to confirm your attendance for the Gala Dinner “Networking Reception” within Palestine Technology Week – Expotech 2022. 

On Monday, 12th / 12/ 2022, at 6:00 pm, in Millennium Hotel.

You will find attached the QR code in order to know your seating; so please show your code upon arriving the venue. 
";
        }
        public static string getEmailHTMLTemplate() {
            return @"
<!DOCTYPE html PUBLIC ""-//W3C//DTD XHTML 1.0 Transitional//EN"" ""http://www.w3.org/TR/xhtml1/DTD/xhtml1-transitional.dtd"">
<html xmlns=""http://www.w3.org/1999/xhtml"" xmlns:o=""urn:schemas-microsoft-com:office:office"">

<head>
    <meta charset=""UTF-8"">
    <meta content=""width=device-width, initial-scale=1"" name=""viewport"">
    <meta name=""x-apple-disable-message-reformatting"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <meta content=""telephone=no"" name=""format-detection"">
    <title></title>
    <!--[if (mso 16)]>
    <style type=""text/css"">
    a {text-decoration: none;}
    </style>
    <![endif]-->
    <!--[if gte mso 9]><style>sup { font-size: 100% !important; }</style><![endif]-->
    <!--[if gte mso 9]>
<xml>
    <o:OfficeDocumentSettings>
    <o:AllowPNG></o:AllowPNG>
    <o:PixelsPerInch>96</o:PixelsPerInch>
    </o:OfficeDocumentSettings>
</xml>
<![endif]-->
    <!--[if !mso]><!-- -->
    <link href=""https://fonts.googleapis.com/css2?family=Licorice&display=swap"" rel=""stylesheet"">
    <link href=""https://fonts.googleapis.com/css2?family=Oswald:wght@500&display=swap"" rel=""stylesheet"">
    <!--<![endif]-->
</head>

<body data-new-gr-c-s-loaded=""14.1088.0"">
    <div class=""es-wrapper-color"">
        <!--[if gte mso 9]>
			<v:background xmlns:v=""urn:schemas-microsoft-com:vml"" fill=""t"">
				<v:fill type=""tile"" color=""#ffffff""></v:fill>
			</v:background>
		<![endif]-->
        <table class=""es-wrapper"" width=""100%"" cellspacing=""0"" cellpadding=""0"">
            <tbody>
                <tr>
                    <td class=""esd-email-paddings"" valign=""top"">
                        <table cellpadding=""0"" cellspacing=""0"" class=""es-header esd-footer-popover"" align=""center"">
                            <tbody>
                                <tr>
                                    <td class=""esd-stripe"" align=""center"">
                                        <table bgcolor=""#ffffff"" class=""es-header-body"" align=""center"" cellpadding=""0"" cellspacing=""0"" width=""600"" style=""background-color: #ffffff;"">
                                            <tbody>
                                                <tr>
                                                    <td class=""esd-structure es-p20t es-p10b es-p20r es-p20l"" align=""left"">
                                                        <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                            <tbody>
                                                                <tr>
                                                                    <td width=""560"" class=""es-m-p0r esd-container-frame"" valign=""top"" align=""center"">
                                                                        <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td align=""center"" class=""esd-block-text"">
                                                                                        <p style=""color: #000000; direction: rtl; line-height: 120%;"">هذا الإيميل لتأكيد تسجيل حضوركم لحفل عشاء التشبيك ضمن فعاليات أسبوع فلسطين التكنولوجي - إكسبوتك 2022<br><br>يوم الاثنين 12/12/2022 الساعة 6 مساءً، {{p1}}.<br><br>تجدون مرفقاً رمز ال QR من أجل معرفة مكان الجلوس، لذا يرجى إشهار الرمز لدى وصولكم القاعة.<br><br>{{tableP1}}</p>
                                                                                    </td>
                                                                                </tr>
                                                                                <tr>
                                                                                    <td align=""center"" class=""esd-block-text"">
                                                                                        <p style=""color: #000000; direction: rtl; line-height: 120%;"">&nbsp;<br>&nbsp;<br></p>
                                                                                    </td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class=""esd-structure es-p20t es-p10b es-p20r es-p20l"" align=""left"">
                                                        <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                            <tbody>
                                                                <tr>
                                                                    <td width=""560"" class=""es-m-p0r esd-container-frame"" valign=""top"" align=""center"">
                                                                        <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td align=""center"" class=""esd-block-text"">
                                                                                        <p style=""color: #000000; direction: ltr; line-height: 120%;"">This email is to confirm your attendance for the Gala Dinner “Networking Reception” within Palestine Technology Week – Expotech 2022.<br><br>On Monday, 12th/12/2022, at 6:00 pm, {{p2}}.<br><br>You will find attached the QR code in order to know your seating; so please show your code upon arriving the venue.<br><br>{{tableP2}}</p>
                                                                                    </td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                                <tr>
                                                    <td class=""esd-structure es-p20t es-p20r es-p20l"" align=""left"">
                                                        <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                            <tbody>
                                                                <tr>
                                                                    <td width=""560"" class=""esd-container-frame"" align=""center"" valign=""top"">
                                                                        <table cellpadding=""0"" cellspacing=""0"" width=""100%"">
                                                                            <tbody>
                                                                                <tr>
                                                                                    <td align=""center"" class=""esd-block-image"" style=""font-size:0""><img class=""adapt-img esdev-empty-img"" src=""data:image/png;base64,{{QRString}}"" alt width=""300"" height=""300""></td>
                                                                                </tr>
                                                                            </tbody>
                                                                        </table>
                                                                    </td>
                                                                </tr>
                                                            </tbody>
                                                        </table>
                                                    </td>
                                                </tr>
                                            </tbody>
                                        </table>
                                    </td>
                                </tr>
                            </tbody>
                        </table>
                    </td>
                </tr>
            </tbody>
        </table>
    </div>
</body>

</html>
";
        }
    }
}
