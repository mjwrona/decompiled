// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.UsageDataAggregator
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.WindowsAzure.Management.Monitoring.ResourceProvider.DataContracts;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  internal abstract class UsageDataAggregator
  {
    protected TimeSpan timeGrain { get; set; }

    protected int RetryThreshold { get; set; } = 3;

    public abstract ResourceMetricResponses AggregateUsage(
      IVssRequestContext requestContext,
      IUsageEventsStore usageEventStore,
      DateTime startTime,
      DateTime endTime,
      IEnumerable<ResourceName> resourceNames);
  }
}
