namespace Task2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Shared;

internal class MailReadService : IMailReadService
{
    private readonly SemaphoreSlim _lock = new SemaphoreSlim(1, 1);
    public async Task Read()
    {
        await GetAllValidMailAttactmentsContent(MailConfig.EmailToAddress);

    }

    private async Task GetAllValidMailAttactmentsContent(string userMail)
    {
        try
        {

            var gmailService = IMailReadService.GetGmailService();

            UsersResource.MessagesResource.ListRequest ListRequest = gmailService.Users.Messages.List(userMail);
            ListRequest.LabelIds = "INBOX";
            ListRequest.IncludeSpamTrash = false;
            //ListRequest.Q = "is:unread"; //ONLY FOR UNDREAD EMAIL'S...

            //GET ALL EMAILS
            ListMessagesResponse? ListResponse = await ListRequest.ExecuteAsync();

            if (ListResponse is null && ListResponse!.Messages is null)
                return;

            await Parallel.ForEachAsync(ListResponse.Messages, async (Msg, _) =>
                  {

                      UsersResource.MessagesResource.GetRequest Message = gmailService.Users.Messages.Get(userMail, Msg.Id);

                      //MAKE ANOTHER REQUEST FOR THAT EMAIL ID...
                      Message MsgContent = await Message.ExecuteAsync();

                      if (MsgContent is null)
                          return;

                      //LOOP THROUGH THE HEADERS AND GET THE FIELDS WE NEED (SUBJECT, MAIL)
                      foreach (var MessageParts in MsgContent.Payload.Headers)
                      {
                          if (!(MessageParts.Name == "Subject" && MessageParts.Value.StartsWith("PatientReport_")))
                              continue;
                          await _lock.WaitAsync();
                          await ShowDesireResult(userMail, Msg, MessageParts);
                          _lock.Release();
                          break;
                      }
                      return;
                  });
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error: " + ex);
            throw;
        }
    }

    private static async Task ShowDesireResult(string userMail, Message Msg, MessagePartHeader MessageParts)
    {

        await IMailReadService.MarkAsRead(userMail, Msg.Id);
        Console.WriteLine("Subject: " + MessageParts.Value);
        Console.WriteLine("--------------------------------------------------------");

        var attachmentContent = IMailReadService.GetValidAttachments(userMail, Msg.Id);
        await foreach (var item in attachmentContent)
        {
            var contentBytes = Convert.FromBase64String(item.Data);
            var content = System.Text.Encoding.ASCII.GetString(contentBytes);
            Console.WriteLine(content);
        }
        Console.WriteLine("--------------------------------------------------------");

    }
}

