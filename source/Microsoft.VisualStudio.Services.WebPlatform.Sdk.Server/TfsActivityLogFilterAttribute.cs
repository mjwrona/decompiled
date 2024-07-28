// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.TfsActivityLogFilterAttribute
// Assembly: Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A7EB5677-18AD-4D09-80BD-B83CBD009DB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server.Contributions;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server
{
  public class TfsActivityLogFilterAttribute : ActionFilterAttribute, IAuthorizationFilter
  {
    public void OnAuthorization(AuthorizationContext filterContext)
    {
      if (filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof (ByPassTfsActivityLogFilterAttribute), false).Length != 0 || !(filterContext.Controller is IPlatformController controller) || controller.TfsRequestContext == null)
        return;
      controller.TfsRequestContext.TraceEnter(520050, controller.TraceArea, WebPlatformTraceLayers.Framework, "OnActionExecuting");
      string webMethodName = (string) null;
      if (string.Equals(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "Apps", StringComparison.OrdinalIgnoreCase) || string.Equals(filterContext.ActionDescriptor.ControllerDescriptor.ControllerName, "ContributedPage", StringComparison.OrdinalIgnoreCase))
        webMethodName = controller.TfsRequestContext.GetService<IContributionRoutingService>().GetCommandName(controller.TfsRequestContext);
      if (string.IsNullOrEmpty(webMethodName))
      {
        webMethodName = filterContext.ActionDescriptor.ControllerDescriptor.ControllerName + "." + filterContext.ActionDescriptor.ActionName;
        string logCommandPrefix = controller.GetActivityLogCommandPrefix();
        if (!string.IsNullOrEmpty(logCommandPrefix))
          webMethodName = logCommandPrefix + "." + webMethodName;
      }
      MethodInformation methodInformation = new MethodInformation(webMethodName, MethodType.Normal, EstimatedMethodCost.Low);
      controller.EnterMethod(methodInformation);
      controller.TfsRequestContext.TraceLeave(520050, controller.TraceArea, WebPlatformTraceLayers.Framework, "OnActionExecuting");
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      if (filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof (ByPassTfsActivityLogFilterAttribute), false).Length != 0 || !(filterContext.Controller is IPlatformController controller) || controller.TfsRequestContext == null)
        return;
      MethodInformation method = controller.TfsRequestContext.Method;
      foreach (string key in (IEnumerable<string>) filterContext.ActionParameters.Keys)
      {
        object actionParameter = filterContext.ActionParameters[key];
        switch (actionParameter)
        {
          case string[] _:
            method.AddArrayParameter<string>(key, (IList<string>) (string[]) actionParameter);
            continue;
          case IList<int> _:
            method.AddArrayParameter<int>(key, (IList<int>) actionParameter);
            continue;
          default:
            method.AddParameter(key, filterContext.ActionParameters[key]);
            continue;
        }
      }
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      base.OnActionExecuted(filterContext);
      if (filterContext.Exception == null || filterContext.ActionDescriptor.ControllerDescriptor.GetCustomAttributes(typeof (ByPassTfsActivityLogFilterAttribute), false).Length != 0 || !(filterContext.Controller is IPlatformController controller) || controller.TfsRequestContext == null)
        return;
      controller.TfsRequestContext.Status = filterContext.Exception;
    }
  }
}
