// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Telemetry.TelemetrySessionInitializer
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.ApplicationInsights;
using Microsoft.VisualStudio.ApplicationInsights.Channel;
using Microsoft.VisualStudio.Telemetry.PropertyProviders.Linux;
using Microsoft.VisualStudio.Telemetry.SessionChannel;
using Microsoft.VisualStudio.Utilities.Internal;
using System;
using System.Collections.Generic;
using System.Threading;

namespace Microsoft.VisualStudio.Telemetry
{
  internal sealed class TelemetrySessionInitializer
  {
    public CancellationTokenSource CancellationTokenSource { get; set; }

    public string SessionId { get; set; }

    public string AppInsightsInstrumentationKey { get; set; }

    public string AsimovInstrumentationKey { get; set; }

    public string CollectorApiKey { get; set; }

    public IDiagnosticTelemetry DiagnosticTelemetry { get; set; }

    public IIdentityTelemetry IdentityTelemetry { get; set; }

    public IEnvironmentTools EnvironmentTools { get; set; }

    public IRegistryTools4 RegistryTools { get; set; }

    public ITelemetryOptinStatusReader OptinStatusReader { get; set; }

    public IProcessTools ProcessTools { get; set; }

    public ILegacyApi LegacyApi { get; set; }

    public IInternalSettings InternalSettings { get; set; }

    public IHostInformationProvider HostInformationProvider { get; set; }

    public IMachineInformationProvider MachineInformationProvider { get; set; }

    public IMACInformationProvider MACInformationProvider { get; set; }

    public IUserInformationProvider UserInformationProvider { get; set; }

    public ITelemetryScheduler EventProcessorScheduler { get; set; }

    public ITelemetryScheduler ContextScheduler { get; set; }

    public ITelemetryScheduler RecurringPropertyUpdateScheduler { get; set; }

    public ITelemetryScheduler RecurringPropertyScheduler { get; set; }

    public ITelemetryManifestManagerBuilder TelemetryManifestManagerBuilder { get; set; }

    public IContextPropertyManager DefaultContextPropertyManager { get; set; }

    public IPersistentPropertyBag PersistentStorage { get; set; }

    public IPersistentPropertyBag PersistentSharedProperties { get; set; }

    public IPersistentPropertyBag PersistentPropertyBag { get; set; }

    public IEnumerable<IChannelValidator> ChannelValidators { get; set; }

    public IEnumerable<ISessionChannel> ChannelsToAdd { get; set; }

    public IEnumerable<IEventProcessorAction> CustomActionToAdd { get; set; }

    public IEnumerable<IPropertyProvider> PropertyProviders { get; set; }

    public EventProcessorChannelBuilder EventProcessorChannelBuilder { get; set; }

    public WatsonSessionChannelBuilder WatsonSessionChannelBuilder { get; set; }

    public IProcessCreationTime ProcessCreationTime { get; set; }

    public static TelemetrySessionInitializer Default => TelemetrySessionInitializer.BuildInitializer((TelemetrySessionSettings) null);

    public static TelemetrySessionInitializer FromSessionSettings(
      TelemetrySessionSettings telemetrySessionSettings)
    {
      return TelemetrySessionInitializer.BuildInitializer(telemetrySessionSettings);
    }

    public IEnumerable<ISessionChannel> CreateSessionChannels(
      TelemetrySession telemetrySession,
      bool checkPendingAsimovEvents)
    {
      IStorageBuilder storageBuilder = TelemetrySessionInitializer.GetStorageBuilder();
      IProcessLockFactory processLockFactory = TelemetrySessionInitializer.GetProcessLock();
      if (!string.IsNullOrEmpty(this.AppInsightsInstrumentationKey))
        yield return (ISessionChannel) new DefaultAppInsightsSessionChannel(this.AppInsightsInstrumentationKey, telemetrySession.UserId.ToString(), storageBuilder, processLockFactory);
      if (!string.IsNullOrEmpty(this.AsimovInstrumentationKey))
      {
        AsimovAppInsightsSessionChannel sessionChannel = new AsimovAppInsightsSessionChannel("aivortex", false, this.AsimovInstrumentationKey, telemetrySession.UserId.ToString(), ChannelProperties.Default | ChannelProperties.NotForUnitTest, telemetrySession, storageBuilder, processLockFactory);
        if (checkPendingAsimovEvents)
          sessionChannel.CheckPendingEventsAndStartChannel(telemetrySession.SessionId);
        yield return (ISessionChannel) sessionChannel;
        yield return (ISessionChannel) new AsimovAppInsightsSessionChannel("aiasimov", Platform.IsWindows, this.AsimovInstrumentationKey, telemetrySession.UserId.ToString(), ChannelProperties.NotForUnitTest, telemetrySession, storageBuilder, processLockFactory);
      }
      if (!string.IsNullOrEmpty(this.CollectorApiKey))
      {
        CollectorSessionChannel sessionChannel = new CollectorSessionChannel("collector", false, this.CollectorApiKey, telemetrySession.UserId.ToString(), ChannelProperties.Default | ChannelProperties.NotForUnitTest, telemetrySession, storageBuilder, processLockFactory);
        if (checkPendingAsimovEvents)
          sessionChannel.CheckPendingEventsAndStartChannel(telemetrySession.SessionId);
        yield return (ISessionChannel) sessionChannel;
      }
      yield return (ISessionChannel) new TelemetryLogToFileChannel();
    }

