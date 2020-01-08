using System;
using System.IO;
using System.Threading.Tasks;
using SendGrid.Helpers.Mail;

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
            //var emailMessage = $"A: {email}\nAsunto: {subject}\nMensaje: {message}\n\n";
            var apiKey = "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";
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
