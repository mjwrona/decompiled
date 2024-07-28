// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.DemandsRequestValidator
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public class DemandsRequestValidator : IBuildRequestValidator
  {
    private ValidationResult m_errorLevel;
    private bool m_requireOnlineAgent;

    public DemandsRequestValidator(bool reportWarningInsteadOfError = false, bool requireOnlineAgent = false)
    {
      this.m_errorLevel = reportWarningInsteadOfError ? ValidationResult.Warning : ValidationResult.Error;
      this.m_requireOnlineAgent = requireOnlineAgent;
    }

    public BuildRequestValidationResult ValidateRequest(
      IVssRequestContext requestContext,
      BuildRequestValidationContext validationContext)
    {
      ArgumentUtility.CheckForNull<BuildRequestValidationContext>(validationContext, nameof (validationContext));
      ArgumentUtility.CheckForNull<AgentPoolQueue>(validationContext.Queue, "validationContext.Queue");
      ArgumentUtility.CheckForNull<TaskAgentPoolReference>(validationContext.Queue.Pool, "validationContext.Queue.Pool");
      ArgumentUtility.CheckForNull<IOrchestrationProcess>(validationContext.Process, "validationContext.Container");
      List<HashSet<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>> source = new List<HashSet<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>>();
      if (validationContext.Process is TaskOrchestrationContainer process)
      {
        foreach (TaskOrchestrationJob orchestrationJob in process.GetJobs().OfType<TaskOrchestrationJob>())
        {
          if (!(orchestrationJob.ExecutionMode == "Server"))
          {
            List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> demandsWithoutVersion = orchestrationJob.Demands.Where<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand, bool>) (x => !this.IsAgentVersionDemand(x))).ToList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>();
            if (!source.Any<HashSet<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>>((Func<HashSet<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>, bool>) (hs => hs.IsSupersetOf((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) demandsWithoutVersion))))
            {
              source.Add(new HashSet<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>((IEnumerable<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) demandsWithoutVersion));
              IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
              IList<TaskAgent> agents = service.GetAgents(requestContext.Elevate(), validationContext.Queue.Pool.Id, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) demandsWithoutVersion);
              if (agents.Count == 0)
              {
                string str = !demandsWithoutVersion.Any<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>() ? BuildServerResources.AgentsNotRegistered() : BuildServerResources.DemandsNotSatisfied((object) string.Join(", ", demandsWithoutVersion.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand, string>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand, string>) (demand => demand.ToString())).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)));
                return new BuildRequestValidationResult()
                {
                  Result = this.m_errorLevel,
                  Message = str
                };
              }
              if (this.m_requireOnlineAgent && !agents.Any<TaskAgent>((Func<TaskAgent, bool>) (agent => agent.Status == TaskAgentStatus.Online)))
              {
                TaskAgentPool agentPool = service.GetAgentPool(requestContext.Elevate(), validationContext.Queue.Pool.Id);
                if (agentPool == null || !agentPool.IsHosted)
                  return new BuildRequestValidationResult()
                  {
                    Result = this.m_errorLevel,
                    Message = BuildServerResources.NoCapableAgentsOnline()
                  };
              }
            }
          }
        }
      }
      return new BuildRequestValidationResult()
      {
        Result = ValidationResult.OK,
        Message = string.Empty
      };
    }

    public List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> GetMatchingDemands(
      IVssRequestContext requestContext,
      BuildDeleteValidationContext validationContext,
      AgentPoolQueue queue)
    {
      ArgumentUtility.CheckForNull<BuildDeleteValidationContext>(validationContext, nameof (validationContext));
      ArgumentUtility.CheckForNull<List<List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>>>(validationContext.DemandsValidationSet, "validationContext.DemandsValidationSet");
      foreach (List<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> demandsValidation in validationContext.DemandsValidationSet)
      {
        if (this.CheckIfMatchingAgentExist(requestContext, queue.Pool.Id, (IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand>) demandsValidation))
        {
          requestContext.TraceInfo(nameof (DemandsRequestValidator), "Selected demands {0}", (object) string.Join(",", (IEnumerable<string>) ((demandsValidation != null ? (object) demandsValidation.Select<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand, string>((Func<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand, string>) (item => item.Name)) : (object) null) ?? (object) Array.Empty<string>())));
          return demandsValidation;
        }
      }
      string str = validationContext.Build.GetProjectName(requestContext) ?? string.Empty;
      throw new AgentsNotFoundException(BuildServerResources.NoCapableAgentsExist((object) validationContext.Build.BuildNumber, (object) validationContext.Definition.Name, (object) str));
    }

    private bool IsAgentVersionDemand(Microsoft.TeamFoundation.DistributedTask.WebApi.Demand demand) => demand is DemandMinimumVersion && demand.Name.Equals(PipelineConstants.AgentVersionDemandName, StringComparison.OrdinalIgnoreCase);

    private bool CheckIfMatchingAgentExist(
      IVssRequestContext requestContext,
      int poolId,
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.Demand> demands)
    {
      IList<TaskAgent> agents = requestContext.GetService<IDistributedTaskPoolService>().GetAgents(requestContext.Elevate(), poolId, demands);
      return agents.Count != 0 && agents.Any<TaskAgent>((Func<TaskAgent, bool>) (agent => agent.Status == TaskAgentStatus.Online));
    }
  }
}
