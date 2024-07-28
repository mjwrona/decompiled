// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking.UpstreamStatusRecord
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamFailureTracking
{
  public class UpstreamStatusRecord
  {
    public UpstreamKey Upstream { get; }

    public FullRefreshStatusRecord FullRefreshStatus { get; }

    public IReadOnlyList<PartialRefreshStatusRecord> PartialRefreshStatuses { get; }

    public DateTime TimeRangeStart { get; }

    public DateTime TimeRangeEnd { get; }

    public UpstreamStatusRecord(
      UpstreamKey upstream,
      FullRefreshStatusRecord fullRefreshStatus,
      IEnumerable<PartialRefreshStatusRecord> partialRefreshStatuses,
      DateTime timeRangeStart,
      DateTime timeRangeEnd)
    {
      this.Upstream = upstream;
      this.FullRefreshStatus = fullRefreshStatus;
      this.TimeRangeStart = timeRangeStart;
      this.TimeRangeEnd = timeRangeEnd;
      this.PartialRefreshStatuses = (IReadOnlyList<PartialRefreshStatusRecord>) partialRefreshStatuses.ToImmutableList<PartialRefreshStatusRecord>();
    }
  }
}
