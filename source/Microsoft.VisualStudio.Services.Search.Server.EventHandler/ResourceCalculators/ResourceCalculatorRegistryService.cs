// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.ResourceCalculators.ResourceCalculatorRegistryService
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Common;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler.ResourceCalculators
{
  public class ResourceCalculatorRegistryService : IVssFrameworkService
  {
    private ResourceCalculatorRegistryService.ResourceCalculatorRegistrySettings m_settings;

    public ResourceCalculatorRegistryService.ResourceCalculatorRegistrySettings RegistrySettings
    {
      get => this.m_settings;
      internal set => this.m_settings = value;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      string[] strArray = new string[2]
      {
        "/Configuration/JobService/*",
        "/Service/ALMSearch/Settings/*"
      };
      systemRequestContext.GetService<IVssRegistryService>().RegisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged), strArray);
      Interlocked.CompareExchange<ResourceCalculatorRegistryService.ResourceCalculatorRegistrySettings>(ref this.m_settings, new ResourceCalculatorRegistryService.ResourceCalculatorRegistrySettings(systemRequestContext), (ResourceCalculatorRegistryService.ResourceCalculatorRegistrySettings) null);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext) => systemRequestContext.GetService<IVssRegistryService>().UnregisterNotification(systemRequestContext, new RegistrySettingsChangedCallback(this.OnRegistrySettingsChanged));

    [Info("InternalForTestPurpose")]
    internal virtual void OnRegistrySettingsChanged(
      IVssRequestContext requestContext,
      RegistryEntryCollection changedEntries)
    {
      Volatile.Write<ResourceCalculatorRegistryService.ResourceCalculatorRegistrySettings>(ref this.m_settings, new ResourceCalculatorRegistryService.ResourceCalculatorRegistrySettings(requestContext));
    }

    public sealed class ResourceCalculatorRegistrySettings
    {
      private static readonly RegistryQuery s_registryJobServiceSettingsQuery = new RegistryQuery("/Configuration/JobService/*");
      private static readonly RegistryQuery s_registryResourceCalculatorSettingsQuery = new RegistryQuery("/Service/ALMSearch/Settings/*");

      public ResourceCalculatorRegistrySettings(IVssRequestContext requestContext)
      {
        RegistryEntryCollection registryEntryCollection1 = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, ResourceCalculatorRegistryService.ResourceCalculatorRegistrySettings.s_registryJobServiceSettingsQuery);
        RegistryEntryCollection registryEntryCollection2 = requestContext.GetService<IVssRegistryService>().ReadEntries(requestContext, ResourceCalculatorRegistryService.ResourceCalculatorRegistrySettings.s_registryResourceCalculatorSettingsQuery);
        this.MinAllottableResourcesPercentage = registryEntryCollection2.GetValueFromPath<int>("MinAllottableResourcesPercentagePerCollection", 10);
        this.MinAllottableResourcesPercentageForCode = registryEntryCollection2.GetValueFromPath<int>("MinAllottableResourcesPercentagePerCollectionForCode", 10);
        this.MinAllottableResourcesPercentageForWorkItem = registryEntryCollection2.GetValueFromPath<int>("MinAllottableResourcesPercentagePerCollectionForWorkItem", 10);
        this.MinAvailableResourcesPercentageThreshold = registryEntryCollection2.GetValueFromPath<int>(nameof (MinAvailableResourcesPercentageThreshold), 10);
        this.MaxAvailableResourcesPercentageThreshold = registryEntryCollection2.GetValueFromPath<int>(nameof (MaxAvailableResourcesPercentageThreshold), 50);
        this.CallbackDelayInMins = registryEntryCollection2.GetValueFromPath<int>(nameof (CallbackDelayInMins), 5);
        this.MinExpectedResourcePercentage = registryEntryCollection2.GetValueFromPath<int>("MinExpectedAllottedResourcePercentage", 70);
        this.IsFallbackEnabled = registryEntryCollection2.GetValueFromPath<bool>("IsResourceCalculatorFallbackEnabled", false);
        this.MaxJobsTotal = registryEntryCollection1.GetValueFromPath<int>(nameof (MaxJobsTotal), 50);
        this.TotalJobAgentInstances = registryEntryCollection1.GetValueFromPath<int>("JobAgentInstanceCount", 3);
        this.LogRusViolation = registryEntryCollection2.GetValueFromPath<bool>("LogRusViolations", false);
      }

      public int MinAllottableResourcesPercentage { get; private set; }

      public int MinAllottableResourcesPercentageForCode { get; private set; }

      public int MinAllottableResourcesPercentageForWorkItem { get; private set; }

      public int MinAvailableResourcesPercentageThreshold { get; private set; }

      public int MaxAvailableResourcesPercentageThreshold { get; private set; }

      public int MaxJobsTotal { get; private set; }

      public int TotalJobAgentInstances { get; private set; }

      public int CallbackDelayInMins { get; private set; }

      public int MinExpectedResourcePercentage { get; private set; }

      public bool IsFallbackEnabled { get; private set; }

      public bool LogRusViolation { get; private set; }
    }
  }
}
