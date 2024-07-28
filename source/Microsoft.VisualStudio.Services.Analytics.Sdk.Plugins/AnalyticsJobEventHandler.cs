// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Analytics.Plugins.AnalyticsJobEventHandler
// Assembly: Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3E9FDCC8-8891-4D47-89A2-C972B6459647
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Analytics.Sdk.Plugins.dll

using Microsoft.VisualStudio.Services.Analytics.WebApi;
using Microsoft.VisualStudio.Services.Analytics.WebApi.Contracts;
using System;

namespace Microsoft.TeamFoundation.Analytics.Plugins
{
  public class AnalyticsJobEventHandler
  {
    private string _tableName = string.Empty;

    public AnalyticsJobEventHandler(string tableName) => this._tableName = tableName;

    public event EventHandler<ShardEventArgs> CreateShard;

    public void RaiseCreateShard(object sender, int shardId)
    {
      EventHandler<ShardEventArgs> createShard = this.CreateShard;
      if (createShard == null)
        return;
      createShard(sender, new ShardEventArgs(this._tableName, shardId));
    }

    public event EventHandler<ShardEventArgs> DeleteShard;

    public void RaiseDeleteShard(object sender, int shardId)
    {
      EventHandler<ShardEventArgs> deleteShard = this.DeleteShard;
      if (deleteShard == null)
        return;
      deleteShard(sender, new ShardEventArgs(this._tableName, shardId));
    }

    public event EventHandler<ShardEventArgs> ProcessingShard;

    public void RaiseProcessingShard(object sender, int shardId)
    {
      EventHandler<ShardEventArgs> processingShard = this.ProcessingShard;
      if (processingShard == null)
        return;
      processingShard(sender, new ShardEventArgs(this._tableName, shardId));
    }

    public event EventHandler<StreamCountEventArgs> StreamsFound;

    public void RaiseFoundStreams(
      object sender,
      int liveStreamsCount,
      int recoveryStreamsCount,
      int shardId)
    {
      EventHandler<StreamCountEventArgs> streamsFound = this.StreamsFound;
      if (streamsFound == null)
        return;
      streamsFound(sender, new StreamCountEventArgs(this._tableName, shardId, liveStreamsCount, recoveryStreamsCount));
    }

    public event EventHandler<StreamEventArgs> ProcessingStream;

    public void RaiseProcessingStream(object sender, int shardId, StageStreamInfo stream)
    {
      EventHandler<StreamEventArgs> processingStream = this.ProcessingStream;
      if (processingStream == null)
        return;
      processingStream(sender, new StreamEventArgs(this._tableName, shardId, stream));
    }

    public event EventHandler<StreamEventArgs> StreamProcessed;

    public void RaiseStreamProcessed(object sender, int shardId, StageStreamInfo stream)
    {
      EventHandler<StreamEventArgs> streamProcessed = this.StreamProcessed;
      if (streamProcessed == null)
        return;
      streamProcessed(sender, new StreamEventArgs(this._tableName, shardId, stream));
    }

    public event EventHandler<StreamEventArgs> StreamDisabled;

    public void RaiseStreamDisabled(object sender, int shardId, StageStreamInfo stream)
    {
      EventHandler<StreamEventArgs> streamDisabled = this.StreamDisabled;
      if (streamDisabled == null)
        return;
      streamDisabled(sender, new StreamEventArgs(this._tableName, shardId, stream));
    }

    public event EventHandler<StreamEventArgs> StreamThrottled;

    public void RaiseStreamThrottled(object sender, int shardId, StageStreamInfo stream)
    {
      EventHandler<StreamEventArgs> streamThrottled = this.StreamThrottled;
      if (streamThrottled == null)
        return;
      streamThrottled(sender, new StreamEventArgs(this._tableName, shardId, stream));
    }

    public event EventHandler<StreamEventArgs> TableInMaintenance;

    public void RaiseTableInMaintenance(object sender, int shardId, StageStreamInfo stream)
    {
      EventHandler<StreamEventArgs> tableInMaintenance = this.TableInMaintenance;
      if (tableInMaintenance == null)
        return;
      tableInMaintenance(sender, new StreamEventArgs(this._tableName, shardId, stream));
    }

    public event EventHandler<StreamEventArgs> StageStreamNotFound;

    public void RaiseStageStreamUnknown(object sender, int shardId, StageStreamInfo stream)
    {
      EventHandler<StreamEventArgs> stageStreamNotFound = this.StageStreamNotFound;
      if (stageStreamNotFound == null)
        return;
      stageStreamNotFound(sender, new StreamEventArgs(this._tableName, shardId, stream));
    }

    public event EventHandler<StagePostEnvelopeEventArgs> RecordsRetrieved;

    public void RaiseRecordsRetrieved(
      object sender,
      int shardId,
      StageStreamInfo stream,
      StagePostEnvelope envelope,
      TimeSpan duration)
    {
      EventHandler<StagePostEnvelopeEventArgs> recordsRetrieved = this.RecordsRetrieved;
      if (recordsRetrieved == null)
        return;
      recordsRetrieved(sender, new StagePostEnvelopeEventArgs(this._tableName, shardId, stream, envelope, duration));
    }

    public event EventHandler<StagePostEnvelopeEventArgs> RecordsStaged;

    public void RaiseRecordsStaged(
      object sender,
      int shardId,
      StageStreamInfo stream,
      StagePostEnvelope envelope,
      TimeSpan duration)
    {
      EventHandler<StagePostEnvelopeEventArgs> recordsStaged = this.RecordsStaged;
      if (recordsStaged == null)
        return;
      recordsStaged(sender, new StagePostEnvelopeEventArgs(this._tableName, shardId, stream, envelope, duration));
    }

    public event EventHandler<ClientResolvedEventArgs> ClientResolved;

    public void RaiseClientResolved(object sender, TimeSpan duration)
    {
      EventHandler<ClientResolvedEventArgs> clientResolved = this.ClientResolved;
      if (clientResolved == null)
        return;
      clientResolved(sender, new ClientResolvedEventArgs(duration));
    }

    public event EventHandler<StreamEventArgs> KeysOnlyNotSupported;

    public void RaiseKeysOnlyNotSupported(object sender, int shardId, StageStreamInfo stream)
    {
      EventHandler<StreamEventArgs> onlyNotSupported = this.KeysOnlyNotSupported;
      if (onlyNotSupported == null)
        return;
      onlyNotSupported(sender, new StreamEventArgs(this._tableName, shardId, stream));
    }
  }
}
