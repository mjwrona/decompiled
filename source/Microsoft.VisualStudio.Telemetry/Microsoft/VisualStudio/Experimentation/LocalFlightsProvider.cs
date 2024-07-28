// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.LocalFlightsProvider
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Utilities.Internal;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Experimentation
{
  internal sealed class LocalFlightsProvider : 
    TelemetryDisposableObject,
    IFlightsProvider,
    IDisposable
  {
    public static readonly string PathToSettingsPrefix = "Software\\Microsoft\\VisualStudio\\ABExp\\";
    public static readonly string PathToConfigSettingsPrefix = "Software\\Microsoft\\VisualStudio\\ABExpConfigs\\";
    private readonly object lockObject = new object();
    private readonly IKeyValueStorage keyValueStorage;
    private readonly string pathToSettings;
    private readonly string pathToConfigSettings;
    private readonly string pathToConfigIds;
    private readonly object lockFlights = new object();
    private IEnumerable<FlightAllocation> flights;
    private IEnumerable<ConfigData> configs;

    public IEnumerable<FlightAllocation> Flights
    {
      get
      {
        if (this.flights == null)
        {
          lock (this.lockFlights)
          {
            if (this.flights == null)
              this.flights = this.ReadFlightsOnce();
          }
        }
        return this.flights;
      }
      set
      {
        lock (this.lockFlights)
        {
          this.keyValueStorage.SetValue<string[]>(this.pathToSettings, value.Select<FlightAllocation, string>((Func<FlightAllocation, string>) (a => a.ToRegistryString())).ToArray<string>());
          this.flights = value;
        }
      }
    }

    public IEnumerable<ConfigData> Configs
    {
      get
      {
        if (this.configs == null)
        {
          lock (this.lockFlights)
          {
            if (this.configs == null)
              this.configs = this.ReadConfigsOnce();
          }
        }
        return this.configs;
      }
      set
      {
        lock (this.lockFlights)
        {
          List<string> stringList1 = new List<string>();
          foreach (ConfigData configData in value)
          {
            List<string> stringList2 = new List<string>();
            stringList1.Add(configData.Id);
            string str = this.pathToConfigSettings + configData.Id + "\\";
            foreach (KeyValuePair<string, object> parameter in (IEnumerable<KeyValuePair<string, object>>) configData.Parameters)
            {
              stringList2.Add(parameter.Key);
              this.keyValueStorage.SetValue<string>(str + parameter.Key, JsonConvert.SerializeObject(parameter.Value));
            }
            this.keyValueStorage.SetValue<string[]>(this.pathToConfigSettings + configData.Id + "_Ids", stringList2.ToArray());
          }
          this.keyValueStorage.SetValue<string[]>(this.pathToConfigIds, stringList1.ToArray());
          this.configs = value;
        }
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

    public LocalFlightsProvider(IKeyValueStorage keyValueStorage, string flightsKey)
    {
      keyValueStorage.RequiresArgumentNotNull<IKeyValueStorage>(nameof (keyValueStorage));
      flightsKey.RequiresArgumentNotNullAndNotEmpty(nameof (flightsKey));
      this.keyValueStorage = keyValueStorage;
      this.pathToSettings = LocalFlightsProvider.PathToSettingsPrefix + flightsKey;
      this.pathToConfigSettings = LocalFlightsProvider.PathToConfigSettingsPrefix + flightsKey + "\\";
      this.pathToConfigIds = this.pathToConfigSettings + "_Ids";
    }

    private IEnumerable<FlightAllocation> ReadFlightsOnce() => ((IEnumerable<string>) this.keyValueStorage.GetValue<string[]>(this.pathToSettings, new string[0])).Where<string>((Func<string, bool>) (s => !string.IsNullOrWhiteSpace(s))).Select<string, FlightAllocation>((Func<string, FlightAllocation>) (s => FlightAllocation.CreateFromRegistryString(s)));

    private IEnumerable<ConfigData> ReadConfigsOnce()
    {
      string[] strArray1 = this.keyValueStorage.GetValue<string[]>(this.pathToConfigIds, new string[0]);
      List<ConfigData> configDataList = new List<ConfigData>();
      foreach (string str1 in strArray1)
      {
        string[] strArray2 = this.keyValueStorage.GetValue<string[]>(this.pathToConfigSettings + str1 + "_Ids", new string[0]);
        Dictionary<string, object> dictionary = new Dictionary<string, object>();
        foreach (string key in strArray2)
        {
          string str2 = this.keyValueStorage.GetValue<string>(this.pathToConfigSettings + str1 + "\\" + key, string.Empty);
          dictionary.Add(key, JsonConvert.DeserializeObject<object>(str2));
        }
        configDataList.Add(new ConfigData()
        {
          Id = str1,
          Parameters = (IDictionary<string, object>) dictionary
        });
      }
      return (IEnumerable<ConfigData>) configDataList;
    }

    public void Start()
    {
    }

    public async Task WaitForReady(CancellationToken token)
    {
      object obj = await Task.FromResult<object>((object) null);
    }
  }
}