    public void Validate()
    {
      if (string.IsNullOrEmpty(this.CollectorApiKey))
      {
        this.AppInsightsInstrumentationKey.RequiresArgumentNotEmptyOrWhitespace("AppInsightsInstrumentationKey");
        this.AsimovInstrumentationKey.RequiresArgumentNotEmptyOrWhitespace("AsimovInstrumentationKey");
      }
      else
        this.CollectorApiKey.RequiresArgumentNotEmptyOrWhitespace("CollectorApiKey");
      this.DiagnosticTelemetry.RequiresArgumentNotNull<IDiagnosticTelemetry>("DiagnosticTelemetry");
      this.HostInformationProvider.RequiresArgumentNotNull<IHostInformationProvider>("HostInformationProvider");
      this.MachineInformationProvider.RequiresArgumentNotNull<IMachineInformationProvider>("MachineInformationProvider");
      this.UserInformationProvider.RequiresArgumentNotNull<IUserInformationProvider>("UserInformationProvider");
      this.EventProcessorScheduler.RequiresArgumentNotNull<ITelemetryScheduler>("EventProcessorScheduler");
      this.DefaultContextPropertyManager.RequiresArgumentNotNull<IContextPropertyManager>("DefaultContextPropertyManager");
      this.ChannelValidators.RequiresArgumentNotNull<IEnumerable<IChannelValidator>>("ChannelValidators");
      this.OptinStatusReader.RequiresArgumentNotNull<ITelemetryOptinStatusReader>("OptinStatusReader");
    }

