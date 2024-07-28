// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Jobs.CatchupWatermarkIterator
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server.Jobs
{
  public class CatchupWatermarkIterator : WatermarkIterator
  {
    private readonly IVssRegistryService registryService;
    private readonly ITfsVersionControlClient tfsVersionControlClient;

    public int LowerBound { get; private set; }

    public CatchupWatermarkIterator(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      ITfsVersionControlClient tfsVersionControl,
      DateTime queueTime)
      : base(requestContext, jobDefinition, tfsVersionControl, queueTime, CatchupWatermarkIterator.GetCatchupRuntime(requestContext))
    {
      this.SliceSource = SliceSource.Catchup;
      this.registryService = requestContext.GetService<IVssRegistryService>();
      this.tfsVersionControlClient = tfsVersionControl;
      this.LowerBound = 1;
    }

    public override int RetryCount
    {
      get
      {
        Watermark returnWatermark = new Watermark(WatermarkKind.Low);
        if (this.registryService.TryGetWatermark(this.requestContext, WatermarkKind.Low, out returnWatermark))
          return returnWatermark.RetryCount;
        this.requestContext.Trace(1025635, TraceLevel.Error, "CodeSense", TraceLayer.Job, "Could not get the low watermark retry count");
        return 0;
      }
      set
      {
        Watermark returnWatermark = new Watermark(WatermarkKind.Low);
        if (this.registryService.TryGetWatermark(this.requestContext, WatermarkKind.Low, out returnWatermark))
          this.registryService.SetWatermark(this.requestContext, new Watermark(WatermarkKind.Low, returnWatermark.ChangesetId, value));
        else
          this.requestContext.Trace(1025615, TraceLevel.Error, "CodeSense", TraceLayer.Job, "Could not set the low watermark retry count");
      }
    }

    protected override void UpdateWatermark(int lastSuccessfulChangeset)
    {
      this.registryService.SetWatermark(this.requestContext, WatermarkKind.Low, lastSuccessfulChangeset);
      this.registryService.UpdateIndexingStatus(this.requestContext, this.tfsVersionControlClient, lastSuccessfulChangeset, false);
    }

    protected override IEnumerable<int> LoadChangesets(int numChangesets)
    {
      Watermark returnWatermark = new Watermark(WatermarkKind.Low);
      if (this.registryService.TryGetWatermark(this.requestContext, WatermarkKind.Low, out returnWatermark))
      {
        int start = Math.Max(this.LowerBound, returnWatermark.ChangesetId - numChangesets);
        this.Complete = start == this.LowerBound;
        int count = Math.Min(numChangesets, returnWatermark.ChangesetId - start);
        return count > 0 ? (IEnumerable<int>) Enumerable.Range(start, count).OrderByDescending<int, int>((Func<int, int>) (i => i)) : Enumerable.Empty<int>();
      }
      this.requestContext.Trace(1025630, TraceLevel.Error, "CodeSense", TraceLayer.Job, "Catchup failed to load low watermark");
      return Enumerable.Empty<int>();
    }

    private static TimeSpan GetCatchupRuntime(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetCatchupRuntime(requestContext);

    private void UpdateChangesetIds()
    {
      if (!this.changesetIDs.Any<int>() || this.changesetIDs.Min() > this.CurrentChangesetId || this.CurrentChangesetId > this.changesetIDs.Max())
        return;
      this.changesetIDs = this.changesetIDs.Take<int>(this.changesetIDs.First<int>() - this.CurrentChangesetId);
    }

    public void OnRetentionRuleFail()
    {
      this.LowerBound = this.CurrentChangesetId + 1;
      this.Complete = true;
      this.UpdateChangesetIds();
      this.AppendToStatus("Earliest change set id {0} within retention period of {1} month(s) has been processed", (object) this.LowerBound, (object) this.registryService.GetRetentionPeriod(this.requestContext));
    }

    public void OnStorageLimitRuleFail()
    {
      this.UpdateChangesetIds();
      this.AppendToStatus("Storage growth exceeds the limit {0} GB", (object) this.registryService.GetStorageGrowthLimit(this.requestContext));
    }
  }
}
