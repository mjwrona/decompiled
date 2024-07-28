// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure.AnalyticsODataRoutingConvention
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.AspNet.OData.Routing.Conventions;
using Microsoft.OData;
using Microsoft.OData.UriParser;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Analytics.OData.Infrastructure
{
  public class AnalyticsODataRoutingConvention : IODataRoutingConvention
  {
    private string controllerName;

    public AnalyticsODataRoutingConvention(string controllerName)
    {
      if (controllerName == null)
        throw new ArgumentNullException(nameof (controllerName));
      if (controllerName.EndsWith("Controller", StringComparison.Ordinal))
        controllerName = controllerName.Substring(0, controllerName.Length - "Controller".Length);
      this.controllerName = controllerName;
    }

    public string SelectController(Microsoft.AspNet.OData.Routing.ODataPath odataPath, HttpRequestMessage request)
    {
      if (!AnalyticsODataRoutingConvention.IsMetadataPath(odataPath))
        return this.controllerName;
      IVssRequestContext vssRequestContext = HttpContext.Current.GetVssRequestContext();
      try
      {
        vssRequestContext.ValidateAnalyticsEnabled();
        return "AnalyticsMetadata";
      }
      catch (Exception ex)
      {
        request.ThrowWrappedException(ex);
        throw;
      }
    }

    public string SelectAction(
      Microsoft.AspNet.OData.Routing.ODataPath odataPath,
      HttpControllerContext controllerContext,
      ILookup<string, HttpActionDescriptor> actionMap)
    {
      if (odataPath == null)
        throw new ArgumentNullException(nameof (odataPath));
      if (controllerContext == null)
        throw new ArgumentNullException(nameof (controllerContext));
      if (actionMap == null)
        throw new ArgumentNullException(nameof (actionMap));
      try
      {
        HttpMethod method = controllerContext.Request.Method;
        if (odataPath.PathTemplate == "~")
          return "GetServiceDocument";
        if (odataPath.PathTemplate == "~/$metadata")
          return "GetMetadata";
        if (!(method == HttpMethod.Get))
          throw new NotSupportedException();
        if (odataPath.Segments.FirstOrDefault<ODataPathSegment>() is OperationSegment || AnalyticsODataRoutingConvention.IsMetadataPath(odataPath))
          return (string) null;
        if (odataPath.PathTemplate == "~/entityset" || odataPath.PathTemplate == "~/entityset/$count" || odataPath.PathTemplate == "~/entityset/cast" || odataPath.PathTemplate == "~/entityset/cast/$count")
          return "Get";
        if (odataPath.PathTemplate == "~/entityset/function")
          return (string) null;
        if (odataPath.PathTemplate == "~/entityset/key" || odataPath.PathTemplate.StartsWith("~/entityset/key/navigation"))
        {
          HttpHeaderValueCollection<ProductInfoHeaderValue> userAgent = controllerContext.Request.Headers.UserAgent;
          if ((userAgent != null ? (userAgent.Any<ProductInfoHeaderValue>((Func<ProductInfoHeaderValue, bool>) (hv => hv.Product?.Name?.Contains("Microsoft.Data.Mashup").GetValueOrDefault())) ? 1 : 0) : 0) != 0)
            throw new ODataException(AnalyticsResources.ODATA_PATH_TEMPLATE_ARENT_SUPPORTED());
          return "GetByPath";
        }
        if (odataPath.PathTemplate.Contains("unresolved"))
          return (string) null;
        throw new ODataException(AnalyticsResources.ODATA_PATH_TEMPLATE_ARENT_SUPPORTED());
      }
      catch (Exception ex)
      {
        controllerContext.Request.ThrowWrappedException(ex);
        throw;
      }
    }

    private static bool IsMetadataPath(Microsoft.AspNet.OData.Routing.ODataPath odataPath) => odataPath.PathTemplate == "~" || odataPath.PathTemplate == "~/$metadata";
  }
}
