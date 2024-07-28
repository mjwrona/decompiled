// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Directories.DiscoveryService.GitHubDirectoryRequestFilter
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Directories.DiscoveryService
{
  internal static class GitHubDirectoryRequestFilter
  {
    internal static bool AllowRequest(IVssRequestContext context, DirectoryInternalRequest request)
    {
      ArgumentUtility.CheckForNull<DirectoryInternalRequest>(request, nameof (request));
      return !context.ExecutionEnvironment.IsOnPremisesDeployment && !context.ServiceHost.Is(TeamFoundationHostType.Deployment) && !DirectoryUtils.IsOrganizationAadBacked(context) && context.IsFeatureEnabled("VisualStudio.Services.Directories.DiscoveryService.EnableGitHubDirectory") && request.Directories != null && request.Directories.Contains<string>("ghb");
    }
  }
}
