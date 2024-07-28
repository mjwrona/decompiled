// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebServer.AgentPoolQueueExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FDDA87C8-3548-4A75-AA18-4FB488450659
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebServer.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.WebServer
{
  internal static class AgentPoolQueueExtensions
  {
    public static void ResolveToProject(
      this Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue queue,
      IVssRequestContext requestContext,
      Guid projectId)
    {
      using (requestContext.TraceScope(nameof (AgentPoolQueueExtensions), nameof (ResolveToProject)))
      {
        IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
        if (queue.Id == 0)
          return;
        TaskAgentQueue agentQueue = service.GetAgentQueue(requestContext, projectId, queue.Id);
        if (agentQueue != null)
          return;
        if (!string.IsNullOrEmpty(queue.Name))
        {
          TaskAgentQueue taskAgentQueue = service.GetAgentQueues(requestContext, projectId, queue.Name).FirstOrDefault<TaskAgentQueue>();
          if (taskAgentQueue != null)
          {
            queue.Id = taskAgentQueue.Id;
            return;
          }
        }
        foreach (ProjectInfo project in requestContext.GetService<IProjectService>().GetProjects(requestContext, ProjectState.WellFormed))
        {
          if (project.Id != projectId)
          {
            agentQueue = service.GetAgentQueue(requestContext, project.Id, queue.Id);
            if (agentQueue != null)
              break;
          }
        }
        if (agentQueue == null)
          return;
        TaskAgentQueue taskAgentQueue1 = service.GetAgentQueues(requestContext, projectId, agentQueue.Name).FirstOrDefault<TaskAgentQueue>();
        if (taskAgentQueue1 == null)
          return;
        queue.Id = taskAgentQueue1.Id;
      }
    }
  }
}
