// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.MSGraph.Client.Invitation
// Assembly: Microsoft.VisualStudio.Services.Cloud.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 52A8E326-8E84-4175-AE92-8ED7AF376B63
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.WebApi.dll

namespace Microsoft.VisualStudio.Services.MSGraph.Client
{
  public class Invitation
  {
    public string InvitedUserDisplayName { get; set; }

    public string InvitedUserEmailAddress { get; set; }

    public bool SendInvitationMessage { get; set; }

    public string InviteRedirectUrl { get; set; }

    public Invitation(
      string invitedUserEmailAddress,
      string invitedUserDisplayName,
      bool sendInvitationMessage,
      string inviteRedirectUrl)
    {
      this.InvitedUserEmailAddress = invitedUserEmailAddress;
      this.InvitedUserDisplayName = invitedUserDisplayName;
      this.SendInvitationMessage = sendInvitationMessage;
      this.InviteRedirectUrl = inviteRedirectUrl;
    }
  }
}
