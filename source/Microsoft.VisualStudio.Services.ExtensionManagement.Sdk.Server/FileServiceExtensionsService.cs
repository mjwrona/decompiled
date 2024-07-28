// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FileServiceExtensionsService
// Assembly: Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FE9DD3E-5758-4D1B-8056-ECAF8A4B7A77
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Gallery.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server
{
  internal class FileServiceExtensionsService : 
    LocalExtensionService,
    IFileServiceExtensionsService,
    ILocalExtensionsService,
    IVssFrameworkService
  {
    private static readonly string s_area = nameof (FileServiceExtensionsService);
    private static readonly string s_layer = "IVssFrameworkService";
    private Dictionary<string, LocalExtension> m_localExtensionsWithFallback;
    private int m_refreshInProgress;

    public override void ServiceStart(IVssRequestContext requestContext)
    {
      base.ServiceStart(requestContext);
      requestContext.GetService<IVssRegistryService>().RegisterNotification(requestContext, new RegistrySettingsChangedCallback(((LocalExtensionService) this).OnRegistryChanged), "/Configuration/LocalContributions/**");
      this.GetExtensions(requestContext);
    }

    public override void ServiceEnd(IVssRequestContext requestContext)
    {
      base.ServiceEnd(requestContext);
      requestContext.GetService<IVssRegistryService>().UnregisterNotification(requestContext, new RegistrySettingsChangedCallback(((LocalExtensionService) this).OnRegistryChanged));
    }

    public override Dictionary<string, LocalExtension> GetExtensions(
      IVssRequestContext requestContext)
    {
      if (this.LocalExtensions != null && this.m_localExtensionsWithFallback != null)
      {
        if (this.IsDataCurrent())
          return !this.IsInFallbackMode(requestContext) ? this.LocalExtensions : this.m_localExtensionsWithFallback;
        if (!requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.RefreshLocalExtensionIfNeededOnAllrequests"))
        {
          if (Interlocked.CompareExchange(ref this.m_refreshInProgress, 1, 0) == 0)
            this.QueueExtensionRefresh(requestContext, true);
          requestContext.Trace(10013310, TraceLevel.Info, FileServiceExtensionsService.s_area, FileServiceExtensionsService.s_layer, "Returning expired value since refresh is currently in progress");
          return !this.IsInFallbackMode(requestContext) ? this.LocalExtensions : this.m_localExtensionsWithFallback;
        }
        requestContext.Trace(10013311, TraceLevel.Info, FileServiceExtensionsService.s_area, FileServiceExtensionsService.s_layer, "All requests attempting to perform local extension refresh.");
      }
      return this.RefreshExtensions(requestContext);
    }

    internal override Dictionary<string, LocalExtension> RefreshExtensions(
      IVssRequestContext requestContext)
    {
      try
      {
        requestContext.Trace(10013315, TraceLevel.Info, FileServiceExtensionsService.s_area, FileServiceExtensionsService.s_layer, "Refreshing local extension cache");
        int currentDataVersion = this.CurrentDataVersion;
        Dictionary<string, LocalExtensionProvider> localProviders = new Dictionary<string, LocalExtensionProvider>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        Dictionary<string, LocalExtension> localExtensions1 = new Dictionary<string, LocalExtension>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        string query1 = string.Format("{0}**/{1}", (object) "/Configuration/LocalContributionProviders/", (object) "DisplayName");
        this.LoadProviders(requestContext, query1, localProviders);
        string query2 = string.Format("{0}**/{1}", (object) "/Configuration/LocalContributions/", (object) "Installed");
        int num1 = this.LoadExtensions(requestContext, query2, localProviders, localExtensions1) ? 1 : 0;
        Dictionary<string, LocalExtension> localExtensions2 = new Dictionary<string, LocalExtension>((IDictionary<string, LocalExtension>) localExtensions1, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        string query3 = string.Format("{0}**/{1}", (object) "/Configuration/LocalContributions/", (object) "Fallback");
        int num2 = this.LoadExtensions(requestContext, query3, localProviders, localExtensions2) ? 1 : 0;
        if ((num1 & num2) != 0)
        {
          this.ResetRefreshAttempts();
          if (currentDataVersion == this.CurrentDataVersion)
          {
            this.LoadedDataVersion = this.CurrentDataVersion;
            this.LocalExtensions = localExtensions1;
            this.m_localExtensionsWithFallback = localExtensions2;
          }
        }
        else
          this.QueueExtensionRefresh(requestContext, false);
        return this.IsInFallbackMode(requestContext) ? localExtensions2 : localExtensions1;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10013320, TraceLevel.Error, FileServiceExtensionsService.s_area, FileServiceExtensionsService.s_layer, ex);
        throw;
      }
      finally
      {
        this.m_refreshInProgress = 0;
      }
    }

    private bool IsInFallbackMode(IVssRequestContext requestContext)
    {
      bool flag = false;
      if (!requestContext.RootContext.TryGetItem<bool>("InExtensionFallbackMode", out flag) && requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.UseFallbackModeWhenEmsMissing"))
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        flag = vssRequestContext.GetService<IInstanceManagementService>().GetServiceInstanceType(vssRequestContext, ExtensionConstants.ServiceOwner) == null;
        requestContext.RootContext.Items["InExtensionFallbackMode"] = (object) flag;
      }
      return flag;
    }

    internal virtual bool LoadExtensions(
      IVssRequestContext requestContext,
      string query,
      Dictionary<string, LocalExtensionProvider> localProviders,
      Dictionary<string, LocalExtension> localExtensions)
    {
      bool flag = true;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service1 = requestContext.GetService<IVssRegistryService>();
      ITeamFoundationFileService service2 = requestContext.GetService<ITeamFoundationFileService>();
      foreach (RegistryEntry readEntry1 in service1.ReadEntries(requestContext, new RegistryQuery(query)))
      {
        if (readEntry1.GetValue<bool>())
        {
          string[] strArray = readEntry1.Path.Split(new char[1]
          {
            '/'
          }, StringSplitOptions.RemoveEmptyEntries);
          if (strArray.Length >= 4)
          {
            string str1 = strArray[2];
            string extensionName = strArray[3];
            string str2 = string.Format("/Configuration/LocalContributions/{0}/{1}/", (object) str1, (object) extensionName);
            IVssRegistryService registryService1 = service1;
            IVssRequestContext requestContext1 = requestContext;
            RegistryQuery registryQuery = (RegistryQuery) (str2 + "Version");
            ref RegistryQuery local1 = ref registryQuery;
            string version = registryService1.GetValue<string>(requestContext1, in local1, "999.999.999.999");
            IVssRegistryService registryService2 = service1;
            IVssRequestContext requestContext2 = requestContext;
            registryQuery = (RegistryQuery) (str2 + "SupportedHosts");
            ref RegistryQuery local2 = ref registryQuery;
            TeamFoundationHostType foundationHostType = (TeamFoundationHostType) registryService2.GetValue<int>(requestContext2, in local2, 0);
            string registryPathPattern = string.Format("/Configuration/LocalContributions/{0}/{1}/Assets/**/FileId", (object) str1, (object) extensionName);
            foreach (RegistryEntry readEntry2 in service1.ReadEntries(requestContext, new RegistryQuery(registryPathPattern)))
            {
              try
              {
                string[] pathParts = readEntry2.Path.Split(new char[1]
                {
                  '/'
                }, StringSplitOptions.RemoveEmptyEntries);
                if (LocalExtensionUtil.AssetTypeFromPath(pathParts).Equals("Microsoft.VisualStudio.Services.Manifest"))
                {
                  int result = 0;
                  string key1 = pathParts[5];
                  if (int.TryParse(readEntry2.Value, out result))
                  {
                    string key2 = string.Format("{0}.{1}", (object) str1, (object) extensionName);
                    using (Stream manifestStream = service2.RetrieveFile(requestContext, (long) result, false, out byte[] _, out long _, out CompressionType _))
                    {
                      IDictionary<string, object> extensionProperties = (IDictionary<string, object>) ExtensionUtil.GetExtensionProperties(PublishedExtensionFlags.BuiltIn | PublishedExtensionFlags.Trusted | PublishedExtensionFlags.Public | PublishedExtensionFlags.MultiVersion);
                      extensionProperties.Add("::Version", (object) version);
                      ExtensionManifest extensionManifest = ExtensionUtil.LoadManifest(str1, extensionName, version, manifestStream, extensionProperties);
                      if (requestContext.IsFeatureEnabled("VisualStudio.Services.ExtensionManagement.LookForContributionMatch") && extensionManifest.Contributions != null)
                      {
                        ContribtuionLookupService service3 = vssRequestContext.GetService<ContribtuionLookupService>();
                        List<Contribution> contributionList = new List<Contribution>();
                        foreach (Contribution contribution in extensionManifest.Contributions)
                          contributionList.Add(service3.GetContribution(vssRequestContext, contribution));
                        extensionManifest.Contributions = (IEnumerable<Contribution>) contributionList;
                      }
                      string str3 = (string) null;
                      LocalExtensionProvider extensionProvider;
                      if (localProviders.TryGetValue(str1, out extensionProvider))
                        str3 = extensionProvider.DisplayName;
                      LocalContributionDetails contributionDetails = new LocalContributionDetails()
                      {
                        PublisherName = str1,
                        PublisherDisplayName = str3,
                        ExtensionName = extensionName,
                        Contributions = extensionManifest.Contributions,
                        ContributionTypes = extensionManifest.ContributionTypes,
                        Constraints = extensionManifest.Constraints,
                        Version = version
                      };
                      LocalExtension localExtension;
                      if (!localExtensions.TryGetValue(key2, out localExtension))
                      {
                        localExtension = new LocalExtension()
                        {
                          SupportedHosts = foundationHostType
                        };
                        localExtensions[key2] = localExtension;
                      }
                      localExtension.ContributionsByLanguage[key1] = (IContributionProvider) new FileServiceContributionProvider(contributionDetails);
                    }
                  }
                }
              }
              catch (Exception ex)
              {
                flag = false;
                requestContext.TraceException(10013300, FileServiceExtensionsService.s_area, FileServiceExtensionsService.s_layer, ex);
              }
            }
          }
        }
      }
      return flag;
    }
  }
}
