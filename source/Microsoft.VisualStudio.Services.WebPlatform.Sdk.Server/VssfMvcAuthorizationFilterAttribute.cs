// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.VssfMvcAuthorizationFilterAttribute
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
  public class VssfMvcAuthorizationFilterAttribute : FilterAttribute, IAuthorizationFilter
  {
    public void OnAuthorization(AuthorizationContext filterContext)
    {
      IVssRequestContext vssRequestContext = filterContext.HttpContext.ApplicationInstance is VisualStudioServicesApplication applicationInstance ? applicationInstance.VssRequestContext : (IVssRequestContext) null;
      if (vssRequestContext == null)
        return;
      RequestRestrictionsAttribute restrictionsAttribute = ((IEnumerable<RequestRestrictionsAttribute>) filterContext.ActionDescriptor.GetCustomAttributes(typeof (RequestRestrictionsAttribute), true)).ToList<RequestRestrictionsAttribute>().ApplyRequestRestrictions(vssRequestContext, (IDictionary<string, object>) filterContext.RouteData?.Values);
      if (restrictionsAttribute != null && restrictionsAttribute is PublicBaseRequestRestrictionsAttribute)
      {
        bool flag = false;
        if (filterContext.ActionDescriptor is ReflectedActionDescriptor actionDescriptor && (actionDescriptor.MethodInfo.ReflectedType.FullName == "Microsoft.TeamFoundation.Server.WebAccess.Presentation.AppsController" || actionDescriptor.MethodInfo.ReflectedType.FullName == "Microsoft.TeamFoundation.Server.WebAccess.Redirect.RedirectWithIdentityController" || actionDescriptor.MethodInfo.ReflectedType == typeof (ContributedPageController)))
          flag = true;
        if (!flag)
        {
          string message = "Cannot set PublicBaseRequestRestrictionsAttribute on MVC method: " + filterContext.ActionDescriptor.ActionName;
          vssRequestContext.Trace(1519284393, TraceLevel.Error, nameof (VssfMvcAuthorizationFilterAttribute), "AnonymousAccessKalypsoAlert", message);
          throw new InvalidOperationException(message);
        }
      }
      IdentityValidationResult validationResult = vssRequestContext.IsValidIdentity();
      if (validationResult.IsSuccess)
        return;
      filterContext.Result = (ActionResult) new HttpStatusCodeResult(validationResult.HttpStatusCode, validationResult.Exception.Message);
      TeamFoundationApplicationCore.CompleteRequest(vssRequestContext, HttpContextFactory.Current.GetApplicationInstance(), validationResult.HttpStatusCode, validationResult.Exception);
    }
  }
}
