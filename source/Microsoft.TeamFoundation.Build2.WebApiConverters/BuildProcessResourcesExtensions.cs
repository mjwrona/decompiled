// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.BuildProcessResourcesExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class BuildProcessResourcesExtensions
  {
    public static List<DefinitionResourceReference> ToDefinitionResourcesList(
      this Microsoft.TeamFoundation.Build2.Server.BuildProcessResources source,
      IVssRequestContext requestContext,
      Guid projectId,
      ISecuredObject securedObject)
    {
      if (source == null)
        return new List<DefinitionResourceReference>();
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      using (PerformanceTimer.StartMeasure(requestContext, "BuildProcessResourcesExtensions.ToDefinitionResourcesList"))
      {
        List<Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueReference> list = source.Queues.Where<Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueReference>((Func<Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueReference, bool>) (q => string.IsNullOrEmpty(q.Name))).ToList<Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueReference>();
        if (list.Count > 0)
        {
          using (PerformanceTimer.StartMeasure(requestContext, "BuildProcessResourcesExtensions.ToDefinitionResourcesList.QueueNames"))
          {
            Dictionary<int, TaskAgentQueue> dictionary = requestContext.GetService<IDistributedTaskPoolService>().GetAgentQueues(requestContext.Elevate(), projectId, list.Select<Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueReference, int>((Func<Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueReference, int>) (q => q.Id))).ToDictionary<TaskAgentQueue, int>((Func<TaskAgentQueue, int>) (q => q.Id));
            foreach (Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueReference poolQueueReference in list)
            {
              TaskAgentQueue taskAgentQueue;
              if (dictionary.TryGetValue(poolQueueReference.Id, out taskAgentQueue))
                poolQueueReference.Name = taskAgentQueue.Name;
            }
          }
        }
        List<DefinitionResourceReference> definitionResourcesList = new List<DefinitionResourceReference>();
        foreach (Microsoft.TeamFoundation.Build2.Server.ResourceReference allResource in source.AllResources)
          definitionResourcesList.Add(new DefinitionResourceReference()
          {
            Name = allResource.Name,
            Authorized = allResource.Authorized,
            Id = allResource.GetId(),
            Type = allResource.Type
          });
        return definitionResourcesList;
      }
    }

    public static Microsoft.TeamFoundation.Build2.Server.BuildProcessResources ToBuildProcessResources(
      this IEnumerable<DefinitionResourceReference> source)
    {
      Microsoft.TeamFoundation.Build2.Server.BuildProcessResources processResources = new Microsoft.TeamFoundation.Build2.Server.BuildProcessResources();
      if (source != null)
      {
        foreach (DefinitionResourceReference resourceReference in source)
        {
          if (string.Equals(resourceReference.Type, "endpoint", StringComparison.OrdinalIgnoreCase))
          {
            Guid guid = Guid.Parse(resourceReference.Id);
            ISet<Microsoft.TeamFoundation.Build2.Server.ServiceEndpointReference> endpoints = processResources.Endpoints;
            Microsoft.TeamFoundation.Build2.Server.ServiceEndpointReference endpointReference = new Microsoft.TeamFoundation.Build2.Server.ServiceEndpointReference();
            endpointReference.Authorized = resourceReference.Authorized;
            endpointReference.Id = guid;
            endpointReference.Name = resourceReference.Name;
            endpoints.Add(endpointReference);
          }
          else if (string.Equals(resourceReference.Type, "queue", StringComparison.OrdinalIgnoreCase))
          {
            int num = int.Parse(resourceReference.Id);
            ISet<Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueReference> queues = processResources.Queues;
            Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueReference poolQueueReference = new Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueReference();
            poolQueueReference.Authorized = resourceReference.Authorized;
            poolQueueReference.Id = num;
            poolQueueReference.Name = resourceReference.Name;
            queues.Add(poolQueueReference);
          }
          else if (string.Equals(resourceReference.Type, "securefile", StringComparison.OrdinalIgnoreCase))
          {
            Guid guid = Guid.Parse(resourceReference.Id);
            ISet<Microsoft.TeamFoundation.Build2.Server.SecureFileReference> files = processResources.Files;
            Microsoft.TeamFoundation.Build2.Server.SecureFileReference secureFileReference = new Microsoft.TeamFoundation.Build2.Server.SecureFileReference();
            secureFileReference.Authorized = resourceReference.Authorized;
            secureFileReference.Id = guid;
            secureFileReference.Name = resourceReference.Name;
            files.Add(secureFileReference);
          }
          else if (string.Equals(resourceReference.Type, "variablegroup", StringComparison.OrdinalIgnoreCase))
          {
            int num = int.Parse(resourceReference.Id);
            ISet<Microsoft.TeamFoundation.Build2.Server.VariableGroupReference> variableGroups = processResources.VariableGroups;
            Microsoft.TeamFoundation.Build2.Server.VariableGroupReference variableGroupReference = new Microsoft.TeamFoundation.Build2.Server.VariableGroupReference();
            variableGroupReference.Authorized = resourceReference.Authorized;
            variableGroupReference.Id = num;
            variableGroupReference.Name = resourceReference.Name;
            variableGroups.Add(variableGroupReference);
          }
        }
      }
      return processResources;
    }
  }
}
