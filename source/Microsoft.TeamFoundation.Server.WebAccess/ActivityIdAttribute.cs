// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.ActivityIdAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
  public class ActivityIdAttribute : ActionFilterAttribute
  {
    private const string HttpHeaderActivityId = "ActivityId";

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      if (filterContext != null && filterContext.HttpContext != null && filterContext.HttpContext.Response != null && filterContext.HttpContext.Response.Headers != null && string.IsNullOrEmpty(filterContext.HttpContext.Response.Headers.Get("ActivityId")) && filterContext.Controller is TfsController controller)
      {
        if (controller.TfsRequestContext != null)
        {
          try
          {
            filterContext.HttpContext.Response.AppendHeader("ActivityId", controller.TfsRequestContext.ActivityId.ToString("D"));
          }
          catch (Exception ex)
          {
            controller.TfsRequestContext.TraceException(599999, "WebAccess", TfsTraceLayers.Content, ex);
          }
        }
      }
      base.OnActionExecuting(filterContext);
    }
  }
}
