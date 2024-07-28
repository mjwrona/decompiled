// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.SetFlightsProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Experimentation
{
  internal sealed class SetFlightsProvider : TelemetryDisposableObject, IFlightsProvider, IDisposable
  {
    private readonly Lazy<LocalFlightsProvider> cachedFlightsProvider;
    private readonly object lockObject = new object();
    private volatile HashSet<FlightAllocation> flights;
    private volatile IList<ConfigData> configs;

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
              this.flights = new HashSet<FlightAllocation>(this.ConvertRawDataToPlainFlights(this.GetRawFlights()));
          }
        }
        return (IEnumerable<FlightAllocation>) this.flights;
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
              this.configs = (IList<ConfigData>) this.GetRawConfigs().ToList<ConfigData>();
          }
        }
        return (IEnumerable<ConfigData>) this.configs;
      }
    }

    public event EventHandler<FlightsEventArgs> FlightsUpdated
    {
      add
      {
      }
      remove
      {
      }
    }

    public SetFlightsProvider(IKeyValueStorage keyValueStorage, string flightsKey)
    {
      keyValueStorage.RequiresArgumentNotNull<IKeyValueStorage>(nameof (keyValueStorage));
      flightsKey.RequiresArgumentNotNullAndNotEmpty(nameof (flightsKey));
      this.cachedFlightsProvider = new Lazy<LocalFlightsProvider>((Func<LocalFlightsProvider>) (() => new LocalFlightsProvider(keyValueStorage, flightsKey)));
    }

    public async Task WaitForReady(CancellationToken token)
    {
      object obj = await Task.FromResult<object>((object) null);
    }

    public void Start()
    {
      List<FlightAllocation> rawFlights = new List<FlightAllocation>();
      bool flag = false;
      lock (this.lockObject)
      {
        foreach (FlightAllocation rawFlight in this.GetRawFlights())
        {
          SetFlightsProvider.FlightInformation flightInformation = SetFlightsProvider.FlightInformation.Parse(rawFlight.FlightName);
          if (flightInformation != null && flightInformation.ExpirationTime > DateTimeOffset.UtcNow)
            rawFlights.Add(rawFlight);
          else
            flag = true;
        }
        if (!flag)
          return;
        this.SetRawFlights((IEnumerable<FlightAllocation>) rawFlights);
      }
    }

    public void SetFlight(string flightName, int timeoutInMinutes)
    {
      flightName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (flightName));
      if (timeoutInMinutes < 0)
        throw new ArgumentException("Flight expiration timeout can't be negative", nameof (timeoutInMinutes));
      flightName = flightName.ToLowerInvariant();
      DateTimeOffset expiration = DateTimeOffset.UtcNow.AddMinutes((double) timeoutInMinutes);
      lock (this.lockObject)
      {
        IEnumerable<FlightAllocation> rawFlights = this.GetRawFlights();
        foreach (FlightAllocation flightAllocation in rawFlights)
        {
          SetFlightsProvider.FlightInformation flightInformation = SetFlightsProvider.FlightInformation.Parse(flightAllocation.FlightName);
          if (flightInformation != null && flightInformation.Flight == flightName)
            return;
        }
        this.SetRawFlights((IEnumerable<FlightAllocation>) rawFlights.UnionWithFlights((IEnumerable<FlightAllocation>) new FlightAllocation[1]
        {
          new FlightAllocation(new SetFlightsProvider.FlightInformation(flightName, expiration).ToString())
        }, StringComparer.Ordinal));
      }
    }

    private IEnumerable<FlightAllocation> GetRawFlights() => this.cachedFlightsProvider.Value.Flights;

    private IEnumerable<ConfigData> GetRawConfigs() => this.cachedFlightsProvider.Value.Configs;

    private void SetRawFlights(IEnumerable<FlightAllocation> rawFlights) => this.cachedFlightsProvider.Value.Flights = rawFlights;

    private IEnumerable<FlightAllocation> ConvertRawDataToPlainFlights(
      IEnumerable<FlightAllocation> rawFlights)
    {
      foreach (FlightAllocation rawFlight in rawFlights)
      {
        SetFlightsProvider.FlightInformation flightInformation = SetFlightsProvider.FlightInformation.Parse(rawFlight.FlightName);
        if (flightInformation != null && flightInformation.ExpirationTime > DateTimeOffset.UtcNow)
          yield return new FlightAllocation(flightInformation.Flight, rawFlight.AllocationId);
      }
    }

    internal sealed class FlightInformation
    {
      private static readonly char[] SplitCharacter = new char[1]
      {
        '#'
      };

      public string Flight { get; private set; }

      public DateTimeOffset ExpirationTime { get; private set; }

      public FlightInformation(string flight, DateTimeOffset expiration)
      {
        this.Flight = flight.ToLowerInvariant();
        this.ExpirationTime = expiration;
      }

      public override string ToString() => this.Flight + "#" + this.ExpirationTime.ToString("u", (IFormatProvider) CultureInfo.InvariantCulture);

      public static SetFlightsProvider.FlightInformation Parse(string rawValue)
      {
        string[] source = rawValue.Split(SetFlightsProvider.FlightInformation.SplitCharacter);
        if (((IEnumerable<string>) source).Count<string>() != 2)
          return (SetFlightsProvider.FlightInformation) null;
        DateTimeOffset expiration;
        try
        {
          expiration = DateTimeOffset.Parse(source[1], (IFormatProvider) CultureInfo.InvariantCulture);
        }
        catch
        {
          return (SetFlightsProvider.FlightInformation) null;
        }
        return new SetFlightsProvider.FlightInformation(source[0], expiration);
      }
    }
  }
}
