using Microsoft.AspNetCore.Mvc;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;
using static QRCoder.PayloadGenerator;

namespace eventRegistration.Controllers
{
    public class HomeController : Controller
    {

        [HttpGet]
        public IActionResult CreateQRCode()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult CreateQRCode(Guest QRCodeText)
        {
            QRCodeGenerator QrGenerator = new QRCodeGenerator();
            //QRCodeText:
            //[
            //    { Name: QRCodeText.Name, Position: QRCodeText.Position, Email: QRCodeText.Email,CompanyName: QRCodeText.CompanyName,PhoneNumber: QRCodeText.PhoneNumber,Source:QRCodeText.Source}
            //    ];

            QRCodeData QrCodeInfo = QrGenerator.CreateQrCode(QRCodeText.Name, QRCodeGenerator.ECCLevel.Q);
            QRCode QrCode = new QRCode(QrCodeInfo);
            Bitmap QrBitmap = QrCode.GetGraphic(60);
            byte[] BitmapArray = QrBitmap.BitmapToByteArray();
            string QrUri = string.Format("data:image/png;base64,{0}", Convert.ToBase64String(BitmapArray));
            ViewBag.QrCodeUri = QrUri;
            return View();
        }
    }

    //Extension method to convert Bitmap to Byte Array
    public static class BitmapExtension
    {
        public static byte[] BitmapToByteArray(this Bitmap bitmap)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }
    }
   
}
//using Microsoft.AspNetCore.Mvc;

//namespace eventRegistration
//{
//    public class HomeController : Controller
//    {
//        public IActionResult Index()
//        {
//            return View();
//        }
//    }
//}
