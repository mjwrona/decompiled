// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.ResultMessageAnalyticsJobCommandListener
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using System;
using System.Text;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  internal class ResultMessageAnalyticsJobCommandListener : IDisposable
  {
    private StringBuilder _resultMessage;
    private AnalyticsJobEventHandler _eventHandler;

    public StringBuilder ResultMessage => this._resultMessage;

    public ResultMessageAnalyticsJobCommandListener(
      AnalyticsJobEventHandler eventHandler,
      StringBuilder resultMessage = null)
    {
      this._resultMessage = resultMessage ?? new StringBuilder();
      this._eventHandler = eventHandler;
      this._eventHandler.StreamsFound += new EventHandler<StreamCountEventArgs>(this.OnStreamsFound);
      this._eventHandler.ProcessingStream += new EventHandler<StreamEventArgs>(this.OnProcessingStream);
      this._eventHandler.StreamProcessed += new EventHandler<StreamEventArgs>(this.OnStreamProcessed);
      this._eventHandler.RecordsRetrieved += new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsRetrieved);
      this._eventHandler.RecordsStaged += new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsStaged);
      this._eventHandler.StreamDisabled += new EventHandler<StreamEventArgs>(this.StreamDisabled);
      this._eventHandler.TableInMaintenance += new EventHandler<StreamEventArgs>(this.TableInMaintenance);
      this._eventHandler.StageStreamNotFound += new EventHandler<StreamEventArgs>(this.StageStreamNotFound);
      this._eventHandler.ProcessingShard += new EventHandler<ShardEventArgs>(this.OnProcessingShard);
      this._eventHandler.CreateShard += new EventHandler<ShardEventArgs>(this.OnCreateShard);
      this._eventHandler.DeleteShard += new EventHandler<ShardEventArgs>(this.OnDeleteShard);
      this._eventHandler.StreamThrottled += new EventHandler<StreamEventArgs>(this.OnStreamThrottled);
      this._eventHandler.KeysOnlyNotSupported += new EventHandler<StreamEventArgs>(this.OnKeysOnlyNotSupported);
    }

    public void Dispose()
    {
      this._eventHandler.StreamsFound -= new EventHandler<StreamCountEventArgs>(this.OnStreamsFound);
      this._eventHandler.ProcessingStream -= new EventHandler<StreamEventArgs>(this.OnProcessingStream);
      this._eventHandler.StreamProcessed -= new EventHandler<StreamEventArgs>(this.OnStreamProcessed);
      this._eventHandler.RecordsRetrieved -= new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsRetrieved);
      this._eventHandler.RecordsStaged -= new EventHandler<StagePostEnvelopeEventArgs>(this.OnRecordsStaged);
      this._eventHandler.StreamDisabled -= new EventHandler<StreamEventArgs>(this.StreamDisabled);
      this._eventHandler.TableInMaintenance -= new EventHandler<StreamEventArgs>(this.TableInMaintenance);
      this._eventHandler.ProcessingShard -= new EventHandler<ShardEventArgs>(this.OnProcessingShard);
      this._eventHandler.CreateShard -= new EventHandler<ShardEventArgs>(this.OnCreateShard);
      this._eventHandler.DeleteShard -= new EventHandler<ShardEventArgs>(this.OnDeleteShard);
      this._eventHandler.StreamThrottled -= new EventHandler<StreamEventArgs>(this.OnStreamThrottled);
      this._eventHandler.KeysOnlyNotSupported -= new EventHandler<StreamEventArgs>(this.OnKeysOnlyNotSupported);
      this._resultMessage = (StringBuilder) null;
      this._eventHandler = (AnalyticsJobEventHandler) null;
    }

    private void OnStreamsFound(object sender, StreamCountEventArgs e) => this._resultMessage.AppendLine("Found streams to process. " + string.Format("{{\"Live\":{0}, \"Recovery\":{1}, \"Table\":{2}, \"Shard\":{3}}}", (object) e.LiveStreamsCount, (object) e.RecoveryStreamsCount, (object) e.TableName, (object) e.Shard));

    private void OnProcessingStream(object sender, StreamEventArgs e) => this._resultMessage.AppendLine("Processing stream. " + string.Format("{{\"Id\":{0}, \"Watermark\":\"{1}\", \"Priority\":{2}, \"KeysOnly\":{3}}}", (object) e.Stream.StreamId, (object) e.Stream.Watermark, (object) e.Stream.Priority, (object) e.Stream.KeysOnly));

    private void OnProcessingShard(object sender, ShardEventArgs e) => this._resultMessage.AppendLine("Processing Shard. \n" + string.Format("{{\"Table\":{0}, \"ShardId\":{1} }}", (object) e.TableName, (object) e.ShardId));

    private void OnCreateShard(object sender, ShardEventArgs e) => this._resultMessage.AppendLine("Create Shard. \n" + string.Format("{{\"Table\":{0}, \"ShardId\":{1} }}", (object) e.TableName, (object) e.ShardId));

    private void OnDeleteShard(object sender, ShardEventArgs e) => this._resultMessage.AppendLine("Delete Shard. \n" + string.Format("{{\"Table\":{0}, \"ShardId\":{1} }}", (object) e.TableName, (object) e.ShardId));

    private void OnStreamProcessed(object sender, StreamEventArgs e) => this._resultMessage.AppendLine("Stream processed.");

    private void StreamDisabled(object sender, StreamEventArgs e) => this._resultMessage.AppendLine("Stream was disabled.");

    private void TableInMaintenance(object sender, StreamEventArgs e) => this._resultMessage.AppendLine("Table " + e.TableName + " is in maintenance mode.");

    private void StageStreamNotFound(object sender, StreamEventArgs e) => this._resultMessage.AppendLine(string.Format("Couldn't stage data to table: {0}, shard: {1} and stream {2} because the stream is unknown", (object) e.TableName, (object) e.Shard, (object) e.Stream));

    private void OnRecordsRetrieved(object sender, StagePostEnvelopeEventArgs e) => this._resultMessage.AppendLine("Records retrieved. " + string.Format("{{\"Count\":{0}, \"Watermark\":\"{1}->{2}\", \"IsCurrent\":{3}, \"Duration\":{4}}}", (object) e.RecordCount, (object) e.Envelope.FromWatermark, (object) e.Envelope.ToWatermark, (object) e.Envelope.IsCurrent.ToString().ToLower(), (object) (int) e.Duration.TotalMilliseconds));

    private void OnRecordsStaged(object sender, StagePostEnvelopeEventArgs e) => this._resultMessage.AppendLine("Records staged. " + string.Format("{{\"Count\":{0}, \"Duration\":{1}}}", (object) e.RecordCount, (object) (int) e.Duration.TotalMilliseconds));

    private void NonFatalStreamError(object sender, NonFatalStreamErrorEventArgs e) => this._resultMessage.AppendLine(string.Format("Encountered non-fatal stream error for table: {0}, shard: {1} stream: {2}. Error message: {3}", (object) e.Table, (object) e.ProviderShard, (object) e.Stream, (object) e.ErrorMessage));

    private void OnStreamThrottled(object sender, StreamEventArgs e) => this._resultMessage.AppendLine(string.Format("Stream is throttled for table: {0}, shard: {1}, Stream: {2}", (object) e.TableName, (object) e.Shard, (object) e.Stream));

    private void OnKeysOnlyNotSupported(object sender, StreamEventArgs e) => this._resultMessage.AppendLine(string.Format("Keys only is not supported for table: {0}, shard: {1}, Stream: {2}", (object) e.TableName, (object) e.Shard, (object) e.Stream));
  }
}
