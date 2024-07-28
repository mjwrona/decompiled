// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions.IContributionRoutingService
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions
{
  [DefaultServiceImplementation(typeof (ContributionManagementService))]
  public interface IContributionRoutingService : IVssFrameworkService
  {
    RouteCollection GetContributedRoutes(IVssRequestContext requestContext);

    string RouteUrl(
      IVssRequestContext requestContext,
      string contributionId,
      RouteValueDictionary routeValues = null,
      bool includeImplicitValues = false,
      bool allowFallbackRoute = true);

    string RouteUrl(
      IVssRequestContext requestContext,
      string routeName,
      string actionName,
      string controllerName,
      RouteValueDictionary routeValues = null,
      bool includeImplicitValues = false);

    Contribution GetRoutedContribution(IVssRequestContext requestContext);

    void SetRequestRoute(IVssRequestContext requestContext, RouteData routeData);

    IEnumerable<ContributedRequestHandler<T>> GetRequestHandlers<T>(
      IVssRequestContext requestContext)
      where T : IContributedRequestHandler;

    string GetCommandName(IVssRequestContext requestContext);

    string GetUrl(IVssRequestContext requestContext);

    string GetUrl(IVssRequestContext requestContext, out bool isOriginalUrl);

    IContributedRoute GetRoute(IVssRequestContext requestContext);

    IDictionary<string, object> GetRouteValues(IVssRequestContext requestContext);

    T GetRouteValue<T>(IVssRequestContext requestContext, string routeParameter);

    NameValueCollection GetQueryParameters(IVssRequestContext requestContext);

    string GetQueryParameter(IVssRequestContext requestContext, string paramName);

    void SetQueryParameter(IVssRequestContext requestContext, string paramName, string paramValue);

    void RemoveQueryStringParameter(IVssRequestContext requestContext, string paramName);

    void SetUrl(IVssRequestContext requestContext, string url);

    void SetRoute(
      IVssRequestContext requestContext,
      string routeContributionId,
      RouteValueDictionary routeValues,
      bool preserveQueryParameters = false,
      bool mergeRouteValues = true);

    string GetRequestedTemplateName(IVssRequestContext requestContext);

    int GetRequestedTemplateVersion(IVssRequestContext requestContext);

    void SetRequestedTemplate(IVssRequestContext requestContext, string templateName, int version = 0);
  }
}