    private static TelemetrySessionInitializer BuildInitializer(
      TelemetrySessionSettings telemetrySessionSettings)
    {
      CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
      Microsoft.VisualStudio.Telemetry.DiagnosticTelemetry diagnosticTelemetry = new Microsoft.VisualStudio.Telemetry.DiagnosticTelemetry();
      Microsoft.VisualStudio.Telemetry.EnvironmentTools environmentTools = new Microsoft.VisualStudio.Telemetry.EnvironmentTools();
      IRegistryTools4 registryTools = Platform.IsWindows ? (IRegistryTools4) new Microsoft.VisualStudio.Utilities.Internal.RegistryTools() : (IRegistryTools4) new FileBasedRegistryTools();
      Microsoft.VisualStudio.Telemetry.ProcessTools processTools = new Microsoft.VisualStudio.Telemetry.ProcessTools();
      ILegacyApi legacyApi = TelemetrySessionInitializer.GetLegacyApi((IRegistryTools) registryTools);
      IPersistentPropertyBag persistentPropertyBag1 = TelemetrySessionInitializer.CreatePersistentPropertyBag(string.Empty);
      IPersistentPropertyBag persistentPropertyBag2 = TelemetrySessionInitializer.CreatePersistentPropertyBag("c57a9efce9b74de382d905a89852db71");
      IMACInformationProvider informationProvider1 = TelemetrySessionInitializer.GetMACInformationProvider((IProcessTools) processTools, persistentPropertyBag1, legacyApi);
      IInternalSettings internalSettings = TelemetrySessionInitializer.GetInternalSettings((IDiagnosticTelemetry) diagnosticTelemetry, (IRegistryTools) registryTools);
      IHostInformationProvider informationProvider2 = TelemetrySessionInitializer.GetHostInformationProvider();
      IUserInformationProvider informationProvider3 = TelemetrySessionInitializer.GetUserInformationProvider((IRegistryTools) registryTools, internalSettings, (IEnvironmentTools) environmentTools, legacyApi, telemetrySessionSettings);
      Microsoft.VisualStudio.Telemetry.MachineInformationProvider machine = new Microsoft.VisualStudio.Telemetry.MachineInformationProvider(legacyApi, informationProvider3, informationProvider1);
      IIdentityInformationProvider informationProvider4 = TelemetrySessionInitializer.GetIdentityInformationProvider();
      IPersistentPropertyBag persistentPropertyBag3 = TelemetrySessionInitializer.CreatePersistentPropertyBag(informationProvider2.ProcessName);
      TelemetryScheduler scheduler = new TelemetryScheduler();
      OptOutAction optOutAction = new OptOutAction();
      optOutAction.AddOptOutFriendlyEventName("vs/telemetryapi/session/initialized");
      optOutAction.AddOptOutFriendlyEventName("vs/telemetryapi/session/cloned");
      optOutAction.AddOptOutFriendlyPropertiesList((IEnumerable<string>) new List<string>()
      {
        "Context.Default.VS.Core.BranchName",
        "Context.Default.VS.Core.BuildNumber",
        "Context.Default.VS.Core.ExeName",
        "Context.Default.VS.Core.ExeVersion",
        "Context.Default.VS.Core.HardwareId",
        "Context.Default.VS.Core.MacAddressHash",
        "Context.Default.VS.Core.Machine.Id",
        "Context.Default.VS.Core.SkuName",
        "Context.Default.VS.Core.OS.Version",
        "Context.Default.VS.Core.ProcessId",
        "Context.Default.VS.Core.Is64BitProcess",
        "Context.Default.VS.Core.OSBitness",
        "Context.Default.VS.Core.User.Id",
        "Context.Default.VS.Core.User.IsMicrosoftInternal",
        "Context.Default.VS.Core.User.IsOptedIn",
        "Context.Default.VS.Core.User.Location.GeoId",
        "Context.Default.VS.Core.User.Type",
        "Context.Default.VS.Core.Version",
        "Reserved.EventId",
        "Reserved.SessionId",
        "Reserved.TimeSinceSessionStart",
        "VS.Core.Locale.Product",
        "VS.Core.Locale.System",
        "VS.Core.Locale.User",
        "VS.Core.Locale.UserUI",
        "VS.Core.SkuId",
        "VS.Sqm.SkuId"
      });
      ClientSideThrottlingAction throttlingAction = new ClientSideThrottlingAction((IEnumerable<string>) new List<string>()
      {
        "context/create",
        "context/close",
        "context/postproperty",
        "vs/core/command",
        "vs/core/extension/installed",
        "vs/core/sessionstart",
        "vs/core/sessionend",
        "vs/telemetryapi/session/initialized",
        "vs/telemetryapi/clientsidethrottling",
        "vs/telemetryapi/manifest/load",
        "vs/telemetryapi/piiproperties"
      });
      PIIPropertyProcessor propertyProcessor = new PIIPropertyProcessor();
      PiiAction piiAction = new PiiAction((IPiiPropertyProcessor) propertyProcessor);
      ComplexPropertyAction complexPropertyAction = new ComplexPropertyAction((IComplexObjectSerializerFactory) new JsonComplexObjectSerializerFactory(), (IPiiPropertyProcessor) propertyProcessor);
      return new TelemetrySessionInitializer()
      {
        CancellationTokenSource = cancellationTokenSource,
        SessionId = Guid.NewGuid().ToString(),
        AppInsightsInstrumentationKey = telemetrySessionSettings?.AppInsightsInstrumentationKey ?? "f144292e-e3b2-4011-ac90-20e5c03fbce5",
        AsimovInstrumentationKey = telemetrySessionSettings?.AsimovInstrumentationKey ?? "AIF-312cbd79-9dbb-4c48-a7da-3cc2a931cb70",
        CollectorApiKey = telemetrySessionSettings?.CollectorApiKey ?? KeysConstants.VSCollectorApiKey,
        DiagnosticTelemetry = (IDiagnosticTelemetry) diagnosticTelemetry,
        IdentityTelemetry = (IIdentityTelemetry) new Microsoft.VisualStudio.Telemetry.IdentityTelemetry(informationProvider4, (ITelemetryScheduler) scheduler),
        EnvironmentTools = (IEnvironmentTools) environmentTools,
        RegistryTools = registryTools,
        ProcessTools = (IProcessTools) processTools,
        LegacyApi = legacyApi,
        InternalSettings = internalSettings,
        HostInformationProvider = informationProvider2,
        MachineInformationProvider = (IMachineInformationProvider) machine,
        UserInformationProvider = informationProvider3,
        MACInformationProvider = informationProvider1,
        EventProcessorScheduler = (ITelemetryScheduler) scheduler,
        TelemetryManifestManagerBuilder = (ITelemetryManifestManagerBuilder) new Microsoft.VisualStudio.Telemetry.TelemetryManifestManagerBuilder(),
        DefaultContextPropertyManager = (IContextPropertyManager) new Microsoft.VisualStudio.Telemetry.DefaultContextPropertyManager(TelemetrySessionInitializer.GetPropertyProviders((IRegistryTools) registryTools, (IEnvironmentTools) environmentTools, informationProvider2, (IMachineInformationProvider) machine, informationProvider1, informationProvider3, persistentPropertyBag2, informationProvider4)),
        PersistentStorage = persistentPropertyBag1,
        PersistentSharedProperties = persistentPropertyBag2,
        PersistentPropertyBag = persistentPropertyBag3,
        ChannelValidators = (IEnumerable<IChannelValidator>) new List<IChannelValidator>()
        {
          (IChannelValidator) new RegistryChannelValidator(internalSettings),
          (IChannelValidator) new InternalChannelValidator(informationProvider3),
          (IChannelValidator) new DisabledTelemetryChannelValidator(internalSettings)
        },
        CustomActionToAdd = (IEnumerable<IEventProcessorAction>) new List<IEventProcessorAction>()
        {
          (IEventProcessorAction) optOutAction,
          (IEventProcessorAction) new EnforceAIRestrictionAction(),
          (IEventProcessorAction) piiAction,
          (IEventProcessorAction) complexPropertyAction,
          (IEventProcessorAction) new MetricAction(),
          (IEventProcessorAction) new SettingAction(),
          (IEventProcessorAction) throttlingAction,
          (IEventProcessorAction) new SuppressEmptyPostPropertyEventAction(),
          (IEventProcessorAction) new CredScanAction()
        },
        EventProcessorChannelBuilder = new EventProcessorChannelBuilder(persistentPropertyBag3, (ITelemetryScheduler) scheduler),
        WatsonSessionChannelBuilder = new WatsonSessionChannelBuilder(internalSettings.FaultEventWatsonSamplePercent(), internalSettings.FaultEventMaximumWatsonReportsPerSession(), internalSettings.FaultEventMinimumSecondsBetweenWatsonReports(), ChannelProperties.Default),
        OptinStatusReader = TelemetrySessionInitializer.GetOptInStatusReader((IRegistryTools2) registryTools),
        ProcessCreationTime = TelemetrySessionInitializer.GetProcessCreationTime(telemetrySessionSettings?.CollectorApiKey)
      };
    }

