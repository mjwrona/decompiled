// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.IAgentCloudServiceExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class IAgentCloudServiceExtensions
  {
    internal static TaskAgentCloud AddAgentCloud(
      this IAgentCloudService agentCloudService,
      IVssRequestContext requestContext,
      TaskAgentCloud agentCloud)
    {
      return requestContext.RunSynchronously<TaskAgentCloud>((Func<Task<TaskAgentCloud>>) (() => agentCloudService.AddAgentCloudAsync(requestContext, agentCloud)));
    }

    internal static IList<TaskAgentCloud> GetAgentClouds(
      this IAgentCloudService agentCloudService,
      IVssRequestContext requestContext)
    {
      return requestContext.RunSynchronously<IList<TaskAgentCloud>>((Func<Task<IList<TaskAgentCloud>>>) (() => agentCloudService.GetAgentCloudsAsync(requestContext)));
    }
  }
}
