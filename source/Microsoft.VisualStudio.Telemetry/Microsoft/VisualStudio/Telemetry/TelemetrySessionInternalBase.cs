// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetrySessionInternalBase
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.CSharp.RuntimeBinder;
using Microsoft.VisualStudio.ApplicationInsights;
using Microsoft.VisualStudio.LocalLogger;
using Microsoft.VisualStudio.Telemetry.CommonProperty;
using Microsoft.VisualStudio.Telemetry.Metrics.Events;
using Microsoft.VisualStudio.Telemetry.SessionChannel;
using Microsoft.VisualStudio.Telemetry.Tools;
using Microsoft.VisualStudio.Telemetry.WindowsErrorReporting;
using Microsoft.VisualStudio.Utilities.Internal;
using Microsoft.Win32;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Telemetry
{
  internal abstract class TelemetrySessionInternalBase : 
    TelemetryDisposableObject,
    ITelemetrySessionInternal,
    IDisposable
  {
    protected const string UseCollectorRegKeyName = "UseCollector";
    protected readonly string useCollectorRegKeyPath = string.Empty;
    private const string CommonPropertyBagPathEnvironmentVariableName = "CommonPropertyBagPath";
    public const string ValueNotAvailable = "Unknown";
    public const string DefaultContextName = "Default";
    private const int WaitForPendingPostingTimeout = 1000;
    private const int DisposingIsStarted = 1;
    private const int DisposingNotStarted = 0;
    private const int RecurringPropertyUpdateDelaySeconds = 15;
    private const int RecurringPropertyDelayMinutes = 60;
    protected readonly TelemetrySessionInitializer sessionInitializer;
    private readonly TelemetryPropertyBags.Concurrent<TelemetryContext> sessionContexts = new TelemetryPropertyBags.Concurrent<TelemetryContext>();
    private readonly LinkedList<TelemetryContext> sessionContextStack = new LinkedList<TelemetryContext>();
    private readonly object startedLock = new object();
    private readonly object initializedLock = new object();
    private readonly TelemetryPropertyBags.Concurrent<TelemetryPropertyBag> propertyBagDictionary = new TelemetryPropertyBags.Concurrent<TelemetryPropertyBag>();
    private readonly TelemetryPropertyBags.Concurrent<object> recurringProperties = new TelemetryPropertyBags.Concurrent<object>();
    protected readonly TelemetryPropertyBags.Concurrent<object> commonProperties = new TelemetryPropertyBags.Concurrent<object>();
    private readonly IMachineInformationProvider machineInformationProvider;
    private readonly IMACInformationProvider macInformationProvider;
    private readonly IUserInformationProvider userInformationProvider;
    private readonly TelemetryContext defaultContext;
    protected readonly TelemetrySessionSettings sessionSettings;
    private readonly IContextPropertyManager defaultContextPropertyManager;
    private readonly EventProcessorChannel eventProcessorChannel;
    private readonly IEnumerable<IChannelValidator> channelValidators;
    private readonly Lazy<ITelemetryManifestManager> telemetryManifestManager;
    private readonly IPersistentPropertyBag persistentPropertyBag;
    private readonly IPersistentPropertyBag persistentSharedProperties;
    private readonly ITelemetryScheduler contextScheduler;
    private readonly ITelemetryScheduler recurringPropertyUpdateScheduler;
    private readonly ITelemetryScheduler recurringPropertyScheduler;
    private readonly CancellationTokenSource cancellationTokenSource;
    private readonly IDiagnosticTelemetry diagnosticTelemetry;
    private readonly IIdentityTelemetry identityTelemetry;
    private readonly ITelemetryOptinStatusReader optinStatusReader;
    private readonly HashSet<string> defaultSessionChannelsId = new HashSet<string>();
    private readonly bool isSessionCloned;
    private readonly Lazy<string> previousSessionShutdownRegistryPath;
    private readonly ReaderWriterLockSlim customEventPostProtection = new ReaderWriterLockSlim(LockRecursionPolicy.SupportsRecursion);
    private TelemetryBufferChannel telemetryBufferChannel;
    private WatsonSessionChannel watsonSessionChannel;
    private Stopwatch disposeLatencyTimer;
    private int numberOfDroppedEventsInDisposing;
    private bool isSessionStarted;
    private bool isInitialized;
    private bool isManifestCompleted;
    private bool isMACAddressCompleted;
    private bool isHardwareIdCompleted;
    private int startedDisposing;
    private EventHandler<TelemetryTestChannelEventArgs> rawTelemetryEventReceived;
    private TelemetrySession hostTelemetrySession;

    public bool UseCollector { get; set; }

    public string SessionId => this.sessionSettings.Id;

    public ProductTarget ProductScenario { get; internal set; }

    public bool IsOptedIn
    {
      get => this.sessionSettings.IsOptedIn;
      set
      {
        this.sessionSettings.IsOptedIn = value;
        this.SetOptedInProperty();
      }
    }

    public List<BucketFilter> BucketFiltersToEnableWatsonForFaults
    {
      get => this.sessionSettings.BucketFiltersToEnableWatsonForFaults;
      set => this.sessionSettings.BucketFiltersToEnableWatsonForFaults = value;
    }

    public List<BucketFilter> BucketFiltersToAddDumpsToFaults
    {
      get => this.sessionSettings.BucketFiltersToAddDumpsToFaults;
      set => this.sessionSettings.BucketFiltersToAddDumpsToFaults = value;
    }

    public string HostName
    {
      get => this.sessionSettings.HostName;
      set
      {
        value.RequiresArgumentNotNullAndNotEmpty(nameof (value));
        if (!this.ValidateHostName(value))
          throw new ArgumentException("hostname must be more than 0 characters length, less than 65 characters length and consist of alphanumeric and/or characters '_', '-', ' ', '+'");
        if (!this.sessionSettings.CanOverrideHostName)
          return;
        this.sessionSettings.HostName = value;
      }
    }

    public CancellationToken CancellationToken => this.cancellationTokenSource.Token;

    public uint AppId
    {
      get => this.sessionSettings.AppId;
      set
      {
        if (value < 1000U)
          throw new ArgumentException("AppId for the Telemetry API starts at 1000");
        if (!this.sessionSettings.CanOverrideAppId)
          return;
        this.sessionSettings.AppId = value;
      }
    }

    public bool CanCollectPrivateInformation => this.userInformationProvider.CanCollectPrivateInformation;

    public bool IsUserMicrosoftInternal => this.userInformationProvider.IsUserMicrosoftInternal || this.userInformationProvider.IsMicrosoftAADJoined;

    public string CalculatedSamplings { get; private set; }

    public Guid MachineId => this.machineInformationProvider.MachineId;

    public Guid UserId => this.userInformationProvider.UserId;

    public string MacAddressHash => this.macInformationProvider.GetMACAddressHash();

    public Action InitializedAction
    {
      set => this.eventProcessorChannel.InitializedAction = value;
    }

    public EventHandler<TelemetryTestChannelEventArgs> RawTelemetryEventReceived
    {
      get => this.rawTelemetryEventReceived;
      set => this.rawTelemetryEventReceived += value;
    }

    EventProcessor ITelemetrySessionInternal.EventProcessor => this.EventProcessor;

    TelemetryContext ITelemetrySessionInternal.DefaultContext => this.DefaultContext;

    ITelemetryManifestManager ITelemetrySessionInternal.ManifestManager => this.ManifestManager;

    TelemetrySessionSettings ITelemetrySessionInternal.SessionSettings => this.SessionSettings;

    bool ITelemetrySessionInternal.IsSessionCloned => this.IsSessionCloned;

    internal EventProcessor EventProcessor { get; }

    internal TelemetryContext DefaultContext => this.defaultContext;

    internal bool IsSessionCloned => this.isSessionCloned;

    internal ITelemetryManifestManager ManifestManager => this.telemetryManifestManager.Value;

    internal TelemetrySessionSettings SessionSettings => this.sessionSettings;

    internal TelemetrySessionInternalBase(
      TelemetrySessionSettings settings,
      bool isCloned,
      TelemetrySessionInitializer initializerObject,
      TelemetrySession session)
    {
      TelemetrySessionInternalBase sessionInternalBase = this;
      initializerObject.RequiresArgumentNotNull<TelemetrySessionInitializer>(nameof (initializerObject));
      initializerObject.Validate();
      this.hostTelemetrySession = session;
      if (settings.IsInitialSession)
        isCloned = false;
      initializerObject.AppInsightsInstrumentationKey = settings.AppInsightsInstrumentationKey;
      initializerObject.AsimovInstrumentationKey = settings.AsimovInstrumentationKey;
      initializerObject.CollectorApiKey = settings.CollectorApiKey;
      TelemetryService.EnsureEtwProviderInitialized();
      this.sessionInitializer = initializerObject;
      this.sessionSettings = settings;
      this.isSessionCloned = isCloned;
      this.useCollectorRegKeyPath = this.GetUseCollectorRegKeyPath();
      this.UseCollector = this.GetCachedUseCollectorFromRegistry();
      this.cancellationTokenSource = initializerObject.CancellationTokenSource;
      this.diagnosticTelemetry = initializerObject.DiagnosticTelemetry;
      this.identityTelemetry = initializerObject.IdentityTelemetry;
      this.optinStatusReader = initializerObject.OptinStatusReader;
      this.channelValidators = initializerObject.ChannelValidators;
      this.telemetryBufferChannel = new TelemetryBufferChannel();
      initializerObject.WatsonSessionChannelBuilder.Build(this.hostTelemetrySession);
      WatsonSessionChannel watsonSessionChannel = initializerObject.WatsonSessionChannelBuilder.WatsonSessionChannel;
      if (this.IsValidChannel((ISessionChannel) watsonSessionChannel))
        this.watsonSessionChannel = watsonSessionChannel;
      this.machineInformationProvider = initializerObject.MachineInformationProvider;
      this.macInformationProvider = initializerObject.MACInformationProvider;
      this.userInformationProvider = initializerObject.UserInformationProvider;
      this.defaultContextPropertyManager = initializerObject.DefaultContextPropertyManager;
      this.persistentPropertyBag = initializerObject.PersistentPropertyBag;
      initializerObject.EventProcessorChannelBuilder.Build(this.hostTelemetrySession);
      this.persistentSharedProperties = initializerObject.PersistentSharedProperties;
      this.EventProcessor = initializerObject.EventProcessorChannelBuilder.EventProcessor;
      if (initializerObject.CustomActionToAdd != null)
      {
        foreach (IEventProcessorAction eventProcessorAction in initializerObject.CustomActionToAdd)
          this.EventProcessor.AddCustomAction(eventProcessorAction);
      }
      this.eventProcessorChannel = initializerObject.EventProcessorChannelBuilder.EventProcessorChannel;
      this.telemetryManifestManager = new Lazy<ITelemetryManifestManager>((Func<ITelemetryManifestManager>) (() => initializerObject.TelemetryManifestManagerBuilder.Build(sessionInternalBase.hostTelemetrySession)));
      this.contextScheduler = initializerObject.ContextScheduler;
      this.recurringPropertyUpdateScheduler = initializerObject.RecurringPropertyUpdateScheduler;
      this.recurringPropertyScheduler = initializerObject.RecurringPropertyScheduler;
      if (this.recurringPropertyUpdateScheduler == null)
      {
        this.recurringPropertyUpdateScheduler = (ITelemetryScheduler) new TelemetryScheduler();
        this.recurringPropertyUpdateScheduler.InitializeTimed(TimeSpan.FromSeconds(15.0));
      }
      if (this.recurringPropertyScheduler == null)
      {
        this.recurringPropertyScheduler = (ITelemetryScheduler) new TelemetryScheduler();
        this.recurringPropertyScheduler.InitializeTimed(TimeSpan.FromMinutes(60.0));
      }
      if (initializerObject.ChannelsToAdd != null)
        this.AddSessionChannels(initializerObject.ChannelsToAdd);
      this.defaultContext = this.CreateDefaultContext();
      this.macInformationProvider.MACAddressHashCalculationCompleted += new EventHandler<EventArgs>(this.MACAddressHashCalculationCompleted);
      this.identityTelemetry.IdentityInformationProvider.HardwareIdCalculationCompleted += new EventHandler<EventArgs>(this.HardwareIdCalculationCompleted);
      this.previousSessionShutdownRegistryPath = new Lazy<string>((Func<string>) (() => sessionInternalBase.InitializeReliabilityRegistryPath()), false);
    }

    public void UseVsIsOptedIn(string productVersion)
    {
      productVersion.RequiresArgumentNotNullAndNotEmpty(nameof (productVersion));
      this.IsOptedIn = this.optinStatusReader.ReadIsOptedInStatus(productVersion);
    }

    public void UseVsIsOptedIn() => this.IsOptedIn = this.optinStatusReader.ReadIsOptedInStatus(this.hostTelemetrySession);

    public TelemetryContext CreateContext(string contextName)
    {
      this.RequiresNotDisposed();
      TelemetrySessionInternalBase.RequiresNotDefaultContextName(contextName);
      return new TelemetryContext(contextName, (ITelemetrySessionInternal) this, this.contextScheduler);
    }

    public TelemetryContext GetContext(string contextName)
    {
      TelemetrySessionInternalBase.RequiresNotDefaultContextName(contextName);
      return this.sessionContexts.GetOrDefault<string, TelemetryContext>(contextName);
    }

    public void PostEvent(string eventName)
    {
      if (this.IsDisposed)
        return;
      this.PostEvent(new TelemetryEvent(eventName));
    }

    public void PostEvent(TelemetryEvent telemetryEvent)
    {
      if (this.IsDisposed)
        return;
      bool flag = true;
      if (telemetryEvent is FaultEvent faultEvent && this.watsonSessionChannel != null)
      {
        this.watsonSessionChannel.PostEvent((TelemetryEvent) faultEvent);
        flag = faultEvent.PostThisEventToTelemetry;
      }
      if (flag)
      {
        if (!this.customEventPostProtection.TryEnterReadLock(0))
        {
          ++this.numberOfDroppedEventsInDisposing;
          return;
        }
        TelemetrySession.ValidateEvent(telemetryEvent);
        TelemetryContext.ValidateEvent(telemetryEvent);
        this.PostValidatedEvent(telemetryEvent);
        this.customEventPostProtection.ExitReadLock();
      }
      if (telemetryEvent is TelemetryActivity activity)
        TelemetryService.TelemetryEventSource.WriteActivityPostEvent(activity, this.hostTelemetrySession);
      else
        TelemetryService.TelemetryEventSource.WriteTelemetryPostEvent(telemetryEvent, this.hostTelemetrySession);
    }

    public void PostMetricEvent(TelemetryMetricEvent metricEvent)
    {
      metricEvent.SetProperties();
      this.PostEvent(metricEvent.MetricEvent);
    }

    public void PostProperty(string propertyName, object propertyValue)
    {
      if (this.IsDisposed)
        return;
      this.DefaultContext.PostProperty(propertyName, propertyValue);
    }

    public void PostRecurringProperty(string propertyName, object propertyValue)
    {
      if (this.IsDisposed)
        return;
      propertyName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (propertyName));
      propertyValue.RequiresArgumentNotNull<object>(nameof (propertyValue));
      this.recurringProperties["VS.TelemetryApi.RecurringProperties." + propertyName] = propertyValue;
      this.recurringPropertyUpdateScheduler.ScheduleTimed(new Action(this.FlushRecurringProperties));
      this.recurringPropertyScheduler.ScheduleTimed(new Action(this.FlushRecurringProperties), true);
    }

    public void AddSessionChannel(ISessionChannel sessionChannel)
    {
      this.RequiresNotDisposed();
      sessionChannel.RequiresArgumentNotNull<ISessionChannel>(nameof (sessionChannel));
      if (!this.IsValidChannel(sessionChannel))
        return;
      this.EventProcessor.AddChannel(sessionChannel);
      if ((sessionChannel.Properties & ChannelProperties.Default) != ChannelProperties.Default)
        return;
      this.defaultSessionChannelsId.Add(sessionChannel.ChannelId);
    }

    public string SerializeSettings()
    {
      this.RequiresNotDisposed();
      this.sessionSettings.UserId = new Guid?(this.UserId);
      if (string.Equals(this.sessionInitializer.HostInformationProvider.ProcessName, "devenv", StringComparison.OrdinalIgnoreCase) || string.Equals(this.sessionInitializer.HostInformationProvider.ProcessName, "blend", StringComparison.OrdinalIgnoreCase))
      {
        this.sessionSettings.VSExeVersion = this.sessionInitializer.HostInformationProvider.ProcessExeVersion;
        if (this.DefaultContext.SharedProperties.ContainsKey("VS.Core.SkuName") && this.DefaultContext.SharedProperties["VS.Core.SkuName"] is string sharedProperty)
          this.sessionSettings.SkuName = sharedProperty;
      }
      return this.sessionSettings.ToString();
    }

    public void SetSharedProperty(string propertyName, object propertyValue)
    {
      if (this.IsDisposed)
        return;
      propertyName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (propertyName));
      this.DefaultContext.SharedProperties[propertyName] = propertyValue;
    }

    public void RemoveSharedProperty(string propertyName)
    {
      if (this.IsDisposed)
        return;
      propertyName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (propertyName));
      this.DefaultContext.SharedProperties.Remove(propertyName);
    }

    public object GetSharedProperty(string propertyName) => this.DefaultContext.SharedProperties.GetOrDefault<string, object>(propertyName);

    public object GetSharedPropertyAsObject(string propertyName) => this.DefaultContext.SharedProperties.GetOrDefault<string, object>(propertyName);

    public void RemovePersistedSharedProperty(string propertyName)
    {
      if (this.IsDisposed)
        return;
      propertyName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (propertyName));
      object persistedSharedProperty = this.GetPersistedSharedProperty(propertyName);
      this.persistentSharedProperties.RemoveProperty(propertyName);
      this.DefaultContext.SharedProperties.Remove(propertyName);
      // ISSUE: reference to a compiler-generated field
      if (TelemetrySessionInternalBase.\u003C\u003Eo__135.\u003C\u003Ep__1 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TelemetrySessionInternalBase.\u003C\u003Eo__135.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (TelemetrySessionInternalBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      Func<CallSite, object, bool> target = TelemetrySessionInternalBase.\u003C\u003Eo__135.\u003C\u003Ep__1.Target;
      // ISSUE: reference to a compiler-generated field
      CallSite<Func<CallSite, object, bool>> p1 = TelemetrySessionInternalBase.\u003C\u003Eo__135.\u003C\u003Ep__1;
      // ISSUE: reference to a compiler-generated field
      if (TelemetrySessionInternalBase.\u003C\u003Eo__135.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TelemetrySessionInternalBase.\u003C\u003Eo__135.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (TelemetrySessionInternalBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj = TelemetrySessionInternalBase.\u003C\u003Eo__135.\u003C\u003Ep__0.Target((CallSite) TelemetrySessionInternalBase.\u003C\u003Eo__135.\u003C\u003Ep__0, persistedSharedProperty, (object) null);
      if (!target((CallSite) p1, obj))
        return;
      this.PostEvent(new TelemetryEvent("VS/TelemetryApi/PersistedSharedProperty/Remove")
      {
        Properties = {
          ["VS.TelemetryApi.PersistedSharedProperty.Name"] = (object) propertyName
        }
      });
    }

    public object GetPersistedSharedProperty(string propertyName) => this.persistentSharedProperties.GetProperty(propertyName);

    public void RegisterPropertyBag(string name, TelemetryPropertyBag propertyBag)
    {
      name.RequiresArgumentNotNull<string>(nameof (name));
      this.propertyBagDictionary[name] = propertyBag;
    }

    public void UnregisterPropertyBag(string name) => this.propertyBagDictionary.Remove<string, TelemetryPropertyBag>(name);

    public TelemetryPropertyBag GetPropertyBag(string name) => this.propertyBagDictionary.GetOrDefault<string, TelemetryPropertyBag>(name);

    public async Task DisposeToNetworkAsync(CancellationToken token)
    {
      if (!this.DisposeStart())
        return;
      await this.eventProcessorChannel.DisposeAndTransmitAsync(token).ConfigureAwait(false);
      this.DisposeEnd();
    }

    protected abstract string InitializeReliabilityRegistryPath();

    protected abstract string GetUseCollectorRegKeyPath();

    public void RegisterForReliabilityEvent()
    {
      if (Platform.IsWindows)
      {
        try
        {
          int num1 = (int) NativeMethods.WerRegisterCustomMetadata("VS.SessionId", this.SessionId);
          TelemetrySessionInitializer sessionInitializer = this.sessionInitializer;
          int num2;
          if (sessionInitializer == null)
          {
            num2 = 0;
          }
          else
          {
            IHostInformationProvider informationProvider = sessionInitializer.HostInformationProvider;
            if (informationProvider == null)
            {
              num2 = 0;
            }
            else
            {
              int num3 = informationProvider.Is64BitProcess ? 1 : 0;
              num2 = 1;
            }
          }
          if (num2 != 0)
          {
            int num4 = (int) NativeMethods.WerRegisterCustomMetadata("VS.Is64BitProcess", this.sessionInitializer.HostInformationProvider.Is64BitProcess.ToString() ?? "False");
          }
        }
        catch (EntryPointNotFoundException ex)
        {
        }
      }
      this.SetReliabiltyValuesInRegistry(this.previousSessionShutdownRegistryPath.Value);
    }

    private void SetReliabiltyValuesInRegistry(string regPath)
    {
      if (this.sessionInitializer?.HostInformationProvider == null || this.sessionInitializer?.RegistryTools == null)
        return;
      this.sessionInitializer.RegistryTools.SetRegistryFromCurrentUserRoot(regPath, "PID", (object) (int) this.sessionInitializer.HostInformationProvider.ProcessId);
      this.sessionInitializer.RegistryTools.SetRegistryFromCurrentUserRoot(regPath, "ExeName", (object) this.GetExeName());
      this.sessionInitializer.RegistryTools.SetRegistryFromCurrentUserRoot(regPath, "VSExeVersion", (object) this.sessionInitializer.HostInformationProvider.ProcessExeVersion);
      this.sessionInitializer.RegistryTools.SetRegistryFromCurrentUserRoot(regPath, "ShutdownReason", (object) 3);
      this.sessionInitializer.RegistryTools.SetRegistryFromCurrentUserRoot(regPath, "VSIs64BitProcess", (object) this.sessionInitializer.HostInformationProvider.Is64BitProcess.ToString());
      this.sessionInitializer.RegistryTools.SetRegistryFromCurrentUserRoot(regPath, "VSInstanceId", (object) this.SessionId);
      if (Platform.IsWindows)
      {
        ulong? creationFileTime = NativeMethods.GetProcessCreationFileTime();
        if (creationFileTime.HasValue)
          this.sessionInitializer.RegistryTools.SetRegistryFromCurrentUserRoot(regPath, "CTime", (object) creationFileTime, RegistryValueKind.QWord);
      }
      else
        this.sessionInitializer.RegistryTools.SetRegistryFromCurrentUserRoot(regPath, "CTime", (object) this.ProcessStartTime);
      this.StartReliabilityHeartbeat();
    }

    private string GetExeName()
    {
      if (!Platform.IsWindows)
        return this.sessionInitializer.HostInformationProvider.ProcessName;
      string fullProcessExeName = NativeMethods.GetFullProcessExeName();
      if (!string.IsNullOrEmpty(fullProcessExeName))
        return System.IO.Path.GetFileName(fullProcessExeName);
      return !string.Equals(System.IO.Path.GetExtension(this.sessionInitializer.HostInformationProvider.ProcessName), ".exe", StringComparison.OrdinalIgnoreCase) ? this.sessionInitializer.HostInformationProvider.ProcessName + ".exe" : this.sessionInitializer.HostInformationProvider.ProcessName;
    }

    public long ProcessStartTime => this.sessionSettings != null ? this.sessionSettings.ProcessStartTime : 0L;

    public int ProcessPid => this.sessionInitializer != null ? (int) this.sessionInitializer.HostInformationProvider.ProcessId : -1;

    public void Start(bool checkPendingAsimovEvents)
    {
      if (this.isSessionStarted)
        return;
      lock (this.startedLock)
      {
        if (this.isSessionStarted)
          return;
        this.defaultContextPropertyManager.AddDefaultContextProperties(this.DefaultContext);
        LocalFileLoggerService.Default.Enabled = this.sessionInitializer.InternalSettings.IsLocalLoggerEnabled();
        this.InitializeWithDefaultChannels(checkPendingAsimovEvents);
        if (!this.isSessionCloned)
          this.defaultContextPropertyManager.PostDefaultContextProperties(this.DefaultContext);
        this.SetOptedInProperty();
        this.SetSharedPropertiesFromSessionSettings();
        this.isSessionStarted = true;
      }
    }

    private void SetSharedPropertiesFromSessionSettings()
    {
      if (!string.IsNullOrWhiteSpace(this.sessionSettings.VSExeVersion))
        this.SetSharedProperty("VS.Core.VSExeVersion", (object) this.sessionSettings.VSExeVersion);
      if (string.IsNullOrWhiteSpace(this.sessionSettings.SkuName))
        return;
      this.SetSharedProperty("VS.Core.VSSkuName", (object) this.sessionSettings.SkuName);
    }

    public bool Equals(TelemetrySession other) => this.sessionSettings.Equals((object) other.SessionSettings);

    public void PostValidatedEvent(TelemetryEvent telemetryEvent)
    {
      if (this.IsDisposed)
        return;
      TelemetryEvent telemetryEvent1 = telemetryEvent.BuildChannelEvent(this.ProcessStartTime, this.SessionId);
      this.AddAdditionalProperties(telemetryEvent1);
      telemetryEvent1.PostTimestamp = DateTimeOffset.Now;
      EventHandler<TelemetryTestChannelEventArgs> telemetryEventReceived = this.RawTelemetryEventReceived;
      if (telemetryEventReceived != null)
        telemetryEventReceived((object) this, new TelemetryTestChannelEventArgs()
        {
          Event = telemetryEvent1
        });
      if (!this.isInitialized)
        this.telemetryBufferChannel.PostEvent(telemetryEvent1);
      else
        this.PostProcessedEvent(telemetryEvent1);
    }

    private void AddAdditionalProperties(TelemetryEvent channelTelemetryEvent)
    {
      this.AddContextProperties(channelTelemetryEvent);
      this.AddCommonPropertiesToEvent(channelTelemetryEvent);
    }

    public void AddContext(TelemetryContext telemetryContext)
    {
      if (this.IsDisposed)
        return;
      telemetryContext.RequiresArgumentNotNull<TelemetryContext>(nameof (telemetryContext));
      string contextName = telemetryContext.ContextName;
      if (!this.sessionContexts.TryAdd(contextName, telemetryContext))
        throw new ArgumentException("a context with name '" + contextName + "' already exist.");
      lock (this.sessionContextStack)
        this.sessionContextStack.AddLast(telemetryContext);
    }

    public void RemoveContext(TelemetryContext telemetryContext)
    {
      if (this.IsDisposed)
        return;
      telemetryContext.RequiresArgumentNotNull<TelemetryContext>(nameof (telemetryContext));
      this.sessionContexts.Remove<string, TelemetryContext>(telemetryContext.ContextName);
      lock (this.sessionContextStack)
        this.sessionContextStack.Remove(telemetryContext);
    }

    public void AddSessionChannels(IEnumerable<ISessionChannel> channels)
    {
      channels.RequiresArgumentNotNull<IEnumerable<ISessionChannel>>(nameof (channels));
      foreach (ISessionChannel channel in channels)
        this.AddSessionChannel(channel);
    }

    protected override void DisposeManagedResources()
    {
      if (!this.DisposeStart())
        return;
      this.eventProcessorChannel.Dispose();
      this.hostTelemetrySession = (TelemetrySession) null;
      this.DisposeEnd();
    }

    private bool DisposeStart()
    {
      if (Interlocked.CompareExchange(ref this.startedDisposing, 1, 0) == 1)
        return false;
      this.disposeLatencyTimer = new Stopwatch();
      this.disposeLatencyTimer.Start();
      if (!this.isInitialized)
      {
        if (!this.isSessionStarted)
          this.Start(false);
        if (!this.ManifestManager.ForceReadManifest())
          this.TelemetryManifestUpdateStatus((object) this, new TelemetryManifestEventArgs((TelemetryManifest) null));
        this.MACAddressHashCalculationCompleted((object) this, EventArgs.Empty);
        this.HardwareIdCalculationCompleted((object) this, EventArgs.Empty);
      }
      this.macInformationProvider.MACAddressHashCalculationCompleted -= new EventHandler<EventArgs>(this.MACAddressHashCalculationCompleted);
      this.identityTelemetry.IdentityInformationProvider.HardwareIdCalculationCompleted -= new EventHandler<EventArgs>(this.HardwareIdCalculationCompleted);
      this.ManifestManager.UpdateTelemetryManifestStatusEvent -= new EventHandler<TelemetryManifestEventArgs>(this.TelemetryManifestUpdateStatus);
      this.ManifestManager.Dispose();
      this.cancellationTokenSource.Cancel();
      this.defaultContextPropertyManager.Dispose();
      this.EventProcessor.PostDiagnosticInformationIfNeeded();
      this.customEventPostProtection.TryEnterWriteLock(1000);
      List<TelemetryContext> list;
      lock (this.sessionContextStack)
        list = this.sessionContextStack.Reverse<TelemetryContext>().ToList<TelemetryContext>();
      this.recurringPropertyUpdateScheduler.CancelTimed(true);
      this.recurringPropertyScheduler.CancelTimed();
      foreach (TelemetryDisposableObject disposableObject in list)
        disposableObject.Dispose();
      if (this.previousSessionShutdownRegistryPath.IsValueCreated)
        this.IndicateNormalShutdown();
      this.SetUseCollectorToRegistry(new bool?(this.UseCollector));
      return true;
    }

    private void DisposeEnd()
    {
      lock (this.startedLock)
        this.isSessionStarted = false;
      this.persistentPropertyBag.SetProperty("VS.TelemetryApi.DroppedEventsDuringDisposing", this.numberOfDroppedEventsInDisposing);
      this.persistentPropertyBag.SetProperty("VS.TelemetryApi.TotalDisposeLatency", (int) this.disposeLatencyTimer.ElapsedMilliseconds);
    }

    private bool IsValidChannel(ISessionChannel sessionChannel)
    {
      foreach (IChannelValidator channelValidator in this.channelValidators)
      {
        if (!channelValidator.IsValid(sessionChannel))
          return false;
      }
      return true;
    }

    private static void RequiresNotDefaultContextName(string contextName)
    {
      if (TelemetryPropertyBags.KeyComparer.Equals(contextName, "Default"))
        throw new ArgumentException("attempt to create context with reserved 'Default' name");
    }

    private void PostProcessedEvent(TelemetryEvent telemetryEvent)
    {
      if (this.IsDisposed)
        return;
      this.eventProcessorChannel.PostEvent(telemetryEvent);
    }

    public abstract bool GetCachedUseCollectorFromRegistry();

    public bool SetUseCollectorToRegistry(bool? value) => string.IsNullOrEmpty(this.useCollectorRegKeyPath) || this.sessionInitializer.RegistryTools.SetRegistryFromCurrentUserRoot(this.useCollectorRegKeyPath, "UseCollector", (object) value);

    private TelemetryContext CreateDefaultContext() => new TelemetryContext("Default", (ITelemetrySessionInternal) this, this.contextScheduler, this.isSessionCloned, new Action<TelemetryContext>(this.defaultContextPropertyManager.AddRealtimeDefaultContextProperties));

    private void InitializeWithDefaultChannels(bool checkPendingAsimovEvents)
    {
      TelemetryManifest manifest = TelemetryManifest.BuildDefaultManifest();
      manifest.UseCollector = this.UseCollector;
      this.ProcessManifestUseCollectorProperty(manifest);
      this.EventProcessor.CurrentManifest = manifest;
      this.ManifestManager.UpdateTelemetryManifestStatusEvent += new EventHandler<TelemetryManifestEventArgs>(this.TelemetryManifestUpdateStatus);
      this.AddSessionChannels(this.sessionInitializer.CreateSessionChannels(this.hostTelemetrySession, checkPendingAsimovEvents));
      this.AddSessionChannel((ISessionChannel) GlobalTelemetryTestChannel.Instance);
      this.ManifestManager.Start(this.HostName, this.startedDisposing == 1);
    }

    private void SetOptedInProperty() => this.SetSharedProperty("VS.Core.User.IsOptedIn", (object) this.sessionSettings.IsOptedIn);

    private void SetInternalInformationProperties(TelemetryManifest telemetryManifest)
    {
      this.SetInternalUserNameIfApplicable(telemetryManifest);
      this.SetInternalComputerNameIfApplicable();
      this.SetInternalDomainNameIfApplicable();
    }

    private void SetInternalUserNameIfApplicable(TelemetryManifest telemetryManifest)
    {
      if ((!this.IsUserMicrosoftInternal || !telemetryManifest.ShouldSendAliasForAllInternalUsers) && !this.CanCollectPrivateInformation)
        return;
      this.DefaultContext.SharedProperties["VS.Core.Internal.UserName"] = (object) TelemetrySession.Guard((Func<string>) (() => Environment.UserName), "Unknown");
    }

    private void SetInternalComputerNameIfApplicable()
    {
      if (!this.CanCollectPrivateInformation)
        return;
      this.DefaultContext.SharedProperties["VS.Core.Internal.ComputerName"] = (object) TelemetrySession.Guard((Func<string>) (() => Environment.MachineName), "Unknown");
    }

    private void SetInternalDomainNameIfApplicable()
    {
      if (!this.CanCollectPrivateInformation)
        return;
      this.DefaultContext.SharedProperties["VS.Core.Internal.UserDomainName"] = (object) TelemetrySession.Guard((Func<string>) (() => Environment.UserDomainName), "Unknown");
    }

    public void AddContextProperties(TelemetryEvent telemetryEvent)
    {
      if (this.IsDisposed)
        return;
      foreach (KeyValuePair<string, TelemetryContext> sessionContext in (ConcurrentDictionary<string, TelemetryContext>) this.sessionContexts)
        sessionContext.Value.ProcessEvent(telemetryEvent);
      foreach (KeyValuePair<string, TelemetryContext> sessionContext in (ConcurrentDictionary<string, TelemetryContext>) this.sessionContexts)
        sessionContext.Value.ProcessEventRealtime(telemetryEvent);
    }

    public void SetPersistedSharedProperty(string propertyName, string propertyValue) => this.SetPersistedSharedProperty(propertyName, (object) propertyValue, (Action) (() => this.persistentSharedProperties.SetProperty(propertyName, propertyValue)));

    public void SetPersistedSharedProperty(string propertyName, double propertyValue) => this.SetPersistedSharedProperty(propertyName, (object) propertyValue, (Action) (() => this.persistentSharedProperties.SetProperty(propertyName, propertyValue)));

    public void SetPersistedSharedProperty(
      string propertyName,
      object propertyValue,
      Action addToBagAction)
    {
      if (this.IsDisposed)
        return;
      propertyName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (propertyName));
      object persistedSharedProperty = this.GetPersistedSharedProperty(propertyName);
      addToBagAction();
      this.DefaultContext.SharedProperties[propertyName] = propertyValue;
      // ISSUE: reference to a compiler-generated field
      if (TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__0 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__0 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.Equal, typeof (TelemetrySessionInternalBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj1 = TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__0.Target((CallSite) TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__0, persistedSharedProperty, (object) null);
      // ISSUE: reference to a compiler-generated field
      if (TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__5 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__5 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (TelemetrySessionInternalBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      if (!TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__5.Target((CallSite) TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__5, obj1))
      {
        // ISSUE: reference to a compiler-generated field
        if (TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__4 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__4 = CallSite<Func<CallSite, object, bool>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.IsTrue, typeof (TelemetrySessionInternalBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, bool> target1 = TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__4.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, bool>> p4 = TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__4;
        // ISSUE: reference to a compiler-generated field
        if (TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__3 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__3 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.BinaryOperationLogical, ExpressionType.Or, typeof (TelemetrySessionInternalBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object, object> target2 = TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__3.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object, object>> p3 = TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__3;
        object obj2 = obj1;
        // ISSUE: reference to a compiler-generated field
        if (TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__2 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__2 = CallSite<Func<CallSite, object, object>>.Create(Binder.UnaryOperation(CSharpBinderFlags.None, ExpressionType.Not, typeof (TelemetrySessionInternalBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[1]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        Func<CallSite, object, object> target3 = TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__2.Target;
        // ISSUE: reference to a compiler-generated field
        CallSite<Func<CallSite, object, object>> p2 = TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__2;
        // ISSUE: reference to a compiler-generated field
        if (TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__1 == null)
        {
          // ISSUE: reference to a compiler-generated field
          TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__1 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.InvokeMember(CSharpBinderFlags.None, "Equals", (IEnumerable<Type>) null, typeof (TelemetrySessionInternalBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
          {
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
            CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.UseCompileTimeType, (string) null)
          }));
        }
        // ISSUE: reference to a compiler-generated field
        // ISSUE: reference to a compiler-generated field
        object obj3 = TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__1.Target((CallSite) TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__1, persistedSharedProperty, propertyValue);
        object obj4 = target3((CallSite) p2, obj3);
        object obj5 = target2((CallSite) p3, obj2, obj4);
        if (!target1((CallSite) p4, obj5))
          return;
      }
      TelemetryEvent telemetryEvent = new TelemetryEvent("VS/TelemetryApi/PersistedSharedProperty/Set");
      telemetryEvent.Properties["VS.TelemetryApi.PersistedSharedProperty.Name"] = (object) propertyName;
      IDictionary<string, object> properties = telemetryEvent.Properties;
      // ISSUE: reference to a compiler-generated field
      if (TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__6 == null)
      {
        // ISSUE: reference to a compiler-generated field
        TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__6 = CallSite<Func<CallSite, object, object, object>>.Create(Binder.BinaryOperation(CSharpBinderFlags.None, ExpressionType.NotEqual, typeof (TelemetrySessionInternalBase), (IEnumerable<CSharpArgumentInfo>) new CSharpArgumentInfo[2]
        {
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.None, (string) null),
          CSharpArgumentInfo.Create(CSharpArgumentInfoFlags.Constant, (string) null)
        }));
      }
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      object obj6 = TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__6.Target((CallSite) TelemetrySessionInternalBase.\u003C\u003Eo__176.\u003C\u003Ep__6, persistedSharedProperty, (object) null);
      properties["VS.TelemetryApi.PersistedSharedProperty.IsChangedValue"] = obj6;
      this.PostEvent(telemetryEvent);
    }

    private void FlushRecurringProperties()
    {
      if (this.IsDisposed)
        return;
      bool flag1 = this.recurringPropertyUpdateScheduler.CanEnterTimedDelegate();
      bool flag2 = this.recurringPropertyScheduler.CanEnterTimedDelegate();
      if (!flag1 && !flag2)
        return;
      TelemetryEvent telemetryEvent = new TelemetryEvent("VS/TelemetryApi/RecurringProperties");
      telemetryEvent.Properties.AddRange<string, object>((IDictionary<string, object>) this.recurringProperties);
      this.PostEvent(telemetryEvent);
      if (flag1)
        this.recurringPropertyUpdateScheduler.ExitTimedDelegate();
      if (!flag2)
        return;
      this.recurringPropertyScheduler.ExitTimedDelegate();
    }

    private void TelemetryManifestUpdateStatus(object sender, TelemetryManifestEventArgs e)
    {
      if (this.IsDisposed)
        return;
      if (e.IsSuccess)
      {
        TelemetryManifest telemetryManifest = e.TelemetryManifest;
        this.CalculatedSamplings = telemetryManifest.CalculateAllSamplings(this.hostTelemetrySession);
        this.EventProcessor.CurrentManifest = telemetryManifest;
        if (this.UseCollector != telemetryManifest.UseCollector)
          this.ProcessManifestUseCollectorProperty(telemetryManifest);
      }
      else
        this.CalculatedSamplings = "e.IsSuccess == false";
      if (!this.isManifestCompleted)
      {
        this.identityTelemetry.IdentityInformationProvider.Initialize(this.DefaultContext, this.contextScheduler, e.TelemetryManifest?.MachineIdentityConfig);
        this.SetInternalInformationProperties(this.EventProcessor.CurrentManifest);
      }
      this.isManifestCompleted = true;
      this.InitializeSession();
    }

    public void ProcessManifestUseCollectorProperty(TelemetryManifest manifest)
    {
      bool flag1 = string.IsNullOrEmpty(this.sessionSettings.AppInsightsInstrumentationKey);
      bool flag2 = string.IsNullOrEmpty(this.sessionSettings.AsimovInstrumentationKey);
      bool flag3 = string.IsNullOrEmpty(this.sessionSettings.CollectorApiKey);
      this.UseCollector = manifest.UseCollector;
      if (flag1 | flag2 | flag3 && (this.UseCollector || flag2))
      {
        if (!flag3 && !KeysConstants.VSCollectorApiKey.Equals(this.sessionSettings.CollectorApiKey))
        {
          if (flag2)
            this.UseCollector = true;
        }
        else if (KeysConstants.VSCollectorApiKey.Equals(this.sessionSettings.CollectorApiKey))
        {
          if (flag2)
            this.sessionSettings.AsimovInstrumentationKey = "AIF-312cbd79-9dbb-4c48-a7da-3cc2a931cb70";
          if (flag1)
            this.sessionSettings.AppInsightsInstrumentationKey = "f144292e-e3b2-4011-ac90-20e5c03fbce5";
        }
        else
        {
          if (!flag3 || flag2)
            throw new ArgumentException(flag3 ? "CollectorApiKey" : "AsimovInstrumentationKey");
          if ("AIF-312cbd79-9dbb-4c48-a7da-3cc2a931cb70".Equals(this.sessionSettings.AsimovInstrumentationKey))
            this.sessionSettings.CollectorApiKey = KeysConstants.VSCollectorApiKey;
          else
            this.UseCollector = false;
        }
      }
      this.SetUseCollectorToRegistry(new bool?(this.UseCollector));
    }

    private void MACAddressHashCalculationCompleted(object sender, EventArgs e)
    {
      this.isMACAddressCompleted = true;
      this.InitializeSession();
    }

    private void HardwareIdCalculationCompleted(object sender, EventArgs e)
    {
      this.isHardwareIdCompleted = true;
      this.InitializeSession();
      this.identityTelemetry.IdentityInformationProvider.HardwareIdCalculationCompleted -= new EventHandler<EventArgs>(this.HardwareIdCalculationCompleted);
    }

    private void InitializeSession()
    {
      if (this.isInitialized)
        return;
      lock (this.initializedLock)
      {
        if (this.isInitialized || !this.isManifestCompleted || !this.isMACAddressCompleted || !this.isHardwareIdCompleted)
          return;
        List<KeyValuePair<string, object>> list = this.persistentPropertyBag.GetAllProperties().ToList<KeyValuePair<string, object>>();
        list.Add(new KeyValuePair<string, object>("VS.Core.Session.IsCloned", (object) this.isSessionCloned));
        list.Add(new KeyValuePair<string, object>("VS.TelemetryApi.Session.IsShort", (object) this.startedDisposing));
        list.Add(new KeyValuePair<string, object>("VS.TelemetryApi.Session.ForcedReadManifest", (object) this.ManifestManager.ForcedReadManifest));
        list.Add(new KeyValuePair<string, object>("VS.TelemetryApi.DefaultChannels", (object) this.defaultSessionChannelsId.Join(",")));
        this.diagnosticTelemetry.PostDiagnosticTelemetryWhenSessionInitialized(this.hostTelemetrySession, (IEnumerable<KeyValuePair<string, object>>) list);
        this.persistentPropertyBag.Clear();
        TelemetryEvent telemetryEvent;
        while (this.telemetryBufferChannel.TryDequeue(out telemetryEvent))
        {
          this.DefaultContext.ProcessEvent(telemetryEvent);
          this.PostProcessedEvent(telemetryEvent);
        }
        this.isInitialized = true;
        while (this.telemetryBufferChannel.TryDequeue(out telemetryEvent))
        {
          this.DefaultContext.ProcessEvent(telemetryEvent, false);
          this.PostProcessedEvent(telemetryEvent);
        }
        this.telemetryBufferChannel = (TelemetryBufferChannel) null;
        this.identityTelemetry.IdentityInformationProvider.SchedulePostPersistedSharedPropertyAndSendAnyFaults(this.hostTelemetrySession, this.contextScheduler ?? (ITelemetryScheduler) new TelemetryScheduler());
        this.identityTelemetry.PostIdentityTelemetryWhenSessionInitialized(this.hostTelemetrySession);
      }
    }

    private bool ValidateHostName(string hostName) => !string.IsNullOrEmpty(hostName) && hostName.Length >= 1 && hostName.Length <= 64 && hostName.All<char>((Func<char, bool>) (c => char.IsLetterOrDigit(c) || c == '_' || c == '-' || c == '+' || c == ' '));

    private void IndicateNormalShutdown()
    {
      if (this.sessionInitializer?.RegistryTools == null)
        return;
      string regKeyPath = this.previousSessionShutdownRegistryPath.Value;
      if (!this.sessionInitializer.RegistryTools.DoesRegistryKeyExistInCurrentUserRoot(regKeyPath))
        return;
      this.sessionInitializer.RegistryTools.DeleteRegistryKeyFromCurrentUserRoot(regKeyPath);
    }

    private async Task StartReliabilityHeartbeat()
    {
      string stillAliveString = "StillAlive";
      if (this.sessionInitializer?.RegistryTools == null)
      {
        stillAliveString = (string) null;
      }
      else
      {
        while (!this.CancellationToken.IsCancellationRequested)
        {
          try
          {
            long fileTimeUtc = DateTime.Now.ToFileTimeUtc();
            if (Platform.IsWindows)
              this.sessionInitializer.RegistryTools.SetRegistryFromCurrentUserRoot(this.previousSessionShutdownRegistryPath.Value, stillAliveString, (object) fileTimeUtc, RegistryValueKind.QWord);
            else
              this.sessionInitializer.RegistryTools.SetRegistryFromCurrentUserRoot(this.previousSessionShutdownRegistryPath.Value, stillAliveString, (object) fileTimeUtc);
            await Task.Delay(300000, this.CancellationToken).ConfigureAwait(false);
          }
          catch (TaskCanceledException ex)
          {
          }
        }
        stillAliveString = (string) null;
      }
    }

    private static void SetRegistryFromCurrentUserRoot(
      string regKeyPath,
      string regKeyName,
      object value,
      RegistryValueKind kind)
    {
      if (!Platform.IsWindows)
        return;
      try
      {
        using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(regKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree))
          registryKey?.SetValue(regKeyName, value, kind);
      }
      catch
      {
      }
    }

    public void AddCommonProperties(IDictionary<string, object> properties)
    {
      if (this.IsDisposed)
        return;
      properties.RequiresArgumentNotNull<IDictionary<string, object>>(nameof (properties));
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) properties)
      {
        if (!this.TryAddCommonProperty(property.Key, property.Value))
          throw new ArgumentException(property.Key);
      }
    }

    public bool TryAddCommonProperty(string propertyName, object propertyValue)
    {
      if (this.IsDisposed)
        return false;
      propertyName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (propertyName));
      propertyValue.RequiresArgumentNotNull<object>(nameof (propertyValue));
      return this.commonProperties.TryAdd(propertyName, propertyValue);
    }

    public bool TryGetCommonPropertyValue(string propertyName, out object value)
    {
      propertyName.RequiresArgumentNotNullAndNotWhiteSpace(nameof (propertyName));
      if (this.commonProperties.ContainsKey(propertyName))
      {
        value = this.commonProperties[propertyName];
        return true;
      }
      value = (object) null;
      return false;
    }

    private void AddCommonPropertiesToEvent(TelemetryEvent channelTelemetryEvent)
    {
      if (this.IsDisposed)
        return;
      channelTelemetryEvent.Properties.AddRange<string, object>((IDictionary<string, object>) this.commonProperties);
    }

    public IDictionary<string, object> GetCommonPropertyDictionary() => this.commonProperties == null ? (IDictionary<string, object>) null : (IDictionary<string, object>) this.commonProperties;

    public override string ToString() => string.Format("'{0}' Started = {1} OptIn={2} IsInitialized = {3} Cloned = {4}", (object) this.SessionId, (object) this.isSessionStarted, (object) this.IsOptedIn, (object) this.isInitialized, (object) this.IsSessionCloned);

    public TelemetrySession GetHostTelemetrySession() => this.hostTelemetrySession;

    public void LoadCommonProperties() => this.LoadCommonPropertiesFromFile();

    private void LoadCommonPropertiesFromFile()
    {
      string environmentVariable = Environment.GetEnvironmentVariable("CommonPropertyBagPath");
      if (string.IsNullOrEmpty(environmentVariable))
      {
        this.GetHostTelemetrySession().PostFault("VS/TelemetryApi/LoadCommonProps", "EmptyLoadCommonPropsPathId");
      }
      else
      {
        this.hostTelemetrySession.PostEvent(new TelemetryEvent("VS/TelemetryApi/LoadCommonProps")
        {
          Properties = {
            ["vs.telemetryapi.loadcommonprops.filepath"] = (object) new TelemetryPiiProperty((object) environmentVariable)
          }
        });
        this.AddCommonProperties(new FileBasedKeyValuesLoader().GetData(this.hostTelemetrySession, environmentVariable));
      }
    }
  }
}
