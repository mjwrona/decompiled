// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.WindowsMachineDirectoryRequestFilter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class WindowsMachineDirectoryRequestFilter
  {
    internal const string WindowsMachineDirectoryIgnoreFlagName = "DirectoryDiscovery.WindowsMachineDirectory.Ignore";

    internal static bool AllowRequest(IVssRequestContext context, DirectoryInternalRequest request)
    {
      if (!context.ExecutionEnvironment.IsOnPremisesDeployment || !request.Directories.Contains<string>("wmd"))
        return false;
      IVssRequestContext vssRequestContext = context.To(TeamFoundationHostType.Deployment);
      return !vssRequestContext.GetService<ITeamFoundationFeatureAvailabilityService>().IsFeatureEnabled(vssRequestContext, "DirectoryDiscovery.WindowsMachineDirectory.Ignore");
    }
  }
}
