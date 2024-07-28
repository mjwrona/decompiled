// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.AgentPoolQueueExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class AgentPoolQueueExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue ToWebApiUnsecuredAgentPoolQueue(
      this Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue srvAgentPoolQueue,
      IVssRequestContext requestContext)
    {
      Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue unsecuredAgentPoolQueue = new Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue()
      {
        Id = srvAgentPoolQueue.Id,
        Name = srvAgentPoolQueue.Name,
        Url = srvAgentPoolQueue.GetRestUrl(requestContext)
      };
      if (srvAgentPoolQueue.Pool != null)
        unsecuredAgentPoolQueue.Pool = new Microsoft.TeamFoundation.Build.WebApi.TaskAgentPoolReference()
        {
          Id = srvAgentPoolQueue.Pool.Id,
          Name = srvAgentPoolQueue.Pool.Name,
          IsHosted = srvAgentPoolQueue.Pool.IsHosted
        };
      unsecuredAgentPoolQueue.Links.TryUnsecuredAddLink("self", unsecuredAgentPoolQueue.Url);
      return unsecuredAgentPoolQueue;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue ToWebApiAgentPoolQueue(
      this Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue srvAgentPoolQueue,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvAgentPoolQueue == null)
        return (Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue) null;
      using (PerformanceTimer.StartMeasure(requestContext, "AgentPoolQueueExtensions.ToWebApiAgentPoolQueue"))
      {
        Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue apiAgentPoolQueue = new Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue(securedObject)
        {
          Id = srvAgentPoolQueue.Id,
          Name = srvAgentPoolQueue.Name,
          Url = srvAgentPoolQueue.GetRestUrl(requestContext)
        };
        if (srvAgentPoolQueue.Pool != null)
          apiAgentPoolQueue.Pool = new Microsoft.TeamFoundation.Build.WebApi.TaskAgentPoolReference(securedObject)
          {
            Id = srvAgentPoolQueue.Pool.Id,
            Name = srvAgentPoolQueue.Pool.Name,
            IsHosted = srvAgentPoolQueue.Pool.IsHosted
          };
        apiAgentPoolQueue.Links.TryAddLink("self", securedObject, apiAgentPoolQueue.Url);
        return apiAgentPoolQueue;
      }
    }

    public static Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue ToBuildServerAgentPoolQueue(
      this Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueue webApiAgentPoolQueue)
    {
      if (webApiAgentPoolQueue == null)
        return (Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue) null;
      Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue serverAgentPoolQueue = new Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue()
      {
        Id = webApiAgentPoolQueue.Id,
        Name = webApiAgentPoolQueue.Name
      };
      if (webApiAgentPoolQueue.Pool != null)
        serverAgentPoolQueue.Pool = new Microsoft.TeamFoundation.Build2.Server.TaskAgentPoolReference()
        {
          Id = webApiAgentPoolQueue.Pool.Id,
          Name = webApiAgentPoolQueue.Pool.Name,
          IsHosted = webApiAgentPoolQueue.Pool.IsHosted
        };
      return serverAgentPoolQueue;
    }

    public static string GetRestUrl(
      this Microsoft.TeamFoundation.Build2.Server.AgentPoolQueue agentPoolQueue,
      IVssRequestContext requestContext)
    {
      using (PerformanceTimer.StartMeasure(requestContext, "AgentPoolQueueExtensions.GetRestUrl"))
        return requestContext.GetService<IBuildRouteService>().GetQueueRestUrl(requestContext, agentPoolQueue.Id);
    }
  }
}
