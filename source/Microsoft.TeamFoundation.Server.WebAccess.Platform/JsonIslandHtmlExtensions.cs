// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.JsonIslandHtmlExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class JsonIslandHtmlExtensions
  {
    private static int s_defaultJsonMaxLength = JsonIslandHtmlExtensions.GetDefaultJsonMaxLength();

    public static MvcHtmlString DataContractJsonIsland<T>(
      this HtmlHelper htmlHelper,
      T data,
      bool useSimpleDictionaryFormat = false)
    {
      return JsonIslandHtmlExtensions.DataContractJsonIsland<T>(htmlHelper, data, (IDictionary<string, object>) null, useSimpleDictionaryFormat);
    }

    public static MvcHtmlString DataContractJsonIsland<T>(
      this HtmlHelper htmlHelper,
      T data,
      object htmlAttributes,
      bool useSimpleDictionaryFormat = false)
    {
      return JsonIslandHtmlExtensions.DataContractJsonIsland<T>(htmlHelper, data, (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes), useSimpleDictionaryFormat);
    }

    public static MvcHtmlString DataContractJsonIsland<T>(
      this HtmlHelper htmlHelper,
      T data,
      IDictionary<string, object> htmlAttributes,
      bool useSimpleDictionaryFormat = false)
    {
      TagBuilder tagBuilder = new TagBuilder("script");
      if (htmlAttributes != null)
        tagBuilder.MergeAttributes<string, object>(htmlAttributes);
      tagBuilder.MergeAttribute("type", "application/json");
      tagBuilder.MergeAttribute("defer", "defer");
      tagBuilder.InnerHtml = ConvertUtility.DataContractJson(data.GetType(), (object) data, useSimpleDictionaryFormat);
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static MvcHtmlString GatherWebPerformanceTimings(
      this HtmlHelper htmlHelper,
      object htmlAttributes = null)
    {
      WebContext webContext = htmlHelper.ViewContext.WebContext();
      WebPerformanceTimerAttribute.EndResultExecutedTimer(webContext.RequestContext);
      IDictionary<string, PerformanceTimingGroup> data = WebPerformanceTimerHelpers.SendCustomerIntelligenceData(webContext);
      return webContext.Diagnostics.TracePointCollectionEnabled ? htmlHelper.RestApiJsonIsland((object) data, htmlAttributes) : MvcHtmlString.Empty;
    }

    public static MvcHtmlString RestApiJsonIsland(
      this HtmlHelper htmlHelper,
      object data,
      object htmlAttributes = null)
    {
      return JsonIslandHtmlExtensions.RestApiJsonIsland(htmlHelper, data, htmlAttributes == null ? (IDictionary<string, object>) null : (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes));
    }

    public static MvcHtmlString RestApiJsonIsland(
      this HtmlHelper htmlHelper,
      object data,
      IDictionary<string, object> htmlAttributes)
    {
      TagBuilder tagBuilder = new TagBuilder("script");
      if (htmlAttributes != null)
        tagBuilder.MergeAttributes<string, object>(htmlAttributes);
      tagBuilder.MergeAttribute("type", "application/json");
      tagBuilder.MergeAttribute("defer", "defer");
      tagBuilder.InnerHtml = RestApiJsonResult.SerializeRestApiData(htmlHelper.ViewContext.TfsRequestContext(false), data, RestApiJsonResult.Destination.JsonIsland);
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static MvcHtmlString JsonIsland(this HtmlHelper htmlHelper, object data) => JsonIslandHtmlExtensions.JsonIsland(htmlHelper, data, (IDictionary<string, object>) null, JsonIslandHtmlExtensions.s_defaultJsonMaxLength);

    public static MvcHtmlString JsonIsland(
      this HtmlHelper htmlHelper,
      object data,
      object htmlAttributes)
    {
      return JsonIslandHtmlExtensions.JsonIsland(htmlHelper, data, (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes), JsonIslandHtmlExtensions.s_defaultJsonMaxLength);
    }

    public static MvcHtmlString JsonIsland(
      this HtmlHelper htmlHelper,
      object data,
      object htmlAttributes,
      int maxJsonLength)
    {
      return JsonIslandHtmlExtensions.JsonIsland(htmlHelper, data, (IDictionary<string, object>) new RouteValueDictionary(htmlAttributes), maxJsonLength);
    }

    public static MvcHtmlString JsonIsland(
      this HtmlHelper htmlHelper,
      object data,
      IDictionary<string, object> htmlAttributes)
    {
      return JsonIslandHtmlExtensions.JsonIsland(htmlHelper, data, htmlAttributes, JsonIslandHtmlExtensions.s_defaultJsonMaxLength);
    }

    public static MvcHtmlString JsonIsland(
      this HtmlHelper htmlHelper,
      object data,
      IDictionary<string, object> htmlAttributes,
      int maxJsonLength)
    {
      TagBuilder tagBuilder = new TagBuilder("script");
      if (htmlAttributes != null)
        tagBuilder.MergeAttributes<string, object>(htmlAttributes);
      JavaScriptSerializer scriptSerializer = new JavaScriptSerializer();
      if (maxJsonLength > 0)
        scriptSerializer.MaxJsonLength = maxJsonLength;
      tagBuilder.MergeAttribute("type", "application/json");
      tagBuilder.MergeAttribute("defer", "defer");
      tagBuilder.InnerHtml = scriptSerializer.Serialize(data);
      return MvcHtmlString.Create(tagBuilder.ToString());
    }

    public static MvcHtmlString JavaScript(this HtmlHelper htmlHelper, string javaScript) => MvcHtmlString.Create("<script type=\"text/javascript\"" + htmlHelper.GenerateNonce(true) + "> " + javaScript + "</script>");

    private static int GetDefaultJsonMaxLength()
    {
      int defaultJsonMaxLength = new JavaScriptSerializer().MaxJsonLength;
      int result;
      if (int.TryParse(ConfigurationManager.AppSettings["maxJsonLength"], out result))
        defaultJsonMaxLength = result;
      return defaultJsonMaxLength;
    }

    public static MvcHtmlString AreaLocations(this HtmlHelper htmlHelper, string[] areas)
    {
      ApiResourceLocationCollection resourceLocations = VersionedApiResourceRegistration.ResourceLocations;
      ApiResourceLocation[] array = ((IEnumerable<string>) areas).SelectMany<string, ApiResourceLocation>((Func<string, IEnumerable<ApiResourceLocation>>) (area => resourceLocations.GetAreaLocations(area))).ToArray<ApiResourceLocation>();
      Dictionary<string, ApiResourceLocation[]> data = new Dictionary<string, ApiResourceLocation[]>()
      {
        ["value"] = array
      };
      return htmlHelper.RestApiJsonIsland((object) data, (object) new
      {
        @class = "area-locations"
      });
    }
  }
}
