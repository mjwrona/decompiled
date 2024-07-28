// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Experimentation.ExperimentationServiceInitializer
// Assembly: Microsoft.VisualStudio.Telemetry, Version=16.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0E58FD5B-7E43-40D6-A963-E92D0F67BACC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Telemetry.dll

using Microsoft.VisualStudio.Telemetry;
using Microsoft.VisualStudio.Telemetry.Services;
using Microsoft.VisualStudio.Utilities.Internal;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Experimentation
{
  [ExcludeFromCodeCoverage]
  public sealed class ExperimentationServiceInitializer
  {
    private static readonly string localTestFlightsPathSuffix = "LocalTest\\";
    private static readonly string enabledFlightsKey = "EnabledFlights";
    private static readonly string disabledFlightsKey = "DisabledFlights";
    private static readonly string shippedFlightsKey = "ShippedFlights";
    private static readonly string setFlightsKey = "SetFlights";

    public IExperimentationTelemetry ExperimentationTelemetry { get; set; }

    public IExperimentationFilterProvider ExperimentationFilterProvider { get; set; }

    public IKeyValueStorage KeyValueStorage { get; set; }

    public IExperimentationOptinStatusReader ExperimentationOptinStatusReader { get; set; }

    internal IFlightsProvider FlightsProvider { get; set; }

    internal SetFlightsProvider SetFlightsProvider { get; set; }

    internal IRemoteFileReaderFactory ShippedRemoteFileReaderFactory { get; set; }

    internal IRemoteFileReaderFactory DisabledSetRemoteFileReaderFactory { get; set; }

    internal IFlightsStreamParser FlightsStreamParser { get; set; }

    internal IHttpWebRequestFactory HttpWebRequestFactory { get; set; }

    internal IRegistryTools3 RegistryTools { get; set; }

    public static ExperimentationServiceInitializer BuildDefault() => new ExperimentationServiceInitializer().FillWithDefaults();

    public ExperimentationServiceInitializer FillWithDefaults()
    {
      if (this.RegistryTools == null)
        this.RegistryTools = Platform.IsWindows ? (IRegistryTools3) new Microsoft.VisualStudio.Utilities.Internal.RegistryTools() : (IRegistryTools3) new FileBasedRegistryTools();
      if (this.ExperimentationFilterProvider == null)
        this.ExperimentationFilterProvider = (IExperimentationFilterProvider) new DefaultExperimentationFilterProvider(TelemetryService.DefaultSession);
      if (this.ExperimentationTelemetry == null)
        this.ExperimentationTelemetry = (IExperimentationTelemetry) new DefaultExperimentationTelemetry(TelemetryService.DefaultSession);
      if (this.KeyValueStorage == null)
        this.KeyValueStorage = (IKeyValueStorage) new DefaultRegistryKeyValueStorage(this.RegistryTools);
      if (this.ShippedRemoteFileReaderFactory == null)
        this.ShippedRemoteFileReaderFactory = (IRemoteFileReaderFactory) new ShippedFlightsRemoteFileReaderFactory();
      if (this.DisabledSetRemoteFileReaderFactory == null)
        this.DisabledSetRemoteFileReaderFactory = (IRemoteFileReaderFactory) new DisabledFlightsRemoteFileReaderFactory();
      if (this.FlightsStreamParser == null)
        this.FlightsStreamParser = (IFlightsStreamParser) new JsonFlightsStreamParser();
      if (this.HttpWebRequestFactory == null)
        this.HttpWebRequestFactory = (IHttpWebRequestFactory) new Microsoft.VisualStudio.Telemetry.Services.HttpWebRequestFactory();
      if (this.ExperimentationOptinStatusReader == null)
        this.ExperimentationOptinStatusReader = (IExperimentationOptinStatusReader) new DefaultExperimentationOptinStatusReader(TelemetryService.DefaultSession, (IRegistryTools) this.RegistryTools);
      if (this.SetFlightsProvider == null)
        this.SetFlightsProvider = new SetFlightsProvider(this.KeyValueStorage, ExperimentationServiceInitializer.setFlightsKey);
      if (this.FlightsProvider == null)
      {
        RemoteFlightsProvider<ShippedFlightsData> shippedFlightsProvider = new RemoteFlightsProvider<ShippedFlightsData>(this.KeyValueStorage, ExperimentationServiceInitializer.shippedFlightsKey, this.ShippedRemoteFileReaderFactory, this.FlightsStreamParser);
        this.FlightsProvider = (IFlightsProvider) new MasterFlightsProvider((IEnumerable<IFlightsProvider>) new IFlightsProvider[4]
        {
          (IFlightsProvider) new LocalFlightsProvider(this.KeyValueStorage, ExperimentationServiceInitializer.localTestFlightsPathSuffix + ExperimentationServiceInitializer.enabledFlightsKey),
          (IFlightsProvider) shippedFlightsProvider,
          (IFlightsProvider) new AFDFlightsProvider(this.KeyValueStorage, ExperimentationServiceInitializer.enabledFlightsKey, this.FlightsStreamParser, this.ExperimentationFilterProvider, this.HttpWebRequestFactory),
          (IFlightsProvider) this.SetFlightsProvider
        }, (IEnumerable<IFlightsProvider>) new IFlightsProvider[2]
        {
          (IFlightsProvider) new LocalFlightsProvider(this.KeyValueStorage, ExperimentationServiceInitializer.localTestFlightsPathSuffix + ExperimentationServiceInitializer.disabledFlightsKey),
          (IFlightsProvider) new RemoteFlightsProvider<DisabledFlightsData>(this.KeyValueStorage, ExperimentationServiceInitializer.disabledFlightsKey, this.DisabledSetRemoteFileReaderFactory, this.FlightsStreamParser)
        }, (IFlightsProvider) shippedFlightsProvider, this.ExperimentationOptinStatusReader);
      }
      return this;
    }
  }
}
