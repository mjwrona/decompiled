// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.SignalR.RequestExtensions
// Assembly: Microsoft.VisualStudio.Services.SignalR, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: BD148864-3B8A-4D7D-BD16-EF04E9549DC9
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.SignalR.dll

using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Microsoft.TeamFoundation.Framework.Server;
using System.Web;

namespace Microsoft.VisualStudio.Services.SignalR
{
  public static class RequestExtensions
  {
    public static HttpContextBase GetHttpContext(this IOwinRequest request)
    {
      object obj;
      return request.Environment.TryGetValue(typeof (HttpContextBase).FullName, out obj) ? obj as HttpContextBase : (HttpContextBase) null;
    }

    public static IVssRequestContext GetVssRequestContext(this IOwinRequest request)
    {
      HttpContextBase httpContext = request.GetHttpContext();
      return httpContext == null ? (IVssRequestContext) null : httpContext.GetVssRequestContext();
    }

    public static IVssRequestContext GetVssRequestContext(this HttpContext httpContext) => httpContext.Items[(object) HttpContextConstants.IVssRequestContext] as IVssRequestContext;

    public static IVssRequestContext GetVssRequestContext(this HttpContextBase httpContext) => httpContext.Items[(object) HttpContextConstants.IVssRequestContext] as IVssRequestContext;

    public static IVssRequestContext GetVssRequestContext(this IRequest request)
    {
      HttpContextBase httpContext = request.GetHttpContext();
      return httpContext == null ? (IVssRequestContext) null : httpContext.GetVssRequestContext();
    }
  }
}
