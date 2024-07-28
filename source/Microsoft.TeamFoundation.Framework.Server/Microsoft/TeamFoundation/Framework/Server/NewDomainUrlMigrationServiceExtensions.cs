// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.NewDomainUrlMigrationServiceExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class NewDomainUrlMigrationServiceExtensions
  {
    private const string UseDevOpsDomainUrlsKey = "VisualStudio.Services.HostResolution.UseCodexDomainUrls";

    public static bool UseDevOpsDomainUrls(this IVssRequestContext requestContext) => requestContext.UseDevOpsDomainUrls(requestContext.ServiceHost.InstanceId);

    public static bool UseDevOpsDomainUrls(this IVssRequestContext requestContext, Guid hostId)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      NewDomainUrlMigrationService service = vssRequestContext.GetService<NewDomainUrlMigrationService>();
      if (!(requestContext.ServiceHost.InstanceId == hostId))
        return service.UseDevOpsDomainUrls(vssRequestContext, hostId);
      bool flag;
      if (!requestContext.TryGetItem<bool>("VisualStudio.Services.HostResolution.UseCodexDomainUrls", out flag))
      {
        flag = service.UseDevOpsDomainUrls(vssRequestContext, hostId);
        requestContext.Items["VisualStudio.Services.HostResolution.UseCodexDomainUrls"] = (object) flag;
      }
      return flag;
    }
  }
}
