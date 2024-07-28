// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.InstalledExtensionManager
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CircuitBreaker;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Extension;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.Types.Server;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class InstalledExtensionManager : IVssFrameworkService, IInstalledExtensionManager
  {
    private static readonly string s_area = nameof (InstalledExtensionManager);
    private static readonly string s_layer = "IVssFrameworkService";
    private const string c_InstalledExtensions = "InstalledExtensions";
    private const string c_DefaultLanguage = "default";
    private static readonly RegistryQuery s_useCachedExtensions = new RegistryQuery("/Configuration/ExtensionService/Cache");
    private InstalledExtensionManager.CircuitBreakerSettings m_circuitBreakerSettings = InstalledExtensionManager.CircuitBreakerSettings.Default;
    private static readonly TimeSpan s_errorStateCacheInterval = TimeSpan.FromMinutes(1.0);

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public virtual InstalledExtension GetInstalledExtension(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      bool includeDisabledExtensions = false)
    {
      return this.GetInstalledExtensions(requestContext, includeDisabledExtensions).FirstOrDefault<InstalledExtension>((Func<InstalledExtension, bool>) (ext => string.Equals(ext.PublisherName, publisherName, StringComparison.OrdinalIgnoreCase) && string.Equals(ext.ExtensionName, extensionName, StringComparison.OrdinalIgnoreCase)));
    }

    public virtual List<InstalledExtension> GetInstalledExtensions(
      IVssRequestContext requestContext,
      bool includeDisabledExtensions = false)
    {
      requestContext.TraceEnter(10013470, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, nameof (GetInstalledExtensions));
      try
      {
        List<InstalledExtension> installedExtensions = new List<InstalledExtension>();
        object obj;
        List<InstalledExtension> installedExtensionList;
        if (!requestContext.Items.TryGetValue("InstalledExtensions", out obj))
        {
          bool flag;
          if (requestContext.ExecutionEnvironment.IsHostedDeployment)
          {
            IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
            flag = vssRequestContext.GetService<IInstanceManagementService>().GetServiceInstanceType(vssRequestContext, ExtensionConstants.ServiceOwner) != null;
          }
          else
            flag = true;
          if (flag)
          {
            CommandSetter setter = CommandSetter.WithGroupKey((CommandGroupKey) "Framework.").AndCommandKey((CommandKey) this.m_circuitBreakerSettings.CommandKeyForFetchInstalledExtensions).AndCommandPropertiesDefaults(this.m_circuitBreakerSettings.CircuitBreakerSettingsForFetchingInstalledExtensions);
            installedExtensionList = new CommandService<List<InstalledExtension>>(requestContext, setter, (Func<List<InstalledExtension>>) (() => this.FetchInstalledExtensionsInternal(requestContext)), (Func<List<InstalledExtension>>) (() => this.FetchFallbackInstalledExtensions(requestContext))).Execute();
          }
          else
            installedExtensionList = this.FetchFallbackInstalledExtensions(requestContext);
          requestContext.Items["InstalledExtensions"] = (object) installedExtensionList;
        }
        else
          installedExtensionList = (List<InstalledExtension>) obj;
        foreach (InstalledExtension installedExtension in installedExtensionList)
        {
          if (includeDisabledExtensions || (installedExtension.InstallState.Flags & Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled) != Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Disabled && (installedExtension.InstallState.Flags & Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.VersionCheckError) != Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.VersionCheckError)
            installedExtensions.Add(installedExtension);
        }
        return installedExtensions;
      }
      finally
      {
        requestContext.TraceLeave(10013470, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, nameof (GetInstalledExtensions));
      }
    }

    public virtual List<InstalledExtension> GetInstalledExtensions(
      IVssRequestContext requestContext,
      IEnumerable<string> targetContributionIds)
    {
      IEnumerable<Contribution> contributions = requestContext.GetService<IContributionService>().QueryContributions(requestContext, targetContributionIds, queryOptions: ContributionQueryOptions.IncludeAll);
      Dictionary<string, InstalledExtension> dictionary1 = new Dictionary<string, InstalledExtension>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<string, List<Contribution>> dictionary2 = new Dictionary<string, List<Contribution>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (Contribution contribution in contributions)
      {
        ContributionIdentifier contributionIdentifier = new ContributionIdentifier(contribution.Id);
        List<Contribution> contributionList = (List<Contribution>) null;
        string key = string.Format("{0}.{1}", (object) contributionIdentifier.PublisherName, (object) contributionIdentifier.ExtensionName);
        InstalledExtension sourceExtension;
        if (!dictionary1.TryGetValue(key, out sourceExtension))
        {
          sourceExtension = this.GetInstalledExtension(requestContext, contributionIdentifier.PublisherName, contributionIdentifier.ExtensionName, false);
          if (sourceExtension != null)
          {
            sourceExtension = new InstalledExtension(sourceExtension);
            sourceExtension.Contributions = (IEnumerable<Contribution>) (contributionList = new List<Contribution>());
            sourceExtension.ContributionTypes = (IEnumerable<ContributionType>) null;
            dictionary1[key] = sourceExtension;
            dictionary2[key] = contributionList;
          }
        }
        if (contributionList != null || dictionary2.TryGetValue(key, out contributionList))
          contributionList.Add(contribution);
      }
      return dictionary1.Values.ToList<InstalledExtension>();
    }

    public virtual bool IsExtensionActive(
      IVssRequestContext requestContext,
      string publisherName,
      string extensionName,
      bool checkLicenseOnFallback = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(publisherName, nameof (publisherName));
      ArgumentUtility.CheckStringForNullOrEmpty(extensionName, nameof (extensionName));
      bool flag1 = true;
      string fullyQualifiedName = GalleryUtil.CreateFullyQualifiedName(publisherName, extensionName);
      if (this.GetInstalledExtension(requestContext, publisherName, extensionName, false) == null)
      {
        requestContext.Trace(10013565, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "Extension {0}.{1} is not available", (object) publisherName, (object) extensionName);
        flag1 = false;
        bool flag2 = false;
        if (requestContext.RootContext.TryGetItem<bool>("InExtensionFallbackMode", out flag2) & flag2)
        {
          requestContext.Trace(10013570, TraceLevel.Error, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "Extension {0}.{1} is not available because ems in fallback mode.  Using license Service: {2}", (object) publisherName, (object) extensionName, (object) checkLicenseOnFallback);
          if (checkLicenseOnFallback)
            flag1 = requestContext.GetService<IExtensionLicensingService>().IsExtensionLicensed(requestContext, fullyQualifiedName);
        }
      }
      else
      {
        requestContext.TraceConditionally(100136245, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, (Func<string>) (() => string.Format("Extension is installed. {0}.{1}", (object) publisherName, (object) extensionName)));
        if (requestContext.IsFeatureEnabled("VisualStudio.Services.DelayedIdentityValidation"))
          flag1 = requestContext.GetService<IExtensionLicensingService>().IsExtensionLicensed(requestContext, fullyQualifiedName);
      }
      return flag1;
    }

    public void InvalidateHost(IVssRequestContext requestContext, Guid hostId) => requestContext.To(TeamFoundationHostType.Deployment).GetService<InstalledExtensionManager.IExtensionStateCache>().Remove(requestContext, hostId);

    private List<InstalledExtension> FetchFallbackInstalledExtensions(
      IVssRequestContext requestContext)
    {
      requestContext.Items.TryAdd<string, object>("InExtensionFallbackMode", (object) true);
      return this.LoadFallbackInstalledExtensions(requestContext);
    }

    private List<InstalledExtension> LoadFallbackInstalledExtensions(
      IVssRequestContext requestContext)
    {
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.LocalExtensionsOnlyOnFallback"))
        return new List<InstalledExtension>();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IInstalledExtensionFallbackService service = vssRequestContext.GetService<IInstalledExtensionFallbackService>();
      List<InstalledExtension> installedExtensionList = new List<InstalledExtension>();
      List<string> stringList = new List<string>();
      CultureInfo cultureInfo;
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
      {
        cultureInfo = Thread.CurrentThread.CurrentUICulture;
      }
      else
      {
        using (new RequestLanguage(requestContext))
          cultureInfo = Thread.CurrentThread.CurrentUICulture;
      }
      for (; cultureInfo != null && cultureInfo.Parent != cultureInfo; cultureInfo = cultureInfo.Parent)
      {
        if (!string.IsNullOrEmpty(cultureInfo.Name))
          stringList.Add(cultureInfo.Name);
      }
      foreach (Dictionary<string, InstalledExtension> installedExtension in service.GetInstalledExtensions(vssRequestContext))
      {
        InstalledExtension sourceExtension1 = (InstalledExtension) null;
        InstalledExtension sourceExtension2 = (InstalledExtension) null;
        foreach (string a in stringList)
        {
          foreach (string key in installedExtension.Keys)
          {
            if (string.Equals(a, key, StringComparison.OrdinalIgnoreCase))
            {
              sourceExtension1 = installedExtension[key];
              break;
            }
            if (key.StartsWith(a, StringComparison.OrdinalIgnoreCase))
            {
              sourceExtension1 = installedExtension[key];
              break;
            }
            if (sourceExtension2 == null && string.IsNullOrEmpty(key))
              sourceExtension2 = installedExtension[key];
          }
          if (sourceExtension1 != null)
            break;
        }
        if (sourceExtension1 != null)
          installedExtensionList.Add((InstalledExtension) new ProcessedInstalledExtension(sourceExtension1));
        else if (sourceExtension2 != null)
          installedExtensionList.Add((InstalledExtension) new ProcessedInstalledExtension(sourceExtension2));
      }
      return installedExtensionList;
    }

    private List<InstalledExtension> FetchInstalledExtensionsInternal(
      IVssRequestContext requestContext)
    {
      requestContext.To(TeamFoundationHostType.Deployment);
      return requestContext.ExecutionEnvironment.IsOnPremisesDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Application) ? this.FetchFallbackInstalledExtensions(requestContext) : this.FetchInstalledExtensionsByStatesInternal(requestContext);
    }

    private List<InstalledExtension> FetchInstalledExtensionsByStatesInternal(
      IVssRequestContext requestContext)
    {
      List<InstalledExtension> source = new List<InstalledExtension>();
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        IVssRegistryService service1 = vssRequestContext.GetService<IVssRegistryService>();
        Dictionary<string, SupportedExtension> supportedVersionMap = vssRequestContext.GetService<IVersionDiscoveryService>().GetSupportedVersionMap(vssRequestContext);
        InstalledExtensionManager.IExtensionStateCache service2 = vssRequestContext.GetService<InstalledExtensionManager.IExtensionStateCache>();
        List<ExtensionState> extensionStates;
        if (!service1.GetValue<bool>(vssRequestContext, in InstalledExtensionManager.s_useCachedExtensions, true) || !service2.TryGetValue(vssRequestContext, requestContext.ServiceHost.InstanceId, out extensionStates))
        {
          extensionStates = requestContext.GetService<IExtensionService>().GetExtensionStates(requestContext, true, true, false);
          if (extensionStates != null)
          {
            bool flag = false;
            foreach (ExtensionState extensionState in extensionStates)
            {
              if (extensionState != null && (extensionState.Flags & Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error) == Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error && (extensionState.Flags & Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.VersionCheckError) != Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.VersionCheckError && ((extensionState.Flags & Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn) != Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn || (extensionState.Flags & Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn) == Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn && !supportedVersionMap.TryGetValue(extensionState.FullyQualifiedName, out SupportedExtension _)))
              {
                flag = true;
                break;
              }
            }
            if (flag)
            {
              requestContext.Trace(100136268, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "At least one extension in error state.  Lowering cache duration");
              service2.Set(vssRequestContext, requestContext.ServiceHost.InstanceId, extensionStates, InstalledExtensionManager.s_errorStateCacheInterval);
            }
            else
            {
              requestContext.Trace(100136270, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "ErrorCheck: Caching extension states for default duration.");
              service2.Set(vssRequestContext, requestContext.ServiceHost.InstanceId, extensionStates);
            }
          }
          else
          {
            requestContext.Trace(100136269, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "Caching extension states for default duration.");
            service2.Set(vssRequestContext, requestContext.ServiceHost.InstanceId, extensionStates);
          }
        }
        List<ExtensionState> extensionStateList = new List<ExtensionState>();
        Dictionary<string, InstalledExtension> dictionary1 = (Dictionary<string, InstalledExtension>) null;
        string str;
        if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          str = "default";
        }
        else
        {
          using (new RequestLanguage(requestContext))
            str = Thread.CurrentThread.CurrentUICulture.Name;
        }
        InstalledExtensionManager.IInstalledExtensionCache service3 = vssRequestContext.GetService<InstalledExtensionManager.IInstalledExtensionCache>();
        bool flag1 = requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.UseSupportedExtensions");
        foreach (ExtensionState extensionState in extensionStates)
        {
          requestContext.Trace(10013571, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "Processing state {0} {1}", (object) extensionState.FullyQualifiedName, (object) extensionState.Version);
          bool flag2 = false;
          SupportedExtension supportedExtension;
          if (flag1 && (extensionState.Flags & Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn) == Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn && supportedVersionMap.TryGetValue(extensionState.FullyQualifiedName, out supportedExtension) && !string.IsNullOrEmpty(extensionState.Version) && !extensionState.Version.Equals(supportedExtension.Version, StringComparison.OrdinalIgnoreCase))
          {
            extensionState.Version = supportedExtension.Version;
            flag2 = true;
          }
          if ((extensionState.Flags & Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error) == Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.Error)
          {
            requestContext.Trace(10013572, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "State in error {0} {1}", (object) extensionState.FullyQualifiedName, (object) extensionState.Version);
            if ((extensionState.Flags & Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn) == Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn)
            {
              if (dictionary1 == null)
                dictionary1 = this.LoadFallbackInstalledExtensions(requestContext).ToDictionary<InstalledExtension, string>((Func<InstalledExtension, string>) (i => i.FullyQualifiedName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
              InstalledExtension sourceExtension;
              if (dictionary1.TryGetValue(extensionState.FullyQualifiedName, out sourceExtension))
                source.Add((InstalledExtension) new ProcessedInstalledExtension(sourceExtension));
            }
          }
          else
          {
            ProcessedInstalledExtension installedExtension1;
            if (service3.TryGetExtension(vssRequestContext, extensionState.FullyQualifiedName, extensionState.Version, str, out installedExtension1))
            {
              requestContext.Trace(10013573, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "Extension loaded from cache {0} {1}", (object) extensionState.FullyQualifiedName, (object) extensionState.Version);
              if (installedExtension1 != null)
              {
                ProcessedInstalledExtension installedExtension2 = new ProcessedInstalledExtension(installedExtension1);
                installedExtension2.InstallState = (InstalledExtensionState) extensionState;
                source.Add((InstalledExtension) installedExtension2);
              }
            }
            else
            {
              bool flag3 = false;
              if (flag1 & flag2)
              {
                if (dictionary1 == null)
                  dictionary1 = this.LoadFallbackInstalledExtensions(requestContext).ToDictionary<InstalledExtension, string>((Func<InstalledExtension, string>) (i => i.FullyQualifiedName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
                InstalledExtension sourceExtension;
                if (dictionary1.TryGetValue(extensionState.FullyQualifiedName, out sourceExtension))
                {
                  ProcessedInstalledExtension extension = new ProcessedInstalledExtension(sourceExtension);
                  extension.Version = extensionState.Version;
                  extension.InstallState = (InstalledExtensionState) extensionState;
                  source.Add((InstalledExtension) extension);
                  if (service1.GetValue<bool>(vssRequestContext, in InstalledExtensionManager.s_useCachedExtensions, true))
                    service3.SetExtension(vssRequestContext, extensionState.FullyQualifiedName, extensionState.Version, extension, str);
                  flag3 = true;
                  vssRequestContext.GetService<InstalledExtensionManager.IExtensionRefreshService>().UpdateExtension(vssRequestContext, extensionState.PublisherName, extensionState.ExtensionName, extensionState.Version, str);
                }
              }
              if (!flag3)
              {
                requestContext.Trace(10013574, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "Extension not found in cache {0} {1}", (object) extensionState.FullyQualifiedName, (object) extensionState.Version);
                extensionStateList.Add(extensionState);
              }
            }
          }
        }
        if (extensionStateList.Count > 0)
        {
          InstalledExtensionQuery extensionQuery = new InstalledExtensionQuery()
          {
            Monikers = new List<ExtensionIdentifier>(),
            AssetTypes = new List<string>() { "*" }
          };
          foreach (ExtensionState extensionState in extensionStateList)
            extensionQuery.Monikers.Add(new ExtensionIdentifier()
            {
              PublisherName = extensionState.PublisherName,
              ExtensionName = extensionState.ExtensionName
            });
          IDictionary<string, InstalledExtension> dictionary2 = (IDictionary<string, InstalledExtension>) requestContext.GetService<IExtensionService>().GetInstalledExtensions(requestContext, extensionQuery).ToDictionary<InstalledExtension, string>((Func<InstalledExtension, string>) (ie => ie.FullyQualifiedName), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (ExtensionState extensionState in extensionStateList)
          {
            InstalledExtension sourceExtension = (InstalledExtension) null;
            ProcessedInstalledExtension extension = (ProcessedInstalledExtension) null;
            if (dictionary2.TryGetValue(extensionState.FullyQualifiedName, out sourceExtension))
            {
              requestContext.Trace(10013574, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "Missing Extension loaded from query {0} {1}", (object) extensionState.FullyQualifiedName, (object) extensionState.Version);
              extension = new ProcessedInstalledExtension(sourceExtension);
              source.Add((InstalledExtension) extension);
            }
            else if ((extensionState.Flags & Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn) != Microsoft.VisualStudio.Services.ExtensionManagement.WebApi.ExtensionStateFlags.BuiltIn)
              requestContext.Trace(10013573, TraceLevel.Error, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "Missing Extension not found from query call {0} {1}", (object) extensionState.FullyQualifiedName, (object) extensionState.Version);
            else
              requestContext.Trace(10013576, TraceLevel.Error, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "Missing BuiltIn Extension not found from query call {0} {1}", (object) extensionState.FullyQualifiedName, (object) extensionState.Version);
            if (service1.GetValue<bool>(vssRequestContext, in InstalledExtensionManager.s_useCachedExtensions, true))
            {
              if (extension != null)
                service3.SetExtension(vssRequestContext, extensionState.FullyQualifiedName, extensionState.Version, extension, str);
              else
                service3.SetExtension(vssRequestContext, extensionState.FullyQualifiedName, extensionState.Version, extension, str, TimeSpan.FromMinutes(2.0));
            }
          }
        }
        if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) && requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionsLicenseCheck") && !requestContext.IsFeatureEnabled("VisualStudio.Services.DelayedIdentityValidation"))
        {
          if (requestContext.IsSystemContext)
            requestContext.TraceConditionally(10013296, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, (Func<string>) (() => string.Format("FetchInstalledExtensions called with SystemContext: {0}", (object) Environment.StackTrace)));
          IExtensionLicensingService service4 = requestContext.GetService<IExtensionLicensingService>();
          IEnumerable<string> strings = source.Select<InstalledExtension, string>((Func<InstalledExtension, string>) (ext => ext.FullyQualifiedName));
          IVssRequestContext requestContext1 = requestContext;
          IEnumerable<string> extensionIds = strings;
          IDictionary<string, bool> extensionRights = service4.GetExtensionRights(requestContext1, extensionIds);
          for (int index = source.Count - 1; index >= 0; --index)
          {
            InstalledExtension installedExtension = source[index];
            bool flag4;
            if ((!extensionRights.TryGetValue(installedExtension.FullyQualifiedName, out flag4) || !flag4) && (installedExtension.Licensing == null || installedExtension.Licensing.Overrides == null))
            {
              requestContext.Trace(10013577, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "No license for extension {0} {1}", (object) installedExtension.FullyQualifiedName, (object) installedExtension.Version);
              source.RemoveAt(index);
            }
          }
        }
        else
          requestContext.Trace(10013295, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "FetchInstalledExtensions license check skipped.  Current host level: {0}", (object) requestContext.ServiceHost.HostType);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013297, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, ex);
        throw;
      }
      return source;
    }

    internal class CircuitBreakerSettings
    {
      internal const string DefaultCommandGroupKey = "Contributions.Framework";
      internal const string DefaultCommandKeyForFetchInstalledExtensions = "FetchInstalledExtensions";
      internal static readonly CommandPropertiesSetter DefaultCommandPropertiesForFetchingInstalledExtensions = new CommandPropertiesSetter().WithCircuitBreakerDisabled(false).WithFallbackDisabled(false).WithCircuitBreakerForceClosed(false).WithCircuitBreakerForceOpen(false).WithCircuitBreakerRequestVolumeThreshold(20).WithCircuitBreakerErrorThresholdPercentage(50).WithCircuitBreakerMinBackoff(TimeSpan.FromSeconds(0.0)).WithCircuitBreakerMaxBackoff(TimeSpan.FromSeconds(30.0)).WithCircuitBreakerDeltaBackoff(TimeSpan.FromMilliseconds(300.0)).WithExecutionTimeout(TimeSpan.FromSeconds(10.0)).WithMetricsHealthSnapshotInterval(TimeSpan.FromSeconds(0.5)).WithMetricsRollingStatisticalWindowInMilliseconds(10000).WithMetricsRollingStatisticalWindowBuckets(10);

      internal string CommandGroupKey { get; set; }

      internal string CommandKeyForFetchInstalledExtensions { get; set; }

      internal CommandPropertiesSetter CircuitBreakerSettingsForFetchingInstalledExtensions { get; set; }

      internal static InstalledExtensionManager.CircuitBreakerSettings Default => new InstalledExtensionManager.CircuitBreakerSettings()
      {
        CommandGroupKey = "Contributions.Framework",
        CommandKeyForFetchInstalledExtensions = "FetchInstalledExtensions",
        CircuitBreakerSettingsForFetchingInstalledExtensions = InstalledExtensionManager.CircuitBreakerSettings.DefaultCommandPropertiesForFetchingInstalledExtensions
      };
    }

    [DefaultServiceImplementation(typeof (InstalledExtensionManager.ExtensionRefreshService))]
    internal interface IExtensionRefreshService : IVssFrameworkService
    {
      void UpdateExtension(
        IVssRequestContext requestContext,
        string publisherName,
        string extensionName,
        string version,
        string requestedLanguage);
    }

    internal class ExtensionRefreshService : 
      VssBaseService,
      InstalledExtensionManager.IExtensionRefreshService,
      IVssFrameworkService
    {
      private ILockName m_extensionFetchLockName;
      private HashSet<string> m_extensionRefreshNeeded = new HashSet<string>();
      private const string s_useCdnAssetUriFlag = "Microsoft.VisualStudio.Services.Gallery.Client.UseCdnAssetUri";
      private static char[] s_nameSeparator = new char[1]
      {
        '|'
      };

      public void ServiceStart(IVssRequestContext requestContext)
      {
        requestContext.CheckDeploymentRequestContext();
        this.m_extensionFetchLockName = this.CreateLockName(requestContext, "extensionFetch");
      }

      public void ServiceEnd(IVssRequestContext systemRequestContext)
      {
      }

      public void UpdateExtension(
        IVssRequestContext requestContext,
        string publisherName,
        string extensionName,
        string version,
        string requestedLanguage)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        TeamFoundationTaskService service = vssRequestContext.GetService<TeamFoundationTaskService>();
        string taskArgs = string.Format("{0}|{1}|{2}|{3}", (object) publisherName, (object) extensionName, (object) version, (object) requestedLanguage);
        bool flag = false;
        using (vssRequestContext.AcquireReaderLock(this.m_extensionFetchLockName))
          flag = this.m_extensionRefreshNeeded.Contains(taskArgs);
        if (flag)
          return;
        using (vssRequestContext.AcquireWriterLock(this.m_extensionFetchLockName))
        {
          if (!this.m_extensionRefreshNeeded.Add(taskArgs))
            return;
          service.AddTask(vssRequestContext, new TeamFoundationTask(new TeamFoundationTaskCallback(this.FetchExtensionTask), (object) taskArgs, DateTime.UtcNow, 0));
        }
      }

      private void FetchExtensionTask(IVssRequestContext requestContext, object args)
      {
        if (!(args is string str))
          return;
        requestContext.Trace(10013298, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "Starting extension refrsh for: {0}", (object) str);
        string[] strArray = str.Split(InstalledExtensionManager.ExtensionRefreshService.s_nameSeparator);
        if (strArray.Length != 4)
        {
          requestContext.Trace(10013299, TraceLevel.Error, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "Did not find correct number of parts in extension identifier string. args: {0}", (object) str);
        }
        else
        {
          IGalleryService service = requestContext.GetService<IGalleryService>();
          ExtensionQueryFlags extensionQueryFlags = ExtensionQueryFlags.IncludeVersions | ExtensionQueryFlags.IncludeFiles | ExtensionQueryFlags.IncludeAssetUri | ExtensionQueryFlags.IncludeLatestVersionOnly;
          if (!requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Gallery.Client.UseCdnAssetUri"))
            extensionQueryFlags |= ExtensionQueryFlags.UseFallbackAssetUri;
          IVssRequestContext requestContext1 = requestContext;
          string publisherName = strArray[0];
          string extensionName = strArray[1];
          string version = strArray[2];
          int flags = (int) extensionQueryFlags;
          PublishedExtension extension = service.GetExtension(requestContext1, publisherName, extensionName, version, (ExtensionQueryFlags) flags);
          ProcessedInstalledExtension installedExtension;
          if (requestContext.GetService<InstalledExtensionManager.IInstalledExtensionCache>().TryGetExtension(requestContext, string.Format("{0}.{1}", (object) strArray[0], (object) strArray[1]), strArray[2], strArray[3], out installedExtension))
          {
            installedExtension.PublisherName = extension.Publisher.PublisherName;
            installedExtension.PublisherDisplayName = extension.Publisher.DisplayName;
            installedExtension.ExtensionName = extension.ExtensionName;
            installedExtension.ExtensionDisplayName = extension.DisplayName;
            installedExtension.LastPublished = extension.LastUpdated;
            if (extension.Versions != null && extension.Versions.Count == 1)
            {
              installedExtension.BaseUri = extension.Versions[0].AssetUri;
              installedExtension.FallbackBaseUri = extension.Versions[0].FallbackAssetUri;
            }
          }
          using (requestContext.AcquireWriterLock(this.m_extensionFetchLockName))
            this.m_extensionRefreshNeeded.Remove(str);
        }
      }
    }

    [DefaultServiceImplementation(typeof (InstalledExtensionManager.ExtensionStateCache))]
    internal interface IExtensionStateCache : IVssFrameworkService
    {
      bool TryGetValue(
        IVssRequestContext requestContext,
        Guid hostId,
        out List<ExtensionState> value);

      bool Remove(IVssRequestContext requestContext, Guid hostId);

      void Set(IVssRequestContext requestContext, Guid hostId, List<ExtensionState> value);

      void Set(
        IVssRequestContext requestContext,
        Guid hostId,
        List<ExtensionState> value,
        TimeSpan timeSpanUntilExpiry);
    }

    internal class ExtensionStateCache : 
      VssMemoryCacheService<Guid, List<ExtensionState>>,
      InstalledExtensionManager.IExtensionStateCache,
      IVssFrameworkService
    {
      private int m_refreshDelaySeconds;
      private const int c_minRefreshDelaySeconds = 2;
      private const int c_defaultRefreshDelaySeconds = 600;
      private static readonly RegistryQuery s_refreshDelay = new RegistryQuery("/Service/ExtensionManagement/Settings/RefreshDelay");
      private INotificationRegistration m_installedExtensionRegistration;
      private INotificationRegistration m_installedExtensionUpdateRegistration;
      private INotificationRegistration m_extensionRegistration;
      private INotificationRegistration m_cacheRefreshRegistration;
      private IVssMemoryCacheGrouping<Guid, List<ExtensionState>, string> m_cacheGrouping;
      private static readonly TimeSpan s_maxCacheLife = TimeSpan.FromMinutes(60.0);
      private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(15.0);
      private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(5.0);
      private static readonly TimeSpan s_versionChangeDelay = TimeSpan.FromSeconds(7.0);
      private static readonly string s_area = "ExtensionStateService";
      private static readonly string s_layer = "Contributions";

      public ExtensionStateCache()
        : base(InstalledExtensionManager.ExtensionStateCache.s_cacheCleanupInterval)
      {
        this.ExpiryInterval.Value = InstalledExtensionManager.ExtensionStateCache.s_maxCacheLife;
        this.InactivityInterval.Value = InstalledExtensionManager.ExtensionStateCache.s_maxCacheInactivityAge;
      }

      protected override void ServiceStart(IVssRequestContext requestContext)
      {
        base.ServiceStart(requestContext);
        if (!requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
        this.m_cacheGrouping = VssMemoryCacheGroupingFactory.Create<Guid, List<ExtensionState>, string>(requestContext, this.MemoryCache, (Func<Guid, List<ExtensionState>, IEnumerable<string>>) ((k, v) => v.Select<ExtensionState, string>((Func<ExtensionState, string>) (x => x.FullyQualifiedName))));
        ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
        this.m_installedExtensionRegistration = service.CreateRegistration(requestContext, "Default", ExtensionManagementSdkSqlNotificationClasses.InstalledExtensionChanged, new SqlNotificationCallback(this.OnExtensionChanged), requestContext.ExecutionEnvironment.IsHostedDeployment, false);
        this.m_installedExtensionUpdateRegistration = service.CreateRegistration(requestContext, "Default", ExtensionManagementSdkSqlNotificationClasses.InstalledExtensionVersionUpdate, new SqlNotificationCallback(this.OnExtensionVersionChanged), requestContext.ExecutionEnvironment.IsHostedDeployment, false);
        this.m_extensionRegistration = service.CreateRegistration(requestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnPublishedExtensionChanged), requestContext.ExecutionEnvironment.IsHostedDeployment, false);
        this.m_cacheRefreshRegistration = service.CreateRegistration(requestContext, "Default", GallerySdkSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), requestContext.ExecutionEnvironment.IsHostedDeployment, false);
        requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged), "/Service/ExtensionManagement/Settings/...");
        this.LoadConfiguration(requestContext);
      }

      protected override void ServiceEnd(IVssRequestContext requestContext)
      {
        this.m_installedExtensionRegistration.Unregister(requestContext);
        this.m_extensionRegistration.Unregister(requestContext);
        this.m_cacheRefreshRegistration.Unregister(requestContext);
        this.m_installedExtensionUpdateRegistration.Unregister(requestContext);
        requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(this.OnRegistryChanged));
        base.ServiceEnd(requestContext);
      }

      private void OnRegistryChanged(
        IVssRequestContext requestContext,
        RegistryEntryCollection changedEntries)
      {
        this.LoadConfiguration(requestContext);
      }

      private void LoadConfiguration(IVssRequestContext requestContext)
      {
        IVssRequestContext vssRequestContext = requestContext.Elevate();
        this.m_refreshDelaySeconds = vssRequestContext.GetService<IVssRegistryService>().GetValue<int>(vssRequestContext, in InstalledExtensionManager.ExtensionStateCache.s_refreshDelay, 600);
        if (this.m_refreshDelaySeconds > 2)
          return;
        this.m_refreshDelaySeconds = 4;
      }

      public virtual void Set(
        IVssRequestContext requestContext,
        Guid hostId,
        List<ExtensionState> states,
        TimeSpan timeSpanUntilExpiry)
      {
        VssCacheExpiryProvider<Guid, List<ExtensionState>> expiryProvider = new VssCacheExpiryProvider<Guid, List<ExtensionState>>(Capture.Create<TimeSpan>(timeSpanUntilExpiry), Capture.Create<TimeSpan>(InstalledExtensionManager.ExtensionStateCache.s_maxCacheInactivityAge));
        this.MemoryCache.Add(hostId, states, true, expiryProvider);
      }

      private void OnExtensionChanged(
        IVssRequestContext requestContext,
        Guid eventClass,
        string eventData)
      {
        try
        {
          InstalledExtensionMessage extensionMessage = TeamFoundationSerializationUtility.Deserialize<InstalledExtensionMessage>(eventData);
          requestContext.Trace(10013289, TraceLevel.Info, InstalledExtensionManager.ExtensionStateCache.s_area, InstalledExtensionManager.ExtensionStateCache.s_layer, "OnExtensionChanged: Extension states have changed for host: {0}", (object) extensionMessage.HostId);
          this.Remove(requestContext, extensionMessage.HostId);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10013285, InstalledExtensionManager.ExtensionStateCache.s_area, InstalledExtensionManager.ExtensionStateCache.s_layer, ex);
        }
      }

      private void OnExtensionVersionChanged(
        IVssRequestContext requestContext,
        Guid eventClass,
        string eventData)
      {
        InstalledExtensionUpdate installedExtensionUpdate = TeamFoundationSerializationUtility.Deserialize<InstalledExtensionUpdate>(eventData);
        List<ExtensionState> extensionStateList;
        if (installedExtensionUpdate == null || !this.TryGetValue(requestContext, installedExtensionUpdate.HostId, out extensionStateList))
          return;
        bool flag = false;
        foreach (ExtensionState extensionState in extensionStateList)
        {
          if (extensionState.PublisherName.Equals(installedExtensionUpdate.PublisherId, StringComparison.OrdinalIgnoreCase) && extensionState.ExtensionName.Equals(installedExtensionUpdate.ExtensionId, StringComparison.OrdinalIgnoreCase))
          {
            flag = string.Equals(extensionState.Version, installedExtensionUpdate.Version);
            break;
          }
        }
        if (flag)
          return;
        this.Remove(requestContext, installedExtensionUpdate.HostId);
      }

      private void OnPublishedExtensionChanged(
        IVssRequestContext requestContext,
        Guid eventClass,
        string eventData)
      {
        try
        {
          ExtensionChangeNotification changeNotification = TeamFoundationSerializationUtility.Deserialize<ExtensionChangeNotification>(eventData);
          requestContext.Trace(100136300, TraceLevel.Info, InstalledExtensionManager.ExtensionStateCache.s_area, InstalledExtensionManager.ExtensionStateCache.s_layer, "OnPublishedExtensionChanged.ExtensionStateCache: Received update message for {0}.{1}", (object) changeNotification.PublisherName, (object) changeNotification.ExtensionName);
          if (changeNotification.EventType == ExtensionEventType.ExtensionEnabled)
            this.Clear(requestContext);
          else if (changeNotification.EventType != ExtensionEventType.ExtensionShared)
          {
            string groupingKey = string.Format("{0}.{1}", (object) changeNotification.PublisherName, (object) changeNotification.ExtensionName);
            int flags = changeNotification.Flags;
            bool flag1 = (flags & 2) == 2;
            bool flag2 = (flags & 512) == 512 && requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.SpreadOutStateCheckForMultiVersion");
            bool flag3 = true;
            if (requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.UseSupportedExtensions") && flag1)
              flag3 = !requestContext.GetService<IVersionDiscoveryService>().IsExtensionSupported(requestContext, changeNotification.PublisherName, changeNotification.ExtensionName);
            IEnumerable<Guid> keys;
            if (!flag3 || !this.m_cacheGrouping.TryGetKeys(groupingKey, out keys))
              return;
            if (flag1 | flag2 && requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.SpreadOutStateCheck"))
            {
              bool flag4 = requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.RefreshStatesOnTaskService");
              foreach (Guid guid in keys)
              {
                Guid hostId = guid;
                List<ExtensionState> states;
                if (this.TryGetValue(requestContext, hostId, out states))
                {
                  TimeSpan taskDelay = TimeSpan.FromSeconds((double) (Math.Abs(hostId.GetHashCode() % (this.m_refreshDelaySeconds - 2)) + 2));
                  if (flag4)
                  {
                    InstalledExtensionUpdate taskArgs = new InstalledExtensionUpdate()
                    {
                      HostId = hostId,
                      PublisherId = changeNotification.PublisherName,
                      ExtensionId = changeNotification.ExtensionName
                    };
                    TeamFoundationTaskService service = requestContext.GetService<TeamFoundationTaskService>();
                    DateTime startTime = DateTime.UtcNow + taskDelay;
                    IVssRequestContext requestContext1 = requestContext;
                    TeamFoundationTask task = new TeamFoundationTask(new TeamFoundationTaskCallback(this.RefreshStatesTask), (object) taskArgs, startTime, 0);
                    service.AddTask(requestContext1, task);
                  }
                  else
                  {
                    requestContext.TraceConditionally(100136400, TraceLevel.Info, InstalledExtensionManager.ExtensionStateCache.s_area, InstalledExtensionManager.ExtensionStateCache.s_layer, (Func<string>) (() => string.Format("Invalidating cache for host {0} in {1}.", (object) hostId, (object) taskDelay)));
                    this.Set(requestContext, hostId, states, taskDelay);
                  }
                }
              }
            }
            else
            {
              foreach (Guid key in keys)
                this.Remove(requestContext, key);
            }
          }
          else
          {
            if (changeNotification.EventType != ExtensionEventType.ExtensionShared)
              return;
            IVssRequestContext requestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
            TeamFoundationServiceHostProperties serviceHostProperties = requestContext.GetService<ITeamFoundationHostManagementService>().QueryServiceHostProperties(requestContext2, changeNotification.HostId, ServiceHostFilterFlags.IncludeChildren);
            if (serviceHostProperties == null || serviceHostProperties.Children == null)
              return;
            foreach (TeamFoundationServiceHostProperties child in serviceHostProperties.Children)
            {
              if (child != null)
                this.Remove(requestContext, child.Id);
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10013285, InstalledExtensionManager.ExtensionStateCache.s_area, InstalledExtensionManager.ExtensionStateCache.s_layer, ex);
        }
      }

      private void RefreshStatesTask(IVssRequestContext requestContext, object taskArgs)
      {
        InstalledExtensionUpdate installedExtensionUpdate = (InstalledExtensionUpdate) taskArgs;
        if (installedExtensionUpdate == null)
          return;
        bool flag = false;
        try
        {
          using (IVssRequestContext vssRequestContext = requestContext.GetService<ITeamFoundationHostManagementService>().BeginRequest(requestContext, installedExtensionUpdate.HostId, RequestContextType.SystemContext))
          {
            List<ExtensionState> extensionStates = vssRequestContext.GetService<IExtensionService>().GetExtensionStates(vssRequestContext, true, true, true);
            this.Set(vssRequestContext, installedExtensionUpdate.HostId, extensionStates);
            foreach (ExtensionState extensionState in extensionStates)
            {
              if (extensionState.PublisherName.Equals(installedExtensionUpdate.PublisherId, StringComparison.OrdinalIgnoreCase) && extensionState.ExtensionName.Equals(installedExtensionUpdate.ExtensionId, StringComparison.OrdinalIgnoreCase))
              {
                if (!string.IsNullOrEmpty(extensionState.Version))
                {
                  flag = true;
                  installedExtensionUpdate.Version = extensionState.Version;
                  break;
                }
                break;
              }
            }
            if (flag)
            {
              DateTime startTime = DateTime.UtcNow + InstalledExtensionManager.ExtensionStateCache.s_versionChangeDelay;
              requestContext.GetService<ITeamFoundationTaskService>().AddTask(requestContext, new TeamFoundationTask((TeamFoundationTaskCallback) ((requestContextTask, updateTaskArgs) =>
              {
                string eventData = TeamFoundationSerializationUtility.SerializeToString<InstalledExtensionUpdate>(installedExtensionUpdate);
                requestContextTask.GetService<TeamFoundationSqlNotificationService>().SendNotification(requestContextTask, ExtensionManagementSdkSqlNotificationClasses.InstalledExtensionVersionUpdate, eventData);
              }), (object) null, startTime, 0));
            }
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10013291, InstalledExtensionManager.ExtensionStateCache.s_area, InstalledExtensionManager.ExtensionStateCache.s_layer, ex);
        }
        if (flag)
          return;
        this.Remove(requestContext, installedExtensionUpdate.HostId);
      }

      private void OnForceFlush(
        IVssRequestContext requestContext,
        Guid eventClass,
        string eventData)
      {
        this.Clear(requestContext.To(TeamFoundationHostType.Deployment));
        requestContext.Trace(10013099, TraceLevel.Info, InstalledExtensionManager.ExtensionStateCache.s_area, InstalledExtensionManager.ExtensionStateCache.s_layer, "ExtensionStateCache.OnForceFlush processed");
      }
    }

    [DefaultServiceImplementation(typeof (InstalledExtensionManager.InstalledExtensionCache))]
    internal interface IInstalledExtensionCache : IVssFrameworkService
    {
      bool TryGetExtension(
        IVssRequestContext requestContext,
        string extensionIdentifier,
        string version,
        string language,
        out ProcessedInstalledExtension installedExtension);

      void SetExtension(
        IVssRequestContext requestContext,
        string extensionIdentifier,
        string version,
        ProcessedInstalledExtension extension,
        string language);

      void SetExtension(
        IVssRequestContext requestContext,
        string extensionIdentifier,
        string version,
        ProcessedInstalledExtension extension,
        string language,
        TimeSpan timeSpanUntilExpiry);
    }

    internal class InstalledExtensionCache : 
      VssMemoryCacheService<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ProcessedInstalledExtension>>>,
      InstalledExtensionManager.IInstalledExtensionCache,
      IVssFrameworkService
    {
      private static readonly TimeSpan s_maxCacheLife = TimeSpan.FromHours(6.0);
      private static readonly TimeSpan s_maxCacheInactivityAge = TimeSpan.FromMinutes(15.0);
      private static readonly TimeSpan s_cacheCleanupInterval = TimeSpan.FromMinutes(5.0);
      private INotificationRegistration m_extensionChangedRegistration;
      private INotificationRegistration m_cacheRefreshRegistration;

      public InstalledExtensionCache()
        : base(InstalledExtensionManager.InstalledExtensionCache.s_cacheCleanupInterval)
      {
        this.InactivityInterval.Value = InstalledExtensionManager.InstalledExtensionCache.s_maxCacheInactivityAge;
        this.ExpiryInterval.Value = InstalledExtensionManager.InstalledExtensionCache.s_maxCacheLife;
      }

      protected override void ServiceStart(IVssRequestContext requestContext)
      {
        base.ServiceStart(requestContext);
        ITeamFoundationSqlNotificationService notificationService = requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment) ? requestContext.GetService<ITeamFoundationSqlNotificationService>() : throw new UnexpectedHostTypeException(requestContext.ServiceHost.HostType);
        this.m_extensionChangedRegistration = notificationService.CreateRegistration(requestContext, "Default", GallerySdkSqlNotificationClasses.ExtensionChanged, new SqlNotificationCallback(this.OnExtensionChanged), requestContext.ExecutionEnvironment.IsHostedDeployment, false);
        this.m_cacheRefreshRegistration = notificationService.CreateRegistration(requestContext, "Default", GallerySdkSqlNotificationClasses.ForceCacheRefresh, new SqlNotificationCallback(this.OnForceFlush), requestContext.ExecutionEnvironment.IsHostedDeployment, false);
      }

      protected override void ServiceEnd(IVssRequestContext requestContext)
      {
        this.m_extensionChangedRegistration.Unregister(requestContext);
        this.m_cacheRefreshRegistration.Unregister(requestContext);
        base.ServiceEnd(requestContext);
      }

      public virtual void SetExtension(
        IVssRequestContext requestContext,
        string extensionIdentifier,
        string version,
        ProcessedInstalledExtension extension,
        string language,
        TimeSpan timeSpanUntilExpiry)
      {
        VssCacheExpiryProvider<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ProcessedInstalledExtension>>> expiryProvider = new VssCacheExpiryProvider<string, ConcurrentDictionary<string, ConcurrentDictionary<string, ProcessedInstalledExtension>>>(Capture.Create<TimeSpan>(timeSpanUntilExpiry), Capture.Create<TimeSpan>(InstalledExtensionManager.InstalledExtensionCache.s_maxCacheInactivityAge));
        ConcurrentDictionary<string, ConcurrentDictionary<string, ProcessedInstalledExtension>> extensions;
        if (!this.TryGetValue(requestContext, extensionIdentifier, out extensions))
          extensions = new ConcurrentDictionary<string, ConcurrentDictionary<string, ProcessedInstalledExtension>>();
        this.MemoryCache.Add(extensionIdentifier, extensions, true, expiryProvider);
        this.UpdateExtension(extensions, version, extension, language);
      }

      public virtual void SetExtension(
        IVssRequestContext requestContext,
        string extensionIdentifier,
        string version,
        ProcessedInstalledExtension extension,
        string language)
      {
        ConcurrentDictionary<string, ConcurrentDictionary<string, ProcessedInstalledExtension>> extensions;
        if (!this.TryGetValue(requestContext, extensionIdentifier, out extensions))
        {
          extensions = new ConcurrentDictionary<string, ConcurrentDictionary<string, ProcessedInstalledExtension>>();
          this.Set(requestContext, extensionIdentifier, extensions);
        }
        this.UpdateExtension(extensions, version, extension, language);
      }

      public virtual bool TryGetExtension(
        IVssRequestContext requestContext,
        string extensionIdentifier,
        string version,
        string language,
        out ProcessedInstalledExtension installedExtension)
      {
        ConcurrentDictionary<string, ConcurrentDictionary<string, ProcessedInstalledExtension>> concurrentDictionary;
        if (this.TryGetValue(requestContext, extensionIdentifier, out concurrentDictionary))
          return concurrentDictionary.GetOrAdd(version, (Func<string, ConcurrentDictionary<string, ProcessedInstalledExtension>>) (k => new ConcurrentDictionary<string, ProcessedInstalledExtension>())).TryGetValue(language, out installedExtension);
        installedExtension = (ProcessedInstalledExtension) null;
        return false;
      }

      private void UpdateExtension(
        ConcurrentDictionary<string, ConcurrentDictionary<string, ProcessedInstalledExtension>> extensions,
        string version,
        ProcessedInstalledExtension extension,
        string language)
      {
        extensions.GetOrAdd(version, (Func<string, ConcurrentDictionary<string, ProcessedInstalledExtension>>) (k => new ConcurrentDictionary<string, ProcessedInstalledExtension>())).AddOrUpdate(language == null ? "default" : language, extension, (Func<string, ProcessedInstalledExtension, ProcessedInstalledExtension>) ((k, b) => extension));
      }

      private void OnExtensionChanged(
        IVssRequestContext requestContext,
        Guid eventClass,
        string eventData)
      {
        try
        {
          ExtensionChangeNotification changeNotification = TeamFoundationSerializationUtility.Deserialize<ExtensionChangeNotification>(eventData);
          string key = string.Format("{0}.{1}", (object) changeNotification.PublisherName, (object) changeNotification.ExtensionName);
          if (changeNotification.EventType == ExtensionEventType.ExtensionDeleted || string.IsNullOrEmpty(changeNotification.Version))
          {
            this.Remove(requestContext, key);
          }
          else
          {
            ConcurrentDictionary<string, ConcurrentDictionary<string, ProcessedInstalledExtension>> concurrentDictionary;
            if (changeNotification.EventType == ExtensionEventType.ExtensionShared || !this.TryGetValue(requestContext, key, out concurrentDictionary))
              return;
            concurrentDictionary.TryRemove(changeNotification.Version, out ConcurrentDictionary<string, ProcessedInstalledExtension> _);
            if (concurrentDictionary.Count != 0)
              return;
            this.Remove(requestContext, key);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10013290, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, ex);
        }
      }

      private void OnForceFlush(
        IVssRequestContext requestContext,
        Guid eventClass,
        string eventData)
      {
        this.Clear(requestContext.To(TeamFoundationHostType.Deployment));
        requestContext.Trace(10013098, TraceLevel.Info, InstalledExtensionManager.s_area, InstalledExtensionManager.s_layer, "InstalledExtensionCache.OnForceFlush processed");
      }
    }
  }
}
