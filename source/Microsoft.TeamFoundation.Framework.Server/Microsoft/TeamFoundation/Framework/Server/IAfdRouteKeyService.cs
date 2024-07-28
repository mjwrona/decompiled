// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IAfdRouteKeyService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation(typeof (AfdRouteKeyService))]
  internal interface IAfdRouteKeyService : IVssFrameworkService
  {
    void SetAfdRouteKeyHeaders(
      IVssRequestContext requestContext,
      HostRouteContext routeContext,
      HttpContextBase httpContext,
      IProxyData proxyData);

    void ResetAfdRouteKeyHeaders(IVssRequestContext requestContext, HttpContextBase httpContext);

    void RegisterRouteKeyHandler(IRouteKeyVersionHandler handler);
  }
}
