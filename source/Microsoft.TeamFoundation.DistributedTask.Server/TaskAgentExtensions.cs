// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.TaskAgentExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class TaskAgentExtensions
  {
    private static bool s_routesRegistered = true;

    public static TaskAgentReference AsReference(this TaskAgent agent)
    {
      if (agent == null)
        return (TaskAgentReference) null;
      return new TaskAgentReference()
      {
        Id = agent.Id,
        Name = agent.Name,
        Version = agent.Version,
        Enabled = agent.Enabled,
        Status = agent.Status,
        Links = agent.Links
      };
    }

    public static T PopulateReferenceLinks<T>(
      this T agent,
      IVssRequestContext requestContext,
      int poolId,
      IDictionary<int, T> resolvedAgents = null)
      where T : TaskAgentReference
    {
      if ((object) agent == null)
        return default (T);
      if (!TaskAgentExtensions.s_routesRegistered || agent.Links.Links.Count > 0)
        return agent;
      T obj;
      if (resolvedAgents != null && resolvedAgents.TryGetValue(agent.Id, out obj))
      {
        agent.Links = obj.Links;
        return agent;
      }
      try
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        if (!agent.Links.Links.ContainsKey("self"))
        {
          Uri resourceUri = service.GetResourceUri(requestContext, "distributedtask", TaskResourceIds.Agents, (object) new
          {
            poolId = poolId,
            agentId = agent.Id
          });
          agent.Links.AddLink("self", resourceUri.AbsoluteUri);
        }
        if (!agent.Links.Links.ContainsKey("web"))
        {
          string empty = string.Empty;
          string str = !requestContext.ExecutionEnvironment.IsOnPremisesDeployment ? service.DetermineAccessMapping(requestContext).AccessPoint : TaskAgentExtensions.GetCollectionUrl(requestContext.RootContext);
          if (!str.EndsWith("/", StringComparison.OrdinalIgnoreCase))
            str += "/";
          string href = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_settings/agentpools?view=jobs&poolId={1}&agentId={2}", (object) str, (object) poolId, (object) agent.Id);
          agent.Links.AddLink("web", href);
        }
        resolvedAgents?.Add(agent.Id, agent);
      }
      catch (VssResourceNotFoundException ex)
      {
        TaskAgentExtensions.s_routesRegistered = false;
      }
      return agent;
    }

    public static TaskAgent PopulateImplicitCapabilities(this TaskAgent agent)
    {
      if (agent == null)
        return (TaskAgent) null;
      agent.SystemCapabilities[PipelineConstants.AgentName] = agent.Name;
      if (!string.IsNullOrEmpty(agent.Version))
        agent.SystemCapabilities[PipelineConstants.AgentVersionDemandName] = agent.Version;
      return agent;
    }

    public static IDictionary<string, string> GetEffectiveCapabilities(this TaskAgent agent)
    {
      if (agent.UserCapabilities.Count == 0)
        return agent.SystemCapabilities;
      Dictionary<string, string> effectiveCapabilities = new Dictionary<string, string>(agent.SystemCapabilities, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (KeyValuePair<string, string> userCapability in (IEnumerable<KeyValuePair<string, string>>) agent.UserCapabilities)
        effectiveCapabilities[userCapability.Key] = userCapability.Value;
      return (IDictionary<string, string>) effectiveCapabilities;
    }

    internal static void PopulateProperties(
      this TaskAgent agent,
      IVssRequestContext requestContext,
      TaskAgentPoolData poolData)
    {
      using (new MethodScope(requestContext, "ResourceService", nameof (PopulateProperties)))
      {
        ILocationService service = requestContext.GetService<ILocationService>();
        agent.Properties["ServerUrl"] = (object) service.GetLocationServiceUrl(requestContext, Guid.Empty, AccessMappingConstants.ClientAccessMappingMoniker);
      }
    }

    private static string GetCollectionUrl(IVssRequestContext requestContext) => requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, ServiceInstanceTypes.TFS, AccessMappingConstants.PublicAccessMappingMoniker);

    private static void GrantPoolPermissions(
      IVssRequestContext requestContext,
      TaskAgentPoolData poolData,
      ServiceIdentity serviceIdentity)
    {
      requestContext.GetService<DistributedTaskResourceService>().GetAgentPoolSecurity(requestContext, poolData.Pool.Id).GrantListenPermissionToPool(requestContext, poolData.Pool.Id, serviceIdentity.Identity);
    }
  }
}
