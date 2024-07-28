// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.TokenAdmin.Client.SshTokenRevokedEvent
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Notifications;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.TokenAdmin.Client
{
  [DataContract]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-token-notifications.ssh-revoked-event")]
  public class SshTokenRevokedEvent
  {
    [DataMember]
    public IList<string> SshNames { get; set; }

    [DataMember]
    public string AdminName { get; set; }

    [DataMember]
    public string AdminEmailAddress { get; set; }

    [DataMember]
    public string AdminEmailAddressMailTo { get; set; }

    [DataMember]
    public string AccountUrl { get; set; }

    [DataMember]
    public string ManageSshUrl { get; set; }
  }
}
