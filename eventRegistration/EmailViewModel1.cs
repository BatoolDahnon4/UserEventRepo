using System.Text;

namespace MedcorSL.Services
{
    public class EmailViewModel
    {
        public string ToAdressTitle;

        public string ToAddress { get; set; }
        public object Subject { get; set; }
        public string Body { get; set; }
        //public byte[] Image { get; set; }
    }
}