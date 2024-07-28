// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UrlRoutingModule
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Diagnostics;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  internal class UrlRoutingModule : System.Web.Routing.UrlRoutingModule
  {
    public override void PostResolveRequestCache(HttpContextBase context)
    {
      if (context.Items.Contains((object) HttpContextConstants.ArrRequestRouted))
      {
        IVssRequestContext requestContext = (IVssRequestContext) context.Items[(object) "IVssRequestContext"];
        if (requestContext == null)
          return;
        requestContext.Trace(1050001, TraceLevel.Info, HostRoutingService.Area, HostRoutingService.Layer, "Skipping UrlRoutingModule.PostResolveRequestCache due to ARR request");
      }
      else
        base.PostResolveRequestCache(context);
    }
  }
}
