// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tokens.PATAddedEvent
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Notifications;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Tokens
{
  [DataContract]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-token-notifications.pat-added-event")]
  public class PATAddedEvent
  {
    [DataMember]
    public string Name { get; set; }

    [DataMember]
    public string Scopes { get; set; }

    [DataMember]
    public string Expiration { get; set; }

    [DataMember]
    public string AccountUrl { get; set; }

    [DataMember]
    public string IPAddress { get; set; }

    [DataMember]
    public string UserAgent { get; set; }

    [DataMember]
    public string AdditionalInfo { get; set; }

    [DataMember]
    public string OrganizationName { get; set; }
  }
}
