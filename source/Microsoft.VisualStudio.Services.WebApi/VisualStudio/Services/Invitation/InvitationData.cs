// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Invitation.InvitationData
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Invitation
{
  [DataContract]
  public class InvitationData
  {
    [DataMember]
    public InvitationType InvitationType { get; set; } = InvitationType.AccountInvite;

    [DataMember]
    public Guid SenderId { get; set; } = Guid.Empty;

    [DataMember]
    public List<Invitee> Invitees { get; set; }

    [DataMember]
    public Dictionary<string, string> Attributes { get; set; } = new Dictionary<string, string>();
  }
}
