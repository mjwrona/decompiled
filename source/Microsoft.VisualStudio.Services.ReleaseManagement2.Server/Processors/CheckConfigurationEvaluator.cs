// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors.CheckConfigurationEvaluator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.Pipelines.Authorization.WebApi;
using Microsoft.Azure.Pipelines.Checks.WebApi;
using Microsoft.Azure.Pipelines.TaskCheck.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Processors
{
  internal class CheckConfigurationEvaluator
  {
    private readonly IVssRequestContext requestContext;
    private readonly Guid projectId;
    private readonly Release release;
    private readonly ReleaseEnvironment environment;
    private readonly CheckToGateConverter gateGenerator;

    public CheckConfigurationEvaluator(
      IVssRequestContext requestContext,
      Guid projectId,
      Release release,
      ReleaseEnvironment environment)
    {
      this.requestContext = requestContext;
      this.projectId = projectId;
      this.release = release;
      this.environment = environment;
      ServiceEndpointGuidMappingProviderCache serviceEndpointGuidProvider = new ServiceEndpointGuidMappingProviderCache((IServiceEndpointGuidMappingProvider) new ServiceEndpointGuidMappingProvider());
      this.gateGenerator = new CheckToGateConverter(requestContext, projectId, (IServiceEndpointGuidMappingProvider) serviceEndpointGuidProvider);
    }

    public IList<CheckConfiguration> GetCheckConfigurationsOnResources(
      bool excludeAuthorizationChecks = true)
    {
      List<Resource> resourcesForConfigs = this.GetResourcesForConfig();
      if (resourcesForConfigs == null || resourcesForConfigs.Count <= 0)
        return (IList<CheckConfiguration>) null;
      PipelinesChecksHttpClient pipelinesChecksHttpClient = this.requestContext.Elevate().GetClient<PipelinesChecksHttpClient>();
      List<CheckConfiguration> result = this.requestContext.ExecuteAsyncAndGetResult<List<CheckConfiguration>>(new Func<Task<List<CheckConfiguration>>>(getQueryAsyncFunc));
      // ISSUE: explicit non-virtual call
      return excludeAuthorizationChecks && result != null && __nonvirtual (result.Count) > 0 ? (IList<CheckConfiguration>) result.Where<CheckConfiguration>((Func<CheckConfiguration, bool>) (c => c.Resource != null && !string.IsNullOrWhiteSpace(c.Resource.Type) && (!string.IsNullOrWhiteSpace(c.Resource.Id) || !string.IsNullOrWhiteSpace(c.Resource.Name)) && c.Type != null && c.Type.Id != AuthorizationCheckConstants.AuthorizationCheckTypeId)).ToList<CheckConfiguration>() : (IList<CheckConfiguration>) result;

      Task<List<CheckConfiguration>> getQueryAsyncFunc() => pipelinesChecksHttpClient.QueryCheckConfigurationsOnResourcesAsync(this.projectId, (IEnumerable<Resource>) resourcesForConfigs, new CheckConfigurationExpandParameter?(CheckConfigurationExpandParameter.Settings));
    }

    public string GetCheckValidationExceptionMessage(CheckConfiguration checkConfig)
    {
      string exceptionMessage = string.Empty;
      if (checkConfig != null)
      {
        string str = string.IsNullOrWhiteSpace(checkConfig.Resource.Name) ? checkConfig.Resource.Id?.ToUpperInvariant() : checkConfig.Resource.Name;
        switch (checkConfig.Resource.Type)
        {
          case "queue":
            exceptionMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.InjectCheckAsGate.AgentQueue") ? Resources.StageReferringQueueWithUnsupportedChecksFailureMessage : Resources.StageReferringQueueWithChecksFailureMessage, (object) str);
            break;
          case "endpoint":
            exceptionMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.InjectCheckAsGate.ServiceEndpoint") ? Resources.StageReferringEndpointWithUnsupportedChecksFailureMessage : Resources.StageReferringEndpointWithChecksFailureMessage, (object) str);
            break;
          default:
            exceptionMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.StageReferringResourceWithChecksFailureMessage, (object) checkConfig.Resource.Type?.ToUpperInvariant(), (object) str);
            break;
        }
      }
      return exceptionMessage;
    }

    public (IList<CheckConfiguration>, IList<CheckConfiguration>) SplitBySupportedType(
      IList<CheckConfiguration> checkConfigs)
    {
      IList<CheckConfiguration> checkConfigurationList = (IList<CheckConfiguration>) new List<CheckConfiguration>();
      IList<CheckConfiguration> collection = (IList<CheckConfiguration>) new List<CheckConfiguration>();
      if (checkConfigs != null && checkConfigs.Count == 0)
        return (checkConfigurationList, collection);
      foreach (KeyValuePair<Resource, List<CheckConfiguration>> keyValuePair in (IEnumerable<KeyValuePair<Resource, List<CheckConfiguration>>>) checkConfigs.GroupBy<CheckConfiguration, Resource>((Func<CheckConfiguration, Resource>) (c => c.Resource)).ToDictionary<IGrouping<Resource, CheckConfiguration>, Resource, List<CheckConfiguration>>((Func<IGrouping<Resource, CheckConfiguration>, Resource>) (c => c.Key), (Func<IGrouping<Resource, CheckConfiguration>, List<CheckConfiguration>>) (c => c.ToList<CheckConfiguration>())))
      {
        Resource key = keyValuePair.Key;
        List<CheckConfiguration> values1 = keyValuePair.Value;
        if (this.IsResourceSupportedForCheckInjection(key.Type))
        {
          bool flag = false;
          IList<CheckConfiguration> values2 = (IList<CheckConfiguration>) null;
          foreach (CheckConfiguration config in values1)
          {
            if (CheckConfigurationEvaluator.IsCheckConfigurationSupportedForCheckInjection(config))
            {
              flag = true;
              checkConfigurationList.Add(config);
            }
            else
            {
              if (values2 == null)
                values2 = (IList<CheckConfiguration>) new List<CheckConfiguration>();
              values2.Add(config);
            }
          }
          if (!flag)
            collection.AddRange<CheckConfiguration, IList<CheckConfiguration>>((IEnumerable<CheckConfiguration>) values2);
        }
        else
          collection.AddRange<CheckConfiguration, IList<CheckConfiguration>>((IEnumerable<CheckConfiguration>) values1);
      }
      return (checkConfigurationList, collection);
    }

    public bool UpdateGatesWithInjectedChecks(IList<CheckConfiguration> checkConfigs)
    {
      bool flag = false;
      HashSet<string> existingGateNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      Dictionary<int, ReleaseDefinitionGate> checkConfigIdToGate = new Dictionary<int, ReleaseDefinitionGate>();
      Dictionary<int, int> checkConfigIdToVersion = new Dictionary<int, int>();
      IList<ReleaseDefinitionGate> gates = this.environment.PreDeploymentGates.Gates;
      if (gates != null)
        gates.SelectMany((Func<ReleaseDefinitionGate, IEnumerable<WorkflowTask>>) (gate => (IEnumerable<WorkflowTask>) gate.Tasks), (gate, task) => new
        {
          gate = gate,
          task = task
        }).ForEach(gateTasks =>
        {
          existingGateNames.Add(gateTasks.task.Name);
          if (gateTasks.task.CheckConfig == null)
            return;
          checkConfigIdToGate[gateTasks.task.CheckConfig.Id] = gateTasks.gate;
          checkConfigIdToVersion[gateTasks.task.CheckConfig.Id] = gateTasks.task.CheckConfig.Version;
        });
      if (checkConfigIdToGate.Count > 0)
        this.RemoveInjectedGates(checkConfigIdToGate);
      foreach (CheckConfiguration checkConfig in (IEnumerable<CheckConfiguration>) checkConfigs)
      {
        int num;
        if (checkConfigIdToVersion.TryGetValue(checkConfig.Id, out num) && checkConfig.Version != num)
          flag = true;
        if (this.environment.PreDeploymentGates.Gates == null)
          this.environment.PreDeploymentGates.Gates = (IList<ReleaseDefinitionGate>) new List<ReleaseDefinitionGate>();
        this.environment.PreDeploymentGates.Gates.Add(this.gateGenerator.ConvertCheckToGate(existingGateNames, checkConfig));
      }
      int? count1 = checkConfigIdToVersion?.Count;
      int? count2 = checkConfigs?.Count;
      if (!(count1.GetValueOrDefault() == count2.GetValueOrDefault() & count1.HasValue == count2.HasValue))
        flag = true;
      if (flag)
        this.UpdateGateOptionsWithInjectedCheckConfigurations(checkConfigs);
      return flag;
    }

    private static void GetMaxCheckConfigurationControlOptions(
      IList<CheckConfiguration> checkConfigs,
      out int maxInjectedGateTimeout,
      out int maxInjectedGateSamplingInterval)
    {
      maxInjectedGateTimeout = 0;
      maxInjectedGateSamplingInterval = 0;
      foreach (CheckConfiguration checkConfig in (IEnumerable<CheckConfiguration>) checkConfigs)
      {
        if (checkConfig.Timeout.HasValue)
        {
          int? timeout = checkConfig.Timeout;
          int num = maxInjectedGateTimeout;
          if (timeout.GetValueOrDefault() > num & timeout.HasValue)
            maxInjectedGateTimeout = checkConfig.Timeout.Value;
        }
        TaskCheckConfig configurationSettings = checkConfig.GetCheckConfigurationSettings() as TaskCheckConfig;
        if (configurationSettings.RetryInterval.HasValue)
        {
          int? retryInterval = configurationSettings.RetryInterval;
          int num = maxInjectedGateSamplingInterval;
          if (retryInterval.GetValueOrDefault() > num & retryInterval.HasValue)
            maxInjectedGateSamplingInterval = configurationSettings.RetryInterval.Value;
        }
      }
    }

    private static bool IsCheckConfigurationSupportedForCheckInjection(CheckConfiguration config)
    {
      if (!config.IsDisabled && config.GetCheckConfigurationSettings() is TaskCheckConfig configurationSettings)
      {
        TaskCheckDefinitionReference definitionRef = configurationSettings.DefinitionRef;
        int num;
        if (definitionRef == null)
        {
          num = 0;
        }
        else
        {
          Guid id = definitionRef.Id;
          num = 1;
        }
        if (num != 0)
          return TaskCheckConstants.InvokeRestApiTaskId.Equals(configurationSettings.DefinitionRef.Id);
      }
      return false;
    }

    private void RemoveInjectedGates(
      Dictionary<int, ReleaseDefinitionGate> checkConfigIdToGate)
    {
      foreach (int key in checkConfigIdToGate.Keys)
        this.environment.PreDeploymentGates.Gates.Remove(checkConfigIdToGate[key]);
    }

    private bool IsResourceSupportedForCheckInjection(string resourceType)
    {
      if (string.Equals("queue", resourceType, StringComparison.OrdinalIgnoreCase))
        return this.requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.InjectCheckAsGate.AgentQueue");
      return string.Equals("endpoint", resourceType, StringComparison.OrdinalIgnoreCase) && this.requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.InjectCheckAsGate.ServiceEndpoint");
    }

    private void UpdateGateOptionsWithInjectedCheckConfigurations(
      IList<CheckConfiguration> checkConfigs)
    {
      bool flag = this.environment.PreDeploymentGates.GatesOptions != null && this.environment.PreDeploymentGates.GatesOptions.IsEnabled;
      if (!checkConfigs.Any<CheckConfiguration>())
        return;
      if (!flag)
        this.EnableGatesInGatesOptions();
      ReleaseDefinitionGatesOptions gatesOptions = this.environment.PreDeploymentGates.GatesOptions;
      if (this.environment.PreDeploymentGates.Gates.Where<ReleaseDefinitionGate>((Func<ReleaseDefinitionGate, bool>) (g => !g.IsGenerated)).SelectMany<ReleaseDefinitionGate, WorkflowTask>((Func<ReleaseDefinitionGate, IEnumerable<WorkflowTask>>) (g => (IEnumerable<WorkflowTask>) g.Tasks)).Any<WorkflowTask>((Func<WorkflowTask, bool>) (t => t.Enabled)))
        return;
      int maxInjectedGateTimeout;
      int maxInjectedGateSamplingInterval;
      CheckConfigurationEvaluator.GetMaxCheckConfigurationControlOptions(checkConfigs, out maxInjectedGateTimeout, out maxInjectedGateSamplingInterval);
      gatesOptions.Timeout = Math.Min(21600, Math.Max(6, maxInjectedGateTimeout));
      int val1 = gatesOptions.Timeout > 4320 ? 30 : 5;
      if (maxInjectedGateSamplingInterval < val1)
        maxInjectedGateSamplingInterval = maxInjectedGateSamplingInterval == 0 ? Math.Max(val1, gatesOptions.Timeout - 1) : val1;
      int num = Math.Min(1440, maxInjectedGateSamplingInterval);
      gatesOptions.SamplingInterval = num;
      gatesOptions.StabilizationTime = 0;
    }

    private void EnableGatesInGatesOptions()
    {
      if (this.environment.PreDeploymentGates.GatesOptions == null)
        this.environment.PreDeploymentGates.GatesOptions = new ReleaseDefinitionGatesOptions();
      this.environment.PreDeploymentGates.GatesOptions.IsEnabled = true;
      this.environment.PreDeploymentGates.Gates.Where<ReleaseDefinitionGate>((Func<ReleaseDefinitionGate, bool>) (g => !g.IsGenerated)).SelectMany<ReleaseDefinitionGate, WorkflowTask>((Func<ReleaseDefinitionGate, IEnumerable<WorkflowTask>>) (g => (IEnumerable<WorkflowTask>) g.Tasks)).ForEach<WorkflowTask>((Action<WorkflowTask>) (t => t.Enabled = false));
    }

    private List<Resource> GetResourcesForConfig()
    {
      List<Resource> resourcesForConfigs = new List<Resource>();
      IList<DeployPhaseSnapshot> deployPhaseSnapshots = this.environment.GetDesignerDeployPhaseSnapshots();
      IList<TaskDefinition> allTaskDefinitions = this.requestContext.GetService<IDistributedTaskPoolService>().GetTaskDefinitions(this.requestContext.Elevate());
      IDictionary<string, ConfigurationVariableValue> variables = this.GetAllVariables();
      Action<DeployPhaseSnapshot> action = (Action<DeployPhaseSnapshot>) (snapshot =>
      {
        if (snapshot == null)
          return;
        this.PopulateReferencedEndpointResources(resourcesForConfigs, snapshot, allTaskDefinitions, variables);
        this.PopulateReferencedQueueResources(resourcesForConfigs, variables);
      });
      deployPhaseSnapshots.ForEach<DeployPhaseSnapshot>(action);
      List<Resource> source1 = resourcesForConfigs;
      List<Resource> resourceList;
      if (source1 == null)
      {
        resourceList = (List<Resource>) null;
      }
      else
      {
        IEnumerable<Resource> source2 = source1.Distinct<Resource>();
        resourceList = source2 != null ? source2.ToList<Resource>() : (List<Resource>) null;
      }
      resourcesForConfigs = resourceList;
      return resourcesForConfigs;
    }

    private IDictionary<string, ConfigurationVariableValue> GetAllVariables()
    {
      IDictionary<string, ConfigurationVariableValue> dictionary1 = DictionaryMerger.MergeDictionaries<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, ConfigurationVariableValue>>) new IDictionary<string, ConfigurationVariableValue>[2]
      {
        this.release.Variables,
        (IDictionary<string, ConfigurationVariableValue>) VariableGroupsMerger.MergeVariableGroups(this.release.VariableGroups).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, ConfigurationVariableValue>) (p => new ConfigurationVariableValue()
        {
          Value = p.Value.Value,
          IsSecret = p.Value.IsSecret
        }))
      });
      Dictionary<string, ConfigurationVariableValue> dictionary2 = VariableGroupsMerger.MergeVariableGroups(this.environment.VariableGroups).ToDictionary<KeyValuePair<string, MergedConfigurationVariableValue>, string, ConfigurationVariableValue>((Func<KeyValuePair<string, MergedConfigurationVariableValue>, string>) (p => p.Key), (Func<KeyValuePair<string, MergedConfigurationVariableValue>, ConfigurationVariableValue>) (p => new ConfigurationVariableValue()
      {
        Value = p.Value.Value,
        IsSecret = p.Value.IsSecret
      }));
      Dictionary<string, ConfigurationVariableValue> dictionary3 = new Dictionary<string, ConfigurationVariableValue>();
      if (this.environment.ProcessParameters != null)
        dictionary3 = this.environment.ProcessParameters.GetProcessParametersAsDataModelVariables();
      return DictionaryMerger.MergeDictionaries<string, ConfigurationVariableValue>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, (IEnumerable<IDictionary<string, ConfigurationVariableValue>>) new IDictionary<string, ConfigurationVariableValue>[4]
      {
        (IDictionary<string, ConfigurationVariableValue>) dictionary3,
        this.environment.Variables,
        (IDictionary<string, ConfigurationVariableValue>) dictionary2,
        dictionary1
      });
    }

    private void PopulateReferencedEndpointResources(
      List<Resource> resourcesForConfigs,
      DeployPhaseSnapshot snapshot,
      IList<TaskDefinition> allTaskDefinitions,
      IDictionary<string, ConfigurationVariableValue> variables)
    {
      Dictionary<string, string> variableInputs = new Dictionary<string, string>();
      if (variables != null && variables.Keys != null && variables.Keys.Count > 0)
        variables.Keys.ForEach<string>((Action<string>) (key => variableInputs[key] = variables[key].Value));
      IList<ServiceEndpointTasksReference> serviceEndpointsInUse = snapshot.Workflow.GetServiceEndpointsInUse(this.requestContext, (IDictionary<string, string>) variableInputs, allTaskDefinitions);
      if (serviceEndpointsInUse == null || !serviceEndpointsInUse.Any<ServiceEndpointTasksReference>())
        return;
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> existingEndpoints;
      ServiceEndpointPermissionValidator.FilterReferencedServiceEndpoints(this.requestContext, this.projectId, serviceEndpointsInUse, out existingEndpoints, out IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> _);
      if (existingEndpoints == null || !existingEndpoints.Any<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>())
        return;
      existingEndpoints.ForEach<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Action<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) (endpoint => resourcesForConfigs.Add(new Resource()
      {
        Id = endpoint.Id.ToString(),
        Type = nameof (endpoint)
      })));
    }

    private void PopulateReferencedQueueResources(
      List<Resource> resourcesForConfigs,
      IDictionary<string, ConfigurationVariableValue> variables)
    {
      IEnumerable<int> allQueueIds = this.environment.GetAllQueueIds(variables);
      if (allQueueIds == null)
        return;
      allQueueIds.ForEach<int>((Action<int>) (queueId =>
      {
        if (queueId <= 0)
          return;
        resourcesForConfigs.Add(new Resource()
        {
          Id = queueId.ToString((IFormatProvider) CultureInfo.InvariantCulture),
          Type = "queue"
        });
      }));
    }
  }
}
