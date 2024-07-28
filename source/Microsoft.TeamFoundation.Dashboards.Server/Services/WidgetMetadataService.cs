// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.WidgetMetadataService
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Dashboards.Telemetry;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server.FeatureManagement;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class WidgetMetadataService : IWidgetMetadataService, IVssFrameworkService
  {
    public static readonly string[] WidgetTargetContributions = new string[1]
    {
      "ms.vss-dashboards-web.widget-catalog"
    };
    public static readonly string[] WidgetConfigurationTargetContributions = new string[1]
    {
      "ms.vss-dashboards-web.widget-configuration"
    };
    public static readonly HashSet<string> WidgetTypeContributions = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-dashboards-web.widget"
    };
    public static readonly HashSet<string> WidgetConfigurationTypeContributions = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
    {
      "ms.vss-dashboards-web.widget-configuration"
    };

    public IEnumerable<WidgetMetadata> GetWidgets(IVssRequestContext requestContext)
    {
      using (TelemetryCollector.TraceMonitor(requestContext, 10017414, "WidgetTypesService", "AppStoreWidgetMetadataService.GetWidgets"))
      {
        IContributionService service = requestContext.GetService<IContributionService>();
        ContributionQueryOptions contributionQueryOptions = ContributionQueryOptions.IncludeChildren;
        IVssRequestContext requestContext1 = requestContext;
        string[] targetContributions = WidgetMetadataService.WidgetTargetContributions;
        HashSet<string> typeContributions = WidgetMetadataService.WidgetTypeContributions;
        int queryOptions = (int) contributionQueryOptions;
        return service.QueryContributions(requestContext1, (IEnumerable<string>) targetContributions, typeContributions, (ContributionQueryOptions) queryOptions).Select<Contribution, WidgetMetadata>((Func<Contribution, WidgetMetadata>) (o => WidgetMetadataService.ConvertContribution(requestContext, o)));
      }
    }

    public IEnumerable<WidgetConfigurationMetadata> GetWidgetConfigurations(
      IVssRequestContext requestContext)
    {
      using (TelemetryCollector.TraceMonitor(requestContext, 10017414, "WidgetTypesService", "AppStoreWidgetMetadataService.GetWidgetConfigurations"))
        return requestContext.GetService<IContributionService>().QueryContributions(requestContext, (IEnumerable<string>) WidgetMetadataService.WidgetConfigurationTargetContributions, WidgetMetadataService.WidgetConfigurationTypeContributions, ContributionQueryOptions.IncludeChildren).Select<Contribution, WidgetConfigurationMetadata>((Func<Contribution, WidgetConfigurationMetadata>) (o => WidgetMetadataService.ConvertConfigurationContribution(requestContext, o)));
    }

    private static WidgetMetadata ConvertContribution(
      IVssRequestContext requestContext,
      Contribution contribution)
    {
      object obj = new object();
      ContributionIdentifier contribution1 = new ContributionIdentifier(contribution.Id);
      WidgetMetadata catalogEntry = new WidgetMetadata();
      catalogEntry.PublisherName = contribution1.PublisherName;
      catalogEntry.TypeId = contribution1.RelativeId;
      catalogEntry.Name = contribution.GetProperty<string>("name");
      catalogEntry.Targets = contribution.Targets;
      catalogEntry.Tags = contribution.GetProperty<List<string>>("tags");
      catalogEntry.Keywords = contribution.GetProperty<List<string>>("keywords");
      catalogEntry.AllowedSizes = (IEnumerable<WidgetSize>) contribution.GetProperty<WidgetSize[]>("supportedSizes");
      catalogEntry.SupportedScopes = (IEnumerable<WidgetScope>) contribution.GetProperty<WidgetScope[]>("supportedScopes");
      catalogEntry.CatalogIconUrl = WidgetMetadataService.GetCatalogIconUrl(requestContext, contribution);
      catalogEntry.LoadingImageUrl = WidgetMetadataService.GetContributionTemplateUriProperty(requestContext, contribution, "loadingImageUrl");
      catalogEntry.Description = contribution.GetProperty<string>("description");
      catalogEntry.IsVisibleFromCatalog = contribution.GetProperty<bool>("isVisibleFromCatalog", true);
      string templateUriProperty = WidgetMetadataService.GetContributionTemplateUriProperty(requestContext, contribution, "uri");
      if (!string.IsNullOrWhiteSpace(templateUriProperty))
        catalogEntry.ContentUri = new Uri(templateUriProperty, UriKind.RelativeOrAbsolute);
      catalogEntry.CatalogInfoUrl = contribution.GetProperty<string>("catalogInfoURI");
      if (string.IsNullOrEmpty(catalogEntry.CatalogInfoUrl))
        catalogEntry.CatalogInfoUrl = GalleryHelper.TryGetMarketPlaceExtensionUrl(requestContext, contribution1);
      catalogEntry.IsEnabled = contribution.GetProperty<bool>("isEnabled", true);
      catalogEntry.IsNameConfigurable = contribution.GetProperty<bool>("isNameConfigurable");
      catalogEntry.ContributionId = contribution.Id;
      catalogEntry.DefaultSettings = contribution.GetProperty<string>("defaultSettings");
      catalogEntry.ConfigurationRequired = contribution.GetProperty<bool>("configurationRequired");
      catalogEntry.AnalyticsServiceRequired = contribution.GetProperty<bool>("analyticsServiceRequired");
      catalogEntry.CanStoreCrossProjectSettings = contribution.GetProperty<bool>("canStoreCrossProjectSettings", true);
      catalogEntry.LightboxOptions = contribution.GetProperty<LightboxOptions>("lightbox");
      WidgetMetadataService.PatchContributionEntry(requestContext, catalogEntry);
      return catalogEntry;
    }

    public static string GetCatalogIconUrl(
      IVssRequestContext requestContext,
      Contribution contribution)
    {
      string templateUriProperty1 = WidgetMetadataService.GetContributionTemplateUriProperty(requestContext, contribution, "catalogIconUrl");
      string templateUriProperty2 = WidgetMetadataService.GetContributionTemplateUriProperty(requestContext, contribution, "previewImageUrl");
      return !string.IsNullOrWhiteSpace(templateUriProperty1) ? templateUriProperty1 : templateUriProperty2;
    }

    private static string GetContributionTemplateUriProperty(
      IVssRequestContext requestContext,
      Contribution contribution,
      string keyName)
    {
      object replacementValues = new object();
      try
      {
        return contribution.GetTemplateUriProperty(requestContext, keyName, replacementValues, PlatformMustacheExtensions.Parser);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10017420, "Dashboards", nameof (WidgetMetadataService), ex);
        return (string) null;
      }
    }

    private static WidgetConfigurationMetadata ConvertConfigurationContribution(
      IVssRequestContext requestContext,
      Contribution contribution)
    {
      ContributionIdentifier contributionIdentifier = new ContributionIdentifier(contribution.Id);
      return new WidgetConfigurationMetadata()
      {
        TypeId = contributionIdentifier.RelativeId,
        ContributionId = contribution.Id,
        Targets = contribution.Targets
      };
    }

    private static void PatchContributionEntry(
      IVssRequestContext requestContext,
      WidgetMetadata catalogEntry)
    {
      IContributedFeatureService service = requestContext.GetService<IContributedFeatureService>();
      if (catalogEntry.ContributionId == "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.AnalyticsSprintBurndownWidget" && service.IsFeatureEnabled(requestContext, "ms.vss-dashboards-web.sprint-burndown-availability"))
      {
        catalogEntry.IsVisibleFromCatalog = true;
      }
      else
      {
        if (!(catalogEntry.ContributionId == "ms.vss-dashboards-web.Microsoft.VisualStudioOnline.Dashboards.SprintBurndownWidget"))
          return;
        if (service.IsFeatureEnabled(requestContext, "ms.vss-dashboards-web.sprint-burndown-availability"))
        {
          catalogEntry.Name = Microsoft.TeamFoundation.Dashboards.DashboardResources.SprintBurndownLegacyCatalogName();
          catalogEntry.LightboxOptions = new LightboxOptions(new int?(900), new int?(700), new bool?(true));
        }
        else
          catalogEntry.AllowedSizes = (IEnumerable<WidgetSize>) new List<WidgetSize>()
          {
            new WidgetSize() { RowSpan = 1, ColumnSpan = 2 }
          };
      }
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }
  }
}
