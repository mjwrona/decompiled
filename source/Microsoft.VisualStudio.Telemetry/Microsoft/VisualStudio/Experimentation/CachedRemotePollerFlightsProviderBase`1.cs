// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.CachedRemotePollerFlightsProviderBase`1
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Experimentation
{
  internal abstract class CachedRemotePollerFlightsProviderBase<TFlightStreamType> : 
    TelemetryDisposableObject,
    IFlightsProvider,
    IDisposable
    where TFlightStreamType : IFlightsData
  {
    private readonly object lockObject = new object();
    private readonly Lazy<LocalFlightsProvider> cachedFlightsProvider;
    private readonly IFlightsStreamParser flightsStreamParser;
    private readonly Timer timer;
    private readonly int timerInterval;
    private bool isStarted;
    private Task firstTaskRequest;
    private HashSet<FlightAllocation> flights;
    private IList<ConfigData> configs;

    public IEnumerable<FlightAllocation> Flights
    {
      get
      {
        this.RequiresNotDisposed();
        if (this.flights == null)
        {
          lock (this.lockObject)
          {
            if (this.flights == null)
              this.flights = new HashSet<FlightAllocation>(this.cachedFlightsProvider.Value.Flights);
          }
        }
        return (IEnumerable<FlightAllocation>) this.flights;
      }
      private set
      {
        value.RequiresArgumentNotNull<IEnumerable<FlightAllocation>>(nameof (value));
        if (this.IsDisposed)
          return;
        bool flag = false;
        lock (this.lockObject)
        {
          HashSet<FlightAllocation> other = new HashSet<FlightAllocation>(value);
          if (this.flights != null)
          {
            if (this.flights.SetEquals((IEnumerable<FlightAllocation>) other))
              goto label_10;
          }
          this.flights = other;
          this.cachedFlightsProvider.Value.Flights = value;
          flag = true;
        }
label_10:
        if (!flag)
          return;
        this.OnFlightsUpdated();
      }
    }

    public IEnumerable<ConfigData> Configs
    {
      get
      {
        this.RequiresNotDisposed();
        if (this.configs == null)
        {
          lock (this.lockObject)
          {
            if (this.configs == null)
              this.configs = (IList<ConfigData>) this.cachedFlightsProvider.Value.Configs.ToList<ConfigData>();
          }
        }
        return (IEnumerable<ConfigData>) this.configs;
      }
      private set
      {
        if (this.IsDisposed)
          return;
        if (value == null)
          value = (IEnumerable<ConfigData>) new List<ConfigData>();
        lock (this.lockObject)
          this.cachedFlightsProvider.Value.Configs = value;
      }
    }

    public event EventHandler<FlightsEventArgs> FlightsUpdated;

    public CachedRemotePollerFlightsProviderBase(
      IKeyValueStorage keyValueStorage,
      IFlightsStreamParser flightsStreamParser,
      int timerInterval)
    {
      CachedRemotePollerFlightsProviderBase<TFlightStreamType> flightsProviderBase = this;
      keyValueStorage.RequiresArgumentNotNull<IKeyValueStorage>(nameof (keyValueStorage));
      flightsStreamParser.RequiresArgumentNotNull<IFlightsStreamParser>(nameof (flightsStreamParser));
      this.cachedFlightsProvider = new Lazy<LocalFlightsProvider>((Func<LocalFlightsProvider>) (() => new LocalFlightsProvider(keyValueStorage, flightsProviderBase.BuildFlightsKey())));
      this.flightsStreamParser = flightsStreamParser;
      this.timerInterval = timerInterval;
      this.timer = new Timer((TimerCallback) (async state => await flightsProviderBase.SendRemoteRequestAsync()));
    }

    public void Start()
    {
      this.RequiresNotDisposed();
      if (this.isStarted)
        return;
      this.firstTaskRequest = this.SendRemoteRequestAsync();
      this.isStarted = true;
    }

    public async Task WaitForReady(CancellationToken token)
    {
      CachedRemotePollerFlightsProviderBase<TFlightStreamType> flightsProviderBase = this;
      flightsProviderBase.RequiresNotDisposed();
      if (flightsProviderBase.firstTaskRequest == null)
        throw new InvalidOperationException("WaitForReady can't be called before calling Start()");
      TaskCompletionSource<bool> tcs = new TaskCompletionSource<bool>();
      token.Register((Action) (() => tcs.TrySetCanceled()), false);
      Task task = await Task.WhenAny(flightsProviderBase.firstTaskRequest, (Task) tcs.Task).ConfigureAwait(false);
      token.ThrowIfCancellationRequested();
    }

    protected override void DisposeManagedResources()
    {
      this.timer.Dispose();
      this.InternalDispose();
    }

    protected abstract Task<Stream> SendRemoteRequestInternalAsync();

    protected abstract string BuildFlightsKey();

    protected virtual void InternalDispose()
    {
    }

    private async Task SendRemoteRequestAsync()
    {
      CachedRemotePollerFlightsProviderBase<TFlightStreamType> flightsProviderBase = this;
      flightsProviderBase.RequiresNotDisposed();
      Stream stream = await flightsProviderBase.SendRemoteRequestInternalAsync().ConfigureAwait(false);
      if (stream != null)
      {
        TFlightStreamType flightStreamType = await flightsProviderBase.flightsStreamParser.ParseStreamAsync<TFlightStreamType>(stream).ConfigureAwait(false);
        if ((object) flightStreamType != null)
        {
          if (flightStreamType.AssignmentContext != null)
            flightsProviderBase.Flights = ((IEnumerable<string>) flightStreamType.AssignmentContext.ToLowerInvariant().Split(';')).Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))).Select<string, FlightAllocation>((Func<string, FlightAllocation>) (s => FlightAllocation.CreateFromNetworkString(s)));
          else
            flightsProviderBase.Flights = flightStreamType.Flights.Where<string>((Func<string, bool>) (f => !string.IsNullOrWhiteSpace(f))).Select<string, FlightAllocation>((Func<string, FlightAllocation>) (f => new FlightAllocation(f.ToLowerInvariant())));
          flightsProviderBase.Configs = (IEnumerable<ConfigData>) flightStreamType.Configs;
        }
      }
      if (flightsProviderBase.IsDisposed)
        return;
      flightsProviderBase.timer.Change(flightsProviderBase.timerInterval, -1);
    }

    private void OnFlightsUpdated()
    {
      EventHandler<FlightsEventArgs> flightsUpdated = this.FlightsUpdated;
      if (flightsUpdated == null)
        return;
      flightsUpdated((object) this, new FlightsEventArgs());
    }
  }
}
