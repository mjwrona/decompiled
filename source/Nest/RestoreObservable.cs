// Decompiled with JetBrains decompiler
// Type: Nest.RestoreObservable
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Diagnostics;
using System.Threading;

namespace Nest
{
  public class RestoreObservable : IDisposable, IObservable<RecoveryStatusResponse>
  {
    private readonly IElasticClient _elasticClient;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(2.0);
    private readonly IRestoreRequest _restoreRequest;
    private readonly RestoreStatusHumbleObject _restoreStatusHumbleObject;
    private EventHandler<RestoreCompletedEventArgs> _completedEentHandlers;
    private bool _disposed;
    private EventHandler<RestoreErrorEventArgs> _errorEventHandlers;
    private EventHandler<RestoreNextEventArgs> _nextEventHandlers;
    private Timer _timer;

    public RestoreObservable(IElasticClient elasticClient, IRestoreRequest restoreRequest)
    {
      elasticClient.ThrowIfNull<IElasticClient>(nameof (elasticClient));
      restoreRequest.ThrowIfNull<IRestoreRequest>(nameof (restoreRequest));
      this._elasticClient = elasticClient;
      this._restoreRequest = restoreRequest;
      this._restoreRequest.RequestParameters.SetRequestMetaData(RequestMetaDataFactory.RestoreHelperRequestMetaData());
      this._restoreStatusHumbleObject = new RestoreStatusHumbleObject(elasticClient, restoreRequest);
      this._restoreStatusHumbleObject.Completed += new EventHandler<RestoreCompletedEventArgs>(this.StopTimer);
      this._restoreStatusHumbleObject.Error += new EventHandler<RestoreErrorEventArgs>(this.StopTimer);
    }

    public RestoreObservable(
      IElasticClient elasticClient,
      IRestoreRequest restoreRequest,
      TimeSpan interval)
      : this(elasticClient, restoreRequest)
    {
      interval.ThrowIfNull<TimeSpan>(nameof (interval));
      this._interval = interval.Ticks >= 0L ? interval : throw new ArgumentOutOfRangeException(nameof (interval));
    }

    public void Dispose() => this.Dispose(true);

    public IDisposable Subscribe(IObserver<RecoveryStatusResponse> observer)
    {
      observer.ThrowIfNull<IObserver<RecoveryStatusResponse>>(nameof (observer));
      try
      {
        this._restoreRequest.RequestParameters.WaitForCompletion = new bool?(false);
        RestoreResponse restoreResponse = this._elasticClient.Snapshot.Restore(this._restoreRequest);
        if (!restoreResponse.IsValid)
          throw new ElasticsearchClientException(PipelineFailure.BadResponse, "Failed to restore snapshot.", restoreResponse.ApiCall);
        EventHandler<RestoreNextEventArgs> eventHandler1 = (EventHandler<RestoreNextEventArgs>) ((sender, args) => observer.OnNext(args.RecoveryStatusResponse));
        EventHandler<RestoreCompletedEventArgs> eventHandler2 = (EventHandler<RestoreCompletedEventArgs>) ((sender, args) => observer.OnCompleted());
        EventHandler<RestoreErrorEventArgs> eventHandler3 = (EventHandler<RestoreErrorEventArgs>) ((sender, args) => observer.OnError(args.Exception));
        this._nextEventHandlers = eventHandler1;
        this._completedEentHandlers = eventHandler2;
        this._errorEventHandlers = eventHandler3;
        this._restoreStatusHumbleObject.Next += eventHandler1;
        this._restoreStatusHumbleObject.Completed += eventHandler2;
        this._restoreStatusHumbleObject.Error += eventHandler3;
        this._timer = new Timer(new TimerCallback(this.Restore), (object) observer, this._interval, Timeout.InfiniteTimeSpan);
      }
      catch (Exception ex)
      {
        observer.OnError(ex);
      }
      return (IDisposable) this;
    }

    private void Restore(object state)
    {
      if (!(state is IObserver<RecoveryStatusResponse> observer))
        throw new ArgumentException("must be an IObserver", nameof (state));
      try
      {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        this._restoreStatusHumbleObject.CheckStatus();
        this._timer.Change(TimeSpan.FromMilliseconds(Math.Max(0.0, this._interval.TotalMilliseconds - (double) stopwatch.ElapsedMilliseconds)), Timeout.InfiniteTimeSpan);
      }
      catch (Exception ex)
      {
        observer.OnError(ex);
      }
    }

    private void StopTimer(object sender, EventArgs restoreCompletedEventArgs) => this._timer.Change(-1, -1);

    protected virtual void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      this._timer?.Dispose();
      if (this._restoreStatusHumbleObject != null)
      {
        this._restoreStatusHumbleObject.Next -= this._nextEventHandlers;
        this._restoreStatusHumbleObject.Completed -= this._completedEentHandlers;
        this._restoreStatusHumbleObject.Error -= this._errorEventHandlers;
        this._restoreStatusHumbleObject.Completed -= new EventHandler<RestoreCompletedEventArgs>(this.StopTimer);
        this._restoreStatusHumbleObject.Error -= new EventHandler<RestoreErrorEventArgs>(this.StopTimer);
      }
      this._disposed = true;
    }

    ~RestoreObservable() => this.Dispose(false);
  }
}
