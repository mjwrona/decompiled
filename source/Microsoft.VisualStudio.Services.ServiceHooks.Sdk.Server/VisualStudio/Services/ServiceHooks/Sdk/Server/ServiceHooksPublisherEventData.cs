// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.ServiceHooksPublisherEventData
// Assembly: Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3575E571-FF3A-4E7B-A8CC-64FFB01E8C91
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server.dll

using Microsoft.VisualStudio.Services.ServiceHooks.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ServiceHooks.Sdk.Server
{
  public class ServiceHooksPublisherEventData
  {
    public string EventType { get; set; }

    public SortedList<string, object> Payloads { get; set; }

    public Dictionary<string, ResourceContainer> ResourceContainers { get; set; }

    public FormattedEventMessage Message { get; set; }

    public FormattedEventMessage DetailedMessage { get; set; }

    public IDictionary<string, string> Diagnostics { get; set; }

    public IDictionary<string, string> NotificationData { get; set; }
  }
}
