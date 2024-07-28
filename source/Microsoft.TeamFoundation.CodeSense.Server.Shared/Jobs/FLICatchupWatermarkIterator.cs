// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Jobs.FLICatchupWatermarkIterator
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
  public class FLICatchupWatermarkIterator : WatermarkIterator
  {
    private IVssRegistryService registryService;

    public FLICatchupWatermarkIterator(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      ITfsVersionControlClient tfsVersionControlClient,
      DateTime queueTime)
      : base(requestContext, jobDefinition, tfsVersionControlClient, queueTime, FLICatchupWatermarkIterator.GetFLICatchupRuntime(requestContext))
    {
      this.SliceSource = SliceSource.FLICatchup;
      this.registryService = requestContext.GetService<IVssRegistryService>();
      if (this.registryService.GetFLICatchupStartMark(requestContext) == 0)
      {
        Watermark returnWatermark = new Watermark(WatermarkKind.High);
        if (this.registryService.TryGetWatermark(requestContext, WatermarkKind.High, out returnWatermark))
        {
          int changesetId = returnWatermark.ChangesetId + 1;
          this.registryService.SetFLICatchupStartMark(requestContext, changesetId);
          this.registryService.SetWatermark(requestContext, WatermarkKind.FLI, changesetId);
        }
      }
      if (this.registryService.GetFLICatchupEndMark(requestContext) != 0)
        return;
      Watermark returnWatermark1 = new Watermark(WatermarkKind.Low);
      if (!this.registryService.TryGetWatermark(requestContext, WatermarkKind.Low, out returnWatermark1))
        return;
      this.registryService.SetFLICatchupEndMark(requestContext, returnWatermark1.ChangesetId);
    }

    public override int RetryCount
    {
      get
      {
        Watermark returnWatermark = new Watermark(WatermarkKind.FLI);
        if (this.registryService.TryGetWatermark(this.requestContext, WatermarkKind.FLI, out returnWatermark))
          return returnWatermark.RetryCount;
        this.requestContext.Trace(1025625, TraceLevel.Error, "CodeSense", TraceLayer.Job, "Could not get the FLI watermark retry count");
        return 0;
      }
      set
      {
        Watermark returnWatermark = new Watermark(WatermarkKind.FLI);
        if (this.registryService.TryGetWatermark(this.requestContext, WatermarkKind.FLI, out returnWatermark))
          this.registryService.SetWatermark(this.requestContext, new Watermark(WatermarkKind.FLI, returnWatermark.ChangesetId, value));
        else
          this.requestContext.Trace(1025660, TraceLevel.Error, "CodeSense", TraceLayer.Job, "Could not set the FLI watermark retry count");
      }
    }

    protected override IEnumerable<int> LoadChangesets(int numChangesets)
    {
      Watermark returnWatermark = new Watermark(WatermarkKind.FLI);
      if (this.registryService.TryGetWatermark(this.requestContext, WatermarkKind.FLI, out returnWatermark))
      {
        int fliCatchupEndMark = this.registryService.GetFLICatchupEndMark(this.requestContext);
        int start = Math.Max(fliCatchupEndMark, returnWatermark.ChangesetId - numChangesets);
        this.Complete = start == fliCatchupEndMark;
        int count = Math.Min(numChangesets, returnWatermark.ChangesetId - start);
        if (count > 0)
          return (IEnumerable<int>) Enumerable.Range(start, count).OrderByDescending<int, int>((Func<int, int>) (i => i));
      }
      return Enumerable.Empty<int>();
    }

    protected override void UpdateWatermark(int lastSuccessfulChangeset) => this.registryService.SetWatermark(this.requestContext, WatermarkKind.FLI, lastSuccessfulChangeset);

    public void OnRetentionRuleFail()
    {
      this.Complete = true;
      this.UpdateChangesetIds();
      this.AppendToStatus("Earliest change set within retention period of {0} month(s) has been processed", (object) this.registryService.GetRetentionPeriod(this.requestContext));
    }

    public void OnStorageRuleFail()
    {
      this.UpdateChangesetIds();
      this.AppendToStatus("Storage growth exceeds the limit {0} GB", (object) this.registryService.GetStorageGrowthLimit(this.requestContext));
    }

    private void UpdateChangesetIds()
    {
      if (!this.changesetIDs.Any<int>() || this.changesetIDs.Min() > this.CurrentChangesetId || this.CurrentChangesetId > this.changesetIDs.Max())
        return;
      this.changesetIDs = this.changesetIDs.Take<int>(this.changesetIDs.First<int>() - this.CurrentChangesetId);
    }

    private static TimeSpan GetFLICatchupRuntime(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetFLICatchupRuntime(requestContext);
  }
}
