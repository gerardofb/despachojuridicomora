using System;
using System.IO;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;
using BALWDXDespachoMora;

namespace WDXWebApiDespachoJuridico
{
    public interface IMessageService
    {
        Task<bool> Send(string email, string subject, string message, string messageHtml);
    }

    public class FileMessageService : IMessageService
    {
        async Task<bool> IMessageService.Send(string email, string subject, string message, string messageHtml)
        {
            const string LlaveSendGridApi = "ApiKeySendGrid";

            var apiKey = String.Empty;
            using(ConfiguracionBAL Metodo = new ConfiguracionBAL())
            {
                apiKey = Metodo.ConsultarValorConfiguracion(LlaveSendGridApi);
            }
            var client = new SendGrid.SendGridClient(apiKey);
            var from = new EmailAddress("despachojuridicomora@gmail.com", "Despacho Jurídico Gómez Mora");
            var to = new EmailAddress(email);
            var plainText = $@"Este es un correo autogenerado del Despacho Jurídico Gómez Mora. 
                No es necesario responder a este correo. {message}";
            var htmlText = $@"<h2>Este es un correo autogenerado del Despacho Jurídico Gómez Mora. 
                No es necesario responder a este correo. </h2> {messageHtml}";
            var emailMessage = MailHelper.CreateSingleEmail(from, to, subject, plainText, htmlText);
            var response = await client.SendEmailAsync(emailMessage);
            //File.AppendAllText("emails.txt", emailMessage);

            return true;
        }
    }
}
