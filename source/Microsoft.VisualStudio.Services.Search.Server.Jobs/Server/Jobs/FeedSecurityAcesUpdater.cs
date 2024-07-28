// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.FeedSecurityAcesUpdater
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.FaultManagement;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class FeedSecurityAcesUpdater : SecurityAcesUpdaterBase
  {
    public FeedSecurityAcesUpdater(ExecutionContext executionContext)
      : base(executionContext)
    {
      this.SecurityHashComputationKpiName = "FeedSecurityHashComputationTime";
    }

    protected override void LoadSecurityNamespace(ExecutionContext executionContext) => SecurityChecksUtils.LoadRemoteSecurityNamespace(this.ExecutionContext.RequestContext, CommonConstants.FeedSecurityNamespaceGuid);

    public virtual List<PackageContainer> UpdateFeedsWithSecurityHashAndToken(
      List<PackageContainer> feeds)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      Func<List<PackageContainer>> function = (Func<List<PackageContainer>>) (() => SecurityChecksUtils.UpdateFeedSecurityHashAndToken(this.ExecutionContext.RequestContext, feeds));
      try
      {
        TimeSpan timeSpan1 = TimeSpan.FromSeconds((double) this.ExecutionContext.RequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/CssNodeSecurityAcesUpdateTimeoutInSeconds"));
        TimeSpan timeSpan2 = TimeSpan.FromSeconds(30.0);
        return GenericInvoker.Instance.InvokeWithFaultCheck<List<PackageContainer>>(function, this.ExecutionContext.FaultService, (int) (timeSpan1.TotalSeconds / timeSpan2.TotalSeconds), (int) timeSpan2.TotalMilliseconds, new TraceMetaData(1080055, "Indexing Pipeline", "SecurityChecks"));
      }
      finally
      {
        stopwatch.Stop();
        Tracer.PublishKpi(this.SecurityHashComputationKpiName, "Indexing Pipeline", (double) stopwatch.ElapsedMilliseconds);
      }
    }
  }
}
