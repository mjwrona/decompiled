// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.PipelineResourcesExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class PipelineResourcesExtensions
  {
    public static PipelineResources ToPipelineResources(this BuildProcessResources resources)
    {
      if (resources == null)
        return (PipelineResources) null;
      PipelineResources pipelineResources = new PipelineResources();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      pipelineResources.Endpoints.AddRange<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference, ISet<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>>(resources.Endpoints.Select<ServiceEndpointReference, Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>(PipelineResourcesExtensions.\u003C\u003EO.\u003C0\u003E__Convert ?? (PipelineResourcesExtensions.\u003C\u003EO.\u003C0\u003E__Convert = new Func<ServiceEndpointReference, Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference>(PipelineResourcesExtensions.Convert))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      pipelineResources.Files.AddRange<Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference, ISet<Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference>>(resources.Files.Select<SecureFileReference, Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference>(PipelineResourcesExtensions.\u003C\u003EO.\u003C1\u003E__Convert ?? (PipelineResourcesExtensions.\u003C\u003EO.\u003C1\u003E__Convert = new Func<SecureFileReference, Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference>(PipelineResourcesExtensions.Convert))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      pipelineResources.Queues.AddRange<AgentQueueReference, ISet<AgentQueueReference>>(resources.Queues.Select<AgentPoolQueueReference, AgentQueueReference>(PipelineResourcesExtensions.\u003C\u003EO.\u003C2\u003E__Convert ?? (PipelineResourcesExtensions.\u003C\u003EO.\u003C2\u003E__Convert = new Func<AgentPoolQueueReference, AgentQueueReference>(PipelineResourcesExtensions.Convert))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      pipelineResources.VariableGroups.AddRange<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference, ISet<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference>>(resources.VariableGroups.Select<VariableGroupReference, Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference>(PipelineResourcesExtensions.\u003C\u003EO.\u003C3\u003E__Convert ?? (PipelineResourcesExtensions.\u003C\u003EO.\u003C3\u003E__Convert = new Func<VariableGroupReference, Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference>(PipelineResourcesExtensions.Convert))));
      return pipelineResources;
    }

    public static BuildProcessResources ToBuildProcessResources(this PipelineResources resources)
    {
      if (resources == null)
        return (BuildProcessResources) null;
      BuildProcessResources processResources = new BuildProcessResources();
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      processResources.Endpoints.AddRange<ServiceEndpointReference, ISet<ServiceEndpointReference>>(resources.Endpoints.Select<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference, ServiceEndpointReference>(PipelineResourcesExtensions.\u003C\u003EO.\u003C4\u003E__Convert ?? (PipelineResourcesExtensions.\u003C\u003EO.\u003C4\u003E__Convert = new Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference, ServiceEndpointReference>(PipelineResourcesExtensions.Convert))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      processResources.Files.AddRange<SecureFileReference, ISet<SecureFileReference>>(resources.Files.Select<Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference, SecureFileReference>(PipelineResourcesExtensions.\u003C\u003EO.\u003C5\u003E__Convert ?? (PipelineResourcesExtensions.\u003C\u003EO.\u003C5\u003E__Convert = new Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference, SecureFileReference>(PipelineResourcesExtensions.Convert))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      processResources.Queues.AddRange<AgentPoolQueueReference, ISet<AgentPoolQueueReference>>(resources.Queues.Select<AgentQueueReference, AgentPoolQueueReference>(PipelineResourcesExtensions.\u003C\u003EO.\u003C6\u003E__Convert ?? (PipelineResourcesExtensions.\u003C\u003EO.\u003C6\u003E__Convert = new Func<AgentQueueReference, AgentPoolQueueReference>(PipelineResourcesExtensions.Convert))));
      // ISSUE: reference to a compiler-generated field
      // ISSUE: reference to a compiler-generated field
      processResources.VariableGroups.AddRange<VariableGroupReference, ISet<VariableGroupReference>>(resources.VariableGroups.Select<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference, VariableGroupReference>(PipelineResourcesExtensions.\u003C\u003EO.\u003C7\u003E__Convert ?? (PipelineResourcesExtensions.\u003C\u003EO.\u003C7\u003E__Convert = new Func<Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference, VariableGroupReference>(PipelineResourcesExtensions.Convert))));
      return processResources;
    }

    private static ServiceEndpointReference Convert(Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference resource)
    {
      ServiceEndpointReference endpointReference = new ServiceEndpointReference();
      endpointReference.Id = resource.Id;
      endpointReference.Name = resource.Name?.ToString();
      return endpointReference;
    }

    private static AgentPoolQueueReference Convert(AgentQueueReference resource)
    {
      AgentPoolQueueReference poolQueueReference = new AgentPoolQueueReference();
      poolQueueReference.Id = resource.Id;
      poolQueueReference.Name = resource.Name?.ToString();
      return poolQueueReference;
    }

    private static SecureFileReference Convert(Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference resource) => new SecureFileReference()
    {
      Id = resource.Id
    };

    public static VariableGroupReference Convert(Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference resource)
    {
      VariableGroupReference variableGroupReference = new VariableGroupReference();
      variableGroupReference.Id = resource.Id;
      variableGroupReference.Name = resource.Name?.ToString();
      return variableGroupReference;
    }

    private static Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference Convert(
      ServiceEndpointReference resource)
    {
      Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference endpointReference = new Microsoft.TeamFoundation.DistributedTask.Pipelines.ServiceEndpointReference();
      endpointReference.Id = resource.Id;
      endpointReference.Name = (ExpressionValue<string>) resource.Name;
      return endpointReference;
    }

    private static AgentQueueReference Convert(AgentPoolQueueReference resource)
    {
      AgentQueueReference agentQueueReference = new AgentQueueReference();
      agentQueueReference.Id = resource.Id;
      agentQueueReference.Name = (ExpressionValue<string>) resource.Name;
      return agentQueueReference;
    }

    private static Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference Convert(
      SecureFileReference resource)
    {
      return new Microsoft.TeamFoundation.DistributedTask.Pipelines.SecureFileReference() { Id = resource.Id };
    }

    private static Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference Convert(
      VariableGroupReference resource)
    {
      Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference variableGroupReference = new Microsoft.TeamFoundation.DistributedTask.Pipelines.VariableGroupReference();
      variableGroupReference.Id = resource.Id;
      variableGroupReference.Name = (ExpressionValue<string>) resource.Name;
      return variableGroupReference;
    }
  }
}
