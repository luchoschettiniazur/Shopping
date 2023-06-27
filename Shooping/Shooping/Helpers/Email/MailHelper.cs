using MailKit.Net.Smtp;
using MimeKit;
using Shooping.Common;
using Shooping.Helpers.Email;

namespace Shooping.Helpers
{
    public class MailHelper : IMailHelper
    {
        private readonly IConfiguration _configuration;

        public MailHelper(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        //string toName -> nombre de la persona a la que se lo vamos a enviar.
        //string toEmail -> Email de la persona a la que se lo vamos a enviar.
        //string subject -> El titulo del correo.
        //string body -> El contenido del correo.
        public Response SendMail(string toName, string toEmail, string subject, string body)
        {
            try
            {
                string from = _configuration["Mail:From"]!;
                string name = _configuration["Mail:Name"]!;
                string smtp = _configuration["Mail:Smtp"]!;
                string port = _configuration["Mail:Port"]!;
                string password = _configuration["Mail:Password"]!;

                MimeMessage message = new MimeMessage();
                message.From.Add(new MailboxAddress(name, from));
                message.To.Add(new MailboxAddress(toName, toEmail));
                message.Subject = subject;
                BodyBuilder bodyBuilder = new BodyBuilder
                {
                    HtmlBody = body
                };
                message.Body = bodyBuilder.ToMessageBody();

                using (SmtpClient client = new SmtpClient()) //-> using MailKit.Net.Smtp;
                {
                    client.Connect(smtp, int.Parse(port!), false);
                    client.Authenticate(from, password);
                    client.Send(message);
                    client.Disconnect(true);
                }

                return new Response { IsSuccess = true };

            }
            catch (Exception ex)
            {
                return new Response
                {
                    IsSuccess = false,
                    Message = ex.Message,
                    Result = ex
                };
            }
        }
    }
}
