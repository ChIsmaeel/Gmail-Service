namespace Task1;

using Shared;
using System.Net;
using System.Net.Mail;

internal class MailSendService : IMailSendService
{
    SmtpClient _smtpClient = new(MailConfig.SmtpAddress)
    {
        EnableSsl = MailConfig.EnableSSL,
        Port = MailConfig.PortNumber,
        Credentials = new NetworkCredential(MailConfig.EmailFromAddress, MailConfig.Password)
    };

    public void Send()
    {
        Console.WriteLine("Start connecting Mail...");
        SendMessage();
    }



    public void SendMessage()
    {
        Console.WriteLine("Start Adding Mail Message...");
        MailMessage message = GetMailMessage();
        Console.WriteLine("sending Mail...");
        _smtpClient.Send(message);
        Console.WriteLine("Email Sent.");
    }

    private static MailMessage GetMailMessage()
    {
        var inMemoryFileInfo = DummyFileGenerator.Generate();
        var contentBytes = System.Text.Encoding.ASCII.GetBytes(inMemoryFileInfo.Content);


        var memoryStream = new MemoryStream(contentBytes);
        MailMessage message = new()
        {
            Sender = new MailAddress(MailConfig.EmailFromAddress, "PatientReport_" + inMemoryFileInfo.FileName),
            From = new MailAddress(MailConfig.EmailFromAddress, "PatientReport_" + inMemoryFileInfo.FileName)
        };
        Attachment attachment = new(memoryStream, inMemoryFileInfo.FileName, "text/plain");
        message.Attachments.Add(attachment);
        message.To.Add(new MailAddress(MailConfig.EmailToAddress, "PatientReport_" + inMemoryFileInfo.FileName));

        message.Subject = "PatientReport_" + inMemoryFileInfo.FileName;
        return message;
    }
}
