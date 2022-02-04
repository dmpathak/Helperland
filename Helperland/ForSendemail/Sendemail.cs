using Helperland.Models.ViewModels;
using System.Net;
using System.Net.Mail;


namespace Helperland.ForSendemail
{
    public class Sendemail
    {
        public void emailSend(EmailModel model)
        {
            MailMessage mm = new MailMessage();

            mm.To.Add(model.To);
            mm.From = new MailAddress("chachamehta33@gmail.com");
            mm.Subject = model.Subject;
            mm.Body = model.Body;
            if (model.Attachment != null)
            {
            mm.Attachments.Add(new Attachment(model.Attachment));
            }
            mm.IsBodyHtml = true;




            SmtpClient smtp = new SmtpClient();

            smtp.Host = "smtp.gmail.com";
            smtp.EnableSsl = true;
            NetworkCredential NetworkCred = new NetworkCredential("chachamehta33@gmail.com", "Chacha@2114");
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = NetworkCred;
            smtp.Port = 587;
            smtp.Send(mm);


        }
    }
}
