// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.EmailNotification.EmailInvitationEventData
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.EmailNotification
{
  [DataContract]
  public class EmailInvitationEventData
  {
    [DataMember]
    public string InvitingUserName { get; set; }

    [DataMember]
    public string InvitingUserEmail { get; set; }

    [DataMember]
    public string InvitedUserEmail { get; set; }

    [DataMember]
    public string AccountUrl { get; set; }

    [DataMember]
    public string AccountHost { get; set; }

    [DataMember]
    public string OpenInVsUri { get; set; }

    [DataMember]
    public string HeaderAccountText { get; set; }

    [DataMember]
    public string SupportLink { get; set; }

    [DataMember]
    public string AccountUrlDisplayName { get; set; }
  }
}
