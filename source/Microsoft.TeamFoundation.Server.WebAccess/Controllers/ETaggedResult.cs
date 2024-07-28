// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Controllers.ETaggedResult
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Common.Internal;
using System;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Controllers
{
  public class ETaggedResult : ActionResult
  {
    public ETaggedResult(
      Func<string> calculateEntityTag,
      Func<ActionResult> action,
      TimeSpan maxAge,
      HttpCacheability cacheability,
      System.Action<HttpContextBase> doFinally = null)
    {
      if (calculateEntityTag == null)
        throw new ArgumentNullException("etagCalculator");
      if (action == null)
        throw new ArgumentNullException(nameof (action));
      this.CalculateEntityTag = calculateEntityTag;
      this.Action = action;
      this.MaxAge = maxAge;
      this.Cacheability = cacheability;
      this.DoFinally = doFinally;
    }

    public override void ExecuteResult(ControllerContext context)
    {
      HttpContextBase httpContext = context.RequestContext.HttpContext;
      try
      {
        string header = httpContext.Request.Headers["If-None-Match"];
        string str = context.RequestContext.TfsWebContext().WebApiVersionClient.ToString((IFormatProvider) NumberFormatInfo.InvariantInfo) + "-" + this.CalculateEntityTag();
        if (str.Length > 256)
          str = Convert.ToBase64String(MD5Util.CalculateMD5(Encoding.UTF8.GetBytes(str)));
        if (!string.IsNullOrWhiteSpace(header))
        {
          if (StringComparer.OrdinalIgnoreCase.Equals(header.Trim(' ', '"', '\''), str))
          {
            if (this.MaxAge > TimeSpan.Zero)
            {
              httpContext.Response.Cache.SetCacheability(this.Cacheability);
              httpContext.Response.Cache.SetMaxAge(this.MaxAge);
              httpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            }
            httpContext.Response.StatusCode = 304;
            return;
          }
        }
        httpContext.Response.Cache.SetCacheability(this.Cacheability);
        httpContext.Response.Cache.SetMaxAge(this.MaxAge);
        httpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
        httpContext.Response.Headers["ETag"] = str;
        (this.Action() ?? throw new InvalidOperationException("Action delegate did not return an invokeable action.")).ExecuteResult(context);
      }
      finally
      {
        System.Action<HttpContextBase> doFinally = this.DoFinally;
        if (doFinally != null)
          doFinally(httpContext);
      }
    }

    private Func<string> CalculateEntityTag { get; set; }

    private Func<ActionResult> Action { get; set; }

    public HttpCacheability Cacheability { get; set; }

    public TimeSpan MaxAge { get; set; }

    public System.Action<HttpContextBase> DoFinally { get; set; }
  }
}
