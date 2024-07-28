// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WebContextFactory
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class WebContextFactory
  {
    private const string c_webContextKey = "__webContext";
    private const string c_pageContextKey = "__webPageGlobalContext";
    private const string c_contributedServiceContextKey = "__webContributedServiceContext";
    private static IWebContextProvider s_webContextProvider = (IWebContextProvider) new DefaultWebContextProvider();

    public static void SetContextProvider(IWebContextProvider provider) => WebContextFactory.s_webContextProvider = provider;

    public static T GetWebContext<T>(RequestContext requestContext) where T : WebContext => (T) WebContextFactory.GetWebContext(requestContext);

    public static WebContext GetWebContext(IVssRequestContext requestContext, bool create = true)
    {
      IWebRequestContextInternal requestContextInternal = requestContext.WebRequestContextInternal(create);
      return requestContextInternal != null ? WebContextFactory.GetWebContext(requestContextInternal.HttpContext.Request.RequestContext, create) : (WebContext) null;
    }

    public static WebContext GetWebContext(RequestContext requestContext, bool create = true)
    {
      WebContext webContext = requestContext.HttpContext.Items[(object) "__webContext"] as WebContext;
      if (create && webContext == null)
      {
        webContext = WebContextFactory.s_webContextProvider.CreateWebContext(requestContext);
        webContext.Initialize();
        requestContext.HttpContext.Items[(object) "__webContext"] = (object) webContext;
      }
      return webContext;
    }

    public static PageContext GetPageContext(IVssRequestContext requestContext, bool create = true) => WebContextFactory.GetPageContext(requestContext.WebRequestContextInternal().HttpContext.Request.RequestContext, create);

    public static PageContext GetPageContext(RequestContext requestContext, bool create = true)
    {
      PageContext pageContext = requestContext.HttpContext.Items[(object) "__webPageGlobalContext"] as PageContext;
      if (create && pageContext == null)
      {
        pageContext = WebContextFactory.s_webContextProvider.CreatePageContext(requestContext);
        requestContext.HttpContext.Items[(object) "__webPageGlobalContext"] = (object) pageContext;
      }
      return pageContext;
    }

    public static ContributedServiceContext GetContributedServiceContext(
      RequestContext requestContext)
    {
      if (!(requestContext.HttpContext.Items[(object) "__webContributedServiceContext"] is ContributedServiceContext contributedServiceContext))
      {
        contributedServiceContext = WebContextFactory.s_webContextProvider.CreateContributedServiceContext(requestContext);
        requestContext.HttpContext.Items[(object) "__webContributedServiceContext"] = (object) contributedServiceContext;
      }
      return contributedServiceContext;
    }

    public static WebContext GetCurrentRequestWebContext() => WebContextFactory.GetCurrentRequestWebContext<WebContext>();

    public static T GetCurrentRequestWebContext<T>(bool createNew = true) where T : WebContext
    {
      T requestWebContext = default (T);
      HttpContext current = HttpContext.Current;
      if (current != null)
        requestWebContext = !createNew ? current.Items[(object) "__webContext"] as T : WebContextFactory.GetWebContext(current.Request.RequestContext) as T;
      return requestWebContext;
    }
  }
}
