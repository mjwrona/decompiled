// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.ExperimentationService
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Experimentation
{
  public sealed class ExperimentationService : 
    TelemetryDisposableObject,
    IExperimentationService,
    IDisposable,
    IExperimentationSetterService,
    IExperimentationStatusService,
    IExperimentationService2,
    IExperimentationService3
  {
    private static readonly Lazy<ExperimentationService> defaultExperimentationService = new Lazy<ExperimentationService>((Func<ExperimentationService>) (() => new ExperimentationService(ExperimentationService.GetDefaultInitializer())));
    private static ExperimentationServiceInitializer customInitializer;
    private readonly IExperimentationTelemetry telemetry;
    private readonly IExperimentationFilterProvider filterProvider;
    private readonly ConcurrentDictionary<string, ExperimentationService.FlightStatus> flightStatus = new ConcurrentDictionary<string, ExperimentationService.FlightStatus>();
    private readonly IFlightsProvider flightsProvider;
    private readonly SetFlightsProvider setFlightsProvider;
    private readonly object lockStartFlights = new object();
    private bool isStarted;

    [ExcludeFromCodeCoverage]
    public static IExperimentationService Default => (IExperimentationService) ExperimentationService.defaultExperimentationService.Value;

    [ExcludeFromCodeCoverage]
    public static IExperimentationSetterService DefaultSetter => (IExperimentationSetterService) ExperimentationService.defaultExperimentationService.Value;

    [ExcludeFromCodeCoverage]
    public static IExperimentationStatusService DefaultStatus => (IExperimentationStatusService) ExperimentationService.defaultExperimentationService.Value;

    private static ExperimentationServiceInitializer GetDefaultInitializer() => ExperimentationService.customInitializer != null ? ExperimentationService.customInitializer : ExperimentationServiceInitializer.BuildDefault();

    public static IExperimentationService CreateDefaultExperimentationService(
      ExperimentationServiceInitializer initializer)
    {
      if (ExperimentationService.defaultExperimentationService.IsValueCreated)
      {
        if (initializer.ExperimentationTelemetry is IExperimentationTelemetry2 experimentationTelemetry)
          experimentationTelemetry.PostFault("VS/ABExp/CreateDefault", "Default exists");
      }
      else
        ExperimentationService.customInitializer = initializer;
      return (IExperimentationService) ExperimentationService.defaultExperimentationService.Value;
    }

    public ExperimentationService(ExperimentationServiceInitializer initializer)
    {
      initializer.RequiresArgumentNotNull<ExperimentationServiceInitializer>(nameof (initializer));
      initializer.FillWithDefaults();
      this.telemetry = initializer.ExperimentationTelemetry;
      this.filterProvider = initializer.ExperimentationFilterProvider;
      this.flightsProvider = initializer.FlightsProvider;
      this.setFlightsProvider = initializer.SetFlightsProvider;
      this.flightsProvider.FlightsUpdated += new EventHandler<FlightsEventArgs>(this.OnFlightsUpdated);
      this.SetFlightsTelemetry();
    }

    public bool QueryCachedFlightStatus(string flight) => this.IsCachedFlightEnabledInternal(flight, false);

    public bool IsCachedFlightEnabled(string flight) => this.IsCachedFlightEnabledInternal(flight, true);

    public Task<bool> QueryFlightStatusAsync(string flight, CancellationToken token) => this.IsFlightEnabledInternalAsync(flight, token, false);

    public Task<bool> IsFlightEnabledAsync(string flight, CancellationToken token) => this.IsFlightEnabledInternalAsync(flight, token, true);

    public IEnumerable<string> AllEnabledCachedFlights => this.flightsProvider.Flights.Select<FlightAllocation, string>((Func<FlightAllocation, string>) (f => f.FlightName));

    public void Start()
    {
      this.RequiresNotDisposed();
      lock (this.lockStartFlights)
      {
        if (this.isStarted)
          return;
        this.flightsProvider.Start();
        this.isStarted = true;
      }
    }

    public void SetFlight(string flightName, int timeoutInMinutes)
    {
      this.RequiresNotDisposed();
      this.setFlightsProvider.SetFlight(flightName, timeoutInMinutes);
    }

    protected override void DisposeManagedResources()
    {
      this.flightsProvider.FlightsUpdated -= new EventHandler<FlightsEventArgs>(this.OnFlightsUpdated);
      this.flightsProvider.Dispose();
    }

    private void OnFlightsUpdated(object sender, FlightsEventArgs e) => this.SetFlightsTelemetry();

    private void SetFlightsTelemetry()
    {
      string val = string.Join<FlightAllocation>(";", this.flightsProvider.Flights);
      if (this.telemetry is IExperimentationTelemetry3 telemetry)
      {
        telemetry.SetSharedProperty("VS.ABExp.Flights", (object) new TelemetryComplexProperty((object) val));
      }
      else
      {
        if (val.Length > 1024)
          val = string.Join(";", this.flightsProvider.Flights.Select<FlightAllocation, string>((Func<FlightAllocation, string>) (f => f.FlightName)));
        this.telemetry.SetSharedProperty("VS.ABExp.Flights", val);
      }
    }

    private void PostFlightRequestTelemetry(string flight, bool isEnabled) => this.telemetry.PostEvent("VS/ABExp/FlightRequest", (IDictionary<string, string>) new Dictionary<string, string>()
    {
      {
        "VS.ABExp.Flight",
        flight
      },
      {
        "VS.ABExp.Result",
        isEnabled.ToString()
      }
    });

    private bool IsCachedFlightEnabledInternal(string flight, bool sendTriggeredEvent)
    {
      flight.RequiresArgumentNotNullAndNotEmpty(nameof (flight));
      this.RequiresNotDisposed();
      flight = flight.ToLowerInvariant();
      return this.flightStatus.AddOrUpdate(flight, (Func<string, ExperimentationService.FlightStatus>) (key =>
      {
        bool isEnabled = this.flightsProvider.Flights.Any<FlightAllocation>((Func<FlightAllocation, bool>) (f => string.Equals(flight, f.FlightName, StringComparison.OrdinalIgnoreCase)));
        if (sendTriggeredEvent)
          this.PostFlightRequestTelemetry(flight, isEnabled);
        return new ExperimentationService.FlightStatus()
        {
          IsEnabled = isEnabled,
          WasTriggered = sendTriggeredEvent
        };
      }), (Func<string, ExperimentationService.FlightStatus, ExperimentationService.FlightStatus>) ((key, existingValue) =>
      {
        if (sendTriggeredEvent && !existingValue.WasTriggered)
        {
          this.PostFlightRequestTelemetry(flight, existingValue.IsEnabled);
          existingValue.WasTriggered = true;
        }
        return existingValue;
      })).IsEnabled;
    }

    private async Task<bool> IsFlightEnabledInternalAsync(
      string flight,
      CancellationToken token,
      bool sendTriggeredEvent)
    {
      ExperimentationService experimentationService = this;
      flight.RequiresArgumentNotNullAndNotEmpty(nameof (flight));
      experimentationService.RequiresNotDisposed();
      await experimentationService.flightsProvider.WaitForReady(token).ConfigureAwait(false);
      return experimentationService.IsCachedFlightEnabledInternal(flight, sendTriggeredEvent);
    }

    public IDictionary<string, object> GetCachedTreatmentVariables(string configId)
    {
      configId.RequiresArgumentNotNullAndNotEmpty(nameof (configId));
      this.RequiresNotDisposed();
      foreach (ConfigData config in this.flightsProvider.Configs)
      {
        if (config.Id == configId)
          return (IDictionary<string, object>) new ReadOnlyDictionary<string, object>(config.Parameters);
      }
      return (IDictionary<string, object>) null;
    }

    public Task<IDictionary<string, object>> GetTreatmentVariablesAsync(
      string configId,
      CancellationToken token)
    {
      return this.GetConfigDataInternalAsync(configId, token);
    }

    public Task<string> GetStringTreatmentVariableAsync(
      string configId,
      string varName,
      CancellationToken token)
    {
      return this.GetTreatmentVariableInternalAsync<string>(configId, varName, token);
    }

    public Task<int?> GetIntTreatmentVariableAsync(
      string configId,
      string varName,
      CancellationToken token)
    {
      return this.GetTreatmentVariableInternalAsync<int?>(configId, varName, token);
    }

    public Task<bool?> GetBoolTreatmentVariableAsync(
      string configId,
      string varName,
      CancellationToken token)
    {
      return this.GetTreatmentVariableInternalAsync<bool?>(configId, varName, token);
    }

    public Task<double?> GetDoubleTreatmentVariableAsync(
      string configId,
      string varName,
      CancellationToken token)
    {
      return this.GetTreatmentVariableInternalAsync<double?>(configId, varName, token);
    }

    private async Task<T> GetTreatmentVariableInternalAsync<T>(
      string configId,
      string varName,
      CancellationToken token)
    {
      ExperimentationService experimentationService = this;
      configId.RequiresArgumentNotNullAndNotEmpty(nameof (configId));
      varName.RequiresArgumentNotNullAndNotEmpty(nameof (varName));
      experimentationService.RequiresNotDisposed();
      await experimentationService.flightsProvider.WaitForReady(token).ConfigureAwait(false);
      foreach (ConfigData config in experimentationService.flightsProvider.Configs)
      {
        if (config.Id == configId)
        {
          object obj1;
          config.Parameters.TryGetValue(varName, out obj1);
          return !(obj1 is T obj2) ? default (T) : obj2;
        }
      }
      return default (T);
    }

    private async Task<IDictionary<string, object>> GetConfigDataInternalAsync(
      string configId,
      CancellationToken token)
    {
      ExperimentationService experimentationService = this;
      configId.RequiresArgumentNotNullAndNotEmpty(nameof (configId));
      experimentationService.RequiresNotDisposed();
      await experimentationService.flightsProvider.WaitForReady(token).ConfigureAwait(false);
      return experimentationService.GetCachedTreatmentVariables(configId);
    }

    private sealed class FlightStatus
    {
      public bool IsEnabled { get; set; }

      public bool WasTriggered { get; set; }
    }
  }
}
