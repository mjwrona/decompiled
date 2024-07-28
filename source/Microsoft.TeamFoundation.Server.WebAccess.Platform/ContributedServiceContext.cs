// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ContributedServiceContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Bundling;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class ContributedServiceContext : WebSdkMetadata
  {
    private WebContext m_webContext;

    public ContributedServiceContext(WebContext webContext) => this.m_webContext = webContext;

    public static ContributedServiceContext CreateForCurrentService(
      DataProviderContext providerContext,
      bool includeBundles = true)
    {
      WebContext requestWebContext = WebContextFactory.GetCurrentRequestWebContext();
      WebPageDataProviderPageSource pageSource = WebPageDataProviderUtil.GetPageSource(requestWebContext.TfsRequestContext);
      if (pageSource != null && pageSource.Diagnostics != null)
      {
        requestWebContext.Diagnostics.DebugMode = pageSource.Diagnostics.DebugMode;
        requestWebContext.Diagnostics.BundlingEnabled = pageSource.Diagnostics.BundlingEnabled;
        requestWebContext.Diagnostics.TracePointCollectionEnabled = pageSource.Diagnostics.TracePointCollectionEnabled;
      }
      ContributedServiceContext contributedServiceContext = WebContextFactory.GetContributedServiceContext(requestWebContext.RequestContext);
      bool hostedDeployment = requestWebContext.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment;
      if (hostedDeployment)
      {
        contributedServiceContext.ServiceTypeId = requestWebContext.TfsRequestContext.ServiceInstanceType();
        IVssRequestContext tfsRequestContext = requestWebContext.TfsRequestContext;
        ILocationService service = tfsRequestContext.GetService<ILocationService>();
        contributedServiceContext.ServiceRootUrl = service.GetLocationServiceUrl(tfsRequestContext, LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.PublicAccessMappingMoniker);
        contributedServiceContext.Paths = new ConfigurationContextPaths(requestWebContext.TfsRequestContext);
        if (includeBundles)
          contributedServiceContext.Bundles = contributedServiceContext.AddBundles(providerContext);
      }
      contributedServiceContext.ServiceLocations = new ServiceLocations(requestWebContext);
      contributedServiceContext.FeatureAvailability = new FeatureAvailabilityContext(requestWebContext, hostedDeployment);
      contributedServiceContext.ModuleLoaderConfig = new ModuleLoaderConfiguration(requestWebContext, hostedDeployment);
      if (hostedDeployment)
        contributedServiceContext.ModuleLoaderConfig.AddBaseModulePaths();
      contributedServiceContext.CheckPermission(requestWebContext.TfsRequestContext);
      return contributedServiceContext;
    }

    public DynamicBundlesCollection AddBundles(
      DataProviderContext providerContext,
      bool ignoreContributionConstraints = true)
    {
      WebContext webContext = this.m_webContext;
      if (webContext.Diagnostics.BundlingEnabled)
      {
        WebPageDataProviderPageSource pageSource = WebPageDataProviderUtil.GetPageSource(webContext.TfsRequestContext);
        if (pageSource != null)
        {
          IContributionService service = webContext.TfsRequestContext.GetService<IContributionService>();
          List<Contribution> contributionList = new List<Contribution>();
          ContributionQueryOptions contributionQueryOptions = ignoreContributionConstraints ? ContributionQueryOptions.IgnoreConstraints : ContributionQueryOptions.None;
          if (!string.IsNullOrEmpty(pageSource.SelectedHubGroupId))
            contributionList.AddRange(service.QueryContributions(webContext.TfsRequestContext, (IEnumerable<string>) new string[1]
            {
              pageSource.SelectedHubGroupId
            }, (HashSet<string>) null, ContributionQueryOptions.IncludeChildren | contributionQueryOptions, (ContributionQueryCallback) ((requestContext, contribution, parentContribution, relationship, queryOptions, evaluatedConditions) => !contribution.IsOfType("ms.vss-web.hub") ? ContributionQueryOptions.IncludeSubTree : ContributionQueryOptions.None), (ContributionDiagnostics) null));
          if (!string.IsNullOrEmpty(pageSource.SelectedHubId))
            contributionList.AddRange(service.QueryContributions(webContext.TfsRequestContext, (IEnumerable<string>) new string[1]
            {
              pageSource.SelectedHubId
            }, queryOptions: ContributionQueryOptions.IncludeAll | contributionQueryOptions));
          ISet<string> requiredModules = ContributionUtility.GetRequiredModules((IEnumerable<Contribution>) contributionList);
          if (requiredModules.Count > 0)
            return BundlingHelper.RegisterDynamicBundles(webContext, "ext", requiredModules, (ISet<string>) null, (ISet<string>) null, (ISet<string>) null, (IEnumerable<string>) pageSource.ContributionPaths, (IEnumerable<string>) this.CssModulePrefixes, (string) null, new bool?(pageSource.Diagnostics.DiagnoseBundles), pageSource.Globalization?.Theme);
        }
      }
      return (DynamicBundlesCollection) null;
    }

    [DataMember]
    public Guid ServiceTypeId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<string> CssModulePrefixes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string ServiceRootUrl { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ServiceLocations ServiceLocations { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public FeatureAvailabilityContext FeatureAvailability { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DynamicBundlesCollection Bundles { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ModuleLoaderConfiguration ModuleLoaderConfig { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ConfigurationContextPaths Paths { get; set; }
  }
}
