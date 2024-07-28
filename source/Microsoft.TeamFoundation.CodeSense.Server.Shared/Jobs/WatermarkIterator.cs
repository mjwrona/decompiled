// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.Jobs.WatermarkIterator
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.CodeSense.Server.Rules;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Microsoft.TeamFoundation.CodeSense.Server.Jobs
{
  public abstract class WatermarkIterator : RuleRunner, IBatchQueue<int>
  {
    private const int YieldInterval = 60;
    private readonly ITfsVersionControlClient tfsVersionControlClient;
    private readonly DateTime stopTime;
    protected readonly IVssRequestContext requestContext;
    protected IEnumerable<int> changesetIDs;
    protected List<int> processedChangesetIds;
    protected List<int> droppedChangesetIds;
    private StringBuilder statusBuilder = new StringBuilder(64);

    public WatermarkIterator(
      IVssRequestContext requestContext,
      TeamFoundationJobDefinition jobDefinition,
      ITfsVersionControlClient tfsVersionControlClient,
      DateTime queueTime,
      TimeSpan runTime = default (TimeSpan))
    {
      this.requestContext = requestContext;
      this.tfsVersionControlClient = tfsVersionControlClient;
      this.changesetIDs = Enumerable.Empty<int>();
      this.processedChangesetIds = new List<int>();
      this.droppedChangesetIds = new List<int>();
      this.MinChangeset = int.MaxValue;
      this.MaxChangeset = int.MinValue;
      TimeSpan timeSpan = runTime;
      if (timeSpan == TimeSpan.Zero)
      {
        timeSpan = TimeSpan.FromSeconds(60.0);
        if (jobDefinition.Schedule != null && jobDefinition.Schedule.Any<TeamFoundationJobSchedule>())
          timeSpan = TimeSpan.FromSeconds((double) Math.Max(60, jobDefinition.Schedule.Min<TeamFoundationJobSchedule>((Func<TeamFoundationJobSchedule, int>) (s => s.Interval)) - 60));
      }
      this.stopTime = queueTime.Add(timeSpan);
    }

    public bool Complete { get; protected set; }

    public int MinChangeset { get; private set; }

    public int MaxChangeset { get; private set; }

    public abstract int RetryCount { get; set; }

    public SliceSource SliceSource { get; protected set; }

    public int CurrentChangesetId { get; set; }

    public DateTime CurrentChangesetTime { get; set; }

    public IEnumerable<int> Peek(int numItems) => this.NextBatch(numItems) ? this.changesetIDs : Enumerable.Empty<int>();

    public void Dequeue()
    {
      if (this.changesetIDs.Any<int>())
        this.UpdateWatermark(this.changesetIDs.Last<int>());
      this.changesetIDs = Enumerable.Empty<int>();
    }

    public virtual void HandleException(Exception e) => throw new AggregateException("Exception thrown in watermark iterator.", e);

    public virtual void SkipItem(int changesetId)
    {
      this.requestContext.LogKPI(CodeLensKpiArea.CodeLensService, CodeLensKpiName.ChangesetProcessingSkipped);
      MopupExtensions.AddMopupItemToQueue(this.requestContext, changesetId, this.SliceSource);
      this.droppedChangesetIds.Add(changesetId);
    }

    public virtual string Status()
    {
      this.statusBuilder.Insert(0, this.MaxChangeset < this.MinChangeset ? "Processed 0 changesets" : string.Format("Processing changesets in range {0} - {1}", (object) this.MinChangeset, (object) this.MaxChangeset));
      if (this.droppedChangesetIds.Any<int>())
      {
        this.statusBuilder.Append(" ; Dropped changesets: ");
        this.statusBuilder.Append(string.Join<int>(", ", (IEnumerable<int>) this.droppedChangesetIds));
      }
      return this.statusBuilder.ToString();
    }

    public virtual void Cleanup()
    {
    }

    public virtual void AppendToProcessedList(int changesetId) => this.processedChangesetIds.Add(changesetId);

    public void AppendToStatus(string message, params object[] objects)
    {
      this.statusBuilder.Append("; ");
      this.statusBuilder.AppendFormat(message, objects);
    }

    protected abstract void UpdateWatermark(int lastSuccessfulChangeset);

    protected abstract IEnumerable<int> LoadChangesets(int numChangesets);

    private bool NextBatch(int batchSize)
    {
      if (DateTime.UtcNow >= this.stopTime)
      {
        this.Cleanup();
        return false;
      }
      this.changesetIDs = this.LoadChangesets(batchSize);
      return this.changesetIDs.Any<int>();
    }

    public void UpdateChangesetsProcessed()
    {
      if (!this.changesetIDs.Any<int>())
        return;
      this.MinChangeset = Math.Min(this.changesetIDs.Min(), this.MinChangeset);
      this.MaxChangeset = Math.Max(this.changesetIDs.Max(), this.MaxChangeset);
    }

    public int BatchCount() => this.changesetIDs.Count<int>();

    public int MinChangesetOfBatch() => this.changesetIDs.Min();

    public int MaxChangesetOfBatch() => this.changesetIDs.Max();
  }
}
