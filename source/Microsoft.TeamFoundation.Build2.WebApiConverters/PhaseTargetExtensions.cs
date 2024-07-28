// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.WebApiConverters.PhaseTargetExtensions
// Assembly: Microsoft.TeamFoundation.Build2.WebApiConverters, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9963E502-0ADF-445A-89CE-AAA11161F2F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.WebApiConverters.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.WebApiConverters
{
  public static class PhaseTargetExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.PhaseTarget ToWebApiPhaseTarget(
      this Microsoft.TeamFoundation.Build2.Server.PhaseTarget srvPhaseTarget,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvPhaseTarget == null)
        return (Microsoft.TeamFoundation.Build.WebApi.PhaseTarget) null;
      Microsoft.TeamFoundation.Build.WebApi.PhaseTarget webApiPhaseTarget = (Microsoft.TeamFoundation.Build.WebApi.PhaseTarget) null;
      switch (srvPhaseTarget.Type)
      {
        case 1:
          return (Microsoft.TeamFoundation.Build.WebApi.PhaseTarget) (srvPhaseTarget as Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTarget).ToWebApiAgentPoolQueueTarget(requestContext, securedObject);
        case 2:
          return (Microsoft.TeamFoundation.Build.WebApi.PhaseTarget) (srvPhaseTarget as Microsoft.TeamFoundation.Build2.Server.ServerTarget).ToWebApiServerTarget(securedObject);
        default:
          return webApiPhaseTarget;
      }
    }

    public static Microsoft.TeamFoundation.Build2.Server.PhaseTarget ToServerPhaseTarget(
      this Microsoft.TeamFoundation.Build.WebApi.PhaseTarget webApiPhaseTarget)
    {
      Microsoft.TeamFoundation.Build2.Server.PhaseTarget serverPhaseTarget = (Microsoft.TeamFoundation.Build2.Server.PhaseTarget) null;
      if (webApiPhaseTarget == null)
        return (Microsoft.TeamFoundation.Build2.Server.PhaseTarget) null;
      switch (webApiPhaseTarget.Type)
      {
        case 1:
          return (Microsoft.TeamFoundation.Build2.Server.PhaseTarget) (webApiPhaseTarget as Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueueTarget).ToServerAgentPoolQueueTarget();
        case 2:
          Microsoft.TeamFoundation.Build.WebApi.ServerTarget webApiSrvTarget = webApiPhaseTarget as Microsoft.TeamFoundation.Build.WebApi.ServerTarget;
          if (webApiSrvTarget.ExecutionOptions == null)
            webApiSrvTarget.ExecutionOptions = new Microsoft.TeamFoundation.Build.WebApi.ServerTargetExecutionOptions();
          return (Microsoft.TeamFoundation.Build2.Server.PhaseTarget) webApiSrvTarget.ToServerServerTarget();
        default:
          return serverPhaseTarget;
      }
    }

    public static Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueueTarget ToWebApiAgentPoolQueueTarget(
      this Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTarget srvAgentPoolQueueTarget,
      IVssRequestContext requestContext,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvAgentPoolQueueTarget == null)
        return (Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueueTarget) null;
      Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueueTarget agentPoolQueueTarget1 = new Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueueTarget(securedObject)
      {
        AllowScriptsAuthAccessOption = srvAgentPoolQueueTarget.AllowScriptsAuthAccessOption
      };
      if (srvAgentPoolQueueTarget.Queue != null)
        agentPoolQueueTarget1.Queue = srvAgentPoolQueueTarget.Queue.ToWebApiAgentPoolQueue(requestContext, securedObject);
      if (srvAgentPoolQueueTarget.AgentSpecification != null)
        agentPoolQueueTarget1.AgentSpecification = srvAgentPoolQueueTarget.AgentSpecification.ToWebApiAgentSpecification(requestContext, securedObject);
      if (srvAgentPoolQueueTarget.Demands != null)
        agentPoolQueueTarget1.Demands.AddRange(srvAgentPoolQueueTarget.Demands.Select<Microsoft.TeamFoundation.Build2.Server.Demand, Microsoft.TeamFoundation.Build.WebApi.Demand>((Func<Microsoft.TeamFoundation.Build2.Server.Demand, Microsoft.TeamFoundation.Build.WebApi.Demand>) (x => x.ToWebApiDemand(securedObject))));
      if (srvAgentPoolQueueTarget.ExecutionOptions != null)
      {
        switch (srvAgentPoolQueueTarget.ExecutionOptions.Type)
        {
          case 1:
            Microsoft.TeamFoundation.Build2.Server.VariableMultipliersAgentExecutionOptions executionOptions1 = srvAgentPoolQueueTarget.ExecutionOptions as Microsoft.TeamFoundation.Build2.Server.VariableMultipliersAgentExecutionOptions;
            Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueueTarget agentPoolQueueTarget2 = agentPoolQueueTarget1;
            Microsoft.TeamFoundation.Build.WebApi.VariableMultipliersAgentExecutionOptions executionOptions2 = new Microsoft.TeamFoundation.Build.WebApi.VariableMultipliersAgentExecutionOptions(securedObject);
            executionOptions2.Type = executionOptions1.Type;
            executionOptions2.ContinueOnError = executionOptions1.ContinueOnError;
            executionOptions2.MaxConcurrency = executionOptions1.MaxConcurrency;
            executionOptions2.Multipliers = executionOptions1.Multipliers;
            agentPoolQueueTarget2.ExecutionOptions = (Microsoft.TeamFoundation.Build.WebApi.AgentTargetExecutionOptions) executionOptions2;
            break;
          case 2:
            Microsoft.TeamFoundation.Build2.Server.MultipleAgentExecutionOptions executionOptions3 = srvAgentPoolQueueTarget.ExecutionOptions as Microsoft.TeamFoundation.Build2.Server.MultipleAgentExecutionOptions;
            Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueueTarget agentPoolQueueTarget3 = agentPoolQueueTarget1;
            Microsoft.TeamFoundation.Build.WebApi.MultipleAgentExecutionOptions executionOptions4 = new Microsoft.TeamFoundation.Build.WebApi.MultipleAgentExecutionOptions(securedObject);
            executionOptions4.Type = 2;
            executionOptions4.ContinueOnError = executionOptions3.ContinueOnError;
            executionOptions4.MaxConcurrency = executionOptions3.MaxConcurrency;
            agentPoolQueueTarget3.ExecutionOptions = (Microsoft.TeamFoundation.Build.WebApi.AgentTargetExecutionOptions) executionOptions4;
            break;
          default:
            Microsoft.TeamFoundation.Build2.Server.AgentTargetExecutionOptions executionOptions5 = srvAgentPoolQueueTarget.ExecutionOptions;
            agentPoolQueueTarget1.ExecutionOptions = new Microsoft.TeamFoundation.Build.WebApi.AgentTargetExecutionOptions(securedObject)
            {
              Type = 0
            };
            break;
        }
      }
      return agentPoolQueueTarget1;
    }

    public static Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTarget ToServerAgentPoolQueueTarget(
      this Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueueTarget webApiAgentPoolQueueTarget)
    {
      if (webApiAgentPoolQueueTarget == null)
        return (Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTarget) null;
      Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTarget agentPoolQueueTarget1 = new Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTarget()
      {
        AllowScriptsAuthAccessOption = webApiAgentPoolQueueTarget.AllowScriptsAuthAccessOption
      };
      if (webApiAgentPoolQueueTarget.Queue != null)
        agentPoolQueueTarget1.Queue = webApiAgentPoolQueueTarget.Queue.ToBuildServerAgentPoolQueue();
      if (webApiAgentPoolQueueTarget.AgentSpecification != null)
        agentPoolQueueTarget1.AgentSpecification = webApiAgentPoolQueueTarget.AgentSpecification.ToServerAgentSpecification();
      if (webApiAgentPoolQueueTarget.Demands != null)
        agentPoolQueueTarget1.Demands.AddRange(webApiAgentPoolQueueTarget.Demands.Select<Microsoft.TeamFoundation.Build.WebApi.Demand, Microsoft.TeamFoundation.Build2.Server.Demand>((Func<Microsoft.TeamFoundation.Build.WebApi.Demand, Microsoft.TeamFoundation.Build2.Server.Demand>) (x => x.ToServerDemand())));
      if (webApiAgentPoolQueueTarget.ExecutionOptions != null)
      {
        switch (webApiAgentPoolQueueTarget.ExecutionOptions.Type)
        {
          case 1:
            Microsoft.TeamFoundation.Build.WebApi.VariableMultipliersAgentExecutionOptions executionOptions1 = webApiAgentPoolQueueTarget.ExecutionOptions as Microsoft.TeamFoundation.Build.WebApi.VariableMultipliersAgentExecutionOptions;
            Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTarget agentPoolQueueTarget2 = agentPoolQueueTarget1;
            Microsoft.TeamFoundation.Build2.Server.VariableMultipliersAgentExecutionOptions executionOptions2 = new Microsoft.TeamFoundation.Build2.Server.VariableMultipliersAgentExecutionOptions();
            executionOptions2.Type = 1;
            executionOptions2.ContinueOnError = executionOptions1.ContinueOnError;
            executionOptions2.MaxConcurrency = executionOptions1.MaxConcurrency;
            executionOptions2.Multipliers = executionOptions1.Multipliers;
            agentPoolQueueTarget2.ExecutionOptions = (Microsoft.TeamFoundation.Build2.Server.AgentTargetExecutionOptions) executionOptions2;
            break;
          case 2:
            Microsoft.TeamFoundation.Build.WebApi.MultipleAgentExecutionOptions executionOptions3 = webApiAgentPoolQueueTarget.ExecutionOptions as Microsoft.TeamFoundation.Build.WebApi.MultipleAgentExecutionOptions;
            Microsoft.TeamFoundation.Build2.Server.AgentPoolQueueTarget agentPoolQueueTarget3 = agentPoolQueueTarget1;
            Microsoft.TeamFoundation.Build2.Server.MultipleAgentExecutionOptions executionOptions4 = new Microsoft.TeamFoundation.Build2.Server.MultipleAgentExecutionOptions();
            executionOptions4.Type = 2;
            executionOptions4.ContinueOnError = executionOptions3.ContinueOnError;
            executionOptions4.MaxConcurrency = executionOptions3.MaxConcurrency;
            agentPoolQueueTarget3.ExecutionOptions = (Microsoft.TeamFoundation.Build2.Server.AgentTargetExecutionOptions) executionOptions4;
            break;
          default:
            Microsoft.TeamFoundation.Build.WebApi.AgentTargetExecutionOptions executionOptions5 = webApiAgentPoolQueueTarget.ExecutionOptions;
            agentPoolQueueTarget1.ExecutionOptions = new Microsoft.TeamFoundation.Build2.Server.AgentTargetExecutionOptions()
            {
              Type = 0
            };
            break;
        }
      }
      return agentPoolQueueTarget1;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.ServerTarget ToWebApiServerTarget(
      this Microsoft.TeamFoundation.Build2.Server.ServerTarget srvTarget,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (srvTarget == null)
        return (Microsoft.TeamFoundation.Build.WebApi.ServerTarget) null;
      Microsoft.TeamFoundation.Build.WebApi.ServerTarget webApiServerTarget = new Microsoft.TeamFoundation.Build.WebApi.ServerTarget(securedObject);
      switch (srvTarget.ExecutionOptions.Type)
      {
        case 1:
          Microsoft.TeamFoundation.Build2.Server.VariableMultipliersServerExecutionOptions executionOptions = srvTarget.ExecutionOptions as Microsoft.TeamFoundation.Build2.Server.VariableMultipliersServerExecutionOptions;
          webApiServerTarget.ExecutionOptions = (Microsoft.TeamFoundation.Build.WebApi.ServerTargetExecutionOptions) new Microsoft.TeamFoundation.Build.WebApi.VariableMultipliersServerExecutionOptions(securedObject)
          {
            MaxConcurrency = executionOptions.MaxConcurrency,
            ContinueOnError = executionOptions.ContinueOnError,
            Multipliers = executionOptions.Multipliers
          };
          break;
        default:
          webApiServerTarget.ExecutionOptions = new Microsoft.TeamFoundation.Build.WebApi.ServerTargetExecutionOptions(securedObject);
          break;
      }
      return webApiServerTarget;
    }

    public static Microsoft.TeamFoundation.Build2.Server.ServerTarget ToServerServerTarget(
      this Microsoft.TeamFoundation.Build.WebApi.ServerTarget webApiSrvTarget)
    {
      Microsoft.TeamFoundation.Build2.Server.ServerTarget serverServerTarget = new Microsoft.TeamFoundation.Build2.Server.ServerTarget();
      if (webApiSrvTarget == null)
        return (Microsoft.TeamFoundation.Build2.Server.ServerTarget) null;
      switch (webApiSrvTarget.ExecutionOptions.Type)
      {
        case 1:
          Microsoft.TeamFoundation.Build.WebApi.VariableMultipliersServerExecutionOptions executionOptions = webApiSrvTarget.ExecutionOptions as Microsoft.TeamFoundation.Build.WebApi.VariableMultipliersServerExecutionOptions;
          serverServerTarget.ExecutionOptions = (Microsoft.TeamFoundation.Build2.Server.ServerTargetExecutionOptions) new Microsoft.TeamFoundation.Build2.Server.VariableMultipliersServerExecutionOptions()
          {
            MaxConcurrency = executionOptions.MaxConcurrency,
            ContinueOnError = executionOptions.ContinueOnError,
            Multipliers = executionOptions.Multipliers
          };
          break;
        default:
          serverServerTarget.ExecutionOptions = new Microsoft.TeamFoundation.Build2.Server.ServerTargetExecutionOptions();
          break;
      }
      return serverServerTarget;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.IVariableMultiplierExecutionOptions GetMultiplierOptions(
      this Microsoft.TeamFoundation.Build.WebApi.PhaseTarget target)
    {
      switch (target)
      {
        case Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueueTarget _:
          return ((Microsoft.TeamFoundation.Build.WebApi.AgentPoolQueueTarget) target).ExecutionOptions as Microsoft.TeamFoundation.Build.WebApi.IVariableMultiplierExecutionOptions;
        case Microsoft.TeamFoundation.Build.WebApi.ServerTarget _:
          return ((Microsoft.TeamFoundation.Build.WebApi.ServerTarget) target).ExecutionOptions as Microsoft.TeamFoundation.Build.WebApi.IVariableMultiplierExecutionOptions;
        default:
          return (Microsoft.TeamFoundation.Build.WebApi.IVariableMultiplierExecutionOptions) null;
      }
    }
  }
}
