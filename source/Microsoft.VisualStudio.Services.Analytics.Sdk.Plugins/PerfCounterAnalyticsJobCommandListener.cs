// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.PerfCounterAnalyticsJobCommandListener
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  internal class PerfCounterAnalyticsJobCommandListener : IDisposable
  {
    private string _jobName;
    private AnalyticsJobEventHandler _eventHandler;

    public PerfCounterAnalyticsJobCommandListener(
      AnalyticsJobEventHandler eventHandler,
      string jobName)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(jobName, nameof (jobName));
      this._jobName = jobName;
      this._eventHandler = eventHandler;
      this._eventHandler.StreamsFound += new EventHandler<StreamCountEventArgs>(this.OnStreamsFound);
      this._eventHandler.ProcessingStream += new EventHandler<StreamEventArgs>(this.OnProcessingStream);
      this._eventHandler.StreamProcessed += new EventHandler<StreamEventArgs>(this.OnStreamProcessed);
      this._eventHandler.RecordsRetrieved += new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsRetrieved);
      this._eventHandler.RecordsStaged += new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsStaged);
    }

    public void Dispose()
    {
      this._eventHandler.StreamsFound -= new EventHandler<StreamCountEventArgs>(this.OnStreamsFound);
      this._eventHandler.ProcessingStream -= new EventHandler<StreamEventArgs>(this.OnProcessingStream);
      this._eventHandler.StreamProcessed -= new EventHandler<StreamEventArgs>(this.OnStreamProcessed);
      this._eventHandler.RecordsRetrieved -= new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsRetrieved);
      this._eventHandler.RecordsStaged -= new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsStaged);
      this._eventHandler = (AnalyticsJobEventHandler) null;
    }

    private void OnStreamsFound(object sender, StreamCountEventArgs e)
    {
    }

    private void OnProcessingStream(object sender, StreamEventArgs e)
    {
    }

    private void OnStreamProcessed(object sender, StreamEventArgs e)
    {
    }

    private void OnRecordsRetrieved(object sender, StagePostEnvelopeEventArgs e)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Analytics.Plugins.PerformanceCounters.AverageRetrieveRecordsBase", this._jobName).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Analytics.Plugins.PerformanceCounters.AverageRetrieveRecordsResultCount", this._jobName).IncrementBy((long) e.RecordCount);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Analytics.Plugins.PerformanceCounters.AverageRetrieveRecordsTime", this._jobName).IncrementTicks(e.Duration);
    }

    private void OnRecordsStaged(object sender, StagePostEnvelopeEventArgs e)
    {
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Analytics.Plugins.PerformanceCounters.AverageStageRecordsBase", this._jobName).Increment();
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Analytics.Plugins.PerformanceCounters.AverageStageRecordsResultCount", this._jobName).IncrementBy((long) e.RecordCount);
      VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Analytics.Plugins.PerformanceCounters.AverageStageRecordsTime", this._jobName).IncrementTicks(e.Duration);
    }
  }
}
