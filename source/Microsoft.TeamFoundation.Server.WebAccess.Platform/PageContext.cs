// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.PageContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [DataContract]
  public class PageContext : WebSdkMetadata
  {
    private HubsContext m_hubsContext;
    private bool m_creatingHubsContext;
    private Dictionary<string, Contribution> m_contributions;
    private HashSet<string> m_queriedContributionIds;

    public PageContext(WebContext webContext)
      : this()
    {
      this.WebContext = webContext;
      this.ModuleLoaderConfig = new ModuleLoaderConfiguration(webContext, true);
      this.CoreReferences = new CoreReferencesContext(webContext);
      this.WebAccessConfiguration = new ConfigurationContext(webContext.TfsRequestContext, webContext.RequestContext);
      this.MicrosoftAjaxConfig = new MicrosoftAjaxConfig();
      this.TimeZonesConfiguration = new TimeZonesConfiguration(webContext);
      this.FeatureAvailability = new FeatureAvailabilityContext(webContext, true);
      this.AppInsightsConfiguration = new AppInsightsConfiguration(webContext);
      this.ServiceLocations = new ServiceLocations(webContext);
      this.ServiceLocations.Add(ServiceInstanceTypes.SPS, TeamFoundationHostType.Application);
      this.ServiceLocations.Add(ServiceInstanceTypes.SPS, TeamFoundationHostType.ProjectCollection);
      this.ServiceLocations.Add(ServiceInstanceTypes.SPS, TeamFoundationHostType.Deployment);
      if (!webContext.TfsRequestContext.ExecutionEnvironment.IsHostedDeployment)
        return;
      this.ServiceInstanceId = webContext.TfsRequestContext.ServiceInstanceType();
    }

    public PageContext()
    {
      this.m_contributions = new Dictionary<string, Contribution>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      this.m_queriedContributionIds = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    }

    [DataMember(EmitDefaultValue = false)]
    public WebContext WebContext { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ModuleLoaderConfiguration ModuleLoaderConfig { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public CoreReferencesContext CoreReferences { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ConfigurationContext WebAccessConfiguration { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public List<string> CssModulePrefixes { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public MicrosoftAjaxConfig MicrosoftAjaxConfig { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public TimeZonesConfiguration TimeZonesConfiguration { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public FeatureAvailabilityContext FeatureAvailability { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public AppInsightsConfiguration AppInsightsConfiguration { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DiagnosticsContext Diagnostics => this.WebContext?.Diagnostics;

    [DataMember(EmitDefaultValue = false)]
    public NavigationContext Navigation => this.WebContext?.NavigationContext;

    [DataMember(EmitDefaultValue = false)]
    public GlobalizationContext Globalization => this.WebContext?.Globalization;

    [DataMember(EmitDefaultValue = false)]
    public Guid ServiceInstanceId { get; private set; }

    [DataMember(EmitDefaultValue = false)]
    public HubsContext HubsContext
    {
      get
      {
        this.EnsureHubContributionsLoaded();
        return this.m_hubsContext;
      }
    }

    private void EnsureHubContributionsLoaded()
    {
      if (this.m_hubsContext != null || this.WebContext == null || this.m_creatingHubsContext)
        return;
      this.WebContext.TfsRequestContext.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (EnsureHubContributionsLoaded));
      try
      {
        using (WebPerformanceTimer.StartMeasure(this.WebContext.RequestContext, "PageContext.GetHubContributions"))
        {
          this.EnsureHubsContext();
          if (!string.IsNullOrEmpty(this.m_hubsContext.HubGroupsCollectionContributionId))
            this.QueryContributions((IEnumerable<string>) new string[1]
            {
              this.m_hubsContext.HubGroupsCollectionContributionId
            }, (ContributionQueryCallback) ((requestContext, contribution, parentContribution, relationship, queryOptions, evaluatedConditions) => !contribution.IsOfType("ms.vss-web.hub-group") ? ContributionQueryOptions.IncludeAll : ContributionQueryOptions.IncludeSelf));
          if (!string.IsNullOrEmpty(this.m_hubsContext.SelectedHubGroupId))
            this.QueryContributions((IEnumerable<string>) new string[1]
            {
              this.m_hubsContext.SelectedHubGroupId
            }, (ContributionQueryCallback) ((requestContext, contribution, parentContribution, relationship, queryOptions, evaluatedConditions) => !contribution.IsOfType("ms.vss-web.hub") ? ContributionQueryOptions.IncludeAll : ContributionQueryOptions.None));
          if (!string.IsNullOrEmpty(this.m_hubsContext.SelectedHubId))
            this.QueryContributions((IEnumerable<string>) new string[1]
            {
              this.m_hubsContext.SelectedHubId
            }, (ContributionQueryCallback) ((requestContext, contribution, parentContribution, relationship, queryOptions, evaluatedConditions) => !contribution.IsOfType("ms.vss-web.navigation") ? ContributionQueryOptions.IncludeAll : ContributionQueryOptions.None));
          if (this.m_hubsContext.SelectedNavigationIds == null || this.m_hubsContext.SelectedNavigationIds.Length == 0)
            return;
          this.QueryContributions((IEnumerable<string>) this.m_hubsContext.SelectedNavigationIds, (ContributionQueryCallback) ((requestContext, contribution, parentContribution, relationship, queryOptions, evaluatedConditions) => contribution.IsOfType("ms.vss-web.navigation") && !((IEnumerable<string>) this.m_hubsContext.SelectedNavigationIds).Contains<string>(contribution.Id, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) ? ContributionQueryOptions.IncludeSelf : ContributionQueryOptions.IncludeAll));
        }
      }
      finally
      {
        this.WebContext.TfsRequestContext.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (EnsureHubContributionsLoaded));
      }
    }

    protected virtual HubsContext CreateHubsContext(WebContext webContext)
    {
      HubsContext hubsContext = new HubsContext();
      hubsContext.PopulateFromContributions(webContext);
      return hubsContext;
    }

    private void EnsureHubsContext()
    {
      if (this.m_hubsContext != null || this.m_creatingHubsContext)
        return;
      this.m_creatingHubsContext = true;
      this.WebContext.TfsRequestContext.TraceEnter(0, "WebAccess", TfsTraceLayers.Content, nameof (EnsureHubsContext));
      try
      {
        using (WebPerformanceTimer.StartMeasure(this.WebContext.RequestContext, "PageContext.CreateHubsContext"))
          this.m_hubsContext = this.CreateHubsContext(this.WebContext);
      }
      finally
      {
        this.WebContext.TfsRequestContext.TraceLeave(0, "WebAccess", TfsTraceLayers.Content, nameof (EnsureHubsContext));
      }
    }

    [DataMember(EmitDefaultValue = false)]
    public ServiceLocations ServiceLocations { get; private set; }

    public IEnumerable<Contribution> Contributions
    {
      get
      {
        this.EnsureHubContributionsLoaded();
        return (IEnumerable<Contribution>) this.m_contributions.Values;
      }
    }

    public IEnumerable<string> QueriedContributionIds => (IEnumerable<string>) this.m_queriedContributionIds;

    private void QueryContributions(
      IEnumerable<string> contributionIds,
      ContributionQueryCallback queryCallback = null)
    {
      foreach (Contribution queryContribution in this.WebContext.TfsRequestContext.GetService<IContributionService>().QueryContributions(this.WebContext.TfsRequestContext, contributionIds, (HashSet<string>) null, ContributionQueryOptions.IncludeAll, queryCallback, (ContributionDiagnostics) null))
        this.m_contributions[queryContribution.Id] = queryContribution;
      if (queryCallback != null)
        return;
      this.m_queriedContributionIds.UnionWith(contributionIds);
    }

    public void AddContributions(IEnumerable<string> contributionIds) => this.QueryContributions(contributionIds);

    public void AddContributions(IEnumerable<Contribution> contributions)
    {
      foreach (Contribution contribution in contributions)
        this.m_contributions[contribution.Id] = contribution;
    }
  }
}