    private static ILegacyApi GetLegacyApi(IRegistryTools registryTools) => Platform.IsWindows ? (ILegacyApi) new Microsoft.VisualStudio.Telemetry.LegacyApi(registryTools) : (ILegacyApi) new MonoLegacyApi(registryTools);

    private static IMACInformationProvider GetMACInformationProvider(
      IProcessTools processTools,
      IPersistentPropertyBag persistentStorage,
      ILegacyApi legacyApi)
    {
      if (Platform.IsWindows)
        return (IMACInformationProvider) new WindowsMACInformationProvider(processTools, persistentStorage);
      return Platform.IsMac ? (IMACInformationProvider) new MacMACInformationProvider(persistentStorage, legacyApi) : (IMACInformationProvider) new MonoMACInformationProvider(processTools, persistentStorage);
    }

    private static IHostInformationProvider GetHostInformationProvider() => Platform.IsWindows ? (IHostInformationProvider) new WindowsHostInformationProvider() : (IHostInformationProvider) new MonoHostInformationProvider();

    private static IIdentityInformationProvider GetIdentityInformationProvider() => Platform.IsWindows ? (IIdentityInformationProvider) new WindowsIdentityInformationProvider() : (IIdentityInformationProvider) new MonoIdentityInformationProvider();

    private static IUserInformationProvider GetUserInformationProvider(
      IRegistryTools registryTools,
      IInternalSettings internalSettings,
      IEnvironmentTools environmentTools,
      ILegacyApi legacyApi,
      TelemetrySessionSettings telemetrySessionSettings)
    {
      return Platform.IsWindows ? (IUserInformationProvider) new WindowsUserInformationProvider(registryTools, internalSettings, environmentTools, legacyApi, (Guid?) telemetrySessionSettings?.UserId) : (IUserInformationProvider) new MonoUserInformationProvider(internalSettings, environmentTools, legacyApi, (Guid?) telemetrySessionSettings?.UserId);
    }

