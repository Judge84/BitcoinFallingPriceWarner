using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinFallingPriceWarner
{
    public class Settings
    {
        public int TimerInMinutes { get; set; }
        public string UrlToGetInformation { get; set; }
        public int AmountDifferenceForSendingWarning { get; set; }
        public SMTPserverSettings SMTPserver { get; set; }
        public string MailReceipientForWarning { get; set; }

    }

    public class SMTPserverSettings
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public bool SSL_true_false { get; set; }
        public string User {get; set; }
        public string Password { get; set; }
    }
}
