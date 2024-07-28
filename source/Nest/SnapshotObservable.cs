// Decompiled with JetBrains decompiler
// Type: Nest.SnapshotObservable
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Diagnostics;
using System.Threading;

namespace Nest
{
  public class SnapshotObservable : IDisposable, IObservable<SnapshotStatusResponse>
  {
    private readonly IElasticClient _elasticClient;
    private readonly TimeSpan _interval = TimeSpan.FromSeconds(2.0);
    private readonly ISnapshotRequest _snapshotRequest;
    private readonly SnapshotStatusHumbleObject _snapshotStatusHumbleObject;
    private EventHandler<SnapshotCompletedEventArgs> _completedEventHandler;
    private bool _disposed;
    private EventHandler<SnapshotErrorEventArgs> _errorEventHandler;
    private EventHandler<SnapshotNextEventArgs> _nextEventHandler;
    private Timer _timer;

    public SnapshotObservable(IElasticClient elasticClient, ISnapshotRequest snapshotRequest)
    {
      elasticClient.ThrowIfNull<IElasticClient>(nameof (elasticClient));
      snapshotRequest.ThrowIfNull<ISnapshotRequest>(nameof (snapshotRequest));
      this._elasticClient = elasticClient;
      this._snapshotRequest = snapshotRequest;
      this._snapshotRequest.RequestParameters.SetRequestMetaData(RequestMetaDataFactory.SnapshotHelperRequestMetaData());
      this._snapshotStatusHumbleObject = new SnapshotStatusHumbleObject(elasticClient, snapshotRequest);
      this._snapshotStatusHumbleObject.Completed += new EventHandler<SnapshotCompletedEventArgs>(this.StopTimer);
      this._snapshotStatusHumbleObject.Error += new EventHandler<SnapshotErrorEventArgs>(this.StopTimer);
    }

    public SnapshotObservable(
      IElasticClient elasticClient,
      ISnapshotRequest snapshotRequest,
      TimeSpan interval)
      : this(elasticClient, snapshotRequest)
    {
      interval.ThrowIfNull<TimeSpan>(nameof (interval));
      this._interval = interval.Ticks >= 0L ? interval : throw new ArgumentOutOfRangeException(nameof (interval));
    }

    public void Dispose() => this.Dispose(true);

    public IDisposable Subscribe(IObserver<SnapshotStatusResponse> observer)
    {
      observer.ThrowIfNull<IObserver<SnapshotStatusResponse>>(nameof (observer));
      try
      {
        this._snapshotRequest.RequestParameters.WaitForCompletion = new bool?(false);
        SnapshotResponse snapshotResponse = this._elasticClient.Snapshot.Snapshot(this._snapshotRequest);
        if (!snapshotResponse.IsValid)
          throw new ElasticsearchClientException(PipelineFailure.BadResponse, "Failed to create snapshot.", snapshotResponse.ApiCall);
        EventHandler<SnapshotNextEventArgs> eventHandler1 = (EventHandler<SnapshotNextEventArgs>) ((sender, args) => observer.OnNext(args.SnapshotStatusResponse));
        EventHandler<SnapshotCompletedEventArgs> eventHandler2 = (EventHandler<SnapshotCompletedEventArgs>) ((sender, args) => observer.OnCompleted());
        EventHandler<SnapshotErrorEventArgs> eventHandler3 = (EventHandler<SnapshotErrorEventArgs>) ((sender, args) => observer.OnError(args.Exception));
        this._nextEventHandler = eventHandler1;
        this._completedEventHandler = eventHandler2;
        this._errorEventHandler = eventHandler3;
        this._snapshotStatusHumbleObject.Next += eventHandler1;
        this._snapshotStatusHumbleObject.Completed += eventHandler2;
        this._snapshotStatusHumbleObject.Error += eventHandler3;
        this._timer = new Timer(new TimerCallback(this.Snapshot), (object) observer, this._interval, Timeout.InfiniteTimeSpan);
      }
      catch (Exception ex)
      {
        observer.OnError(ex);
      }
      return (IDisposable) this;
    }

    private void Snapshot(object state)
    {
      if (!(state is IObserver<SnapshotStatusResponse> observer))
        throw new ArgumentException(nameof (state));
      try
      {
        Stopwatch stopwatch = new Stopwatch();
        stopwatch.Start();
        this._snapshotStatusHumbleObject.CheckStatus();
        this._timer.Change(TimeSpan.FromMilliseconds(Math.Max(0.0, this._interval.TotalMilliseconds - (double) stopwatch.ElapsedMilliseconds)), Timeout.InfiniteTimeSpan);
      }
      catch (Exception ex)
      {
        observer.OnError(ex);
        this.StopTimer((object) null, (EventArgs) null);
      }
    }

    private void StopTimer(object sender, EventArgs restoreCompletedEventArgs) => this._timer.Change(-1, -1);

    protected virtual void Dispose(bool disposing)
    {
      if (this._disposed)
        return;
      this._timer?.Dispose();
      if (this._snapshotStatusHumbleObject != null)
      {
        this._snapshotStatusHumbleObject.Next -= this._nextEventHandler;
        this._snapshotStatusHumbleObject.Completed -= this._completedEventHandler;
        this._snapshotStatusHumbleObject.Error -= this._errorEventHandler;
        this._snapshotStatusHumbleObject.Completed -= new EventHandler<SnapshotCompletedEventArgs>(this.StopTimer);
        this._snapshotStatusHumbleObject.Error -= new EventHandler<SnapshotErrorEventArgs>(this.StopTimer);
      }
      this._disposed = true;
    }

    ~SnapshotObservable() => this.Dispose(false);
  }
}
