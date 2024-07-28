// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ReportingEvent
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.VisualStudio.Services.Commerce
{
  public class ReportingEvent
  {
    public ReportingEvent() => this.Properties = new SerializableDictionary<string, string>();

    public string EventId { get; set; }

    public DateTime EventTime { get; set; }

    public string EventName { get; set; }

    public Guid OrganizationId { get; set; }

    public string OrganizationName { get; set; }

    public Guid CollectionId { get; set; }

    public string CollectionName { get; set; }

    public string Environment { get; set; }

    public Guid UserIdentity { get; set; }

    public Guid ServiceIdentity { get; set; }

    public string Version { get; set; }

    public SerializableDictionary<string, string> Properties { get; set; }
  }
}
