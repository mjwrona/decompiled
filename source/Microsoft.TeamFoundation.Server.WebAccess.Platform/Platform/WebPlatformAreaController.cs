// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Platform.WebPlatformAreaController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Platform
{
  public abstract class WebPlatformAreaController : WebPlatformController
  {
    private WebContext m_webContext;

    public abstract string AreaName { get; }

    protected override void Initialize(RequestContext requestContext)
    {
      base.Initialize(requestContext);
      if (requestContext.RouteData.Values.ContainsKey("area"))
        requestContext.RouteData.Values["area"] = (object) this.AreaName;
      else
        requestContext.RouteData.DataTokens["area"] = (object) this.AreaName;
    }

    public virtual WebContext WebContext
    {
      get
      {
        if (this.m_webContext == null)
          this.m_webContext = this.Request.RequestContext.WebContext();
        return this.m_webContext;
      }
    }

    public NavigationContext NavigationContext => this.WebContext.NavigationContext;
  }
}
