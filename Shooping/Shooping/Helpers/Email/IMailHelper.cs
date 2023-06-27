using Shooping.Common;

namespace Shooping.Helpers.Email;

public interface IMailHelper
{
    //string toName -> nombre de la persona a la que se lo vamos a enviar.
    //string toEmail -> Email de la persona a la que se lo vamos a enviar.
    //string subject -> El titulo del correo.
    //string body -> El contenido del correo.
    Response SendMail(string toName, string toEmail, string subject, string body);


}
