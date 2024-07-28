// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildQueueService
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.Common;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public sealed class BuildQueueService : IBuildQueueService, IVssFrameworkService
  {
    private readonly IBuildSecurityProvider SecurityProvider;

    public BuildQueueService()
      : this((IBuildSecurityProvider) new BuildSecurityProvider())
    {
    }

    internal BuildQueueService(IBuildSecurityProvider securityProvider) => this.SecurityProvider = securityProvider;

    public AgentPoolQueue GetQueue(IVssRequestContext requestContext, int queueId)
    {
      using (requestContext.TraceScope("Service", nameof (GetQueue)))
      {
        IProjectService service1 = requestContext.GetService<IProjectService>();
        IDistributedTaskPoolService service2 = requestContext.GetService<IDistributedTaskPoolService>();
        IVssRequestContext requestContext1 = requestContext;
        foreach (ProjectInfo project in service1.GetProjects(requestContext1))
        {
          TaskAgentQueue agentQueue = service2.GetAgentQueue(requestContext, project.Id, queueId);
          if (agentQueue != null)
            return agentQueue.AsServerBuildQueue();
        }
        return (AgentPoolQueue) null;
      }
    }

    public IEnumerable<AgentPoolQueue> GetQueues(IVssRequestContext requestContext, string name = "*")
    {
      using (requestContext.TraceScope("Service", nameof (GetQueues)))
      {
        List<TaskAgentQueue> source = new List<TaskAgentQueue>();
        HashSet<string> allNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
        IProjectService service1 = requestContext.GetService<IProjectService>();
        IDistributedTaskPoolService service2 = requestContext.GetService<IDistributedTaskPoolService>();
        IVssRequestContext requestContext1 = requestContext;
        foreach (ProjectInfo project in service1.GetProjects(requestContext1))
          source.AddRange(service2.GetAgentQueues(requestContext, project.Id, name).Where<TaskAgentQueue>((Func<TaskAgentQueue, bool>) (x => allNames.Add(x.Name))));
        return source.Select<TaskAgentQueue, AgentPoolQueue>((Func<TaskAgentQueue, AgentPoolQueue>) (x => x.AsServerBuildQueue()));
      }
    }

    public IEnumerable<AgentPoolQueue> GetQueuesByPoolId(
      IVssRequestContext requestContext,
      int poolId)
    {
      using (requestContext.TraceScope("Service", nameof (GetQueuesByPoolId)))
        return (IEnumerable<AgentPoolQueue>) requestContext.GetService<IDistributedTaskPoolService>().GetAgentQueues(requestContext, Guid.Empty).Where<TaskAgentQueue>((Func<TaskAgentQueue, bool>) (x => x.Pool != null && x.Pool.Id == poolId)).Select<TaskAgentQueue, AgentPoolQueue>((Func<TaskAgentQueue, AgentPoolQueue>) (x => x.AsServerBuildQueue())).ToList<AgentPoolQueue>();
    }

    public void DeleteQueues(IVssRequestContext requestContext, IEnumerable<int> queueIds)
    {
      using (requestContext.TraceScope("Service", nameof (DeleteQueues)))
      {
        this.SecurityProvider.CheckCollectionPermission(requestContext, AdministrationPermissions.ManageBuildResources);
        IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
        foreach (int queueId in queueIds)
          service.DeleteAgentQueue(requestContext, Guid.Empty, queueId);
      }
    }

    public AgentPoolQueue AddQueue(IVssRequestContext requestContext, AgentPoolQueue queue)
    {
      ArgumentUtility.CheckForNull<AgentPoolQueue>(queue, nameof (queue), "Build2");
      using (requestContext.TraceScope("Service", nameof (AddQueue)))
      {
        this.SecurityProvider.CheckCollectionPermission(requestContext, AdministrationPermissions.ManageBuildResources);
        if (queue.Pool == null)
        {
          TaskAgentPool taskAgentPool1 = this.CreateTaskAgentPool(requestContext, queue.Name);
          TaskAgentPool taskAgentPool2 = requestContext.GetService<IDistributedTaskPoolService>().AddAgentPool(requestContext.Elevate(), taskAgentPool1);
          queue.Pool = new TaskAgentPoolReference()
          {
            Id = taskAgentPool2.Id
          };
        }
        TaskAgentQueue queue1 = new TaskAgentQueue()
        {
          Name = queue.Name,
          Pool = new Microsoft.TeamFoundation.DistributedTask.WebApi.TaskAgentPoolReference()
          {
            Id = queue.Pool.Id
          }
        };
        try
        {
          queue = requestContext.GetService<IDistributedTaskPoolService>().AddAgentQueue(requestContext, Guid.Empty, queue1).AsServerBuildQueue();
        }
        catch (TaskAgentQueueExistsException ex)
        {
          throw new QueueExistsException(BuildServerResources.QueueExists((object) queue.Name), (Exception) ex);
        }
      }
      return queue;
    }

    private TaskAgentPool CreateTaskAgentPool(IVssRequestContext requestContext, string poolName)
    {
      using (requestContext.TraceScope("Service", nameof (CreateTaskAgentPool)))
      {
        TaskAgentPool taskAgentPool = new TaskAgentPool(poolName);
        IdentityDescriptor authenticatedDescriptor = requestContext.GetAuthenticatedDescriptor();
        if (authenticatedDescriptor != (IdentityDescriptor) null)
        {
          Microsoft.VisualStudio.Services.Identity.Identity identity = requestContext.GetService<IdentityService>().GetIdentity(requestContext, authenticatedDescriptor);
          taskAgentPool.CreatedBy = new IdentityRef()
          {
            Id = identity.Id.ToString("D")
          };
        }
        return taskAgentPool;
      }
    }

    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }
  }
}
