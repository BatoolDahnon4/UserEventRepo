namespace eventRegistration
{
    public class EmailConfig
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public List<EmailAccountConfig> Accounts { get; set; }
    }

    public class EmailAccountConfig
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }

}

