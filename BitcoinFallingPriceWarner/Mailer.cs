using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinFallingPriceWarner
{
    /// <summary>
    /// Mailer for sendings Mails (Warning, and Errors)
    /// </summary>
    public class Mailer
    {
        static readonly string host = Controller.Settings.SMTPserver.Host;
        static readonly int port = Controller.Settings.SMTPserver.Port;
        static readonly bool ssl = Controller.Settings.SMTPserver.SSL_true_false;
        static readonly string user = Controller.Settings.SMTPserver.User;
        static readonly string password = Controller.Settings.SMTPserver.Password;
        static readonly string receipient = Controller.Settings.MailReceipientForWarning;


        public static void sendEmail(string body)
        {
            var mailMessage = new MimeMessage();

            mailMessage.From.Add(new MailboxAddress(user, user));
            mailMessage.To.Add(new MailboxAddress(receipient, receipient));
            mailMessage.Subject = "BitcoinFallingPriceWarner - Mail";
            mailMessage.Body = new TextPart("plain")
            {
                Text = body
            };

            using (var smtpClient = new SmtpClient())
            {
                smtpClient.Connect(host, port, ssl);
                smtpClient.Authenticate(user, password);
                smtpClient.Send(mailMessage);
                smtpClient.Disconnect(true);
            }
        }
        
    }
}
