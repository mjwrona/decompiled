// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentCloudInternalExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.OAuth2;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.MachineManagement.WebApi;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskAgentCloudInternalExtensions
  {
    public static VssCredentials GetInternalAgentCloudCredentials(
      this TaskAgentCloud agentCloud,
      IVssRequestContext requestContext)
    {
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment).Elevate();
      return vssRequestContext.GetService<IS2SCredentialsService>().GetS2SCredentials(vssRequestContext, MmsResourceIds.InstanceTypeGuid);
    }
  }
}
