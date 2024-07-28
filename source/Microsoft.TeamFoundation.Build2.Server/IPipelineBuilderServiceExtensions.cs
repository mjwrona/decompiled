// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IPipelineBuilderServiceExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class IPipelineBuilderServiceExtensions
  {
    public static BuildProcessResources AuthorizeResources(
      this IPipelineBuilderService builderService,
      IVssRequestContext requestContext,
      Guid projectId,
      BuildProcessResources resourcesToAuthorize,
      ResourceActionFilter actionFilter = ResourceActionFilter.Use,
      bool shouldAuthorizeResource = true)
    {
      if (resourcesToAuthorize == null || resourcesToAuthorize.Count == 0)
        return new BuildProcessResources();
      IResourceStore resourceStore = builderService.GetResourceStore(requestContext, projectId, new PipelineResources(), true);
      BuildProcessResources processResources1 = new BuildProcessResources();
      PipelineResources pipelineResources = resourcesToAuthorize.ToPipelineResources();
      if (pipelineResources.Endpoints.Count > 0)
      {
        IList<ServiceEndpoint> source = resourceStore.Endpoints.Resolver.Resolve((ICollection<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>) pipelineResources.Endpoints, actionFilter);
        if (source != null)
        {
          string str = string.Join<Guid>(", ", source.Select<ServiceEndpoint, Guid>((Func<ServiceEndpoint, Guid>) (x => x.Id)));
          requestContext.TraceInfo(12030205, "Service", "resourcesToAuthorize count {0} value {1}", (object) source.Count, (object) str);
          foreach (ServiceEndpoint serviceEndpoint in (IEnumerable<ServiceEndpoint>) source)
          {
            BuildProcessResources processResources2 = processResources1;
            ServiceEndpointReference endpointReference = new ServiceEndpointReference();
            endpointReference.Id = serviceEndpoint.Id;
            endpointReference.Name = serviceEndpoint.Name;
            endpointReference.Authorized = shouldAuthorizeResource;
            processResources2.Add((ResourceReference) endpointReference);
          }
        }
      }
      if (pipelineResources.Files.Count > 0)
      {
        foreach (SecureFile secureFile in (IEnumerable<SecureFile>) resourceStore.Files.Resolver.Resolve((ICollection<Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference>) pipelineResources.Files, actionFilter))
        {
          BuildProcessResources processResources3 = processResources1;
          SecureFileReference secureFileReference = new SecureFileReference();
          secureFileReference.Id = secureFile.Id;
          secureFileReference.Name = secureFile.Name;
          secureFileReference.Authorized = shouldAuthorizeResource;
          processResources3.Add((ResourceReference) secureFileReference);
        }
      }
      if (pipelineResources.Queues.Count > 0)
      {
        foreach (TaskAgentQueue taskAgentQueue in (IEnumerable<TaskAgentQueue>) resourceStore.Queues.Resolver.Resolve((ICollection<AgentQueueReference>) pipelineResources.Queues, actionFilter))
        {
          BuildProcessResources processResources4 = processResources1;
          AgentPoolQueueReference poolQueueReference = new AgentPoolQueueReference();
          poolQueueReference.Id = taskAgentQueue.Id;
          poolQueueReference.Name = taskAgentQueue.Name;
          poolQueueReference.Authorized = shouldAuthorizeResource;
          processResources4.Add((ResourceReference) poolQueueReference);
        }
      }
      if (pipelineResources.VariableGroups.Count > 0)
      {
        foreach (Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup variableGroup in (IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup>) resourceStore.VariableGroups.Resolver.Resolve((ICollection<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference>) pipelineResources.VariableGroups, actionFilter))
        {
          BuildProcessResources processResources5 = processResources1;
          VariableGroupReference variableGroupReference = new VariableGroupReference();
          variableGroupReference.Id = variableGroup.Id;
          variableGroupReference.Name = variableGroup.Name;
          variableGroupReference.Authorized = shouldAuthorizeResource;
          processResources5.Add((ResourceReference) variableGroupReference);
        }
      }
      return processResources1;
    }
  }
}
