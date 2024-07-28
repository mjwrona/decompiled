// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.ContributionManagementService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  internal class ContributionManagementService : 
    IContentService,
    IVssFrameworkService,
    IContributionNavigationService,
    IContributionRoutingService,
    IContributionManagementService
  {
    private const string c_additionalContentRequestKey = "contentService.additionalContent";
    private const string ContributionDataToken = "crm.contribution";
    private const string SourcePageDataKey = "crm.sourcePage";
    private const int DefaultRequestHandlerOrder = 1000;
    private static HashSet<string> s_handlerTypes = new HashSet<string>((IEnumerable<string>) new string[1]
    {
      "ms.vss-web.request-handler"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static HashSet<string> s_routeTypes = new HashSet<string>((IEnumerable<string>) new string[1]
    {
      "ms.vss-web.route"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static HashSet<string> s_navigationTypes = new HashSet<string>((IEnumerable<string>) new string[5]
    {
      "ms.vss-web.navigation",
      "ms.vss-web.hub",
      "ms.vss-web.hub-group",
      "ms.vss-web.hub-groups-collection",
      "ms.vss-web.page"
    }, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private static char[] s_commaSeparator = new char[1]
    {
      ','
    };
    private static char[] s_dotSeparator = new char[1]
    {
      '.'
    };
    private static JsonSerializer s_serializer = new VssJsonMediaTypeFormatter(true, true, true).CreateJsonSerializer();
    private Dictionary<string, IRouteParameterResolver> m_parameterResolvers = new Dictionary<string, IRouteParameterResolver>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private List<IRouteParametersFilter> m_parameterFilters = new List<IRouteParametersFilter>();
    private Dictionary<string, Type> m_requestHandlerTypes = new Dictionary<string, Type>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private IDisabledContributionConfigurationHelper disabledContributionConfigurationHelper;
    private static ContributedSite s_defaultSite = new ContributedSite();
    private static RegistryQuery s_siteRegistryQuery = new RegistryQuery("/Configuration/WebAccess/SiteId");
    private const string c_webSdkObjectKey = "webSdkObject";
    private Dictionary<string, IContributionTransformer> m_contributionTransformers = new Dictionary<string, IContributionTransformer>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
    private List<IContributedRouteConstraintProvider> m_contributedRouteConstraintProviders = new List<IContributedRouteConstraintProvider>();
    private Dictionary<string, ContributedSite> m_contributedSites;
    private ILockName m_cacheLockName;
    private bool m_siteDefinitionInitialized;

    public string GetRequestContentType(IVssRequestContext requestContext)
    {
      IWebDiagnosticsService service = requestContext.GetService<IWebDiagnosticsService>();
      string requestContentType = string.Empty;
      if (this.IsBundlingEnabledForRequest(requestContext))
        requestContentType = service.IsDebugContentEnabled(requestContext) ? ".bundle" : ".min.bundle";
      return requestContentType;
    }

    public bool IsBundlingEnabledForRequest(IVssRequestContext requestContext)
    {
      IWebDiagnosticsService service = requestContext.GetService<IWebDiagnosticsService>();
      return !service.SupportsESNextScripts(requestContext) || service.IsBundlingEnabled(requestContext);
    }

    public string GetRequestScriptType(IVssRequestContext requestContext)
    {
      IWebDiagnosticsService service = requestContext.GetService<IWebDiagnosticsService>();
      string str1;
      requestContext.GetSessionValue("SCRIPTTYPE", out str1);
      string b;
      requestContext.GetSessionValue("AUTOSCRIPTFALLBACK", out b);
      if (string.Equals("true", b, StringComparison.OrdinalIgnoreCase))
        requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "ContentService", nameof (GetRequestScriptType), new CustomerIntelligenceData());
      if (!this.IsBundlingEnabledForRequest(requestContext))
        str1 = "ES2017";
      IVssRequestContext requestContext1 = requestContext;
      if (!service.SupportsESNextScripts(requestContext1))
        str1 = "javascript";
      string str2 = requestContext.IsFeatureEnabled("VisualStudio.Services.WebPlatform.UseESNextScripts") ? "ES2017" : "javascript";
      return !string.IsNullOrEmpty(str1) ? str1 : str2;
    }

    public string GetRequestStyleType(IVssRequestContext requestContext)
    {
      string str;
      requestContext.GetSessionValue("STYLETYPE", out str);
      return !string.IsNullOrEmpty(str) ? str : "stylesheet";
    }

    public IEnumerable<ContributedContent> QueryContent(IVssRequestContext requestContext)
    {
      IContributionManagementService service = requestContext.GetService<IContributionManagementService>();
      List<ContributedContent> requestedContent = new List<ContributedContent>();
      IEnumerable<ContributionNode> contributionsByType = service.GetContributionsByType(requestContext, "ms.vss-web.content");
      if (contributionsByType != null)
      {
        List<ContributedContent> source = new List<ContributedContent>();
        HashSet<string> uniqueContent = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        foreach (ContributionNode contributionNode in contributionsByType)
        {
          ContributedContent contributedObject = service.GetContributedObject<ContributedContent>(requestContext, contributionNode.Id);
          if (contributedObject != null)
            source.Add(contributedObject);
        }
        List<ContributedContent> additionalRequestContent = this.GetAdditionalRequestContent(requestContext);
        if (additionalRequestContent != null)
          source.AddRange((IEnumerable<ContributedContent>) additionalRequestContent);
        foreach (ContributedContent contributedContent in (IEnumerable<ContributedContent>) source.OrderBy<ContributedContent, LoaderPriority>((Func<ContributedContent, LoaderPriority>) (c => c.LoaderPriority)))
        {
          if (string.IsNullOrEmpty(contributedContent.ContributionId))
            requestedContent.Add(contributedContent);
          else
            this.AddContent(requestContext, contributedContent.ContributionId, requestedContent, service, uniqueContent);
        }
      }
      return (IEnumerable<ContributedContent>) requestedContent;
    }

    public void AddContent(IVssRequestContext requestContext, ContributedContent content) => this.GetAdditionalRequestContent(requestContext, true).Add(content);

    private List<ContributedContent> GetAdditionalRequestContent(
      IVssRequestContext requestContext,
      bool createIfNone = false)
    {
      List<ContributedContent> additionalRequestContent;
      if (!requestContext.Items.TryGetValue<List<ContributedContent>>("contentService.additionalContent", out additionalRequestContent) & createIfNone)
      {
        additionalRequestContent = new List<ContributedContent>();
        requestContext.Items["contentService.additionalContent"] = (object) additionalRequestContent;
      }
      return additionalRequestContent;
    }

    public Dictionary<string, string> QueryContentLocations(
      IVssRequestContext requestContext,
      string contributionId,
      string asset)
    {
      return requestContext.GetService<IContributionService>().QueryAssetLocations(requestContext, contributionId, asset);
    }

    private void AddContent(
      IVssRequestContext requestContext,
      string contributionId,
      List<ContributedContent> requestedContent,
      IContributionManagementService contributionManagementService,
      HashSet<string> uniqueContent)
    {
      ContributedContent contributedObject;
      if (uniqueContent.Contains(contributionId) || (contributedObject = contributionManagementService.GetContributedObject<ContributedContent>(requestContext, contributionId)) == null)
        return;
      foreach (ContributionNode contributionNode in contributionManagementService.GetContribution(requestContext, contributionId).GetChildrenOfType("ms.vss-web.content"))
        this.AddContent(requestContext, contributionNode.Id, requestedContent, contributionManagementService, uniqueContent);
      requestedContent.Add(contributedObject);
      uniqueContent.Add(contributionId);
    }

    public List<ContributedNavigation> GetSelectedElements(IVssRequestContext requestContext) => this.GetSelectedElementsByType(requestContext, (string) null);

    public List<ContributedNavigation> GetSelectedElementsByType(
      IVssRequestContext requestContext,
      string contributionType)
    {
      ContributionManagementService.ContributionRequestData requestData = this.GetRequestData(requestContext);
      List<ContributedNavigation> selectedElementsByType = (List<ContributedNavigation>) null;
      if (requestData != null && requestData.PrimaryContributions != null)
      {
        selectedElementsByType = new List<ContributedNavigation>();
        foreach (Contribution primaryContribution in (IEnumerable<Contribution>) requestData.PrimaryContributions)
        {
          if (primaryContribution.Type != null && requestData.Site.NavigationElementTypes.Contains(primaryContribution.Type) && (contributionType == null || primaryContribution.Type.Equals(contributionType, StringComparison.OrdinalIgnoreCase)))
          {
            ContributedNavigation contributedObject = this.GetContributedObject<ContributedNavigation>(requestContext, primaryContribution.Id);
            if (contributedObject != null)
              selectedElementsByType.Add(contributedObject);
          }
        }
      }
      return selectedElementsByType;
    }

    public ContributedNavigation GetSelectedElementByType(
      IVssRequestContext requestContext,
      string contributionType)
    {
      IEnumerable<ContributedNavigation> selectedElementsByType = (IEnumerable<ContributedNavigation>) this.GetSelectedElementsByType(requestContext, contributionType);
      return selectedElementsByType == null ? (ContributedNavigation) null : selectedElementsByType.FirstOrDefault<ContributedNavigation>();
    }

    public IEnumerable<Contribution> GetPrimaryContributions(IVssRequestContext requestContext) => (IEnumerable<Contribution>) this.GetRequestData(requestContext).PrimaryContributions;

    public string GetPageTitle(IVssRequestContext requestContext)
    {
      ContributionManagementService.ContributionRequestData requestData = this.GetRequestData(requestContext);
      string pageTitle = requestData.PageTitle;
      if (string.IsNullOrEmpty(pageTitle) && requestData.PrimaryContributions != null)
      {
        for (int index = requestData.PrimaryContributions.Count - 1; index >= 0; --index)
        {
          ContributedNavigation contributedObject = this.GetContributedObject<ContributedNavigation>(requestContext, requestData.PrimaryContributions[index].Id);
          if (contributedObject != null && !string.IsNullOrEmpty(contributedObject.Name))
          {
            pageTitle = contributedObject.Name;
            break;
          }
        }
      }
      return pageTitle;
    }

    public void SetPageTitle(IVssRequestContext requestContext, string title) => this.GetRequestData(requestContext).PageTitle = title;

    public bool SupportsMobile(IVssRequestContext requestContext)
    {
      foreach (ContributedNavigation selectedElement in this.GetSelectedElements(requestContext))
      {
        if (selectedElement.SupportsMobile)
          return true;
      }
      return false;
    }

    public ContributionManagementService()
      : this(DisabledContributionConfigurationHelper.Instance)
    {
    }

    public ContributionManagementService(
      IDisabledContributionConfigurationHelper disabledContributionConfigurationHelper)
    {
      this.disabledContributionConfigurationHelper = disabledContributionConfigurationHelper;
    }

    public RouteCollection GetContributedRoutes(IVssRequestContext requestContext)
    {
      ContributionManagementService.ContributionRouteData routeData = this.GetRouteData(requestContext);
      if (routeData.RouteCollection == null)
      {
        this.EnsureSiteDefinitionLoaded(requestContext);
        IContributionServiceWithData service = requestContext.GetService<IContributionServiceWithData>();
        IEnumerable<Contribution> contributions;
        if (!service.QueryContributionsForType<RouteCollection>(requestContext, "ms.vss-web.route", "crm.routeCollection", out contributions, out routeData.RouteCollection))
        {
          List<Tuple<double, string, RouteBase>> tupleList = new List<Tuple<double, string, RouteBase>>();
          foreach (Contribution contribution in contributions)
          {
            RouteBase route = this.CreateRoute(requestContext, contribution);
            if (route != null)
            {
              double property = contribution.GetProperty<double>("order", double.MaxValue);
              tupleList.Add(new Tuple<double, string, RouteBase>(property, contribution.Id, route));
            }
          }
          tupleList.Sort((Comparison<Tuple<double, string, RouteBase>>) ((r1, r2) =>
          {
            if (r1.Item1 == r2.Item1)
              return string.Compare(r1.Item2, r2.Item2, true);
            return r1.Item1 <= r2.Item1 ? -1 : 1;
          }));
          routeData.RouteCollection = new RouteCollection();
          foreach (Tuple<double, string, RouteBase> tuple in tupleList)
            routeData.RouteCollection.Add(tuple.Item2, tuple.Item3);
          service.Set(requestContext, "crm.routeCollection", contributions, (object) routeData.RouteCollection);
        }
      }
      return routeData.RouteCollection;
    }

    public IEnumerable<ContributedRequestHandler<T>> GetRequestHandlers<T>(
      IVssRequestContext requestContext)
      where T : IContributedRequestHandler
    {
      return this.GetRequestData(requestContext).RequestHandlers.Where<ContributedRequestHandler<IContributedRequestHandler>>((Func<ContributedRequestHandler<IContributedRequestHandler>, bool>) (h => h.Handler is T)).Select<ContributedRequestHandler<IContributedRequestHandler>, ContributedRequestHandler<T>>((Func<ContributedRequestHandler<IContributedRequestHandler>, ContributedRequestHandler<T>>) (h => new ContributedRequestHandler<T>(h.Id, (T) h.Handler, h.Properties, h.Order)));
    }

    public string RouteUrl(
      IVssRequestContext requestContext,
      string contributionId,
      RouteValueDictionary routeValues = null,
      bool includeImplicitValues = false,
      bool allowFallbackRoute = true)
    {
      RequestContext requestContext1 = requestContext.WebRequestContextInternal().HttpContext.Request.RequestContext;
      string str1 = (string) null;
      if (requestContext1 != null)
      {
        IContributionService service = requestContext.GetService<IContributionService>();
        ContributionManagementService.ContributionRouteData routeData = this.GetRouteData(requestContext);
        IVssRequestContext requestContext2 = requestContext;
        string contributionId1 = contributionId;
        Contribution routeContribution = service.QueryContribution(requestContext2, contributionId1);
        RouteCollection contributedRoutes = this.GetContributedRoutes(requestContext);
        if (routeContribution != null && contributedRoutes[contributionId] is IParameterizedRoute parameterizedRoute)
        {
          RouteValueDictionary routeValueDictionary = routeValues != null ? new RouteValueDictionary((IDictionary<string, object>) routeValues) : new RouteValueDictionary();
          if (parameterizedRoute.RouteParameters != null)
          {
            foreach (RouteParameter routeParameter in parameterizedRoute.RouteParameters.Values)
            {
              if (!routeValueDictionary.ContainsKey(routeParameter.ParameterString))
              {
                object parameterValue;
                if (routeData != null && routeData.ResolvedRouteValues.TryGetValue(routeParameter.ParameterString, out parameterValue))
                {
                  routeValueDictionary[routeParameter.ParameterName] = parameterValue;
                }
                else
                {
                  IRouteParameterResolver parameterResolver;
                  if (!requestContext1.RouteData.Values.ContainsKey(routeParameter.ParameterName) && routeParameter.ParameterNamespace != null && this.m_parameterResolvers.TryGetValue(routeParameter.ParameterNamespace, out parameterResolver))
                  {
                    bool flag = parameterResolver.Resolve(requestContext, routeContribution, routeParameter.ParameterName, out parameterValue);
                    if (parameterValue != RouteParameterValue.Unavailable)
                    {
                      if (flag && routeData != null)
                        routeData.ResolvedRouteValues[routeParameter.ParameterString] = parameterValue;
                      routeValueDictionary[routeParameter.ParameterName] = parameterValue;
                    }
                  }
                }
              }
            }
          }
          if (this.m_parameterFilters.Any<IRouteParametersFilter>())
          {
            foreach (IRouteParametersFilter parameterFilter in this.m_parameterFilters)
              parameterFilter.Filter(requestContext, routeContribution, routeValueDictionary);
          }
          RouteBase route = routeData.RouteCollection[contributionId];
          if (route != null)
          {
            VirtualPathData virtualPath1 = route.GetVirtualPath(requestContext1, routeValueDictionary);
            if (virtualPath1 != null)
            {
              string virtualPath2 = requestContext.VirtualPath();
              string str2 = string.IsNullOrEmpty(virtualPath2) ? "/" : VirtualPathUtility.AppendTrailingSlash(virtualPath2);
              return !string.IsNullOrEmpty(virtualPath1.VirtualPath) ? str2 + virtualPath1.VirtualPath : str2;
            }
          }
          return (string) null;
        }
        if (allowFallbackRoute)
        {
          ContributionManagementService.ContributionRequestData requestData = this.GetRequestData(requestContext);
          if (!string.IsNullOrEmpty(requestData.Site.FallbackRoute) && !requestData.Site.FallbackRoute.Equals(contributionId))
            str1 = this.RouteUrl(requestContext, requestData.Site.FallbackRoute, new RouteValueDictionary()
            {
              {
                "parameters",
                (object) contributionId
              }
            }, false, true);
        }
      }
      return str1;
    }

    public string RouteUrl(
      IVssRequestContext requestContext,
      string routeName,
      string actionName,
      string controllerName,
      RouteValueDictionary routeValues = null,
      bool includeImplicitValues = false)
    {
      RequestContext requestContext1 = requestContext.WebRequestContextInternal().HttpContext.Request.RequestContext;
      string str = (string) null;
      if (requestContext1 != null)
        str = UrlHelper.GenerateUrl(routeName, actionName, controllerName, routeValues, RouteTable.Routes, requestContext1, includeImplicitValues) ?? UrlHelper.GenerateUrl((string) null, actionName, controllerName, routeValues, RouteTable.Routes, requestContext1, includeImplicitValues);
      return str;
    }

    public string GetUrl(IVssRequestContext requestContext) => this.GetUrl(requestContext, out bool _);

    public string GetUrl(IVssRequestContext requestContext, out bool isOriginalUrl)
    {
      string url = this.GetRequestData(requestContext).ResolvedUrl;
      isOriginalUrl = false;
      if (string.IsNullOrEmpty(url))
      {
        isOriginalUrl = true;
        DataProviderSourcePageData providerSourcePageData = this.GetDataProviderSourcePageData(requestContext);
        url = providerSourcePageData == null || string.IsNullOrEmpty(providerSourcePageData.Url) ? requestContext.WebRequestContextInternal().HttpContext.Request.Url.PathAndQuery : providerSourcePageData.Url;
      }
      return url;
    }

    public IContributedRoute GetRoute(IVssRequestContext requestContext)
    {
      string name = this.GetRequestData(requestContext).ResolvedRouteId;
      if (name == null)
      {
        DataProviderSourcePageData providerSourcePageData = this.GetDataProviderSourcePageData(requestContext);
        if (providerSourcePageData != null && providerSourcePageData.RouteId != null)
        {
          name = providerSourcePageData.RouteId;
        }
        else
        {
          ContributionManagementService.ContributionRouteData routeData = this.GetRouteData(requestContext);
          if (routeData != null && routeData.RoutedContribution != null)
            name = routeData.RoutedContribution.Id;
        }
      }
      IContributedRoute route = (IContributedRoute) null;
      if (name != null)
        route = this.GetContributedRoutes(requestContext)[name] as IContributedRoute;
      return route;
    }

    public IDictionary<string, object> GetRouteValues(IVssRequestContext requestContext)
    {
      IDictionary<string, object> routeValues = this.GetRequestData(requestContext).ResolvedRouteValues;
      if (routeValues == null)
      {
        DataProviderSourcePageData providerSourcePageData = this.GetDataProviderSourcePageData(requestContext);
        if (providerSourcePageData != null && providerSourcePageData.RouteValues != null)
        {
          routeValues = providerSourcePageData.RouteValues;
        }
        else
        {
          ContributionManagementService.ContributionRouteData routeData = this.GetRouteData(requestContext);
          if (routeData.RouteData != null)
          {
            routeValues = (IDictionary<string, object>) routeData.RouteData.Values;
          }
          else
          {
            IWebRequestContextInternal requestContextInternal = requestContext.WebRequestContextInternal(false);
            if (requestContextInternal != null)
              routeValues = (IDictionary<string, object>) requestContextInternal.HttpContext.Request.RequestContext.RouteData?.Values;
          }
        }
      }
      return routeValues;
    }

    public T GetRouteValue<T>(IVssRequestContext requestContext, string routeParameter)
    {
      routeValue = default (T);
      if (!string.IsNullOrEmpty(routeParameter))
      {
        IDictionary<string, object> routeValues = this.GetRouteValues(requestContext);
        object obj;
        if (routeValues == null || !routeValues.TryGetValue(routeParameter, out obj) || !(obj is T routeValue))
          ;
      }
      return routeValue;
    }

    public NameValueCollection GetQueryParameters(IVssRequestContext requestContext)
    {
      string queryString = this.GetQueryString(requestContext);
      NameValueCollection queryParameters = (NameValueCollection) null;
      if (!string.IsNullOrEmpty(queryString))
        queryParameters = HttpUtility.ParseQueryString(queryString);
      return queryParameters;
    }

    public string GetQueryParameter(IVssRequestContext requestContext, string paramName) => this.GetQueryParameters(requestContext)?.Get(paramName);

    public void SetQueryParameter(
      IVssRequestContext requestContext,
      string paramName,
      string paramValue)
    {
      NameValueCollection queryParameters = this.GetQueryParameters(requestContext) ?? new NameValueCollection();
      string str = this.GetUrl(requestContext);
      int length = str.IndexOf("?");
      if (length > -1 && str.Length > length)
        str = str.Substring(0, length);
      queryParameters.Set(paramName, paramValue);
      string url = str + "?" + this.BuildQueryString(queryParameters);
      this.SetUrl(requestContext, url);
    }

    public void RemoveQueryStringParameter(IVssRequestContext requestContext, string paramName)
    {
      NameValueCollection queryParameters = this.GetQueryParameters(requestContext);
      string url1 = this.GetUrl(requestContext);
      string url2 = url1;
      int length = url1.IndexOf("?");
      if (length > -1 && url1.Length > length + 1)
        url2 = url1.Substring(0, length);
      if (queryParameters == null || queryParameters[paramName] == null)
        return;
      queryParameters.Remove(paramName);
      if (queryParameters.Count > 0)
        url2 = url2 + "?" + this.BuildQueryString(queryParameters);
      this.SetUrl(requestContext, url2);
    }

    public void SetUrl(IVssRequestContext requestContext, string url) => this.GetRequestData(requestContext).ResolvedUrl = url;

    public void SetRoute(
      IVssRequestContext requestContext,
      string routeContributionId,
      RouteValueDictionary routeValues,
      bool preserveQueryParameters = false,
      bool mergeRouteValues = true)
    {
      ContributionManagementService.ContributionRequestData requestData = this.GetRequestData(requestContext);
      requestData.ResolvedRouteId = routeContributionId;
      if (this.GetContributedRoutes(requestContext)[routeContributionId] != null)
      {
        string str1 = this.RouteUrl(requestContext, routeContributionId, routeValues, false, true);
        string str2 = string.Empty;
        if (preserveQueryParameters)
          str2 = this.GetQueryString(requestContext);
        requestData.ResolvedUrl = string.IsNullOrEmpty(str2) ? str1 : str1 + "?" + str2;
      }
      if (routeValues == null)
        return;
      requestData.ResolvedRouteValues = (IDictionary<string, object>) routeValues;
      if (!mergeRouteValues)
        return;
      ContributionManagementService.ContributionRouteData routeData = this.GetRouteData(requestContext);
      if (routeData.RouteData == null)
        return;
      foreach (KeyValuePair<string, object> keyValuePair in routeData.RouteData.Values)
      {
        if (!routeValues.ContainsKey(keyValuePair.Key))
          routeValues[keyValuePair.Key] = keyValuePair.Value;
      }
    }

    public Contribution GetRoutedContribution(IVssRequestContext requestContext) => this.GetRouteData(requestContext).RoutedContribution;

    public void SetRequestRoute(IVssRequestContext requestContext, RouteData routeData)
    {
      ContributionManagementService.ContributionRequestData requestData = this.GetRequestData(requestContext);
      ContributionManagementService.ContributionRouteData routeData1 = this.GetRouteData(requestContext);
      routeData1.RouteData = routeData;
      if (routeData1.RouteData == null || routeData1.RouteData.DataTokens == null || !routeData1.RouteData.DataTokens.ContainsKey("crm.contribution") || !(routeData1.RouteData.DataTokens["crm.contribution"] is string dataToken))
        return;
      IContributionService service = requestContext.GetService<IContributionService>();
      routeData1.RoutedContribution = service.QueryContribution(requestContext, dataToken);
      requestData.RequestCallback = new Action<IVssRequestContext, ContributionManagementService.ContributionRequestData>(this.ProcessContributionRequest);
    }

    public string GetRequestedTemplateName(IVssRequestContext requestContext)
    {
      ContributionManagementService.ContributionRequestData requestData = this.GetRequestData(requestContext);
      if (requestData.RequestedTemplateName == null)
      {
        requestData.RequestedTemplateName = this.GetQueryParameter(requestContext, "__rt");
        if (requestData.RequestedTemplateName == null)
          requestData.RequestedTemplateName = string.Empty;
        else
          this.RemoveQueryStringParameter(requestContext, "__rt");
      }
      return requestData.RequestedTemplateName;
    }

    public int GetRequestedTemplateVersion(IVssRequestContext requestContext)
    {
      ContributionManagementService.ContributionRequestData requestData = this.GetRequestData(requestContext);
      if (!requestData.RequestedTemplateVersion.HasValue)
      {
        requestData.RequestedTemplateVersion = new int?(0);
        string s = this.GetQueryParameter(requestContext, "__ver") ?? string.Empty;
        int result;
        if (!string.IsNullOrEmpty(s) && int.TryParse(s, out result))
        {
          requestData.RequestedTemplateVersion = new int?(result);
          this.RemoveQueryStringParameter(requestContext, "__ver");
        }
      }
      return requestData.RequestedTemplateVersion.Value;
    }

    public void SetRequestedTemplate(
      IVssRequestContext requestContext,
      string templateName,
      int version = 0)
    {
      ContributionManagementService.ContributionRequestData requestData = this.GetRequestData(requestContext);
      requestData.RequestedTemplateName = templateName;
      requestData.RequestedTemplateVersion = new int?(version);
    }

    private string GetQueryString(IVssRequestContext requestContext)
    {
      string url = this.GetUrl(requestContext);
      string queryString = string.Empty;
      int num = url.IndexOf("?");
      if (num > -1 && url.Length > num + 1)
        queryString = url.Substring(num + 1);
      return queryString;
    }

    private DataProviderSourcePageData GetDataProviderSourcePageData(
      IVssRequestContext requestContext)
    {
      object obj;
      DataProviderSourcePageData providerSourcePageData;
      if (requestContext.RootContext.Items.TryGetValue("crm.sourcePage", out obj))
      {
        providerSourcePageData = obj as DataProviderSourcePageData;
      }
      else
      {
        providerSourcePageData = requestContext.GetService<IExtensionDataProviderService>().GetRequestDataProviderContextProperty<DataProviderSourcePageData>(requestContext, "sourcePage");
        if (providerSourcePageData != null)
          requestContext.RootContext.Items["crm.sourcePage"] = (object) providerSourcePageData;
      }
      return providerSourcePageData;
    }

    private RouteBase CreateRoute(IVssRequestContext requestContext, Contribution routeContribution)
    {
      Dictionary<string, object> dictionary1 = (Dictionary<string, object>) null;
      TeamFoundationHostType result = TeamFoundationHostType.Unknown;
      RouteBase route = (RouteBase) null;
      try
      {
        string[] strArray;
        if (routeContribution.Properties.TryGetValue<string[]>("routeTemplates", out strArray))
        {
          if (strArray != null)
          {
            string str1;
            if (routeContribution.Properties.TryGetValue<string>("hostType", out str1) && !Enum.TryParse<TeamFoundationHostType>(str1, out result))
              result = TeamFoundationHostType.Unknown;
            RouteValueDictionary dataTokens = new RouteValueDictionary();
            dataTokens["crm.contribution"] = (object) routeContribution.Id;
            Dictionary<string, object> dictionary2;
            RouteValueDictionary defaults = !routeContribution.Properties.TryGetValue<Dictionary<string, object>>("defaults", out dictionary2) ? new RouteValueDictionary() : new RouteValueDictionary((IDictionary<string, object>) dictionary2);
            if (!defaults.ContainsKey("controller"))
              defaults["controller"] = (object) "ContributedPage";
            if (!defaults.ContainsKey("action"))
              defaults["action"] = (object) "Execute";
            routeContribution.Properties.TryGetValue<Dictionary<string, object>>("constraints", out dictionary1);
            RouteValueDictionary routeConstraints1 = dictionary1 != null ? new RouteValueDictionary((IDictionary<string, object>) dictionary1) : new RouteValueDictionary();
            if (result != TeamFoundationHostType.Unknown)
            {
              TfsApiHostTypeConstraint hostTypeConstraint = new TfsApiHostTypeConstraint(result);
              routeConstraints1["TFS_HostType"] = (object) hostTypeConstraint;
            }
            if (strArray.Length == 1)
            {
              Dictionary<string, RouteParameter> routeParameters;
              string str2 = RouteTemplate.Parse(strArray[0], out routeParameters);
              RouteValueDictionary routeConstraints2 = this.AddRouteConstraints(requestContext, str2, routeConstraints1, routeContribution);
              route = (RouteBase) new ContributedRoute(routeContribution.Id, result, str2, routeParameters, defaults, routeConstraints2, dataTokens);
            }
            else
            {
              List<ContributedRoute> routes = new List<ContributedRoute>();
              foreach (string routeTemplate in strArray)
              {
                RouteValueDictionary routeConstraints3 = new RouteValueDictionary((IDictionary<string, object>) routeConstraints1);
                Dictionary<string, RouteParameter> parameters;
                ref Dictionary<string, RouteParameter> local = ref parameters;
                string str3 = RouteTemplate.Parse(routeTemplate, out local);
                RouteValueDictionary routeConstraints4 = this.AddRouteConstraints(requestContext, str3, routeConstraints3, routeContribution);
                routes.Add(new ContributedRoute(routeContribution.Id, result, str3, parameters, defaults, routeConstraints4, dataTokens));
              }
              route = (RouteBase) new AggregatedRoute(routeContribution.Id, (IEnumerable<ContributedRoute>) routes);
            }
          }
        }
      }
      catch (Exception ex)
      {
      }
      return route;
    }

    private RouteValueDictionary AddRouteConstraints(
      IVssRequestContext requestContext,
      string compiledTemplate,
      RouteValueDictionary routeConstraints,
      Contribution routeContribution)
    {
      if (this.m_contributedRouteConstraintProviders != null)
      {
        foreach (IContributedRouteConstraintProvider constraintProvider in this.m_contributedRouteConstraintProviders)
        {
          RouteValueDictionary routeValueDictionary = constraintProvider.QueryConstraints(requestContext, compiledTemplate);
          if (routeValueDictionary != null)
          {
            foreach (string key in routeValueDictionary.Keys)
              routeConstraints[key] = routeValueDictionary[key];
          }
        }
      }
      routeConstraints["contributedConstraint"] = (object) new ContributionManagementService.ContributedConstraint(routeContribution);
      return routeConstraints;
    }

    public string GetCommandName(IVssRequestContext requestContext)
    {
      ContributionManagementService.ContributionRouteData routeData = this.GetRouteData(requestContext);
      string commandName = (string) null;
      if (routeData != null && routeData.RoutedContribution != null)
        commandName = routeData.RoutedContribution.GetProperty<string>("commandName", routeData.RoutedContribution.Id);
      return commandName;
    }

    private void ProcessContributionRequest(
      IVssRequestContext requestContext,
      ContributionManagementService.ContributionRequestData contributionRequestData)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "Routing.ProcessContributionRequest"))
      {
        IContributionFilterService service1 = requestContext.To(TeamFoundationHostType.Deployment).GetService<IContributionFilterService>();
        ContributionManagementService.ContributionRouteData contributionRouteData = this.GetRouteData(requestContext);
        IContributionService service2 = requestContext.GetService<IContributionService>();
        Contribution contribution1 = contributionRouteData.RoutedContribution;
        if (contributionRouteData.RoutedContribution == null)
          return;
        if (!service1.ApplyConstraints(requestContext, contributionRouteData.RoutedContribution, (string) null, string.Empty, (ICollection<EvaluatedCondition>) null))
        {
          requestContext.Trace(100136420, TraceLevel.Info, "WebPlatform", WebPlatformTraceLayers.Routing, "Primary route {0} not matched after full constraints applied.", (object) contributionRouteData.RoutedContribution.Id);
          throw new HttpException(404, WebFrameworkResources.PageNotFound());
        }
        Contribution contribution2;
        for (; contribution1 != null; contribution1 = contribution2)
        {
          IEnumerable<Contribution> contributions = service2.QueryContributionsForChild(requestContext, contribution1.Id);
          contribution2 = (Contribution) null;
          int num1 = -1;
          contributionRequestData.PrimaryContributions.Insert(0, contribution1);
          IEnumerable<ContributedRequestHandler<IPreRequestContributedRequestHandler>> requestHandlers = this.GetRequestHandlers<IPreRequestContributedRequestHandler>(requestContext, contribution1);
          if (requestHandlers != null)
          {
            foreach (ContributedRequestHandler<IPreRequestContributedRequestHandler> contributedRequestHandler in requestHandlers)
              contributedRequestHandler.Handler.OnPreRequest(requestContext, contributedRequestHandler.Properties);
          }
          if (contributions != null)
          {
            string[] strArray1 = contribution1.Id.Split(ContributionManagementService.s_dotSeparator, StringSplitOptions.RemoveEmptyEntries);
            foreach (Contribution contribution3 in contributions)
            {
              int num2 = 0;
              if (service1.ApplyConstraints(requestContext, contribution3, "Routing", contribution1.Id, (ICollection<EvaluatedCondition>) null) && service1.ApplyConstraints(requestContext, contribution1, "Routing", contribution3.Id, (ICollection<EvaluatedCondition>) null))
              {
                if ("ms.vss-web.site".Equals(contribution3.Type, StringComparison.OrdinalIgnoreCase))
                  num2 += 4;
                if (ContributionManagementService.s_navigationTypes.Contains(contribution3.Type))
                  num2 += 2;
                string[] strArray2 = contribution3.Id.Split(ContributionManagementService.s_dotSeparator, StringSplitOptions.RemoveEmptyEntries);
                for (int index = 0; index < strArray1.Length && index < strArray2.Length; ++index)
                {
                  if (string.Equals(strArray1[index], strArray2[index], StringComparison.OrdinalIgnoreCase))
                    ++num2;
                }
                if (num2 > num1)
                {
                  contribution2 = contribution3;
                  num1 = num2;
                }
              }
            }
          }
        }
        if (!this.m_contributedSites.TryGetValue(contributionRequestData.PrimaryContributions[0].Id, out contributionRequestData.Site))
        {
          requestContext.Trace(100136421, TraceLevel.Info, "WebPlatform", WebPlatformTraceLayers.Routing, "Route {0} didn't result in determining a site.", (object) contributionRequestData.PrimaryContributions[0].Id);
          throw new HttpException(404, WebFrameworkResources.PageNotFound());
        }
        if (contributionRequestData.PrimaryContributions.Count > 0)
        {
          Dictionary<string, bool> includedContributions = new Dictionary<string, bool>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          int num = -1;
          int primaryIndex = contributionRequestData.PrimaryContributions.Count - 1;
          while (primaryIndex > num)
          {
            IEnumerable<string> strings;
            if ("ms.vss-web.route".Equals(contributionRequestData.PrimaryContributions[primaryIndex].Type, StringComparison.OrdinalIgnoreCase) && (strings = this.ExecuteNavigationHandler(requestContext, contributionRequestData.PrimaryContributions[primaryIndex])) != null)
              num = --primaryIndex;
            else if (contributionRequestData.Site.NavigationElementTypes.Contains(contributionRequestData.PrimaryContributions[primaryIndex].Type))
            {
              strings = this.ExecuteNavigationHandler(requestContext, contributionRequestData.PrimaryContributions[primaryIndex]);
              if (strings == null)
              {
                string property = contributionRequestData.PrimaryContributions[primaryIndex].GetProperty<string>("defaultNavigation");
                if (!string.IsNullOrEmpty(property))
                  strings = (IEnumerable<string>) property.Split(ContributionManagementService.s_commaSeparator, StringSplitOptions.RemoveEmptyEntries);
              }
              num = primaryIndex;
            }
            else
            {
              primaryIndex--;
              continue;
            }
            if (strings != null)
            {
              foreach (string contributionId in strings)
              {
                Contribution contribution4 = service2.QueryContribution(requestContext, contributionId);
                if (contribution4 != null)
                  contributionRequestData.PrimaryContributions.Insert(++primaryIndex, contribution4);
              }
            }
          }
          IEnumerable<string> primaryContributionIds = contributionRequestData.PrimaryContributions.Select<Contribution, string>((Func<Contribution, string>) (x => x.Id));
          primaryIndex = 0;
          this.ComputeRequestContributions(requestContext, (IEnumerable<string>) new string[1]
          {
            contributionRequestData.PrimaryContributions[0].Id
          }, ContributionQueryOptions.IncludeAll, (ContributionQueryCallback) ((queryContext, contribution, parentContribution, relationship, queryOptions, evaluatedConditions) =>
          {
            ContributionQueryOptions contributionQueryOptions = ContributionQueryOptions.None;
            bool flag = false;
            if (this.EvaluateCondition(primaryIndex < contributionRequestData.PrimaryContributions.Count && primaryContributionIds.Contains<string>(contribution.Id), evaluatedConditions, false, (object) "We include all primary contributions."))
            {
              contributionQueryOptions = ContributionQueryOptions.IncludeSelf | ContributionQueryOptions.IncludeChildren;
              flag = true;
            }
            else if (this.EvaluateCondition("ms.vss-web.route".Equals(contribution.Type), evaluatedConditions, true, (object) "We dont want any route contributions that are not the request routed contribution."))
              contributionQueryOptions = ContributionQueryOptions.None;
            else if (this.EvaluateCondition(parentContribution.Id.Equals(contributionRouteData.RoutedContribution.Id) && "ms.vss-web.request-handler".Equals(contribution.Type, StringComparison.OrdinalIgnoreCase), evaluatedConditions, false, (object) "We only want request handlers where the parent is the routed route contribution."))
              contributionQueryOptions = ContributionQueryOptions.IncludeSelf;
            else if (this.EvaluateCondition(contributionRequestData.Site.NavigationElementTypes.Contains(contribution.Type), evaluatedConditions, false, (object) "Is this contribution a navigation contribution."))
            {
              if (this.EvaluateCondition(contributionRequestData.Site.NavigationElementTypes.Contains(parentContribution.Type), evaluatedConditions, false, (object) "Is your parent a navigation contribution."))
                contributionQueryOptions = ContributionQueryOptions.IncludeSelf | ContributionQueryOptions.IncludeChildren;
            }
            else if (this.EvaluateCondition("ms.vss-web.property-provider".Equals(contribution.Type, StringComparison.OrdinalIgnoreCase), evaluatedConditions, false, (object) "Is this contribution a property provider."))
              contributionQueryOptions = ContributionQueryOptions.IncludeSelf;
            else if (this.EvaluateCondition(includedContributions.TryGetValue(parentContribution.Id, out flag) & flag, evaluatedConditions, false, (object) "We include all other contributions where the parent marked us as include."))
              contributionQueryOptions = ContributionQueryOptions.IncludeSelf | ContributionQueryOptions.IncludeChildren;
            if (queryContext.IsFeatureEnabled("WebPlatform.DisableContributionsAtRuntime") && contributionQueryOptions != ContributionQueryOptions.None && !this.EvaluateCondition(!this.disabledContributionConfigurationHelper.IsContributionDisabled(queryContext, contribution.Id), evaluatedConditions, false, (object) "We exclude any contributions that are disabled at runtime"))
              contributionQueryOptions = ContributionQueryOptions.None;
            if (contributionQueryOptions != ContributionQueryOptions.None)
              includedContributions[contribution.Id] = flag;
            return contributionQueryOptions;
          }));
        }
        List<ContributionNode> contributionNodeList;
        if (!contributionRequestData.ContributionsByType.TryGetValue("ms.vss-web.request-handler", out contributionNodeList))
          return;
        foreach (ContributionNode contributionNode in contributionNodeList)
        {
          string property1 = contributionNode.Contribution.GetProperty<string>("name");
          Type type;
          if (property1 != null && this.m_requestHandlerTypes.TryGetValue(property1, out type))
          {
            int property2 = contributionNode.Contribution.GetProperty<int>("order", 1000);
            IContributedRequestHandler instance = Activator.CreateInstance(type) as IContributedRequestHandler;
            contributionRequestData.RequestHandlers.Add(new ContributedRequestHandler<IContributedRequestHandler>(contributionNode.Id, instance, contributionNode.Contribution.Properties, property2));
          }
        }
        contributionRequestData.RequestHandlers = (IList<ContributedRequestHandler<IContributedRequestHandler>>) contributionRequestData.RequestHandlers.OrderBy<ContributedRequestHandler<IContributedRequestHandler>, int>((Func<ContributedRequestHandler<IContributedRequestHandler>, int>) (a => a.Order)).ToList<ContributedRequestHandler<IContributedRequestHandler>>();
      }
    }

    private string BuildQueryString(NameValueCollection queryParameters)
    {
      if (queryParameters == null)
        return string.Empty;
      List<string> stringList = new List<string>();
      foreach (string allKey in queryParameters.AllKeys)
      {
        foreach (string stringToEscape in queryParameters.GetValues(allKey))
        {
          string str = string.IsNullOrEmpty(allKey) ? string.Empty : Uri.EscapeDataString(allKey);
          if (stringToEscape != null)
          {
            if (!string.IsNullOrEmpty(allKey))
              str += "=";
            str += Uri.EscapeDataString(stringToEscape);
          }
          stringList.Add(str);
        }
      }
      return string.Join("&", stringList.ToArray());
    }

    private bool EvaluateCondition(
      bool condition,
      ICollection<EvaluatedCondition> evaluatedConditions,
      bool filtered,
      object details)
    {
      if (evaluatedConditions != null)
        evaluatedConditions.Add(new EvaluatedCondition()
        {
          Details = details,
          Evaluation = condition,
          Filtered = condition && filtered
        });
      return condition;
    }

    private IEnumerable<string> ExecuteNavigationHandler(
      IVssRequestContext requestContext,
      Contribution currentContribution)
    {
      IEnumerable<string> strings = (IEnumerable<string>) null;
      IEnumerable<ContributedRequestHandler<INavigationElementRequestHandler>> requestHandlers = this.GetRequestHandlers<INavigationElementRequestHandler>(requestContext, currentContribution);
      if (requestHandlers != null)
      {
        foreach (ContributedRequestHandler<INavigationElementRequestHandler> contributedRequestHandler in requestHandlers)
          strings = contributedRequestHandler.Handler.GetSelectedNavigationElements(requestContext, currentContribution.Id, contributedRequestHandler.Properties);
      }
      return strings;
    }

    private IEnumerable<ContributedRequestHandler<T>> GetRequestHandlers<T>(
      IVssRequestContext requestContext,
      Contribution currentContribution)
      where T : class, IContributedRequestHandler
    {
      List<ContributedRequestHandler<T>> source = (List<ContributedRequestHandler<T>>) null;
      IContributionService service = requestContext.GetService<IContributionService>();
      if ((currentContribution.GetProperty<int>("::Attributes") & 64) == 64)
      {
        IDictionary<string, ContributionNode> dictionary = service.QueryContributions(requestContext, (IEnumerable<string>) new string[1]
        {
          currentContribution.Id
        }, ContributionQueryOptions.IncludeChildren, (ContributionQueryCallback) ((queryContext, contribution, parentContribution, relationship, queryOptions, evaluatedConditions) =>
        {
          if (this.EvaluateCondition(string.IsNullOrEmpty(relationship), evaluatedConditions, true, (object) "IsRoot"))
            return ContributionQueryOptions.IncludeChildren;
          return this.EvaluateCondition(string.Equals("ms.vss-web.request-handler", contribution.Type), evaluatedConditions, false, (object) "IsHandler") ? ContributionQueryOptions.IncludeSelf : ContributionQueryOptions.None;
        }), this.CreateDiagnostics(requestContext, "navigationHandler"));
        if (dictionary != null)
        {
          foreach (ContributionNode contributionNode in (IEnumerable<ContributionNode>) dictionary.Values)
          {
            string property = contributionNode.Contribution.GetProperty<string>("name");
            Type type;
            if (property != null && this.m_requestHandlerTypes.TryGetValue(property, out type) && Activator.CreateInstance(type) is T instance)
            {
              if (source == null)
                source = new List<ContributedRequestHandler<T>>();
              source.Add(new ContributedRequestHandler<T>(contributionNode.Id, instance, contributionNode.Contribution.Properties, contributionNode.GetProperty<int>(requestContext, "order", 1000)));
            }
          }
        }
      }
      return source == null ? (IEnumerable<ContributedRequestHandler<T>>) null : (IEnumerable<ContributedRequestHandler<T>>) source.OrderBy<ContributedRequestHandler<T>, int>((Func<ContributedRequestHandler<T>, int>) (a => a.Order));
    }

    public void ServiceStart(IVssRequestContext requestContext)
    {
      IDisposableReadOnlyList<IRouteParameterResolver> extensions1 = requestContext.GetExtensions<IRouteParameterResolver>(ExtensionLifetime.Service, throwOnError: true);
      if (extensions1 != null)
      {
        foreach (IRouteParameterResolver parameterResolver in (IEnumerable<IRouteParameterResolver>) extensions1)
          this.m_parameterResolvers[parameterResolver.ParameterNamespace] = parameterResolver;
      }
      IDisposableReadOnlyList<IRouteParametersFilter> extensions2 = requestContext.GetExtensions<IRouteParametersFilter>(ExtensionLifetime.Service, throwOnError: true);
      if (extensions2 != null)
        this.m_parameterFilters.AddRange((IEnumerable<IRouteParametersFilter>) extensions2);
      IDisposableReadOnlyList<IContributedRequestHandler> extensions3 = requestContext.GetExtensions<IContributedRequestHandler>(ExtensionLifetime.Service, throwOnError: true);
      if (extensions3 != null)
      {
        foreach (IContributedRequestHandler contributedRequestHandler in (IEnumerable<IContributedRequestHandler>) extensions3)
          this.m_requestHandlerTypes[contributedRequestHandler.Name] = contributedRequestHandler.GetType();
      }
      IDisposableReadOnlyList<IContributionTransformer> extensions4 = requestContext.GetExtensions<IContributionTransformer>(ExtensionLifetime.Service, throwOnError: true);
      if (extensions4 != null)
      {
        foreach (IContributionTransformer contributionTransformer in (IEnumerable<IContributionTransformer>) extensions4)
        {
          foreach (string contributionType in contributionTransformer.ContributionTypes)
            this.m_contributionTransformers[contributionType] = contributionTransformer;
        }
      }
      IDisposableReadOnlyList<IContributedRouteConstraintProvider> extensions5 = requestContext.GetExtensions<IContributedRouteConstraintProvider>(ExtensionLifetime.Service, throwOnError: true);
      if (extensions5 != null)
        this.m_contributedRouteConstraintProviders.AddRange((IEnumerable<IContributedRouteConstraintProvider>) extensions5);
      this.m_cacheLockName = requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/emscachelock", (object) this.GetType().FullName));
    }

    public void ServiceEnd(IVssRequestContext requestContext)
    {
    }

    public void ComputeRequestContributions(
      IVssRequestContext requestContext,
      IEnumerable<string> contributionIds,
      ContributionQueryOptions queryOptions,
      ContributionQueryCallback queryCallback)
    {
      ContributionManagementService.ContributionRequestData requestData = this.GetRequestData(requestContext);
      foreach (KeyValuePair<string, ContributionNode> queryContribution in (IEnumerable<KeyValuePair<string, ContributionNode>>) requestContext.GetService<IContributionService>().QueryContributions(requestContext, contributionIds, queryOptions, queryCallback, this.CreateDiagnostics(requestContext, "request")))
      {
        if (!requestData.Contributions.ContainsKey(queryContribution.Key))
        {
          string type = queryContribution.Value.Contribution.Type;
          requestData.Contributions[queryContribution.Key] = queryContribution.Value;
          List<ContributionNode> contributionNodeList;
          if (!requestData.ContributionsByType.TryGetValue(type, out contributionNodeList))
          {
            contributionNodeList = new List<ContributionNode>();
            requestData.ContributionsByType[type] = contributionNodeList;
          }
          requestData.ContributionsByType[type].Add(queryContribution.Value);
        }
      }
    }

    public T GetContributedObject<T>(IVssRequestContext requestContext, string contributionId)
    {
      ContributionManagementService.ContributionRequestData requestData = this.GetRequestData(requestContext);
      object contributedObject = (object) default (T);
      if (!requestData.TransformedObject.TryGetValue(contributionId, out contributedObject))
      {
        ContributionNode contributionNode;
        if (requestData.Contributions != null && requestData.Contributions.TryGetValue(contributionId, out contributionNode))
        {
          bool requestSensitive = contributionNode.IsRequestSensitive(requestContext);
          if (!requestSensitive)
            contributedObject = (object) contributionNode.Contribution.GetAssociatedObject<T>("webSdkObject");
          IContributionTransformer contributionTransformer;
          if (contributedObject == null && this.m_contributionTransformers.TryGetValue(contributionNode.Contribution.Type, out contributionTransformer))
          {
            contributedObject = contributionTransformer.CreateTransformedObject(requestContext, contributionNode, ref requestSensitive);
            if (!requestSensitive)
              contributedObject = contributionNode.Contribution.SetAssociatedObject<object>("webSdkObject", contributedObject);
          }
        }
        requestData.TransformedObject[contributionId] = contributedObject;
      }
      return (T) contributedObject;
    }

    public ContributionNode GetContribution(
      IVssRequestContext requestContext,
      string contributionId)
    {
      ContributionNode contribution;
      this.GetRequestData(requestContext).Contributions.TryGetValue(contributionId, out contribution);
      return contribution;
    }

    public IEnumerable<ContributionNode> GetContributions(IVssRequestContext requestContext) => (IEnumerable<ContributionNode>) this.GetRequestData(requestContext).Contributions.Values;

    public IEnumerable<ContributionNode> GetContributionsByType(
      IVssRequestContext requestContext,
      string contributionType)
    {
      List<ContributionNode> contributionsByType;
      this.GetRequestData(requestContext).ContributionsByType.TryGetValue(contributionType, out contributionsByType);
      return (IEnumerable<ContributionNode>) contributionsByType;
    }

    public Dictionary<string, List<ContributionDiagnostics>> GetDiagnostics(
      IVssRequestContext requestContext)
    {
      return this.GetRequestData(requestContext).Diagnositics;
    }

    private ContributionDiagnostics CreateDiagnostics(
      IVssRequestContext requestContext,
      string diagnosticName)
    {
      ContributionManagementService.ContributionRequestData requestData = this.GetRequestData(requestContext);
      ContributionDiagnostics diagnostics = (ContributionDiagnostics) null;
      if (requestContext.GetService<IWebDiagnosticsService>().IsRequestDiagnosticsEnabled(requestContext))
      {
        if (requestData.Diagnositics == null)
          requestData.Diagnositics = new Dictionary<string, List<ContributionDiagnostics>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        List<ContributionDiagnostics> contributionDiagnosticsList;
        if (!requestData.Diagnositics.TryGetValue(diagnosticName, out contributionDiagnosticsList))
        {
          contributionDiagnosticsList = new List<ContributionDiagnostics>();
          requestData.Diagnositics[diagnosticName] = contributionDiagnosticsList;
        }
        diagnostics = new ContributionDiagnostics();
        contributionDiagnosticsList.Add(diagnostics);
      }
      return diagnostics;
    }

    public ContributedSite GetSite(IVssRequestContext requestContext) => this.GetRequestData(requestContext).Site;

    private void EnsureSiteDefinitionLoaded(IVssRequestContext requestContext)
    {
      if (this.m_siteDefinitionInitialized)
        return;
      string str = requestContext.GetService<IVssRegistryService>().GetValue<string>(requestContext, in ContributionManagementService.s_siteRegistryQuery, true);
      if (!string.IsNullOrEmpty(str))
      {
        Contribution siteContribution = requestContext.GetService<IContributionService>().QueryContribution(requestContext, str);
        if (siteContribution != null)
        {
          Dictionary<string, ContributedSite> dictionary = new Dictionary<string, ContributedSite>();
          dictionary[str] = new ContributedSite(requestContext, siteContribution);
          using (requestContext.AcquireWriterLock(this.m_cacheLockName))
          {
            if (!this.m_siteDefinitionInitialized)
            {
              this.m_contributedSites = dictionary;
              this.m_siteDefinitionInitialized = true;
            }
          }
        }
      }
      if (this.m_siteDefinitionInitialized)
        return;
      IEnumerable<Contribution> contributions = requestContext.GetService<IContributionService>().QueryContributionsForType(requestContext, "ms.vss-web.site");
      Dictionary<string, ContributedSite> dictionary1 = new Dictionary<string, ContributedSite>();
      foreach (Contribution siteContribution in contributions)
      {
        ContributedSite contributedSite = new ContributedSite(requestContext, siteContribution);
        dictionary1[siteContribution.Id] = contributedSite;
      }
      using (requestContext.AcquireWriterLock(this.m_cacheLockName))
      {
        if (this.m_siteDefinitionInitialized)
          return;
        this.m_contributedSites = dictionary1;
        this.m_siteDefinitionInitialized = true;
      }
    }

    public void ResetRequestData(IVssRequestContext requestContext)
    {
      if (requestContext == null || requestContext.Items == null)
        return;
      requestContext.Items.Remove("request.contribution-data");
      requestContext.Items.Remove("request.route-data");
    }

    private ContributionManagementService.ContributionRequestData GetRequestData(
      IVssRequestContext requestContext)
    {
      ContributionManagementService.ContributionRequestData requestData;
      if (requestContext.Items.ContainsKey("request.contribution-data"))
      {
        requestData = requestContext.Items["request.contribution-data"] as ContributionManagementService.ContributionRequestData;
        Action<IVssRequestContext, ContributionManagementService.ContributionRequestData> requestCallback = requestData.RequestCallback;
        requestData.RequestCallback = (Action<IVssRequestContext, ContributionManagementService.ContributionRequestData>) null;
        if (requestCallback != null)
          requestCallback(requestContext, requestData);
      }
      else
      {
        requestData = new ContributionManagementService.ContributionRequestData();
        requestContext.Items["request.contribution-data"] = (object) requestData;
      }
      return requestData;
    }

    private ContributionManagementService.ContributionRouteData GetRouteData(
      IVssRequestContext requestContext)
    {
      ContributionManagementService.ContributionRouteData routeData;
      if (requestContext.Items.ContainsKey("request.route-data"))
      {
        routeData = requestContext.Items["request.route-data"] as ContributionManagementService.ContributionRouteData;
      }
      else
      {
        routeData = new ContributionManagementService.ContributionRouteData();
        requestContext.Items["request.route-data"] = (object) routeData;
      }
      return routeData;
    }

    private class ContributedConstraint : IRouteConstraint
    {
      private readonly Contribution m_contribution;

      public ContributedConstraint(Contribution contribution) => this.m_contribution = contribution;

      public bool Match(
        HttpContextBase httpContext,
        Route route,
        string parameterName,
        RouteValueDictionary values,
        RouteDirection routeDirection)
      {
        bool flag = true;
        if (routeDirection == RouteDirection.IncomingRequest)
        {
          IVssRequestContext requestContext = httpContext.TfsRequestContext();
          if (requestContext != null)
            flag = requestContext.To(TeamFoundationHostType.Deployment).GetService<IContributionFilterService>().ApplyConstraints(requestContext, this.m_contribution, (string) null, string.Empty, (ICollection<EvaluatedCondition>) null, ConstraintEvaluationOptions.IgnoreRestrictedTo, (Func<ContributionConstraint, Contribution, bool?>) ((constraint, contribution) => string.Equals(constraint.Name, "ExtensionLicensed", StringComparison.OrdinalIgnoreCase) ? new bool?(true) : new bool?()));
        }
        return flag;
      }
    }

    private class ContributionRouteData
    {
      public RouteValueDictionary ResolvedRouteValues = new RouteValueDictionary();
      public RouteCollection RouteCollection;
      public RouteData RouteData;
      public Contribution RoutedContribution;
    }

    private class ContributionRequestData
    {
      public string ResolvedUrl;
      public string ResolvedRouteId;
      public IDictionary<string, object> ResolvedRouteValues;
      public IList<Contribution> PrimaryContributions = (IList<Contribution>) new List<Contribution>();
      public string RequestedTemplateName;
      public int? RequestedTemplateVersion;
      public string PageTitle;
      public IList<ContributedRequestHandler<IContributedRequestHandler>> RequestHandlers = (IList<ContributedRequestHandler<IContributedRequestHandler>>) new List<ContributedRequestHandler<IContributedRequestHandler>>();
      public ContributedSite Site = ContributionManagementService.s_defaultSite;
      public IDictionary<string, ContributionNode> Contributions = (IDictionary<string, ContributionNode>) new Dictionary<string, ContributionNode>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      public IDictionary<string, List<ContributionNode>> ContributionsByType = (IDictionary<string, List<ContributionNode>>) new Dictionary<string, List<ContributionNode>>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      public IDictionary<string, object> TransformedObject = (IDictionary<string, object>) new Dictionary<string, object>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      public Action<IVssRequestContext, ContributionManagementService.ContributionRequestData> RequestCallback;
      public Dictionary<string, List<ContributionDiagnostics>> Diagnositics;
    }
  }
}
