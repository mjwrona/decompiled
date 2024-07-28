// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.ITelemetrySessionInternal
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry.Metrics.Events;
using Microsoft.VisualStudio.Telemetry.SessionChannel;
using Microsoft.VisualStudio.Telemetry.WindowsErrorReporting;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal interface ITelemetrySessionInternal : IDisposable
  {
    TelemetrySession GetHostTelemetrySession();

    string SessionId { get; }

    bool IsOptedIn { get; set; }

    bool UseCollector { get; set; }

    List<BucketFilter> BucketFiltersToEnableWatsonForFaults { get; set; }

    List<BucketFilter> BucketFiltersToAddDumpsToFaults { get; set; }

    string HostName { get; set; }

    CancellationToken CancellationToken { get; }

    uint AppId { get; set; }

    bool CanCollectPrivateInformation { get; }

    bool IsUserMicrosoftInternal { get; }

    string CalculatedSamplings { get; }

    Guid MachineId { get; }

    Guid UserId { get; }

    string MacAddressHash { get; }

    Action InitializedAction { set; }

    EventHandler<TelemetryTestChannelEventArgs> RawTelemetryEventReceived { get; set; }

    EventProcessor EventProcessor { get; }

    TelemetryContext DefaultContext { get; }

    ITelemetryManifestManager ManifestManager { get; }

    TelemetrySessionSettings SessionSettings { get; }

    void Start(bool checkPendingAsimovEvents);

    void UseVsIsOptedIn();

    void UseVsIsOptedIn(string productVersion);

    TelemetryContext CreateContext(string contextName);

    TelemetryContext GetContext(string contextName);

    void PostEvent(string eventName);

    void PostEvent(TelemetryEvent telemetryEvent);

    void PostMetricEvent(TelemetryMetricEvent metricEvent);

    void PostProperty(string propertyName, object propertyValue);

    void PostRecurringProperty(string propertyName, object propertyValue);

    void AddSessionChannel(ISessionChannel sessionChannel);

    string SerializeSettings();

    void SetSharedProperty(string propertyName, object propertyValue);

    void RemoveSharedProperty(string propertyName);

    object GetSharedProperty(string propertyName);

    object GetSharedPropertyAsObject(string propertyName);

    void SetPersistedSharedProperty(string propertyName, string propertyValue);

    void SetPersistedSharedProperty(string propertyName, double propertyValue);

    void SetPersistedSharedProperty(
      string propertyName,
      object propertyValue,
      Action addToBagAction);

    void RemovePersistedSharedProperty(string propertyName);

    object GetPersistedSharedProperty(string propertyName);

    void RegisterPropertyBag(string name, TelemetryPropertyBag propertyBag);

    void UnregisterPropertyBag(string name);

    TelemetryPropertyBag GetPropertyBag(string name);

    Task DisposeToNetworkAsync(CancellationToken token);

    void RegisterForReliabilityEvent();

    long ProcessStartTime { get; }

    int ProcessPid { get; }

    void PostValidatedEvent(TelemetryEvent telemetryEvents);

    bool IsSessionCloned { get; }

    bool GetCachedUseCollectorFromRegistry();

    void AddContext(TelemetryContext telemetryContext);

    void RemoveContext(TelemetryContext telemetryContext);

    void AddSessionChannels(IEnumerable<ISessionChannel> channels);

    bool SetUseCollectorToRegistry(bool? value);

    void AddContextProperties(TelemetryEvent telemetryEvent);

    void ProcessManifestUseCollectorProperty(TelemetryManifest manifest);

    void AddCommonProperties(IDictionary<string, object> properties);

    bool TryAddCommonProperty(string propertyName, object propertyValue);

    bool Equals(TelemetrySession other);

    void LoadCommonProperties();

    bool TryGetCommonPropertyValue(string propertyName, out object value);

    IDictionary<string, object> GetCommonPropertyDictionary();
  }
}
