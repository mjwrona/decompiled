// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.MasterFlightsProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Experimentation
{
  internal sealed class MasterFlightsProvider : 
    TelemetryDisposableObject,
    IFlightsProvider,
    IDisposable
  {
    private readonly IEnumerable<IFlightsProvider> inclusiveFlightsProviders;
    private readonly IEnumerable<IFlightsProvider> exclusiveFlightsProviders;
    private readonly IFlightsProvider shippedFlightsProvider;
    private readonly bool isUserOptedIn;
    private HashSet<FlightAllocation> activeFlights;
    private IList<ConfigData> configs;

    public IEnumerable<FlightAllocation> Flights
    {
      get
      {
        this.RequiresNotDisposed();
        if (this.activeFlights == null)
          this.activeFlights = this.BuildListOfFlights();
        return (IEnumerable<FlightAllocation>) this.activeFlights;
      }
    }

    public IEnumerable<ConfigData> Configs
    {
      get
      {
        this.RequiresNotDisposed();
        if (this.configs == null)
          this.configs = this.BuildListOfConfigs();
        return (IEnumerable<ConfigData>) this.configs;
      }
    }

    public event EventHandler<FlightsEventArgs> FlightsUpdated;

    public MasterFlightsProvider(
      IEnumerable<IFlightsProvider> inclusiveFlightsProviders,
      IEnumerable<IFlightsProvider> exclusiveFlightsProviders,
      IFlightsProvider shippedFlightsProvider,
      IExperimentationOptinStatusReader optinStatusReader)
    {
      inclusiveFlightsProviders.RequiresArgumentNotNull<IEnumerable<IFlightsProvider>>(nameof (inclusiveFlightsProviders));
      exclusiveFlightsProviders.RequiresArgumentNotNull<IEnumerable<IFlightsProvider>>(nameof (exclusiveFlightsProviders));
      shippedFlightsProvider.RequiresArgumentNotNull<IFlightsProvider>(nameof (shippedFlightsProvider));
      optinStatusReader.RequiresArgumentNotNull<IExperimentationOptinStatusReader>(nameof (optinStatusReader));
      this.exclusiveFlightsProviders = exclusiveFlightsProviders;
      this.inclusiveFlightsProviders = inclusiveFlightsProviders;
      this.shippedFlightsProvider = shippedFlightsProvider;
      this.ForAllProviders((Action<IFlightsProvider>) (provider => provider.FlightsUpdated += new EventHandler<FlightsEventArgs>(this.OnProviderFlightsUpdated)));
      this.isUserOptedIn = optinStatusReader.IsOptedIn;
    }

    public void Start()
    {
      this.RequiresNotDisposed();
      if (!this.isUserOptedIn)
        return;
      this.ForAllProviders((Action<IFlightsProvider>) (provider => provider.Start()));
    }

    public async Task WaitForReady(CancellationToken token)
    {
      MasterFlightsProvider masterFlightsProvider = this;
      masterFlightsProvider.RequiresNotDisposed();
      if (!masterFlightsProvider.isUserOptedIn)
        return;
      List<Task> tasks = new List<Task>();
      masterFlightsProvider.ForAllProviders((Action<IFlightsProvider>) (provider => tasks.Add(provider.WaitForReady(token))));
      await Task.WhenAll((IEnumerable<Task>) tasks).ConfigureAwait(false);
    }

    protected override void DisposeManagedResources() => this.ForAllProviders((Action<IFlightsProvider>) (provider => provider.Dispose()));

    private void ForAllProviders(Action<IFlightsProvider> action) => this.ForAllProviders(action, this.inclusiveFlightsProviders.Union<IFlightsProvider>(this.exclusiveFlightsProviders));

    private void ForAllProviders(
      Action<IFlightsProvider> action,
      IEnumerable<IFlightsProvider> flightsProviders)
    {
      foreach (IFlightsProvider flightsProvider in flightsProviders)
        action(flightsProvider);
    }

    private void OnProviderFlightsUpdated(object sender, FlightsEventArgs e)
    {
      HashSet<FlightAllocation> flightAllocationSet = this.BuildListOfFlights();
      if (flightAllocationSet.SetEquals((IEnumerable<FlightAllocation>) (this.activeFlights ?? new HashSet<FlightAllocation>())))
        return;
      this.activeFlights = flightAllocationSet;
      this.OnFlightsUpdated();
    }

    private HashSet<FlightAllocation> BuildListOfFlights()
    {
      HashSet<FlightAllocation> flightAllocationSet1 = new HashSet<FlightAllocation>();
      HashSet<FlightAllocation> enabledFlights = new HashSet<FlightAllocation>();
      HashSet<FlightAllocation> disabledFlights = new HashSet<FlightAllocation>();
      this.ForAllProviders((Action<IFlightsProvider>) (provider => enabledFlights = enabledFlights.UnionWithFlights(provider.Flights, StringComparer.OrdinalIgnoreCase)), this.inclusiveFlightsProviders);
      this.ForAllProviders((Action<IFlightsProvider>) (provider => disabledFlights = disabledFlights.UnionWithFlights(provider.Flights, StringComparer.OrdinalIgnoreCase)), this.exclusiveFlightsProviders);
      HashSet<FlightAllocation> flightAllocationSet2;
      if (disabledFlights.Any<FlightAllocation>((Func<FlightAllocation, bool>) (f => string.Equals(f.FlightName, "*"))) || !this.isUserOptedIn)
      {
        flightAllocationSet2 = new HashSet<FlightAllocation>(this.shippedFlightsProvider.Flights.Select<FlightAllocation, FlightAllocation>((Func<FlightAllocation, FlightAllocation>) (flight => flight.ToLowerInvariant())));
      }
      else
      {
        enabledFlights.ExceptWithFlights((IEnumerable<FlightAllocation>) disabledFlights, StringComparer.OrdinalIgnoreCase);
        flightAllocationSet2 = new HashSet<FlightAllocation>(enabledFlights.Select<FlightAllocation, FlightAllocation>((Func<FlightAllocation, FlightAllocation>) (flight => flight.ToLowerInvariant())));
      }
      return flightAllocationSet2;
    }

    private IList<ConfigData> BuildListOfConfigs()
    {
      List<ConfigData> configs = new List<ConfigData>();
      this.ForAllProviders((Action<IFlightsProvider>) (provider => configs = configs.Concat<ConfigData>(provider.Configs).ToList<ConfigData>()), this.inclusiveFlightsProviders);
      return (IList<ConfigData>) configs;
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
