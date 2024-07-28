// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.MailMessageFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class MailMessageFactory
  {
    public static Microsoft.TeamFoundation.Server.WebAccess.Mail.MailMessage CreateMailMessage(
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.MailMessage clientMessage)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.MailMessage>(clientMessage, nameof (clientMessage));
      return new Microsoft.TeamFoundation.Server.WebAccess.Mail.MailMessage()
      {
        Body = clientMessage.Body,
        CC = MailMessageFactory.CreateEmailRecipients(clientMessage.CC),
        InReplyTo = clientMessage.InReplyTo,
        MessageId = clientMessage.MessageId,
        ReplyTo = MailMessageFactory.CreateEmailRecipients(clientMessage.ReplyTo),
        Subject = clientMessage.Subject,
        To = MailMessageFactory.CreateEmailRecipients(clientMessage.To)
      };
    }

    public static Microsoft.TeamFoundation.Server.WebAccess.Mail.EmailRecipients CreateEmailRecipients(
      Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.EmailRecipients clientRecipients)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.EmailRecipients>(clientRecipients, nameof (clientRecipients));
      return new Microsoft.TeamFoundation.Server.WebAccess.Mail.EmailRecipients()
      {
        EmailAddresses = clientRecipients.EmailAddresses,
        TfIds = clientRecipients.TfIds,
        UnresolvedEntityIds = clientRecipients.UnresolvedEntityIds
      };
    }
  }
}
