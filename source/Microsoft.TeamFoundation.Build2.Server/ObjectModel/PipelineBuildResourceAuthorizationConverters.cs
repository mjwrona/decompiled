// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.ObjectModel.PipelineBuildResourceAuthorizationConverters
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Authorization.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server.ObjectModel
{
  public static class PipelineBuildResourceAuthorizationConverters
  {
    public static BuildProcessResources ToBuildProcessResources(
      this PipelineProcessResources resources)
    {
      BuildProcessResources processResources = new BuildProcessResources();
      if (resources != null)
      {
        foreach (PipelineResourceReference resource in resources.Resources)
        {
          ResourceReference resourceReference = (ResourceReference) null;
          string type = resource.Type;
          string id = resource.Id;
          bool authorized = resource.Authorized;
          Guid? authorizedBy = resource.AuthorizedBy;
          DateTime? authorizedOn = resource.AuthorizedOn;
          int? definitionId = resource.DefinitionId;
          switch (type)
          {
            case "endpoint":
              ServiceEndpointReference endpointReference = new ServiceEndpointReference();
              endpointReference.Id = Guid.Parse(id);
              endpointReference.Authorized = authorized;
              endpointReference.AuthorizedBy = authorizedBy;
              endpointReference.AuthorizedOn = authorizedOn;
              endpointReference.DefinitionId = definitionId;
              resourceReference = (ResourceReference) endpointReference;
              break;
            case "queue":
              AgentPoolQueueReference poolQueueReference = new AgentPoolQueueReference();
              poolQueueReference.Id = int.Parse(id);
              poolQueueReference.Authorized = authorized;
              poolQueueReference.AuthorizedBy = authorizedBy;
              poolQueueReference.AuthorizedOn = authorizedOn;
              poolQueueReference.DefinitionId = definitionId;
              resourceReference = (ResourceReference) poolQueueReference;
              break;
            case "securefile":
              SecureFileReference secureFileReference = new SecureFileReference();
              secureFileReference.Id = Guid.Parse(id);
              secureFileReference.Authorized = authorized;
              secureFileReference.AuthorizedBy = authorizedBy;
              secureFileReference.AuthorizedOn = authorizedOn;
              secureFileReference.DefinitionId = definitionId;
              resourceReference = (ResourceReference) secureFileReference;
              break;
            case "variablegroup":
              VariableGroupReference variableGroupReference = new VariableGroupReference();
              variableGroupReference.Id = int.Parse(id);
              variableGroupReference.Authorized = authorized;
              variableGroupReference.AuthorizedBy = authorizedBy;
              variableGroupReference.AuthorizedOn = authorizedOn;
              variableGroupReference.DefinitionId = definitionId;
              resourceReference = (ResourceReference) variableGroupReference;
              break;
          }
          processResources?.Add(resourceReference);
        }
      }
      return processResources;
    }

    public static PipelineProcessResources ToPipelineProcessResources(
      this BuildProcessResources resources)
    {
      PipelineProcessResources processResources = new PipelineProcessResources();
      if (resources != null)
      {
        foreach (ServiceEndpointReference endpoint in (IEnumerable<ServiceEndpointReference>) resources.Endpoints)
        {
          PipelineResourceReference resourceReference = new PipelineResourceReference()
          {
            Type = "endpoint",
            Id = endpoint.Id.ToString("D"),
            Authorized = endpoint.Authorized
          };
          processResources.Add(resourceReference);
        }
        foreach (SecureFileReference file in (IEnumerable<SecureFileReference>) resources.Files)
        {
          PipelineResourceReference resourceReference = new PipelineResourceReference()
          {
            Type = "securefile",
            Id = file.Id.ToString("D"),
            Authorized = file.Authorized
          };
          processResources.Add(resourceReference);
        }
        foreach (AgentPoolQueueReference queue in (IEnumerable<AgentPoolQueueReference>) resources.Queues)
        {
          PipelineResourceReference resourceReference = new PipelineResourceReference()
          {
            Type = "queue",
            Id = queue.Id.ToString(),
            Authorized = queue.Authorized
          };
          processResources.Add(resourceReference);
        }
        foreach (VariableGroupReference variableGroup in (IEnumerable<VariableGroupReference>) resources.VariableGroups)
        {
          PipelineResourceReference resourceReference = new PipelineResourceReference()
          {
            Type = "variablegroup",
            Id = variableGroup.Id.ToString(),
            Authorized = variableGroup.Authorized
          };
          processResources.Add(resourceReference);
        }
      }
      return processResources;
    }

    public static Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType? ToPipelineResourceType(
      this Microsoft.TeamFoundation.Build2.Server.ResourceType? res)
    {
      Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType resourceType = (Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType) 0;
      if (res.HasValue)
      {
        switch (res.GetValueOrDefault())
        {
          case Microsoft.TeamFoundation.Build2.Server.ResourceType.ServiceEndpoint:
            resourceType = Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType.ServiceEndpoint;
            break;
          case Microsoft.TeamFoundation.Build2.Server.ResourceType.Queue:
            resourceType = Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType.Queue;
            break;
          case Microsoft.TeamFoundation.Build2.Server.ResourceType.SecureFile:
            resourceType = Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType.SecureFile;
            break;
          case Microsoft.TeamFoundation.Build2.Server.ResourceType.VariableGroup:
            resourceType = Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType.VariableGroup;
            break;
        }
      }
      return resourceType == (Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType) 0 ? new Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType?() : new Microsoft.Azure.Pipelines.Checks.WebApi.ResourceType?(resourceType);
    }
  }
}
