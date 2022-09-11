namespace Shared;

using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Task1;

public interface IMailReadService : IMailService
{
    static string[] Scopes = { GmailService.Scope.MailGoogleCom };
    static string ApplicationName = "Gmail API Application";

    public static GmailService GetGmailService()
    {
        UserCredential credential;
        using (FileStream stream = new(Path.Combine(MailConfig.AssertDir, "client_secret.json"), FileMode.Open, FileAccess.Read))
        {
            string FolderPath = Convert.ToString(MailConfig.AssertDir);
            string FilePath = Path.Combine(FolderPath, "APITokenCredentials");

            credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                GoogleClientSecrets.FromStream(stream).Secrets,
                Scopes,
                "user",
                CancellationToken.None,
                new FileDataStore(FilePath, true)).Result;
        }
        // Create Gmail API service.
        GmailService service = new(new BaseClientService.Initializer()
        {
            HttpClientInitializer = credential,
            ApplicationName = ApplicationName,
        });
        return service;
    }



    public static async IAsyncEnumerable<MessagePartBody> GetValidAttachments(string userId, string messageId)
    {
        GmailService GServices = GetGmailService();
        Message message = await GServices.Users.Messages.Get(userId, messageId).ExecuteAsync();
        IList<MessagePart>? parts = message.Payload.Parts;
        if (parts is null)
            yield break;
        foreach (MessagePart part in parts)
        {
            if (!string.IsNullOrEmpty(part.Filename))
            {
                string attId = part.Body.AttachmentId;
                yield return await GServices.Users.Messages.Attachments.Get(userId, messageId, attId).ExecuteAsync();
            }
        }
    }

    public static async Task MarkAsRead(string userId, string MsgId)
    {
        //MESSAGE MARKS AS READ AFTER READING MESSAGE
        ModifyMessageRequest mods = new();
        mods.AddLabelIds = null;
        mods.RemoveLabelIds = new List<string> { "UNREAD" };
        await GetGmailService().Users.Messages.Modify(mods, userId, MsgId).ExecuteAsync();
    }

    Task Read();
}
