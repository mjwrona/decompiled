// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TfsAntiForgeryValidation
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class TfsAntiForgeryValidation : FilterAttribute, IAuthorizationFilter
  {
    private static bool sm_initialized;
    private static bool sm_disabled;

    public void OnAuthorization(AuthorizationContext filterContext)
    {
      TfsAntiForgeryValidation.EnsureInitialized();
      if (TfsAntiForgeryValidation.sm_disabled || !StringComparer.OrdinalIgnoreCase.Equals("POST", filterContext.HttpContext.Request.HttpMethod) && !StringComparer.OrdinalIgnoreCase.Equals("PUT", filterContext.HttpContext.Request.HttpMethod) && !StringComparer.OrdinalIgnoreCase.Equals("DELETE", filterContext.HttpContext.Request.HttpMethod))
        return;
      bool flag1 = filterContext.HttpContext.Request.ContentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase);
      IVssRequestContext vssRequestContext = filterContext.RequestContext.TfsRequestContext();
      IVssRequestContext context = vssRequestContext.To(TeamFoundationHostType.Deployment);
      ICrossOriginManagementService service = context.GetService<ICrossOriginManagementService>();
      string header = filterContext.HttpContext.Request.Headers["Origin"];
      IVssRequestContext requestContext = context;
      string origin = header;
      bool flag2 = !service.IsUnsafeCrossOriginRequest(requestContext, origin);
      if (flag1 & flag2)
        return;
      object[] customAttributes = filterContext.ActionDescriptor.GetCustomAttributes(typeof (ValidateAntiForgeryTokenAttribute), false);
      HttpContextBase httpContext = filterContext.HttpContext;
      string name = "__RequestVerificationToken2";
      if (((IEnumerable<string>) httpContext.Request.Form.AllKeys).Contains<string>(name))
      {
        string forgeryTokenName = PlatformHtmlExtensions.GetAntiForgeryTokenName(httpContext.Request.ApplicationPath);
        HttpCookie cookie1 = httpContext.Request.Cookies[forgeryTokenName];
        if (cookie1 != null)
        {
          HttpCookie cookie2 = httpContext.Request.Cookies[httpContext.Request.Form[name]];
          if (cookie2 != null)
          {
            cookie1.Value = cookie2.Value;
            httpContext.Request.Cookies.Set(cookie1);
            httpContext.Request.Cookies.Remove(httpContext.Request.Form[name]);
          }
        }
      }
      if (customAttributes.Length != 0)
        return;
      if (filterContext.ActionDescriptor.GetCustomAttributes(typeof (TfsBypassAntiForgeryValidation), false).Length != 0)
        return;
      try
      {
        new ValidateAntiForgeryTokenAttribute().OnAuthorization(filterContext);
      }
      catch (Exception ex)
      {
        bool flag3 = vssRequestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.RestApi.RejectUnsafeOriginRequests");
        bool flag4 = false;
        if (!string.IsNullOrEmpty(header) && header.StartsWith("chrome-extension://"))
        {
          flag3 = false;
          flag4 = true;
        }
        if (flag1)
        {
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("action", "MVCRequestUsedCookiesFromUnsafeOrigin");
          properties.Add("UserAgent", vssRequestContext.UserAgent);
          properties.Add("HttpMethod", filterContext.HttpContext.Request.HttpMethod);
          properties.Add("Uri", (object) filterContext.HttpContext.Request.Url);
          properties.Add("ControllerName", filterContext.ActionDescriptor.ControllerDescriptor.ControllerName);
          properties.Add("ControllerType", filterContext.ActionDescriptor.ControllerDescriptor.ControllerType.ToString());
          properties.Add("ActionName", filterContext.ActionDescriptor.ActionName);
          properties.Add("ServiceName", vssRequestContext.ServiceName);
          properties.Add("Origin", header ?? string.Empty);
          properties.Add("Reject", flag3.ToString());
          if (flag4)
            properties.Add("Extension", "true");
          vssRequestContext.GetService<CustomerIntelligenceService>().Publish(vssRequestContext, "REST", "RESTUnsafeCrossOriginRequest", properties);
        }
        if (!(!flag1 | flag3))
          return;
        throw;
      }
    }

    private static void EnsureInitialized()
    {
      if (TfsAntiForgeryValidation.sm_initialized)
        return;
      if (!bool.TryParse(ConfigurationManager.AppSettings["disableAntiForgeryValidation"], out TfsAntiForgeryValidation.sm_disabled))
        TfsAntiForgeryValidation.sm_disabled = false;
      TfsAntiForgeryValidation.sm_initialized = true;
    }
  }
}
