// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.IHostedTenantService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

namespace Microsoft.TeamFoundation.Framework.Server
{
  [DefaultServiceImplementation("Microsoft.VisualStudio.Services.Cloud.HostedTenantService, Microsoft.VisualStudio.Services.Cloud")]
  public interface IHostedTenantService : IVssFrameworkService
  {
    string DeploymentId { get; }

    string TenantName { get; }

    string GetProductionDeploymentId(IVssRequestContext requestContext);

    string GetProductionDeploymentIdFromDatabase(IVssRequestContext requestContext);

    bool CloseConnectionsForVipSwap(IVssRequestContext requestContext);

    bool CloseAllConnections(IVssRequestContext requestContext);

    bool ShouldJobAgentAquireJobs(IVssRequestContext requestContext);

    bool IsGoingOfflineTimeElapsed(IVssRequestContext requestContext);
  }
}
