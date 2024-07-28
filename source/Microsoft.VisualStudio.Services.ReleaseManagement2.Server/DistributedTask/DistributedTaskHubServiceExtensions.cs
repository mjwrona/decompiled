// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask.DistributedTaskHubServiceExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask
{
  public static class DistributedTaskHubServiceExtensions
  {
    public const string ReleaseHubName = "Release";
    public const string DeploymentHubName = "Deployment";
    public const string GreenlightingHubName = "Gates";

    public static TaskHub GetReleaseTaskHub(
      this IDistributedTaskHubService hubService,
      IVssRequestContext requestContext)
    {
      return hubService.GetTaskHub(requestContext, "Release");
    }

    public static TaskHub GetDeploymentTaskHub(
      this IDistributedTaskHubService hubService,
      IVssRequestContext requestContext)
    {
      return hubService.GetTaskHub(requestContext, "Deployment");
    }

    public static TaskHub GetGreenlightingHub(
      this IDistributedTaskHubService hubService,
      IVssRequestContext requestContext)
    {
      return hubService.GetTaskHub(requestContext, "Gates");
    }
  }
}
