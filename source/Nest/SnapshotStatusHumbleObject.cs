// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotStatusHumbleObject
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Linq;

namespace Nest
{
  public class SnapshotStatusHumbleObject
  {
    private readonly IElasticClient _elasticClient;
    private readonly ISnapshotRequest _snapshotRequest;

    public SnapshotStatusHumbleObject(
      IElasticClient elasticClient,
      ISnapshotRequest snapshotRequest)
    {
      elasticClient.ThrowIfNull<IElasticClient>(nameof (elasticClient));
      snapshotRequest.ThrowIfNull<ISnapshotRequest>(nameof (snapshotRequest));
      this._elasticClient = elasticClient;
      this._snapshotRequest = snapshotRequest;
    }

    public event EventHandler<SnapshotCompletedEventArgs> Completed;

    public event EventHandler<SnapshotErrorEventArgs> Error;

    public event EventHandler<SnapshotNextEventArgs> Next;

    public void CheckStatus()
    {
      try
      {
        SnapshotStatusRequest snapshotStatusRequest = new SnapshotStatusRequest(this._snapshotRequest.RepositoryName, (Names) this._snapshotRequest.Snapshot);
        snapshotStatusRequest.RequestConfiguration = (IRequestConfiguration) new RequestConfiguration();
        SnapshotStatusRequest request = snapshotStatusRequest;
        request.RequestConfiguration.SetRequestMetaData(RequestMetaDataFactory.SnapshotHelperRequestMetaData());
        SnapshotStatusResponse snapshotStatusResponse = this._elasticClient.Snapshot.Status((ISnapshotStatusRequest) request);
        if (!snapshotStatusResponse.IsValid)
          throw new ElasticsearchClientException(PipelineFailure.BadResponse, "Failed to get snapshot status.", snapshotStatusResponse.ApiCall);
        if (snapshotStatusResponse.Snapshots.All<SnapshotStatus>((Func<SnapshotStatus, bool>) (s => s.ShardsStats.Done == s.ShardsStats.Total)))
          this.OnCompleted(new SnapshotCompletedEventArgs(snapshotStatusResponse));
        else
          this.OnNext(new SnapshotNextEventArgs(snapshotStatusResponse));
      }
      catch (Exception ex)
      {
        this.OnError(new SnapshotErrorEventArgs(ex));
      }
    }

    protected virtual void OnNext(SnapshotNextEventArgs nextEventArgs)
    {
      EventHandler<SnapshotNextEventArgs> next = this.Next;
      if (next == null)
        return;
      next((object) this, nextEventArgs);
    }

    protected virtual void OnCompleted(SnapshotCompletedEventArgs completedEventArgs)
    {
      EventHandler<SnapshotCompletedEventArgs> completed = this.Completed;
      if (completed == null)
        return;
      completed((object) this, completedEventArgs);
    }

    protected virtual void OnError(SnapshotErrorEventArgs errorEventArgs)
    {
      EventHandler<SnapshotErrorEventArgs> error = this.Error;
      if (error == null)
        return;
      error((object) this, errorEventArgs);
    }
  }
}
