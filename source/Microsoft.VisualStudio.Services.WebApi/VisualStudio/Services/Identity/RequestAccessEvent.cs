// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Identity.RequestAccessEvent
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Notifications;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Identity
{
  [DataContract]
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-sps-notifications.request-access-event")]
  public class RequestAccessEvent
  {
    [DataMember]
    public string Message { get; set; }

    [DataMember]
    public string RequesterName { get; set; }

    [DataMember]
    public string RequesterEmail { get; set; }

    [DataMember]
    public string OrganizationUrl { get; set; }

    [DataMember]
    public string OrganizationName { get; set; }

    [DataMember]
    public string SettingsUrl { get; set; }

    [DataMember]
    public string UrlRequested { get; set; }
  }
}
