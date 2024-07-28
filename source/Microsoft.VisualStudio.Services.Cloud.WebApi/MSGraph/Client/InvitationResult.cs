// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MSGraph.Client.InvitationResult
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

using Newtonsoft.Json;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.MSGraph.Client
{
  public class InvitationResult
  {
    public string Id { get; set; }

    public string InviteRedeemUrl { get; set; }

    public string InvitedUserDisplayName { get; set; }

    public string InvitedUserEmailAddress { get; set; }

    public bool SendInvitationMessageInfo { get; set; }

    public InvitationResult.MessageInfo InvitedUserMessageInfo { get; set; }

    public string InviteRedirectUrl { get; set; }

    public string Status { get; set; }

    public InvitationResult.InvitedUserObject InvitedUser { get; set; }

    public InvitationResult.InvitationError Error { get; set; }

    public class EmailAddress
    {
      public string Name { get; set; }

      public string Address { get; set; }
    }

    public class MessageInfo
    {
      public string MessageLanguage { get; set; }

      public List<InvitationResult.EmailAddress> CcRecipients { get; set; }

      public string CustomizedMessageBody { get; set; }
    }

    public class InvitedUserObject
    {
      public string Id { get; set; }
    }

    public class InvitationError
    {
      public string Code { get; set; }

      public string Message { get; set; }

      public object InnerError { get; set; }

      public override string ToString() => JsonConvert.SerializeObject((object) this);
    }
  }
}
