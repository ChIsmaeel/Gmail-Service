
using Shared;
using Task2;
try
{

    IMailReadService _mailSendService = new MailReadService();
    await _mailSendService.Read();
}
catch (Exception e)
{
    Console.WriteLine(e.Message);
    throw;
}