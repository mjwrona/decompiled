// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Jobs.KeepupWatermarkIterator
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
  public class KeepupWatermarkIterator : WatermarkIterator
  {
    private const int KeepupBatchSize = 1;
    private readonly IVssRegistryService registryService;
    private readonly ITfsVersionControlClient tfsVersionControlClient;

    public KeepupWatermarkIterator(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      ITfsVersionControlClient tfsVersionControlClient,
      DateTime queueTime)
      : base(requestContext, jobDefinition, tfsVersionControlClient, queueTime)
    {
      this.SliceSource = SliceSource.Keepup;
      this.registryService = requestContext.GetService<IVssRegistryService>();
      this.tfsVersionControlClient = tfsVersionControlClient;
    }

    public override int RetryCount
    {
      get
      {
        Watermark returnWatermark = new Watermark(WatermarkKind.High);
        if (this.registryService.TryGetWatermark(this.requestContext, WatermarkKind.High, out returnWatermark))
          return returnWatermark.RetryCount;
        this.requestContext.Trace(1025645, TraceLevel.Error, "CodeSense", TraceLayer.Job, "Could not get the high watermark retry count");
        return 0;
      }
      set
      {
        Watermark returnWatermark = new Watermark(WatermarkKind.High);
        if (this.registryService.TryGetWatermark(this.requestContext, WatermarkKind.High, out returnWatermark))
          this.registryService.SetWatermark(this.requestContext, new Watermark(WatermarkKind.High, returnWatermark.ChangesetId, value));
        else
          this.requestContext.Trace(1025620, TraceLevel.Error, "CodeSense", TraceLayer.Job, "Could not set the high watermark retry count");
      }
    }

    protected override void UpdateWatermark(int lastSuccessfulChangeset) => this.registryService.SetWatermark(this.requestContext, WatermarkKind.High, lastSuccessfulChangeset);

    protected override IEnumerable<int> LoadChangesets(int numChangesets)
    {
      Watermark returnWatermark = new Watermark(WatermarkKind.High);
      if (this.registryService.TryGetWatermark(this.requestContext, WatermarkKind.High, out returnWatermark))
      {
        int start = returnWatermark.ChangesetId + 1;
        int num = this.LatestChangeset();
        int count = Math.Min(numChangesets, num - start + 1);
        this.Complete = start + count > num;
        return count > 0 ? Enumerable.Range(start, count) : Enumerable.Empty<int>();
      }
      this.requestContext.Trace(1025640, TraceLevel.Error, "CodeSense", TraceLayer.Job, "Keepup failed to load high watermark");
      return Enumerable.Empty<int>();
    }

    private int LatestChangeset() => this.tfsVersionControlClient.GetLatestChangesetNumber(this.requestContext);
  }
}
