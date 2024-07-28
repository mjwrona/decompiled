// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebPageDataProviderUtil
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class WebPageDataProviderUtil
  {
    private const string c_dataProviderScopeDataKey = "DataProviderQuery.Scope";
    private const string c_projectScopeName = "project";
    private const string c_pageSourceWithPageContextRequestContextKey = "WebPlatform.WebPageSource.WithPageContext";

    public static WebPageDataProviderPageSource GetPageSource(
      IVssRequestContext requestContext,
      bool create = true)
    {
      object obj1;
      if (requestContext.RootContext.Items.TryGetValue("WebPlatform.WebPageSource", out obj1))
      {
        using (PerformanceTimer.StartMeasure(requestContext, "WebPageDataProviderUtil.CachedPageSource"))
        {
          WebPageDataProviderPageSource pageSource = (WebPageDataProviderPageSource) obj1;
          if (!requestContext.RootContext.Items.ContainsKey("WebPlatform.WebPageSource.WithPageContext"))
            WebPageDataProviderUtil.SetPageContextProperties(requestContext, pageSource, create);
          return pageSource;
        }
      }
      else
      {
        WebPageDataProviderPageSource pageSource1 = requestContext.GetService<IExtensionDataProviderService>().GetRequestDataProviderContextProperty<WebPageDataProviderPageSource>(requestContext, "pageSource");
        if (pageSource1 != null)
        {
          object obj2;
          if (requestContext.RootContext.Items.TryGetValue("DataProviderQuery.Scope", out obj2) && obj2 is IDataProviderScope dataProviderScope && string.Equals(dataProviderScope.Name, "project", StringComparison.OrdinalIgnoreCase))
          {
            if (pageSource1 == null)
              pageSource1 = new WebPageDataProviderPageSource();
            if (pageSource1.Project == null)
              pageSource1.Project = new ContextIdentifier();
            pageSource1.Project.Id = Guid.Parse(dataProviderScope.Value);
            pageSource1.Project.Name = dataProviderScope.GetProperty("projectName") as string;
          }
          requestContext.RootContext.Items["WebPlatform.WebPageSource"] = (object) pageSource1;
          requestContext.RootContext.Items["WebPlatform.WebPageSource.WithPageContext"] = (object) true;
          return pageSource1;
        }
        WebContext webContext = WebContextFactory.GetWebContext(requestContext, create);
        if (!create && webContext == null)
          return (WebPageDataProviderPageSource) null;
        WebPageDataProviderPageSource providerPageSource = new WebPageDataProviderPageSource();
        providerPageSource.Project = webContext.ProjectContext;
        providerPageSource.Navigation = webContext.NavigationContext;
        ContextIdentifier contextIdentifier;
        if (webContext.TeamContext != null)
          contextIdentifier = new ContextIdentifier()
          {
            Id = webContext.TeamContext.Id,
            Name = webContext.TeamContext.Name
          };
        else
          contextIdentifier = (ContextIdentifier) null;
        providerPageSource.Team = contextIdentifier;
        providerPageSource.Url = webContext.RequestContext.HttpContext.Request.Url.AbsoluteUri;
        providerPageSource.Diagnostics = new DiagnosticsContext()
        {
          BundlingEnabled = webContext.Diagnostics.BundlingEnabled,
          DebugMode = webContext.Diagnostics.DebugMode,
          TracePointCollectionEnabled = webContext.Diagnostics.TracePointCollectionEnabled,
          DiagnoseBundles = webContext.Diagnostics.DiagnoseBundles
        };
        providerPageSource.Globalization = new WebPageGlobalizationContext()
        {
          Theme = webContext.Globalization.Theme,
          Culture = webContext.Globalization.Culture
        };
        WebPageDataProviderPageSource pageSource2 = providerPageSource;
        WebPageDataProviderUtil.SetPageContextProperties(requestContext, pageSource2, create);
        requestContext.RootContext.Items["WebPlatform.WebPageSource"] = (object) pageSource2;
        return pageSource2;
      }
    }

    public static void SetPageSource(
      IVssRequestContext requestContext,
      WebPageDataProviderPageSource pageSource)
    {
      requestContext.Items["WebPlatform.WebPageSource"] = (object) pageSource;
      requestContext.Items["WebPlatform.WebPageSource.WithPageContext"] = (object) true;
    }

    private static void SetPageContextProperties(
      IVssRequestContext requestContext,
      WebPageDataProviderPageSource pageSource,
      bool force)
    {
      PageContext pageContext = WebContextFactory.GetPageContext(requestContext, false);
      if (pageContext == null || pageContext.HubsContext == null)
        return;
      pageSource.SelectedHubGroupId = pageContext.HubsContext.SelectedHubGroupId;
      pageSource.SelectedHubId = pageContext.HubsContext.SelectedHubId;
      pageSource.ContributionPaths = (IList<string>) pageContext.ModuleLoaderConfig.ContributionPaths.Where<KeyValuePair<string, ContributionPath>>((Func<KeyValuePair<string, ContributionPath>, bool>) (p => p.Value.PathType != ContributionPathType.Resource)).Select<KeyValuePair<string, ContributionPath>, string>((Func<KeyValuePair<string, ContributionPath>, string>) (p => p.Key)).ToArray<string>();
      requestContext.RootContext.Items["WebPlatform.WebPageSource.WithPageContext"] = (object) true;
    }

    public static DataProviderResult ResolveWebDataProviders(
      PageContext pageContext,
      IEnumerable<Contribution> dataProviderContributions)
    {
      IVssRequestContext requestContext = pageContext.WebContext.TfsRequestContext;
      dataProviderContributions = dataProviderContributions.Where<Contribution>((Func<Contribution, bool>) (contribution =>
      {
        WebDataProviderResolution property1 = contribution.GetProperty<WebDataProviderResolution>("resolution");
        if (pageContext.WebContext.IsHosted)
        {
          string property2 = contribution.GetProperty<string>("serviceInstanceType");
          Guid result;
          if (!string.IsNullOrEmpty(property2) && Guid.TryParse(property2, out result) && result != Guid.Empty && result != pageContext.ServiceInstanceId && property1 != WebDataProviderResolution.Server)
          {
            pageContext.ServiceLocations.Add(result, requestContext.ServiceHost.HostType);
            return false;
          }
        }
        return property1 != WebDataProviderResolution.Client;
      }));
      if (!dataProviderContributions.Any<Contribution>())
        return (DataProviderResult) null;
      DataProviderContext providerContext = new DataProviderContext()
      {
        Properties = new Dictionary<string, object>()
      };
      providerContext.Properties.Add("pageSource", (object) WebPageDataProviderUtil.GetPageSource(requestContext));
      if (pageContext.Diagnostics.TracePointCollectionEnabled)
        providerContext.Properties.Add("includePerformanceTimings", (object) true);
      IDataProviderScope providerScopeForRoute = PublicAccessHelpers.GetDataProviderScopeForRoute(requestContext);
      return requestContext.GetService<IExtensionDataProviderService>().GetDataProviderData(requestContext, providerContext, dataProviderContributions, userFriendlySerialization: true, scope: providerScopeForRoute);
    }
  }
}
