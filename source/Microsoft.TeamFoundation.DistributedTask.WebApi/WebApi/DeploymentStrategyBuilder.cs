// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.DeploymentStrategyBuilder
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Validation;
using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal static class DeploymentStrategyBuilder
  {
    private const RollingDeploymentOption DefaultDeploymentOption = RollingDeploymentOption.Percentage;
    private const int DefaultDeploymentOptionValue = 100;

    public static DeploymentStrategyBase2 Build(
      IResourceStore resourceStore,
      Dictionary<string, JToken> strategy,
      ValidationResult validationResult,
      bool allowTaskMinorVersion,
      bool resolveDownloadBuildTask)
    {
      DeploymentStrategyBase2 deploymentStrategyBase2 = (DeploymentStrategyBase2) null;
      if (strategy == null || !strategy.Any<KeyValuePair<string, JToken>>())
      {
        deploymentStrategyBase2 = (DeploymentStrategyBase2) new RunOnceDeploymentStrategy2();
        deploymentStrategyBase2.Hooks.Add((DeploymentLifeCycleHookBase) new DeployHook());
      }
      else
      {
        if (strategy.Count > 1)
        {
          validationResult.Errors.Add(new PipelineValidationError("Deployment job does not support multiple strategies"));
          return deploymentStrategyBase2;
        }
        KeyValuePair<string, JToken> keyValuePair = strategy.FirstOrDefault<KeyValuePair<string, JToken>>();
        switch (keyValuePair.Key)
        {
          case "canary":
            IResourceStore resourceStore1 = resourceStore;
            keyValuePair = strategy.FirstOrDefault<KeyValuePair<string, JToken>>();
            JToken canaryStrategy = keyValuePair.Value;
            ValidationResult validationResult1 = validationResult;
            int num1 = allowTaskMinorVersion ? 1 : 0;
            int num2 = resolveDownloadBuildTask ? 1 : 0;
            deploymentStrategyBase2 = (DeploymentStrategyBase2) DeploymentStrategyBuilder.BuildCanaryDeploymentStrategy(resourceStore1, canaryStrategy, validationResult1, num1 != 0, num2 != 0);
            break;
          case "runOnce":
            IResourceStore resourceStore2 = resourceStore;
            keyValuePair = strategy.FirstOrDefault<KeyValuePair<string, JToken>>();
            JToken runOnceStrategy = keyValuePair.Value;
            ValidationResult validationResult2 = validationResult;
            int num3 = allowTaskMinorVersion ? 1 : 0;
            int num4 = resolveDownloadBuildTask ? 1 : 0;
            deploymentStrategyBase2 = (DeploymentStrategyBase2) DeploymentStrategyBuilder.BuildRunOnceDeploymentStrategy(resourceStore2, runOnceStrategy, validationResult2, num3 != 0, num4 != 0);
            break;
          case "rolling":
            IResourceStore resourceStore3 = resourceStore;
            keyValuePair = strategy.FirstOrDefault<KeyValuePair<string, JToken>>();
            JToken rollingStrategy = keyValuePair.Value;
            ValidationResult validationResult3 = validationResult;
            int num5 = allowTaskMinorVersion ? 1 : 0;
            int num6 = resolveDownloadBuildTask ? 1 : 0;
            deploymentStrategyBase2 = (DeploymentStrategyBase2) DeploymentStrategyBuilder.BuildRollingDeploymentStrategy(resourceStore3, rollingStrategy, validationResult3, num5 != 0, num6 != 0);
            break;
          default:
            IList<PipelineValidationError> errors = validationResult.Errors;
            keyValuePair = strategy.FirstOrDefault<KeyValuePair<string, JToken>>();
            PipelineValidationError pipelineValidationError = new PipelineValidationError("Unexpected deployment strategy " + keyValuePair.Key);
            errors.Add(pipelineValidationError);
            break;
        }
      }
      return deploymentStrategyBase2;
    }

    private static RunOnceDeploymentStrategy2 BuildRunOnceDeploymentStrategy(
      IResourceStore resourceStore,
      JToken runOnceStrategy,
      ValidationResult validationResult,
      bool allowTaskMinorVersion,
      bool resolveDownloadBuildTask)
    {
      RunOnceDeploymentStrategy2 deploymentStrategy = new RunOnceDeploymentStrategy2();
      JEnumerable<JProperty> jenumerable = runOnceStrategy.Children<JProperty>();
      List<DeploymentLifeCycleHookBase> lifeCycleHookBaseList = new List<DeploymentLifeCycleHookBase>();
      foreach (JProperty jproperty in jenumerable)
      {
        switch (jproperty.Name)
        {
          case "preDeploy":
          case "deploy":
          case "routeTraffic":
          case "postRouteTraffic":
            DeploymentStrategyBuilder.AddLifeCycleHook(resourceStore, jproperty, lifeCycleHookBaseList, validationResult, allowTaskMinorVersion, resolveDownloadBuildTask);
            continue;
          case "on":
            DeploymentStrategyBuilder.BuildOnSuccessOrFailureHooks(resourceStore, (JToken) jproperty, lifeCycleHookBaseList, validationResult, allowTaskMinorVersion, resolveDownloadBuildTask);
            continue;
          default:
            validationResult.Errors.Add(new PipelineValidationError(jproperty.Name + " is an unexpected property with runonce strategy"));
            continue;
        }
      }
      deploymentStrategy.Hooks.AddRange<DeploymentLifeCycleHookBase, IList<DeploymentLifeCycleHookBase>>((IEnumerable<DeploymentLifeCycleHookBase>) lifeCycleHookBaseList);
      DeploymentStrategyBuilder.ValidateHooks((DeploymentStrategyBase2) deploymentStrategy, validationResult);
      return deploymentStrategy;
    }

    private static RollingDeploymentStrategy2 BuildRollingDeploymentStrategy(
      IResourceStore resourceStore,
      JToken rollingStrategy,
      ValidationResult validationResult,
      bool allowTaskMinorVersion,
      bool resolveDownloadBuildTask)
    {
      RollingDeploymentOption deploymentOption = RollingDeploymentOption.Percentage;
      int deploymentOptionValue = 100;
      List<DeploymentLifeCycleHookBase> lifeCycleHookBaseList = new List<DeploymentLifeCycleHookBase>();
      foreach (JProperty child in rollingStrategy.Children<JProperty>())
      {
        switch (child.Name)
        {
          case "preDeploy":
          case "deploy":
          case "routeTraffic":
          case "postRouteTraffic":
            DeploymentStrategyBuilder.AddLifeCycleHook(resourceStore, child, lifeCycleHookBaseList, validationResult, allowTaskMinorVersion, resolveDownloadBuildTask);
            continue;
          case "on":
            DeploymentStrategyBuilder.BuildOnSuccessOrFailureHooks(resourceStore, (JToken) child, lifeCycleHookBaseList, validationResult, allowTaskMinorVersion, resolveDownloadBuildTask);
            continue;
          case "maxParallel":
            DeploymentStrategyBuilder.TryToFillRollingOptions(child.Value.ToString(), out deploymentOption, out deploymentOptionValue, validationResult);
            continue;
          default:
            validationResult.Errors.Add(new PipelineValidationError(child.Name + " is an unexpected property with rolling strategy"));
            continue;
        }
      }
      RollingDeploymentStrategy2 deploymentStrategy = new RollingDeploymentStrategy2(deploymentOption, deploymentOptionValue);
      deploymentStrategy.Hooks.AddRange<DeploymentLifeCycleHookBase, IList<DeploymentLifeCycleHookBase>>((IEnumerable<DeploymentLifeCycleHookBase>) lifeCycleHookBaseList);
      DeploymentStrategyBuilder.ValidateHooks((DeploymentStrategyBase2) deploymentStrategy, validationResult);
      return deploymentStrategy;
    }

    private static CanaryDeploymentStrategy BuildCanaryDeploymentStrategy(
      IResourceStore resourceStore,
      JToken canaryStrategy,
      ValidationResult validationResult,
      bool allowTaskMinorVersion,
      bool resolveDownloadBuildTask)
    {
      List<int> deploymentIncrements = new List<int>();
      List<DeploymentLifeCycleHookBase> lifeCycleHookBaseList = new List<DeploymentLifeCycleHookBase>();
      foreach (JProperty child in canaryStrategy.Children<JProperty>())
      {
        switch (child.Name)
        {
          case "preDeploy":
          case "deploy":
          case "routeTraffic":
          case "postRouteTraffic":
            DeploymentStrategyBuilder.AddLifeCycleHook(resourceStore, child, lifeCycleHookBaseList, validationResult, allowTaskMinorVersion, resolveDownloadBuildTask);
            continue;
          case "on":
            DeploymentStrategyBuilder.BuildOnSuccessOrFailureHooks(resourceStore, (JToken) child, lifeCycleHookBaseList, validationResult, allowTaskMinorVersion, resolveDownloadBuildTask);
            continue;
          case "increments":
            DeploymentStrategyBuilder.FillCanaryStrategyIncrements(child.Value, out deploymentIncrements, validationResult);
            continue;
          default:
            validationResult.Errors.Add(new PipelineValidationError(child.Name + " is an unexpected property with canary strategy"));
            continue;
        }
      }
      CanaryDeploymentStrategy deploymentStrategy = new CanaryDeploymentStrategy(deploymentIncrements);
      deploymentStrategy.Hooks.AddRange<DeploymentLifeCycleHookBase, IList<DeploymentLifeCycleHookBase>>((IEnumerable<DeploymentLifeCycleHookBase>) lifeCycleHookBaseList);
      DeploymentStrategyBuilder.ValidateHooks((DeploymentStrategyBase2) deploymentStrategy, validationResult);
      return deploymentStrategy;
    }

    private static void AddLifeCycleHook(
      IResourceStore resourceStore,
      JProperty prop,
      List<DeploymentLifeCycleHookBase> lifeCycleHooks,
      ValidationResult validationResult,
      bool allowTaskMinorVersion,
      bool resolveDownloadBuildTask)
    {
      DeploymentLifeCycleHookBase lifeCycleHook;
      switch (prop.Name)
      {
        case "preDeploy":
          lifeCycleHook = (DeploymentLifeCycleHookBase) new PreDeployHook();
          break;
        case "deploy":
          lifeCycleHook = (DeploymentLifeCycleHookBase) new DeployHook();
          break;
        case "routeTraffic":
          lifeCycleHook = (DeploymentLifeCycleHookBase) new RouteTrafficHook();
          break;
        case "postRouteTraffic":
          lifeCycleHook = (DeploymentLifeCycleHookBase) new PostRouteTrafficHook();
          break;
        case "success":
          lifeCycleHook = (DeploymentLifeCycleHookBase) new OnSuccessHook();
          break;
        case "failure":
          lifeCycleHook = (DeploymentLifeCycleHookBase) new OnFailureHook();
          break;
        default:
          lifeCycleHook = (DeploymentLifeCycleHookBase) null;
          validationResult.Errors.Add(new PipelineValidationError(prop.Name + " is invalid token under deployment strategy"));
          break;
      }
      if (lifeCycleHook == null)
        return;
      DeploymentStrategyBuilder.PopulateSteps(resourceStore, lifeCycleHook, prop.Value, validationResult, allowTaskMinorVersion, resolveDownloadBuildTask);
      DeploymentStrategyBuilder.PopulatePhaseTarget(lifeCycleHook, prop.Value, validationResult);
      lifeCycleHooks.Add(lifeCycleHook);
    }

    private static void BuildOnSuccessOrFailureHooks(
      IResourceStore resourceStore,
      JToken onSuccessOrFailureJToken,
      List<DeploymentLifeCycleHookBase> lifeCycleHooks,
      ValidationResult validationResult,
      bool allowTaskMinorVersion,
      bool resolveDownloadBuildTask)
    {
      List<JToken> list = onSuccessOrFailureJToken.Children<JToken>().ToList<JToken>();
      if (list.Count != 1)
        validationResult.Errors.Add(new PipelineValidationError("Unexpected number of tokens under keyword: 'on'"));
      foreach (JProperty child in list[0].Children<JProperty>())
      {
        string name = child.Name;
        if (name == "failure" || name == "success")
          DeploymentStrategyBuilder.AddLifeCycleHook(resourceStore, child, lifeCycleHooks, validationResult, allowTaskMinorVersion, resolveDownloadBuildTask);
        else
          validationResult.Errors.Add(new PipelineValidationError(child.Name + " is an unexpected property under 'on' in canary strategy"));
      }
    }

    private static void PopulatePhaseTarget(
      DeploymentLifeCycleHookBase lifeCycleHook,
      JToken strategyToken,
      ValidationResult validationResult)
    {
      JProperty poolToken = strategyToken.Children<JProperty>().FirstOrDefault<JProperty>((Func<JProperty, bool>) (x => string.Equals(x.Name, "pool", StringComparison.OrdinalIgnoreCase)));
      if (poolToken == null)
        return;
      string poolName = DeploymentStrategyBuilder.GetPoolName((JToken) poolToken);
      PhaseTarget phaseTarget;
      if (string.Equals(poolName, "server", StringComparison.OrdinalIgnoreCase))
      {
        phaseTarget = (PhaseTarget) new ServerTarget();
      }
      else
      {
        AgentQueueTarget agentQueueTarget = new AgentQueueTarget();
        AgentQueueReference agentQueueReference = new AgentQueueReference();
        agentQueueReference.Name = (ExpressionValue<string>) poolName;
        agentQueueTarget.Queue = agentQueueReference;
        AgentQueueTarget queueTarget = agentQueueTarget;
        List<Demand> demands = DeploymentStrategyBuilder.GetDemands(poolToken, validationResult);
        if (demands.Any<Demand>())
          queueTarget.Demands.AddRange<Demand, ISet<Demand>>((IEnumerable<Demand>) demands);
        DeploymentStrategyBuilder.PopulateAgentSpecification(poolToken, queueTarget);
        phaseTarget = (PhaseTarget) queueTarget;
      }
      lifeCycleHook.Target = phaseTarget;
    }

    private static void PopulateAgentSpecification(
      JProperty poolToken,
      AgentQueueTarget queueTarget)
    {
      IEnumerable<JProperty> specificationsFromPoolToken = DeploymentStrategyBuilder.GetAgentSpecificationsFromPoolToken(poolToken);
      if (!specificationsFromPoolToken.Any<JProperty>())
        return;
      queueTarget.AgentSpecification = new JObject();
      foreach (JProperty jproperty in specificationsFromPoolToken)
      {
        string name = jproperty.Name;
        JToken vmImageToken = jproperty.Children<JToken>().FirstOrDefault<JToken>();
        queueTarget.AgentSpecification.Add(name, vmImageToken);
        if (string.Equals(name, "vmImage", StringComparison.OrdinalIgnoreCase))
          DeploymentStrategyBuilder.FillQueueNameFromAgentSpecificationIfNeeded(vmImageToken, queueTarget);
      }
    }

    private static IEnumerable<JProperty> GetAgentSpecificationsFromPoolToken(JProperty poolToken)
    {
      JToken jtoken = poolToken.Children<JToken>().FirstOrDefault<JToken>();
      return jtoken == null ? (IEnumerable<JProperty>) null : jtoken.Children<JProperty>().Where<JProperty>((Func<JProperty, bool>) (x => !string.Equals(x.Name, "demands", StringComparison.OrdinalIgnoreCase) && !string.Equals(x.Name, "name", StringComparison.OrdinalIgnoreCase)));
    }

    private static void FillQueueNameFromAgentSpecificationIfNeeded(
      JToken vmImageToken,
      AgentQueueTarget queueTarget)
    {
      if (!(queueTarget.Queue.Name == (ExpressionValue<string>) null) && (!queueTarget.Queue.Name.IsLiteral || !string.IsNullOrEmpty(queueTarget.Queue?.Name?.Literal)))
        return;
      bool flag = AgentQueueTarget.IsProbablyExpressionOrMacro(vmImageToken.ToString());
      string str = (string) null;
      if (queueTarget.Demands.Count > 0 && !flag)
        str = AgentQueueTarget.PoolNameForVMImage(vmImageToken.ToString());
      if (string.IsNullOrEmpty(str) && !flag)
        str = "Azure Pipelines";
      if (string.IsNullOrEmpty(str))
        return;
      AgentQueueTarget agentQueueTarget = queueTarget;
      AgentQueueReference agentQueueReference = new AgentQueueReference();
      agentQueueReference.Name = (ExpressionValue<string>) str;
      agentQueueTarget.Queue = agentQueueReference;
    }

    private static List<Demand> GetDemands(JProperty poolToken, ValidationResult validationResult)
    {
      JToken jtoken1 = poolToken.Children<JToken>().FirstOrDefault<JToken>();
      IEnumerable<JProperty> jproperties = jtoken1 != null ? jtoken1.Children<JProperty>().Where<JProperty>((Func<JProperty, bool>) (x => string.Equals(x.Name, "demands", StringComparison.OrdinalIgnoreCase))) : (IEnumerable<JProperty>) null;
      List<Demand> demands = new List<Demand>();
      if (jproperties != null)
      {
        foreach (JProperty jproperty in jproperties)
        {
          foreach (JToken jtoken2 in (IEnumerable<JToken>) jproperty.Value)
          {
            Demand demand;
            if (!Demand.TryParse(jtoken2.Value<string>(), out demand))
              validationResult.Errors.Add(new PipelineValidationError("Invalid demand '" + jtoken2.Value<string>() + "'. The demand should be in the format '<NAME>' to test existence, and '<NAME> -equals <VALUE>' to test for a specific value. For example, 'VISUALSTUDIO' or 'agent.os -equals Windows_NT'."));
            else
              demands.Add(demand);
          }
        }
      }
      return demands;
    }

    private static string GetPoolName(JToken poolToken)
    {
      JToken jtoken = poolToken.Children<JToken>().FirstOrDefault<JToken>();
      JProperty jproperty = jtoken != null ? jtoken.Children<JProperty>().FirstOrDefault<JProperty>((Func<JProperty, bool>) (x => x.Name == "name")) : (JProperty) null;
      if (jproperty == null)
        return string.Empty;
      return jproperty.Value?.ToString();
    }

    private static void PopulateSteps(
      IResourceStore resourceStore,
      DeploymentLifeCycleHookBase lifeCycleHook,
      JToken strategyToken,
      ValidationResult validationResult,
      bool allowTaskMinorVersion,
      bool resolveDowloadBuildTask)
    {
      JProperty jproperty = strategyToken.Children<JProperty>().FirstOrDefault<JProperty>((Func<JProperty, bool>) (x => x.Name == "steps"));
      if (jproperty != null && jproperty.HasValues)
        lifeCycleHook.Steps.AddRange((IEnumerable<Step>) DeploymentStrategyTaskSteps.Build(resourceStore, jproperty.Value.Children<JToken>(), validationResult, allowTaskMinorVersion, resolveDowloadBuildTask));
      else
        validationResult.Errors.Add(new PipelineValidationError(string.Format("Deployment life cycle hook of type {0} must contain steps to be run as part of the hook.", (object) lifeCycleHook.Type)));
    }

    private static void ValidateHooks(
      DeploymentStrategyBase2 deploymentStrategy,
      ValidationResult validationResult)
    {
      foreach (DeploymentLifeCycleHookType lifeCycleHookType in Enum.GetValues(typeof (DeploymentLifeCycleHookType)))
      {
        DeploymentLifeCycleHookType hookType = lifeCycleHookType;
        if (deploymentStrategy.Hooks.Where<DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, bool>) (h => h.Type == hookType)).Count<DeploymentLifeCycleHookBase>() > 1)
          validationResult.Errors.Add(new PipelineValidationError(string.Format("Deployment strategy cannot contain more than one {0} life cycle hook.", (object) hookType)));
      }
      IList<DeploymentLifeCycleHookBase> hooks = deploymentStrategy.Hooks;
      if ((hooks != null ? (hooks.Count<DeploymentLifeCycleHookBase>((Func<DeploymentLifeCycleHookBase, bool>) (h => h.Type == DeploymentLifeCycleHookType.Deploy)) != 1 ? 1 : 0) : 1) == 0)
        return;
      validationResult.Errors.Add(new PipelineValidationError("Deployment strategy should contain life cycle hook of type 'Deploy'"));
    }

    private static void TryToFillRollingOptions(
      string deploymentValue,
      out RollingDeploymentOption deploymentOption,
      out int deploymentOptionValue,
      ValidationResult validationResult)
    {
      deploymentOptionValue = 100;
      deploymentOption = RollingDeploymentOption.Percentage;
      if (string.IsNullOrWhiteSpace(deploymentValue))
        return;
      if (deploymentValue.EndsWith("%"))
      {
        int result;
        if (int.TryParse(deploymentValue.TrimEnd('%'), out result) && result > 0 && result <= 100)
        {
          deploymentOption = RollingDeploymentOption.Percentage;
          deploymentOptionValue = result;
        }
        else
          validationResult.Errors.Add(new PipelineValidationError("Percentage rolling option should be within 1 to 100. Provided maxParallel '" + deploymentValue + "' is not valid"));
      }
      else
      {
        int result;
        if (int.TryParse(deploymentValue, out result) && result > 0)
        {
          deploymentOption = RollingDeploymentOption.Absolute;
          deploymentOptionValue = result;
        }
        else
          validationResult.Errors.Add(new PipelineValidationError("Rolling value should be positive integer. Provided maxParallel '" + deploymentValue + "' is not valid"));
      }
    }

    private static void FillCanaryStrategyIncrements(
      JToken incrementsToken,
      out List<int> deploymentIncrements,
      ValidationResult validationResult)
    {
      deploymentIncrements = new List<int>();
      try
      {
        if (incrementsToken != null)
          deploymentIncrements.AddRange((IEnumerable<int>) incrementsToken.ToObject<List<int>>());
      }
      catch
      {
        validationResult.Errors.Add(new PipelineValidationError("Failed to parse canary strategy increments. Ensure increments input is an array of integers. For e.g.: [ 10, 20 ]"));
      }
      if (deploymentIncrements.Where<int>((Func<int, bool>) (inc => inc >= 100)).Any<int>())
        validationResult.Errors.Add(new PipelineValidationError("Canary strategy increments should contain a value less than 100."));
      for (int index = 1; index < deploymentIncrements.Count; ++index)
      {
        if (deploymentIncrements[index] <= deploymentIncrements[index - 1])
        {
          validationResult.Errors.Add(new PipelineValidationError("Canary strategy increments should be in strictly increasing order. For e.g.: inputs like [ 10, 10 ] (or) [ 20, 10 ] are invalid"));
          break;
        }
      }
    }
  }
}
