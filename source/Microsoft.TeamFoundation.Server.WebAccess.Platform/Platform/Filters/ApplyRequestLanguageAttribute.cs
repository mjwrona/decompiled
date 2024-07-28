// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.Filters.ApplyRequestLanguageAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections;
using System.Collections.Specialized;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform.Filters
{
  public class ApplyRequestLanguageAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      ApplyRequestLanguageAttribute.Apply((ControllerContext) filterContext);
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
      base.OnActionExecuted(filterContext);
      ApplyRequestLanguageAttribute.Revert((ControllerContext) filterContext);
    }

    public override void OnResultExecuting(ResultExecutingContext filterContext)
    {
      base.OnResultExecuting(filterContext);
      ApplyRequestLanguageAttribute.Apply((ControllerContext) filterContext);
    }

    public override void OnResultExecuted(ResultExecutedContext filterContext)
    {
      base.OnResultExecuted(filterContext);
      ApplyRequestLanguageAttribute.Revert((ControllerContext) filterContext);
    }

    private static void Apply(ControllerContext filterContext)
    {
      if (filterContext == null || filterContext.RequestContext == null || filterContext.RequestContext.HttpContext == null || filterContext.RequestContext.HttpContext.Request == null)
        return;
      HttpRequestBase request = filterContext.RequestContext.HttpContext.Request;
      IDictionary items = filterContext.RequestContext.HttpContext.Items;
      NameValueCollection queryString = request.QueryString;
      string[] userLanguages = request.UserLanguages;
      RequestLanguage.Apply(filterContext.RequestContext.TfsRequestContext(), queryString, userLanguages, items);
    }

    private static void Revert(ControllerContext filterContext)
    {
      if (filterContext == null || filterContext.RequestContext == null || filterContext.RequestContext.HttpContext == null)
        return;
      RequestLanguage.Revert(filterContext.RequestContext.HttpContext.Items);
    }
  }
}
