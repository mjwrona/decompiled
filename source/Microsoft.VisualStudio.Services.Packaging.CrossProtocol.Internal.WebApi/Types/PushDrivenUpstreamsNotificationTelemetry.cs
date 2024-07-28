// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types.PushDrivenUpstreamsNotificationTelemetry
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 208E7E0C-C249-4CB0-B738-E2A4534A31E8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.dll

using System;
using System.Runtime.Serialization;


#nullable enable
namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types
{
  public class PushDrivenUpstreamsNotificationTelemetry
  {
    [DataMember(EmitDefaultValue = false)]
    public string? PackageVersion { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? UpstreamFeedId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? UpstreamViewId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string? ExternalUpstreamLocation { get; set; }

    [DataMember]
    public TriggerCommitType TriggerCommitType { get; set; }

    [DataMember]
    public DateTime IngestionTimestamp { get; set; }

    [DataMember]
    public Guid UpstreamToFeedNotificationSendActivityId { get; set; }

    [DataMember]
    public DateTime UpstreamToFeedNotificationSendTimestamp { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? UpstreamToFeedNotificationReceiveActivityId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? UpstreamToFeedNotificationReceiveTimestamp { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public Guid? FeedToDownstreamTriggerActivityId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? FeedToDownstreamTriggerTimestamp { get; set; }
  }
}
