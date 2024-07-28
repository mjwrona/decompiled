// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types.NotifyOfPackagesIngestedInUpstreamsParameters
// Assembly: Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 208E7E0C-C249-4CB0-B738-E2A4534A31E8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.dll

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;

namespace Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types
{
  [DataContract]
  public class NotifyOfPackagesIngestedInUpstreamsParameters
  {
    [DataMember(IsRequired = true)]
    [Required]
    public List<Guid> DownstreamFeeds { get; set; }

    [DataMember(IsRequired = true)]
    [Required]
    public string PackageName { get; set; }

    [DataMember]
    public PushDrivenUpstreamsNotificationTelemetry PushDrivenUpstreamsTelemetry { get; set; }
  }
}
