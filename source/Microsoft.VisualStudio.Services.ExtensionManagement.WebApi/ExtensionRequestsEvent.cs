// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionRequestsEvent
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A4FCC2C3-B106-43A6-A409-E4BF8CFC545C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.dll

using Microsoft.VisualStudio.Services.Gallery.WebApi;
using Microsoft.VisualStudio.Services.Notifications;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.WebApi
{
  [NotificationEventBindings(EventSerializerType.Json, "ms.vss-extmgmt-web.extension-requests-event")]
  [DataContract]
  public class ExtensionRequestsEvent
  {
    [DataMember]
    public ExtensionRequestUpdateType UpdateType { get; set; }

    [DataMember]
    public List<ExtensionRequest> Requests { get; set; }

    [DataMember]
    public PublishedExtension Extension { get; set; }

    [DataMember]
    public ExtensionHost Host { get; set; }

    [DataMember]
    public ExtensionRequestUrls Links { get; set; }
  }
}
