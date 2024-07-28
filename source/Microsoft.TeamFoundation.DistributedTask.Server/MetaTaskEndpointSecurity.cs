// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.MetaTaskEndpointSecurity
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  public static class MetaTaskEndpointSecurity
  {
    private const string c_ServiceEndPointTaskInputTypePrefix = "connectedService:";

    public static void CheckEndpointSecurityForMetaTaskSteps(
      IVssRequestContext requestContext,
      Guid projectId,
      IList<TaskGroupStep> taskGroupSteps)
    {
      IList<Guid> usedInMetaTaskSteps = MetaTaskEndpointSecurity.GetServiceEndpointsUsedInMetaTaskSteps(requestContext, taskGroupSteps);
      if (!usedInMetaTaskSteps.Any<Guid>())
        return;
      IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> source = MetaTaskEndpointSecurity.GetServiceEndpoints(requestContext, projectId, (IEnumerable<Guid>) usedInMetaTaskSteps).Where<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, bool>) (endpoint => endpoint.Url == (Uri) null));
      if (source.Any<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>())
        throw new UnauthorizedAccessException(TaskResources.MetaTaskServiceEndpointSecurityCheckFailedMessage((object) string.Join(", ", source.Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, string>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, string>) (endpoint => endpoint.Name)))));
    }

    private static IList<Guid> GetServiceEndpointsUsedInMetaTaskSteps(
      IVssRequestContext requestContext,
      IList<TaskGroupStep> steps)
    {
      IList<TaskDefinition> taskDefinitions = requestContext.GetService<DistributedTaskService>().GetTaskDefinitions(requestContext, new Guid?(), (TaskVersion) null, (IEnumerable<string>) null, false, false);
      HashSet<Guid> source = new HashSet<Guid>();
      foreach (TaskGroupStep step in (IEnumerable<TaskGroupStep>) steps)
      {
        TaskGroupStep task = step;
        TaskDefinition taskDefinition = taskDefinitions.FirstOrDefault<TaskDefinition>((Func<TaskDefinition, bool>) (definition =>
        {
          Guid? id1 = definition?.Id;
          Guid? id2 = task?.Task?.Id;
          if (id1.HasValue != id2.HasValue)
            return false;
          return !id1.HasValue || id1.GetValueOrDefault() == id2.GetValueOrDefault();
        }));
        if (taskDefinition != null)
        {
          foreach (string key in taskDefinition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (input => input.InputType.Contains("connectedService:"))).Select<TaskInputDefinition, string>((Func<TaskInputDefinition, string>) (input => input.Name)))
          {
            string str1;
            if (task.Inputs.TryGetValue(key, out str1) && str1 != null)
            {
              string str2 = str1;
              char[] chArray = new char[1]{ ',' };
              foreach (string str3 in str2.Split(chArray))
              {
                Guid result;
                if (Guid.TryParse(str3.Trim(), out result))
                  source.Add(result);
              }
            }
          }
        }
      }
      return (IList<Guid>) source.ToList<Guid>();
    }

    private static IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> GetServiceEndpoints(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<Guid> serviceEndpointsGuids)
    {
      return (IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) requestContext.GetService<IServiceEndpointService2>().QueryServiceEndpoints(requestContext, projectId, (string) null, (IEnumerable<string>) null, serviceEndpointsGuids, (string) null, false);
    }
  }
}
