// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Routing.WrappedRequestContext
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Server.WebAccess.Routing
{
  internal class WrappedRequestContext : RequestContext
  {
    private RouteData m_routeData;
    private RequestContext m_context;

    public WrappedRequestContext(RequestContext context, RouteData routeData)
    {
      this.m_context = context != null ? context : throw new ArgumentNullException(nameof (context));
      if (routeData == null)
        this.m_routeData = this.m_context.RouteData.Clone();
      else
        this.m_routeData = routeData;
    }

    public WrappedRequestContext(RequestContext context)
      : this(context, (RouteData) null)
    {
    }

    public override HttpContextBase HttpContext
    {
      get => this.m_context.HttpContext;
      set => this.m_context.HttpContext = value;
    }

    public override RouteData RouteData
    {
      get => this.m_routeData;
      set => this.m_routeData = value;
    }
  }
}
