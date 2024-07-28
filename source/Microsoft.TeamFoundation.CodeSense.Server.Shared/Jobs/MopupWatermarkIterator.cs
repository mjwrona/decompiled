// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Jobs.MopupWatermarkIterator
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.CodeSense.Server.Data;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.CodeSense.Server.Jobs
{
  public class MopupWatermarkIterator : WatermarkIterator
  {
    private readonly IVssRegistryService registryService;
    private readonly ITfsVersionControlClient tfsVersionControlClient;
    private List<int> changesetIds;
    private List<int> mopupProcessedChangesets;
    private int retryCount;
    private StringBuilder statusBuilder = new StringBuilder(64);

    public MopupWatermarkIterator(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      ITfsVersionControlClient tfsVersionControlClient,
      DateTime queueTime)
      : base(requestContext, jobDefinition, tfsVersionControlClient, queueTime, MopupWatermarkIterator.GetMopupRuntime(requestContext))
    {
      this.SliceSource = SliceSource.Mopup;
      this.registryService = requestContext.GetService<IVssRegistryService>();
      this.tfsVersionControlClient = tfsVersionControlClient;
      this.mopupProcessedChangesets = new List<int>();
      this.retryCount = 0;
      this.changesetIds = new List<int>();
      using (requestContext.AcquireExemptionLock())
      {
        using (ICodeSenseSqlResourceComponent component = this.requestContext.CreateComponent<ICodeSenseSqlResourceComponent, CodeSenseSqlResourceComponent>())
          this.changesetIds = component.GetMopupQueueItems().OrderByDescending<int, int>((Func<int, int>) (i => i)).ToList<int>();
      }
    }

    public override int RetryCount
    {
      get => this.retryCount;
      set => this.retryCount = value;
    }

    public override void HandleException(Exception e)
    {
      string format = string.Format("Changeset processing failed in for the changeset {0} from the MopupQueue with exception : {1}", (object) this.CurrentChangesetId, (object) e);
      this.requestContext.Trace(1025030, TraceLayer.Job, format);
    }

    public override void SkipItem(int changesetId)
    {
      this.requestContext.Trace(1025045, TraceLayer.Job, string.Format("Mopup failed processing changeset Id {0} with slice souce {1}", (object) changesetId, (object) this.SliceSource));
      this.droppedChangesetIds.Add(changesetId);
    }

    public override void AppendToProcessedList(int changesetId)
    {
      this.processedChangesetIds.Add(changesetId);
      this.mopupProcessedChangesets.Add(changesetId);
      this.requestContext.LogKPI(CodeLensKpiArea.CodeLensService, CodeLensKpiName.ChangesetProcessedByMopup);
      this.requestContext.Trace(1025050, TraceLayer.Job, string.Format("Mopup job finished processing skipped changeset {0} from the MopupQueue", (object) changesetId));
    }

    protected override void UpdateWatermark(int lastSuccessfulChangeset) => this.changesetIds.Remove(lastSuccessfulChangeset);

    protected override IEnumerable<int> LoadChangesets(int numChangesets)
    {
      this.UdpateMopupQueueAndWaterMarks();
      if (!this.changesetIds.Any<int>())
        return Enumerable.Empty<int>();
      return this.changesetIds.Count<int>() >= numChangesets ? this.changesetIds.Take<int>(numChangesets) : (IEnumerable<int>) this.changesetIds;
    }

    public override string Status()
    {
      this.statusBuilder.Insert(0, string.Format("Processing {0} skipped changesets", (object) this.mopupProcessedChangesets.Count));
      if (this.mopupProcessedChangesets.Any<int>())
      {
        this.statusBuilder.Append(" ; Processed changesets: ");
        this.statusBuilder.Append(string.Join<int>(", ", (IEnumerable<int>) this.mopupProcessedChangesets));
      }
      if (this.droppedChangesetIds.Any<int>())
      {
        this.statusBuilder.Append(" ; Dropped changesets: ");
        this.statusBuilder.Append(string.Join<int>(", ", (IEnumerable<int>) this.droppedChangesetIds));
      }
      return this.statusBuilder.ToString();
    }

    public override void Cleanup() => this.UdpateMopupQueueAndWaterMarks();

    private void UdpateMopupQueueAndWaterMarks()
    {
      if (this.processedChangesetIds.Count<int>() <= 0)
        return;
      using (this.requestContext.AcquireExemptionLock())
      {
        using (ICodeSenseSqlResourceComponent component = this.requestContext.CreateComponent<ICodeSenseSqlResourceComponent, CodeSenseSqlResourceComponent>())
          component.RemoveMopupItems(this.processedChangesetIds);
      }
      this.registryService.UpdateWatermarkIfHigh(this.requestContext, this.processedChangesetIds.Max());
      this.registryService.UpdateWatermarkIfLow(this.requestContext, this.processedChangesetIds.Max());
      this.processedChangesetIds.Clear();
    }

    private static TimeSpan GetMopupRuntime(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetMopupRuntime(requestContext);

    public void OnStorageLimitRuleFail()
    {
      this.statusBuilder.Append("; ");
      this.statusBuilder.Append(string.Format("Storage growth exceeds the limit {0} GB", (object) this.registryService.GetStorageGrowthLimit(this.requestContext)));
    }
  }
}
