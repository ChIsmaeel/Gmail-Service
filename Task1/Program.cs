
using Task1;
try
{

    IMailSendService _mailSendService = new MailSendService();
    _mailSendService.Send();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    throw;
}