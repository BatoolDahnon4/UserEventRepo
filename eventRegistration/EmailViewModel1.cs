using System.Text;

namespace MedcorSL.Services
{
    public class EmailViewModel
    {
        public string ToAdressTitle;

        public string ToAddress { get; set; }
        public object Subject { get; set; }
        public object Body { get; set; }
    }
}