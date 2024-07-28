// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.CdnFallbackAttribute
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class CdnFallbackAttribute : ActionFilterAttribute
  {
    private const string c_area = "BundlingService";
    private const string c_layer = "WebPlatform";
    private const string c_ciArea = "WebPlatform";
    private const string c_ciFeature = "CdnFallback";
    private const int c_tracePoint = 15061000;

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      if (filterContext.HttpContext.Request.Cookies["TFS-CDNTRACE"] == null || !filterContext.HttpContext.Request.Cookies["TFS-CDNTRACE"].Value.StartsWith("report"))
        return;
      CdnFallbackAttribute.UpdateCdnCookies(filterContext.RequestContext, filterContext.HttpContext.Request, filterContext.HttpContext.Response);
    }

    private static void UpdateCdnCookies(
      RequestContext requestContext,
      HttpRequestBase request,
      HttpResponseBase response)
    {
      HttpCookie cookie1 = request.Cookies["TFS-CDNTRACE"];
      string[] strArray = cookie1.Value.Split('-');
      bool flag1 = strArray.Length > 1 && "true".Equals(strArray[1], StringComparison.OrdinalIgnoreCase);
      string str = strArray.Length > 2 ? Uri.UnescapeDataString(strArray[2]) : "(unknown)";
      cookie1.Expires = new DateTime(1970, 1, 1);
      cookie1.Value = "";
      HttpCookie cookie2 = request.Cookies["TFS-CDNFAIL"];
      if (cookie2 == null)
      {
        cookie2 = new HttpCookie("TFS-CDNFAIL", "0");
        HttpCookie httpCookie = cookie2;
        IVssRequestContext vssRequestContext = requestContext.TfsRequestContext();
        int num = vssRequestContext != null ? (vssRequestContext.ExecutionEnvironment.IsSslOnly ? 1 : 0) : 0;
        httpCookie.Secure = num != 0;
      }
      int result;
      if (!int.TryParse(cookie2.Value, out result) || result < 0)
        result = 0;
      int num1 = result + 1;
      double num2 = Math.Min(Math.Pow(2.0, (double) (num1 - 1)), 8.0);
      cookie2.Value = num1.ToString();
      HttpCookie httpCookie1 = cookie2;
      DateTime now = DateTime.Now;
      DateTime dateTime1 = now.AddHours(num2 * 2.0);
      httpCookie1.Expires = dateTime1;
      HttpCookie cookie3 = request.Cookies["TFS-CDN"];
      if (cookie3 == null)
      {
        cookie3 = new HttpCookie("TFS-CDN", "disabled");
        HttpCookie httpCookie2 = cookie3;
        IVssRequestContext vssRequestContext = requestContext.TfsRequestContext();
        int num3 = vssRequestContext != null ? (vssRequestContext.ExecutionEnvironment.IsSslOnly ? 1 : 0) : 0;
        httpCookie2.Secure = num3 != 0;
      }
      HttpCookie httpCookie3 = cookie3;
      now = DateTime.Now;
      DateTime dateTime2 = now.AddHours(num2);
      httpCookie3.Expires = dateTime2;
      bool flag2 = false;
      HttpCookie cookie4 = request.Cookies["TFS-CDNTIMEOUT"];
      if (cookie4 != null)
      {
        flag2 = true;
        cookie4.Expires = new DateTime(1970, 1, 1);
        cookie4.Value = "";
        response.Cookies.Set(cookie4);
      }
      response.Cookies.Set(cookie1);
      response.Cookies.Set(cookie2);
      response.Cookies.Set(cookie3);
      IVssRequestContext vssRequestContext1 = requestContext.TfsRequestContext();
      if (vssRequestContext1 == null)
        return;
      vssRequestContext1.Trace(15061000, TraceLevel.Info, "BundlingService", "WebPlatform", "CDN disabled for user. Failure count={0} FallbackApplied={1} Url={2} FailedModules={3}", (object) num1, (object) flag1, (object) request.Url, (object) str);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      properties.Add("count", (double) num1);
      properties.Add("fallbackApplied", flag1);
      properties.Add("timeout", flag2);
      properties.Add("failedModules", str);
      vssRequestContext1.GetService<CustomerIntelligenceService>().Publish(vssRequestContext1, "WebPlatform", "CdnFallback", properties);
    }
  }
}
