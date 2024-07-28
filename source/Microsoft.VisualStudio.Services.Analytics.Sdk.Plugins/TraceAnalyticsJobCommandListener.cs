// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.TraceAnalyticsJobCommandListener
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  internal class TraceAnalyticsJobCommandListener : IDisposable
  {
    private IVssRequestContext _requestContext;
    private readonly string _traceLayer;
    private AnalyticsJobEventHandler _eventHandler;

    public TraceAnalyticsJobCommandListener(
      IVssRequestContext requestContext,
      AnalyticsJobEventHandler eventHandler,
      string traceLayer)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(traceLayer, nameof (traceLayer));
      this._requestContext = requestContext;
      this._traceLayer = traceLayer;
      this._eventHandler = eventHandler;
      this._eventHandler.StreamsFound += new EventHandler<StreamCountEventArgs>(this.OnStreamsFound);
      this._eventHandler.ProcessingStream += new EventHandler<StreamEventArgs>(this.OnProcessingStream);
      this._eventHandler.StreamProcessed += new EventHandler<StreamEventArgs>(this.OnStreamProcessed);
      this._eventHandler.RecordsRetrieved += new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsRetrieved);
      this._eventHandler.RecordsStaged += new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsStaged);
      this._eventHandler.StreamDisabled += new EventHandler<StreamEventArgs>(this.StreamDisabled);
      this._eventHandler.ProcessingShard += new EventHandler<ShardEventArgs>(this.OnProcessingShard);
      this._eventHandler.CreateShard += new EventHandler<ShardEventArgs>(this.OnCreateShard);
      this._eventHandler.DeleteShard += new EventHandler<ShardEventArgs>(this.OnDeleteShard);
    }

    public void Dispose()
    {
      this._eventHandler.StreamsFound -= new EventHandler<StreamCountEventArgs>(this.OnStreamsFound);
      this._eventHandler.ProcessingStream -= new EventHandler<StreamEventArgs>(this.OnProcessingStream);
      this._eventHandler.StreamProcessed -= new EventHandler<StreamEventArgs>(this.OnStreamProcessed);
      this._eventHandler.RecordsRetrieved -= new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsRetrieved);
      this._eventHandler.RecordsStaged -= new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsStaged);
      this._eventHandler.StreamDisabled -= new EventHandler<StreamEventArgs>(this.StreamDisabled);
      this._eventHandler.ProcessingShard -= new EventHandler<ShardEventArgs>(this.OnProcessingShard);
      this._eventHandler.CreateShard -= new EventHandler<ShardEventArgs>(this.OnCreateShard);
      this._eventHandler.DeleteShard -= new EventHandler<ShardEventArgs>(this.OnDeleteShard);
      this._requestContext = (IVssRequestContext) null;
      this._eventHandler = (AnalyticsJobEventHandler) null;
    }

    private void OnStreamsFound(object sender, StreamCountEventArgs e) => this.TraceInfo(14000003, "Found streams to process.\n" + string.Format("{{\"Live\":{0}, \"Recovery\":{1}, \"Table\":{2}, \"Shard\":{3}}}", (object) e.LiveStreamsCount, (object) e.RecoveryStreamsCount, (object) e.TableName, (object) e.Shard));

    private void OnProcessingStream(object sender, StreamEventArgs e) => this.TraceInfo(14000004, "Processing stream.\n" + string.Format("{{\"Id\":{0}, \"Watermark\":\"{1}\", \"Current\":\"{2}\", \"Priority\":{3}}}", (object) e.Stream.StreamId, (object) e.Stream.Watermark, (object) e.Stream.Current, (object) e.Stream.Priority));

    private void OnStreamProcessed(object sender, StreamEventArgs e) => this.TraceInfo(14000004, "Stream processed.");

    private void OnProcessingShard(object sender, ShardEventArgs e) => this.TraceInfo(14000012, "Processing Shard. \n" + string.Format("{{\"Table\":{0}, \"ShardId\":{1} }}", (object) e.TableName, (object) e.ShardId));

    private void OnCreateShard(object sender, ShardEventArgs e) => this.TraceInfo(14000013, "Create Shard. \n" + string.Format("{{\"Table\":{0}, \"ShardId\":{1} }}", (object) e.TableName, (object) e.ShardId));

    private void OnDeleteShard(object sender, ShardEventArgs e) => this.TraceInfo(14000014, "Delete Shard. \n" + string.Format("{{\"Table\":{0}, \"ShardId\":{1} }}", (object) e.TableName, (object) e.ShardId));

    private void StreamDisabled(object sender, StreamEventArgs e) => this.TraceInfo(14000009, "Stream was disabled.");

    private void OnRecordsRetrieved(object sender, StagePostEnvelopeEventArgs e) => this.TraceVerbose(14000006, "Records retrieved.\n" + string.Format("{{\"Count\":{0}, \"Watermark\":\"{1}->{2}\", \"IsCurrent\":{3}, \"Duration\":{4}}}", (object) e.RecordCount, (object) e.Envelope.FromWatermark, (object) e.Envelope.ToWatermark, (object) e.Envelope.IsCurrent.ToString().ToLower(), (object) (int) e.Duration.TotalMilliseconds));

    private void OnRecordsStaged(object sender, StagePostEnvelopeEventArgs e) => this.TraceInfo(14000007, "Records staged.\n" + string.Format("{{\"Count\":{0}, \"Duration\":{1}}}", (object) e.RecordCount, (object) (int) e.Duration.TotalMilliseconds));

    private void TraceInfo(int tracePoint, string message) => this._requestContext.Trace(tracePoint, TraceLevel.Info, "AnalyticsStaging", this._traceLayer, message);

    private void TraceVerbose(int tracePoint, string message) => this._requestContext.Trace(tracePoint, TraceLevel.Verbose, "AnalyticsStaging", this._traceLayer, message);
  }
}
