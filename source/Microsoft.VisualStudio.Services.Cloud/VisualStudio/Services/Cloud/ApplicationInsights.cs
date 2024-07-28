// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ApplicationInsights
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.ApplicationInsights.Extensibility;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.ApplicationInsights.Extensibility.PerfCounterCollector.QuickPulse;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  internal class ApplicationInsights : IVssFrameworkService
  {
    private QuickPulseTelemetryProcessor quickPulseTelemetryProcessor;
    private const string c_appInsightsAvailabilityRegistryKey = "/FeatureAvailability/Entries/VisualStudio.FrameworkService.ApplicationInsights/AvailabilityState";
    private const string c_appInsightsConfigRootPath = "/Configuration/Logging/ApplicationInsights";
    private const string c_appInsightsFixedRateSamplingPercentageRegistryKey = "/Configuration/Logging/ApplicationInsights/FixedRateSamplingPercentage";
    private const string c_appInsightsAdaptiveRateMaxItemPerSecondRegistryKey = "/Configuration/Logging/ApplicationInsights/AdaptiveRateMaxItemPerSecond";
    private const double DefaultFixedRateSamplingPercentage = 4.0;
    private const double MaxFixedRateSamplingPercentage = 100.0;
    private const double MinFixedRateSamplingPercentage = 0.0;
    private const int DefaultAdaptiveRateMaxItemPerSecond = 5;
    private const int MinFixedAdaptiveRateMaxItemPerSecond = 0;
    private const int MaxFixedAdaptiveRateMaxItemPerSecond = 100;

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.LoadSettings(systemRequestContext);
      TelemetryProcessorChainBuilder processorChainBuilder = TelemetryConfiguration.Active.DefaultTelemetrySink.TelemetryProcessorChainBuilder;
      RegistryEntryCollection registryEntryCollection = systemRequestContext.GetService<CachedRegistryService>().ReadEntriesFallThru(systemRequestContext, (RegistryQuery) "/Configuration/Logging/ApplicationInsights/*");
      processorChainBuilder.Use((Func<ITelemetryProcessor, ITelemetryProcessor>) (next =>
      {
        this.quickPulseTelemetryProcessor = new QuickPulseTelemetryProcessor(next);
        return (ITelemetryProcessor) this.quickPulseTelemetryProcessor;
      }));
      if (systemRequestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ApplicationInsights.FixedRateSampling"))
      {
        double num = registryEntryCollection.GetValueFromPath<double>("/Configuration/Logging/ApplicationInsights/FixedRateSamplingPercentage", 4.0);
        if (num < 0.0)
          num = 0.0;
        else if (num > 100.0)
          num = 100.0;
        TelemetryProcessorChainBuilderExtensions.UseSampling(processorChainBuilder, num, (string) null, (string) null);
      }
      else
      {
        int num = registryEntryCollection.GetValueFromPath<int>("/Configuration/Logging/ApplicationInsights/AdaptiveRateMaxItemPerSecond", 5);
        if (num < 0)
          num = 0;
        else if (num > 100)
          num = 100;
        TelemetryProcessorChainBuilderExtensions.UseAdaptiveSampling(processorChainBuilder, (double) num, (string) null, (string) null);
      }
      processorChainBuilder.Build();
      string str = "";
      ITeamFoundationStrongBoxService service1 = systemRequestContext.GetService<ITeamFoundationStrongBoxService>();
      StrongBoxItemInfo itemInfo = service1.GetItemInfo(systemRequestContext, "ConfigurationSecrets", "AppInsightsLiveMetricsStreamAPIKey", false);
      if (itemInfo != null)
        str = service1.GetString(systemRequestContext, itemInfo);
      QuickPulseTelemetryModule pulseTelemetryModule = new QuickPulseTelemetryModule();
      pulseTelemetryModule.AuthenticationApiKey = str;
      pulseTelemetryModule.DisableFullTelemetryItems = systemRequestContext.IsHostProcessType(HostProcessType.JobAgent);
      pulseTelemetryModule.Initialize(TelemetryConfiguration.Active);
      pulseTelemetryModule.RegisterTelemetryProcessor((ITelemetryProcessor) this.quickPulseTelemetryProcessor);
      CachedRegistryService service2 = systemRequestContext.GetService<CachedRegistryService>();
      if (!string.IsNullOrEmpty(this.Tenant))
        service2.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), false, "/FeatureAvailability/Entries/VisualStudio.FrameworkService.ApplicationInsights/AvailabilityState", this.ProductionDeploymentIdRegistryKey);
      else
        service2.RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged), false, "/FeatureAvailability/Entries/VisualStudio.FrameworkService.ApplicationInsights/AvailabilityState");
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
      systemRequestContext.GetService<CachedRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnSettingsChanged));
      TelemetryConfiguration.Active.Dispose();
    }

    private void LoadSettings(IVssRequestContext systemRequestContext)
    {
      if (this.IsProductionEnvironment(systemRequestContext, this.ProductionDeploymentIdRegistryKey) && systemRequestContext.IsFeatureEnabled("VisualStudio.FrameworkService.ApplicationInsights"))
      {
        TelemetryConfiguration.Active.DisableTelemetry = false;
        ITeamFoundationStrongBoxService service = systemRequestContext.GetService<ITeamFoundationStrongBoxService>();
        StrongBoxItemInfo itemInfo = service.GetItemInfo(systemRequestContext, "ConfigurationSecrets", "AppInsightsInstrumentationKey", true);
        TelemetryConfiguration.Active.ConnectionString = "InstrumentationKey=" + service.GetString(systemRequestContext, itemInfo);
      }
      else
      {
        TelemetryConfiguration.Active.DisableTelemetry = true;
        TelemetryConfiguration.Active.ConnectionString = string.Format("InstrumentationKey={0}", (object) Guid.Empty);
      }
    }

    private void OnSettingsChanged(
      IVssRequestContext systemRequestContext,
      RegistryEntryCollection changedEntries)
    {
      this.LoadSettings(systemRequestContext);
    }

    private bool IsProductionEnvironment(
      IVssRequestContext systemRequestContext,
      string registryPath)
    {
      return !string.IsNullOrEmpty(registryPath) && systemRequestContext.GetService<CachedRegistryService>().GetValue(systemRequestContext, (RegistryQuery) registryPath, (string) null) == AzureRoleUtil.Environment.DeploymentId;
    }

    private string ProductionDeploymentIdRegistryKey => string.IsNullOrEmpty(this.Tenant) ? string.Empty : "/Diagnostics/Hosting/" + this.Tenant + "/ProductionDeploymentId";

    private string Tenant
    {
      get
      {
        string empty = string.Empty;
        if (AzureRoleUtil.Environment.DeploymentEnvironment == DeploymentEnvironment.Vmss)
          AzureRoleUtil.Configuration.Settings.TryGetValue("HostedServiceName", out empty);
        else
          AzureRoleUtil.Configuration.Settings.TryGetValue("CloudServiceName", out empty);
        return empty;
      }
    }
  }
}
