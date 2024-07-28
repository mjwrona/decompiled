// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.ODataBuiltinWhitelistValidator
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Query;
using Microsoft.OData;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.Expressions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class ODataBuiltinWhitelistValidator : IODataValidator
  {
    private static HashSet<string> s_builtinQueryShapes;

    public Action WarningCallback { get; set; }

    public string RequiredFeatureFlag => (string) null;

    public void Validate(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      ODataQueryOptions queryOptions)
    {
      if (!requestContext.GetService<IVssRegistryService>().GetValue<bool>(requestContext, (RegistryQuery) "/Service/Analytics/Settings/OData/ODataDisableWhilelisting", false, false))
      {
        IAnalyticsSecurityService service = requestContext.GetService<IAnalyticsSecurityService>();
        if (requestContext.ExecutionEnvironment.IsDevFabricDeployment || requestContext.IsFeatureEnabled("Analytics.OData.ForceValidateQueryWhiteListing"))
          ODataBuiltinWhitelistValidator.IsWhitelistedQuery(requestContext, request, queryOptions);
        IVssRequestContext requestContext1 = requestContext;
        if (!service.HasExecuteUnrestrictedQueryPermission(requestContext1) && !ODataBuiltinWhitelistValidator.IsWhitelistedQuery(requestContext, request, queryOptions))
          throw new AnalyticsAccessCheckException(AnalyticsResources.ODATA_NOT_AUTHENTICATED_ERROR(), (Exception) null);
      }
      else
        requestContext.TraceAlways(12013037, TraceLevel.Warning, "AnalyticsModel", "IODataValidator", "Validation skipped");
    }

    private static bool IsWhitelistedQuery(
      IVssRequestContext requestContext,
      HttpRequestMessage request,
      ODataQueryOptions queryOptions)
    {
      string str = string.Empty;
      bool flag = false;
      string supportedWidget;
      bool supportedWidgetName = AnalyticsController.TryGetSupportedWidgetName(requestContext.Command(), out supportedWidget);
      try
      {
        str = new ODataQueryShapeFormatter(queryOptions.Context.Model).Format(queryOptions);
        flag = ODataBuiltinWhitelistValidator.GetQueryShapes(requestContext).Contains(str);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(12013033, TraceLevel.Error, "AnalyticsModel", "AnalyticsModel", ex);
      }
      if (supportedWidgetName && !flag)
      {
        requestContext.TraceAlways(12013032, TraceLevel.Info, "AnalyticsModel", "AnalyticsModel", "Widget query does not match any of the builtin query shapes for command '" + supportedWidget + "' and shape: " + str);
        if (requestContext.ExecutionEnvironment.IsDevFabricDeployment || requestContext.IsFeatureEnabled("Analytics.OData.ForceValidateQueryWhiteListing"))
          throw new ODataException(AnalyticsResources.WIDGET_QUERY_IS_NOT_BUILTIN_SHAPE((object) supportedWidget, (object) str));
      }
      return flag;
    }

    private static HashSet<string> GetQueryShapes(IVssRequestContext requestContext)
    {
      if (ODataBuiltinWhitelistValidator.s_builtinQueryShapes == null)
      {
        using (IDisposableReadOnlyList<IODataQueryShapeProvider> extensions = requestContext.GetExtensions<IODataQueryShapeProvider>())
          ODataBuiltinWhitelistValidator.s_builtinQueryShapes = new HashSet<string>(extensions.SelectMany<IODataQueryShapeProvider, string>((Func<IODataQueryShapeProvider, IEnumerable<string>>) (p => p.GetQueryShapes())));
      }
      return ODataBuiltinWhitelistValidator.s_builtinQueryShapes;
    }
  }
}
