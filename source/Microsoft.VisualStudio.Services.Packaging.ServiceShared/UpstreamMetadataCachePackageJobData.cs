// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamMetadataCachePackageJobData
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.CrossProtocol.Internal.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared
{
  public class UpstreamMetadataCachePackageJobData
  {
    [Obsolete("For serialization only")]
    public UpstreamMetadataCachePackageJobData()
    {
    }

    public UpstreamMetadataCachePackageJobData(
      IEnumerable<Guid> feedsToRefresh,
      string normalizedPackageName,
      PushDrivenUpstreamsNotificationTelemetry pushDrivenUpstreamsTelemetry)
    {
      this.FeedsToRefresh = feedsToRefresh.ToList<Guid>();
      this.NormalizedPackageName = normalizedPackageName;
      this.PushDrivenUpstreamsTelemetry = pushDrivenUpstreamsTelemetry;
    }

    public List<Guid> FeedsToRefresh { get; set; }

    public string NormalizedPackageName { get; set; }

    public PushDrivenUpstreamsNotificationTelemetry PushDrivenUpstreamsTelemetry { get; set; }
  }
}
