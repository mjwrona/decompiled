// Decompiled with JetBrains decompiler
// Type: Nest.RestoreStatusHumbleObject
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Nest
{
  public class RestoreStatusHumbleObject
  {
    private readonly IElasticClient _elasticClient;
    private readonly string _renamePattern;
    private readonly string _renameReplacement;
    private readonly IRestoreRequest _restoreRequest;

    public RestoreStatusHumbleObject(IElasticClient elasticClient, IRestoreRequest restoreRequest)
    {
      elasticClient.ThrowIfNull<IElasticClient>(nameof (elasticClient));
      restoreRequest.ThrowIfNull<IRestoreRequest>(nameof (restoreRequest));
      this._elasticClient = elasticClient;
      this._restoreRequest = restoreRequest;
      this._renamePattern = string.IsNullOrEmpty(this._restoreRequest.RenamePattern) ? string.Empty : this._restoreRequest.RenamePattern;
      this._renameReplacement = string.IsNullOrEmpty(this._restoreRequest.RenameReplacement) ? string.Empty : this._restoreRequest.RenameReplacement;
    }

    public event EventHandler<RestoreCompletedEventArgs> Completed;

    public event EventHandler<RestoreErrorEventArgs> Error;

    public event EventHandler<RestoreNextEventArgs> Next;

    public void CheckStatus()
    {
      try
      {
        RecoveryStatusRequest recoveryStatusRequest = new RecoveryStatusRequest((Indices) this._restoreRequest.Indices.Item2.Indices.Select<IndexName, IndexName>((Func<IndexName, IndexName>) (x => IndexName.Rebuild(Regex.Replace(x.Name, this._renamePattern, this._renameReplacement), x.Type))).ToArray<IndexName>());
        recoveryStatusRequest.Detailed = new bool?(true);
        recoveryStatusRequest.RequestConfiguration = (IRequestConfiguration) new RequestConfiguration();
        RecoveryStatusRequest request = recoveryStatusRequest;
        request.RequestConfiguration.SetRequestMetaData(RequestMetaDataFactory.RestoreHelperRequestMetaData());
        RecoveryStatusResponse recoveryStatusResponse = this._elasticClient.Indices.RecoveryStatus((IRecoveryStatusRequest) request);
        if (!recoveryStatusResponse.IsValid)
          throw new ElasticsearchClientException(PipelineFailure.BadResponse, "Failed getting recovery status.", recoveryStatusResponse.ApiCall);
        if (recoveryStatusResponse.Indices.All<KeyValuePair<IndexName, RecoveryStatus>>((Func<KeyValuePair<IndexName, RecoveryStatus>, bool>) (x => x.Value.Shards.All<ShardRecovery>((Func<ShardRecovery, bool>) (s => s.Index.Files.Recovered == s.Index.Files.Total)))))
          this.OnCompleted(new RestoreCompletedEventArgs(recoveryStatusResponse));
        else
          this.OnNext(new RestoreNextEventArgs(recoveryStatusResponse));
      }
      catch (Exception ex)
      {
        this.OnError(new RestoreErrorEventArgs(ex));
      }
    }

    protected virtual void OnNext(RestoreNextEventArgs nextEventArgs)
    {
      EventHandler<RestoreNextEventArgs> next = this.Next;
      if (next == null)
        return;
      next((object) this, nextEventArgs);
    }

    protected virtual void OnCompleted(RestoreCompletedEventArgs completedEventArgs)
    {
      EventHandler<RestoreCompletedEventArgs> completed = this.Completed;
      if (completed == null)
        return;
      completed((object) this, completedEventArgs);
    }

    protected virtual void OnError(RestoreErrorEventArgs errorEventArgs)
    {
      EventHandler<RestoreErrorEventArgs> error = this.Error;
      if (error == null)
        return;
      error((object) this, errorEventArgs);
    }
  }
}
