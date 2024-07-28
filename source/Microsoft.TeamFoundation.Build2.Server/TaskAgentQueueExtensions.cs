// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TaskAgentQueueExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class TaskAgentQueueExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue AsBuildQueue(
      this TaskAgentQueue queue,
      IVssRequestContext requestContext,
      ISecuredObject securedObject = null,
      bool addQueueUrl = true)
    {
      return queue == null ? (Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue) null : queue.CopyTo(requestContext, new Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue(securedObject), addQueueUrl);
    }

    public static Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue CopyTo(
      this TaskAgentQueue agentQueue,
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue buildQueue,
      bool addQueueUrl = true)
    {
      if (agentQueue == null)
        return buildQueue;
      buildQueue.Id = agentQueue.Id;
      buildQueue.Name = agentQueue.Name;
      if (addQueueUrl)
      {
        IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
        buildQueue.Url = service.GetQueueRestUrl(requestContext, agentQueue.Id);
      }
      if (agentQueue.Pool != null)
        buildQueue.Pool = new Microsoft.TeamFoundation.Build.WebApi.TaskAgentPoolReference((ISecuredObject) buildQueue)
        {
          Id = agentQueue.Pool.Id,
          Name = agentQueue.Pool.Name,
          IsHosted = agentQueue.Pool.IsHosted
        };
      return buildQueue;
    }

    public static AgentPoolQueue AsServerBuildQueue(this TaskAgentQueue queue) => queue == null ? (AgentPoolQueue) null : queue.CopyTo(new AgentPoolQueue());

    public static AgentPoolQueue CopyTo(this TaskAgentQueue agentQueue, AgentPoolQueue buildQueue)
    {
      if (agentQueue == null)
        return buildQueue;
      buildQueue.Id = agentQueue.Id;
      buildQueue.Name = agentQueue.Name;
      if (agentQueue.Pool != null)
        buildQueue.Pool = new TaskAgentPoolReference()
        {
          Id = agentQueue.Pool.Id,
          Name = agentQueue.Pool.Name,
          IsHosted = agentQueue.Pool.IsHosted
        };
      return buildQueue;
    }
  }
}
