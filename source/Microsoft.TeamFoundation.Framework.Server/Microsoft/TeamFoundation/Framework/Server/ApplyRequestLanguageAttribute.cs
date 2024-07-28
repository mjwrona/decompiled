// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ApplyRequestLanguageAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class ApplyRequestLanguageAttribute : ActionFilterAttribute
  {
    public override void OnActionExecuting(HttpActionContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      if (filterContext == null || filterContext.Request == null)
        return;
      HttpRequestMessage request = filterContext.Request;
      IDictionary properties = request.Properties as IDictionary;
      NameValueCollection queryString = request.RequestUri == (Uri) null ? (NameValueCollection) null : request.RequestUri.ParseQueryString();
      HttpRequestHeaders headers = request.Headers;
      string[] array = headers == null || headers.AcceptLanguage == null ? (string[]) null : headers.AcceptLanguage.Select<StringWithQualityHeaderValue, string>((Func<StringWithQualityHeaderValue, string>) (language => language.Value)).ToArray<string>();
      IVssRequestContext requestContext = (IVssRequestContext) null;
      object obj;
      if (request.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContext, out obj))
        requestContext = obj as IVssRequestContext;
      RequestLanguage.Apply(requestContext, queryString, array, properties);
    }

    public override void OnActionExecuted(HttpActionExecutedContext filterContext)
    {
      base.OnActionExecuted(filterContext);
      if (filterContext == null || filterContext.Request == null)
        return;
      RequestLanguage.Revert(filterContext.Request.Properties as IDictionary);
    }
  }
}
