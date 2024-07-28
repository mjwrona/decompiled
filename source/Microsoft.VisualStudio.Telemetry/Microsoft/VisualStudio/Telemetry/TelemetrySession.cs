// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetrySession
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights;
using Microsoft.VisualStudio.Telemetry.Metrics.Events;
using Microsoft.VisualStudio.Telemetry.SessionChannel;
using Microsoft.VisualStudio.Telemetry.Tools;
using Microsoft.VisualStudio.Telemetry.WindowsErrorReporting;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  public class TelemetrySession : TelemetryDisposableObject
  {
    public const string ValueNotAvailable = "Unknown";
    public const string DefaultContextName = "Default";
    private ITelemetrySessionInternal internalTelemetrySessionObject;

    internal ITelemetrySessionInternal GetTelemetrySessionInternal() => this.internalTelemetrySessionObject;

    internal void SetTelemetrySessionInternal(ITelemetrySessionInternal internalSession) => this.internalTelemetrySessionObject = this.internalTelemetrySessionObject == null ? internalSession : throw new InvalidOperationException(nameof (internalSession));

    internal bool UseCollector
    {
      get => this.internalTelemetrySessionObject.UseCollector;
      set => this.internalTelemetrySessionObject.UseCollector = value;
    }

    public TelemetrySession(string serializedSession)
      : this(TelemetrySessionSettings.Parse(serializedSession))
    {
    }

    internal TelemetrySession(TelemetrySessionSettings telemetrySessionSettings)
      : this(telemetrySessionSettings, true, TelemetrySessionInitializer.FromSessionSettings(telemetrySessionSettings))
    {
    }

    internal TelemetrySession(
      string serializedSession,
      TelemetrySessionInitializer initializerObject)
      : this(TelemetrySessionSettings.Parse(serializedSession), true, initializerObject)
    {
    }

    public string SessionId => this.internalTelemetrySessionObject.SessionId;

    public bool IsOptedIn
    {
      get => this.internalTelemetrySessionObject.IsOptedIn;
      set => this.internalTelemetrySessionObject.IsOptedIn = value;
    }

    public List<BucketFilter> BucketFiltersToEnableWatsonForFaults
    {
      get => this.internalTelemetrySessionObject.BucketFiltersToEnableWatsonForFaults;
      set => this.internalTelemetrySessionObject.BucketFiltersToEnableWatsonForFaults = value;
    }

    public List<BucketFilter> BucketFiltersToAddDumpsToFaults
    {
      get => this.internalTelemetrySessionObject.BucketFiltersToAddDumpsToFaults;
      set => this.internalTelemetrySessionObject.BucketFiltersToAddDumpsToFaults = value;
    }

    public string HostName
    {
      get => this.internalTelemetrySessionObject.HostName;
      set => this.internalTelemetrySessionObject.HostName = value;
    }

    public CancellationToken CancellationToken => this.internalTelemetrySessionObject.CancellationToken;

    [CLSCompliant(false)]
    public uint AppId
    {
      get => this.internalTelemetrySessionObject.AppId;
      set => this.internalTelemetrySessionObject.AppId = value;
    }

    public bool CanCollectPrivateInformation => this.internalTelemetrySessionObject.CanCollectPrivateInformation;

    public bool IsUserMicrosoftInternal => this.internalTelemetrySessionObject.IsUserMicrosoftInternal;

    public string CalculatedSamplings => this.internalTelemetrySessionObject.CalculatedSamplings;

    public Guid MachineId => this.internalTelemetrySessionObject.MachineId;

    public Guid UserId => this.internalTelemetrySessionObject.UserId;

    public string MacAddressHash => this.internalTelemetrySessionObject.MacAddressHash;

    public long TimeSinceSessionStart => (long) new TimeSpan(DateTime.UtcNow.Ticks - this.ProcessStartTime).TotalMilliseconds;

    internal Action InitializedAction
    {
      set => this.internalTelemetrySessionObject.InitializedAction = value;
    }

    internal EventHandler<TelemetryTestChannelEventArgs> RawTelemetryEventReceived
    {
      get => this.internalTelemetrySessionObject.RawTelemetryEventReceived;
      set => this.internalTelemetrySessionObject.RawTelemetryEventReceived = value;
    }

    internal EventProcessor EventProcessor => this.internalTelemetrySessionObject.EventProcessor;

    internal TelemetryContext DefaultContext => this.internalTelemetrySessionObject.DefaultContext;

    internal bool IsSessionCloned => this.internalTelemetrySessionObject.IsSessionCloned;

    internal TelemetrySessionSettings SessionSettings => this.internalTelemetrySessionObject.SessionSettings;

    internal ITelemetryManifestManager ManifestManager => this.internalTelemetrySessionObject.ManifestManager;

    public void Start() => this.Start(false);

    public void UseVsIsOptedIn(string productVersion) => this.internalTelemetrySessionObject.UseVsIsOptedIn(productVersion);

    public void UseVsIsOptedIn() => this.internalTelemetrySessionObject.UseVsIsOptedIn();

    public TelemetryContext CreateContext(string contextName) => this.internalTelemetrySessionObject.CreateContext(contextName);

    public TelemetryContext GetContext(string contextName) => this.internalTelemetrySessionObject.GetContext(contextName);

    public void PostEvent(string eventName) => this.internalTelemetrySessionObject.PostEvent(eventName);

    public void PostEvent(TelemetryEvent telemetryEvent) => this.internalTelemetrySessionObject.PostEvent(telemetryEvent);

    public void PostMetricEvent(TelemetryMetricEvent metricEvent) => this.internalTelemetrySessionObject.PostMetricEvent(metricEvent);

    public void PostProperty(string propertyName, object propertyValue) => this.internalTelemetrySessionObject.PostProperty(propertyName, propertyValue);

    public void PostRecurringProperty(string propertyName, object propertyValue) => this.internalTelemetrySessionObject.PostRecurringProperty(propertyName, propertyValue);

    public void AddSessionChannel(ISessionChannel sessionChannel) => this.internalTelemetrySessionObject.AddSessionChannel(sessionChannel);

    public string SerializeSettings() => this.internalTelemetrySessionObject.SerializeSettings();

    public void SetSharedProperty(string propertyName, object propertyValue) => this.internalTelemetrySessionObject.SetSharedProperty(propertyName, propertyValue);

    public void RemoveSharedProperty(string propertyName) => this.internalTelemetrySessionObject.RemoveSharedProperty(propertyName);

    public object GetSharedProperty(string propertyName) => this.internalTelemetrySessionObject.GetSharedProperty(propertyName);

    public object GetSharedPropertyAsObject(string propertyName) => this.internalTelemetrySessionObject.GetSharedPropertyAsObject(propertyName);

    public void SetPersistedSharedProperty(string propertyName, string propertyValue) => this.internalTelemetrySessionObject.SetPersistedSharedProperty(propertyName, propertyValue);

    public void SetPersistedSharedProperty(string propertyName, double propertyValue) => this.internalTelemetrySessionObject.SetPersistedSharedProperty(propertyName, propertyValue);

    public void RemovePersistedSharedProperty(string propertyName) => this.internalTelemetrySessionObject.RemovePersistedSharedProperty(propertyName);

    public object GetPersistedSharedProperty(string propertyName) => this.internalTelemetrySessionObject.GetPersistedSharedProperty(propertyName);

    public void RegisterPropertyBag(string name, TelemetryPropertyBag propertyBag) => this.internalTelemetrySessionObject.RegisterPropertyBag(name, propertyBag);

    public void UnregisterPropertyBag(string name) => this.internalTelemetrySessionObject.UnregisterPropertyBag(name);

    public TelemetryPropertyBag GetPropertyBag(string name) => this.internalTelemetrySessionObject.GetPropertyBag(name);

    public async Task DisposeToNetworkAsync(CancellationToken token) => await this.internalTelemetrySessionObject.DisposeToNetworkAsync(token);

    public void RegisterForReliabilityEvent() => this.internalTelemetrySessionObject.RegisterForReliabilityEvent();

    internal long ProcessStartTime => this.internalTelemetrySessionObject.ProcessStartTime;

    internal int ProcessPid => this.internalTelemetrySessionObject.ProcessPid;

    internal void Start(bool checkPendingAsimovEvents) => this.internalTelemetrySessionObject.Start(checkPendingAsimovEvents);

    internal static TelemetrySession Create() => TelemetrySession.Create(TelemetrySessionInitializer.Default);

    internal static TelemetrySession Create(TelemetrySessionInitializer initializerObject)
    {
      initializerObject.RequiresArgumentNotNull<TelemetrySessionInitializer>(nameof (initializerObject));
      if (!TelemetrySessionSettings.IsSessionIdValid(initializerObject.SessionId))
        throw new ArgumentException("Session ID in sessionSettings is not valid", "sessionSettings");
      return new TelemetrySession(new TelemetrySessionSettings(initializerObject.SessionId, initializerObject.InternalSettings, initializerObject.AppInsightsInstrumentationKey, initializerObject.AsimovInstrumentationKey, initializerObject.CollectorApiKey, initializerObject.ProcessCreationTime), false, initializerObject);
    }

    internal TelemetrySession(
      TelemetrySessionSettings settings,
      bool isCloned,
      TelemetrySessionInitializer initializerObject)
    {
      if ("AIF-312cbd79-9dbb-4c48-a7da-3cc2a931cb70".Equals(settings.AsimovInstrumentationKey))
      {
        initializerObject.CollectorApiKey = KeysConstants.VSCollectorApiKey;
        settings.CollectorApiKey = KeysConstants.VSCollectorApiKey;
      }
      else if (KeysConstants.VSCollectorApiKey.Equals(settings.CollectorApiKey))
      {
        initializerObject.AppInsightsInstrumentationKey = "f144292e-e3b2-4011-ac90-20e5c03fbce5";
        initializerObject.AsimovInstrumentationKey = "AIF-312cbd79-9dbb-4c48-a7da-3cc2a931cb70";
        settings.AppInsightsInstrumentationKey = "f144292e-e3b2-4011-ac90-20e5c03fbce5";
        settings.AsimovInstrumentationKey = "AIF-312cbd79-9dbb-4c48-a7da-3cc2a931cb70";
      }
      if (ProductDetectionTools.GetProduct(settings) == ProductTarget.VSCode)
      {
        this.internalTelemetrySessionObject = (ITelemetrySessionInternal) new TelemetrySessionInternalVSCode(settings, isCloned, initializerObject, this);
        this.internalTelemetrySessionObject.LoadCommonProperties();
      }
      else
        this.internalTelemetrySessionObject = (ITelemetrySessionInternal) new TelemetrySessionInternalVS(settings, isCloned, initializerObject, this);
    }

    internal bool Equals(TelemetrySession other) => this.internalTelemetrySessionObject.Equals(other);

    internal void PostValidatedEvent(TelemetryEvent telemetryEvent) => this.internalTelemetrySessionObject.PostValidatedEvent(telemetryEvent);

    internal void AddContext(TelemetryContext telemetryContext) => this.internalTelemetrySessionObject.AddContext(telemetryContext);

    internal void RemoveContext(TelemetryContext telemetryContext) => this.internalTelemetrySessionObject.RemoveContext(telemetryContext);

    internal void AddSessionChannels(IEnumerable<ISessionChannel> channels) => this.internalTelemetrySessionObject.AddSessionChannels(channels);

    internal bool GetCachedUseCollectorFromRegistry() => this.internalTelemetrySessionObject.GetCachedUseCollectorFromRegistry();

    internal bool SetUseCollectorToRegistry(bool? value) => this.internalTelemetrySessionObject.SetUseCollectorToRegistry(value);

    internal void AddContextProperties(TelemetryEvent telemetryEvent) => this.internalTelemetrySessionObject.AddContextProperties(telemetryEvent);

    internal static string Guard(Func<string> provider, string defaultValue)
    {
      try
      {
        string str = provider();
        if (string.IsNullOrEmpty(str))
          str = defaultValue;
        return str;
      }
      catch
      {
        return defaultValue;
      }
    }

    internal static void ValidateEvent(TelemetryEvent telemetryEvent)
    {
      telemetryEvent.RequiresArgumentNotNull<TelemetryEvent>(nameof (telemetryEvent));
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) telemetryEvent.Properties)
      {
        if (property.Key.StartsWith("Reserved.", StringComparison.Ordinal) && TelemetryEvent.IsPropertyNameReserved(property.Key))
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "property '{0}' has reserved prefix 'Reserved'", new object[1]
          {
            (object) property.Key
          }));
      }
    }

    internal void ProcessManifestUseCollectorProperty(TelemetryManifest manifest) => this.internalTelemetrySessionObject.ProcessManifestUseCollectorProperty(manifest);

    public override string ToString() => this.internalTelemetrySessionObject.ToString();

    protected override void DisposeManagedResources() => this.internalTelemetrySessionObject.Dispose();

    public void AddCommonPropertyRange(IDictionary<string, object> properties) => this.internalTelemetrySessionObject.AddCommonProperties(properties);

    public bool TryAddCommonProperty(string propertyName, object propertyValue) => this.internalTelemetrySessionObject.TryAddCommonProperty(propertyName, propertyValue);

    public bool TryGetCommonPropertyValue(string propertyName, out object value) => this.internalTelemetrySessionObject.TryGetCommonPropertyValue(propertyName, out value);

    public IDictionary<string, object> GetCommonPropertyDictionary() => this.internalTelemetrySessionObject.GetCommonPropertyDictionary();
  }
}
