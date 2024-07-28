// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RoutingExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class RoutingExtensions
  {
    public static string VirtualPath(
      this HostProperties hostProperties,
      IVssRequestContext requestContext)
    {
      if (requestContext.ServiceHost.InstanceId == hostProperties.Id)
        return requestContext.VirtualPath();
      if (requestContext.RootContext.ServiceHost.InstanceId == hostProperties.Id)
        return requestContext.RootContext.VirtualPath();
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IHostUriData uriData = vssRequestContext.GetService<IInternalUrlHostResolutionService>().ResolveUriData(vssRequestContext, hostProperties.Id);
      return uriData == null ? (string) null : uriData.AbsoluteVirtualPath();
    }

    internal static bool IsVirtualServiceHost(this HostProperties hostProperties) => hostProperties != null && hostProperties.DatabaseId == -2 && hostProperties.StorageAccountId == -2;
  }
}