    private static IEnumerable<IPropertyProvider> GetPropertyProviders(
      IRegistryTools registryTools,
      IEnvironmentTools environmentTools,
      IHostInformationProvider host,
      IMachineInformationProvider machine,
      IMACInformationProvider macAddress,
      IUserInformationProvider user,
      IPersistentPropertyBag sharedProperties,
      IIdentityInformationProvider identity)
    {
      yield return (IPropertyProvider) new IdentityPropertyProvider();
      if (Platform.IsMac)
      {
        yield return (IPropertyProvider) new MacHostPropertyProvider(host, (INsBundleInformationProvider) new NsBundleInformationProvider());
        yield return (IPropertyProvider) new AssemblyPropertyProvider();
        yield return (IPropertyProvider) new MacLocalePropertyProvider();
        yield return (IPropertyProvider) new MacMachinePropertyProvider(machine, registryTools, macAddress);
        yield return (IPropertyProvider) new MacOSPropertyProvider(environmentTools);
        yield return (IPropertyProvider) new MacUserPropertyProvider(user);
        yield return (IPropertyProvider) new PersistentSharedPropertyProvider(sharedProperties);
      }
      else if (Platform.IsLinux)
      {
        yield return (IPropertyProvider) new LinuxHostPropertyProvider(host);
        yield return (IPropertyProvider) new AssemblyPropertyProvider();
        yield return (IPropertyProvider) new LinuxLocalePropertyProvider();
        yield return (IPropertyProvider) new LinuxMachinePropertyProvider(machine, registryTools, macAddress);
        yield return (IPropertyProvider) new LinuxOSPropertyProvider();
        yield return (IPropertyProvider) new PersistentSharedPropertyProvider(sharedProperties);
      }
      else
      {
        yield return (IPropertyProvider) new WindowsHostPropertyProvider(host, registryTools);
        yield return (IPropertyProvider) new AssemblyPropertyProvider();
        yield return (IPropertyProvider) new WindowsLocalePropertyProvider(registryTools);
        yield return (IPropertyProvider) new WindowsMachinePropertyProvider(machine, registryTools, macAddress, sharedProperties);
        yield return (IPropertyProvider) new WindowsOSPropertyProvider(environmentTools, registryTools);
        yield return (IPropertyProvider) new WindowsUserPropertyProvider(user);
        yield return (IPropertyProvider) new PersistentSharedPropertyProvider(sharedProperties);
      }
    }

    private static IStorageBuilder GetStorageBuilder()
    {
      if (Platform.IsMac)
        return (IStorageBuilder) new MacStorageBuilder();
      return Platform.IsLinux ? (IStorageBuilder) new LinuxStorageBuilder() : (IStorageBuilder) new WindowsStorageBuilder();
    }

    internal static IProcessCreationTime GetProcessCreationTime(string collectorKey = null)
    {
      if (Platform.IsWindows)
        return (IProcessCreationTime) new WindowsProcessCreationTime();
      return !string.IsNullOrWhiteSpace(collectorKey) && KeysConstants.VSCodeCollectorApiKey.Equals(collectorKey) ? (IProcessCreationTime) new MonoFileTimeProcessCreationTime() : (IProcessCreationTime) new MonoProcessCreationTime();
    }

    private static IInternalSettings GetInternalSettings(
      IDiagnosticTelemetry diagnosticTelemetry,
      IRegistryTools registryTools)
    {
      return Platform.IsWindows ? (IInternalSettings) new WindowsInternalSettings(diagnosticTelemetry, registryTools) : (IInternalSettings) new MonoInternalSettings(diagnosticTelemetry, registryTools);
    }

    private static ITelemetryOptinStatusReader GetOptInStatusReader(IRegistryTools2 registryTools)
    {
      if (Platform.IsWindows)
        return (ITelemetryOptinStatusReader) new TelemetryVsOptinStatusReader(registryTools);
      return Platform.IsMac ? (ITelemetryOptinStatusReader) new MacVsOptinStatusReader() : (ITelemetryOptinStatusReader) new NullVsOptinStatusReader();
    }

    private static IPersistentPropertyBag CreatePersistentPropertyBag(string processName) => Platform.IsWindows ? (IPersistentPropertyBag) new RegistryPropertyBag(processName) : (IPersistentPropertyBag) new MonoRegistryPropertyBag(processName);

    private static IProcessLockFactory GetProcessLock() => Platform.IsWindows ? (IProcessLockFactory) new WindowsProcessLockFactory() : (IProcessLockFactory) new MonoProcessLockFactory();
  }
}
