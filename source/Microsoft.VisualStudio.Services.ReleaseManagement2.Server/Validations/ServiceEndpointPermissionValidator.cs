// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations.ServiceEndpointPermissionValidator
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations
{
  public static class ServiceEndpointPermissionValidator
  {
    public static void EnsureServiceEndpointSecurityPermission(
      IVssRequestContext context,
      Guid projectId,
      IEnumerable<ReleaseDefinitionEnvironment> updatedEnvironments,
      IDictionary<string, ConfigurationVariableValue> updatedReleaseDefinitionVariables,
      ReleaseDefinition existingReleaseDefinition,
      IList<TaskDefinition> taskDefinitions)
    {
      Dictionary<string, ReleaseEnvironmentData> newEndpointsEnvironmentMap = new Dictionary<string, ReleaseEnvironmentData>();
      Dictionary<string, ReleaseEnvironmentData> dictionary = new Dictionary<string, ReleaseEnvironmentData>();
      if (updatedEnvironments != null)
      {
        foreach (ReleaseDefinitionEnvironment updatedEnvironment in updatedEnvironments)
        {
          ReleaseDefinitionEnvironment definitionEnvironment = JsonConvert.DeserializeObject<ReleaseDefinitionEnvironment>(JsonConvert.SerializeObject((object) updatedEnvironment));
          ServiceEndpointPermissionValidator.MergeReleaseDefinitionEnvironmentVariables(context, projectId, definitionEnvironment);
          foreach (DeployPhase deployPhase in (IEnumerable<DeployPhase>) definitionEnvironment.DeployPhases)
          {
            deployPhase.WorkflowTasks = TaskGroupHelper.ReplaceMetaTaskWithTasksInWorkflow(context, projectId, deployPhase.WorkflowTasks);
            deployPhase.WorkflowTasks.ValidateTasks(taskDefinitions, updatedEnvironment.Name, deployPhase.Name, deployPhase.PhaseType.ToRunsOnValue());
          }
          ReleaseEnvironmentData releaseEnvironmentData = ServiceEndpointVariablesHelper.ConvertToReleaseEnvironmentData(definitionEnvironment, updatedReleaseDefinitionVariables);
          ServiceEndpointPermissionValidator.PopulateEndpointsEnvironmentMap(context, releaseEnvironmentData, newEndpointsEnvironmentMap, taskDefinitions);
        }
      }
      if (existingReleaseDefinition != null)
      {
        foreach (ReleaseDefinitionEnvironment definitionEnvironment1 in existingReleaseDefinition.Environments.Where<ReleaseDefinitionEnvironment>((Func<ReleaseDefinitionEnvironment, bool>) (env => newEndpointsEnvironmentMap.Keys.Contains<string>(ServiceEndpointVariablesHelper.GetEnvironmentKey(env)))).ToList<ReleaseDefinitionEnvironment>())
        {
          IDictionary<string, ConfigurationVariableValue> releaseDefinition = Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.ReleaseDefinitionExtensions.GetVariablesFromReleaseDefinition(existingReleaseDefinition, context, projectId);
          ReleaseDefinitionEnvironment definitionEnvironment2 = JsonConvert.DeserializeObject<ReleaseDefinitionEnvironment>(JsonConvert.SerializeObject((object) definitionEnvironment1));
          ServiceEndpointPermissionValidator.MergeReleaseDefinitionEnvironmentVariables(context, projectId, definitionEnvironment2);
          ReleaseEnvironmentData releaseEnvironmentData = ServiceEndpointVariablesHelper.ConvertToReleaseEnvironmentData(definitionEnvironment2, releaseDefinition);
          ServiceEndpointPermissionValidator.PopulateEndpointsEnvironmentMap(context, releaseEnvironmentData, dictionary, taskDefinitions);
        }
      }
      if (!newEndpointsEnvironmentMap.Any<KeyValuePair<string, ReleaseEnvironmentData>>())
        return;
      ServiceEndpointPermissionValidator.EnsureServiceEndpointSecurityPermission(context, projectId, (IDictionary<string, ReleaseEnvironmentData>) dictionary, (IDictionary<string, ReleaseEnvironmentData>) newEndpointsEnvironmentMap);
    }

    public static void EnsureServiceEndpointSecurityPermission(
      IVssRequestContext context,
      Guid projectId,
      IList<ReleaseEnvironment> updatedEnvironments,
      IDictionary<string, ConfigurationVariableValue> updatedReleaseVariables,
      Release existingRelease,
      IList<TaskDefinition> taskDefinitions)
    {
      Dictionary<string, ReleaseEnvironmentData> newEndpointsEnvironmentMap = new Dictionary<string, ReleaseEnvironmentData>();
      Dictionary<string, ReleaseEnvironmentData> dictionary = new Dictionary<string, ReleaseEnvironmentData>();
      if (updatedEnvironments != null)
      {
        foreach (object updatedEnvironment in (IEnumerable<ReleaseEnvironment>) updatedEnvironments)
        {
          ReleaseEnvironment environment = JsonConvert.DeserializeObject<ReleaseEnvironment>(JsonConvert.SerializeObject(updatedEnvironment));
          ServiceEndpointPermissionValidator.MergeReleaseEnvironmentVariables(environment);
          foreach (DeployPhase deployPhase in (IEnumerable<DeployPhase>) environment.DeployPhasesSnapshot)
            deployPhase.WorkflowTasks = TaskGroupHelper.ReplaceMetaTaskWithTasksInWorkflow(context, projectId, deployPhase.WorkflowTasks);
          ReleaseEnvironmentData releaseEnvironmentData = ServiceEndpointVariablesHelper.ConvertToReleaseEnvironmentData(environment, updatedReleaseVariables);
          ServiceEndpointPermissionValidator.PopulateEndpointsEnvironmentMap(context, releaseEnvironmentData, newEndpointsEnvironmentMap, taskDefinitions);
        }
      }
      if (existingRelease != null)
      {
        foreach (object obj in existingRelease.Environments.Where<ReleaseEnvironment>((Func<ReleaseEnvironment, bool>) (env => newEndpointsEnvironmentMap.Keys.Contains<string>(ServiceEndpointVariablesHelper.GetEnvironmentKey(env)))).ToList<ReleaseEnvironment>())
        {
          ReleaseEnvironment environment = JsonConvert.DeserializeObject<ReleaseEnvironment>(JsonConvert.SerializeObject(obj));
          ServiceEndpointPermissionValidator.MergeReleaseEnvironmentVariables(environment);
          ReleaseEnvironmentData releaseEnvironmentData = ServiceEndpointVariablesHelper.ConvertToReleaseEnvironmentData(environment, (IDictionary<string, ConfigurationVariableValue>) existingRelease.Variables.Concat<KeyValuePair<string, ConfigurationVariableValue>>((IEnumerable<KeyValuePair<string, ConfigurationVariableValue>>) VariableGroupsMerger.GetMergedGroupVariables(existingRelease.VariableGroups)).GroupBy<KeyValuePair<string, ConfigurationVariableValue>, string>((Func<KeyValuePair<string, ConfigurationVariableValue>, string>) (x => x.Key)).ToDictionary<IGrouping<string, KeyValuePair<string, ConfigurationVariableValue>>, string, ConfigurationVariableValue>((Func<IGrouping<string, KeyValuePair<string, ConfigurationVariableValue>>, string>) (y => y.Key), (Func<IGrouping<string, KeyValuePair<string, ConfigurationVariableValue>>, ConfigurationVariableValue>) (y => y.First<KeyValuePair<string, ConfigurationVariableValue>>().Value)));
          ServiceEndpointPermissionValidator.PopulateEndpointsEnvironmentMap(context, releaseEnvironmentData, dictionary, taskDefinitions);
        }
      }
      if (!newEndpointsEnvironmentMap.Any<KeyValuePair<string, ReleaseEnvironmentData>>())
        return;
      ServiceEndpointPermissionValidator.EnsureServiceEndpointSecurityPermission(context, projectId, (IDictionary<string, ReleaseEnvironmentData>) dictionary, (IDictionary<string, ReleaseEnvironmentData>) newEndpointsEnvironmentMap);
    }

    [SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "by design")]
    private static void EnsureServiceEndpointSecurityPermission(
      IVssRequestContext context,
      Guid projectId,
      IDictionary<string, ReleaseEnvironmentData> existingEnvironmentServiceEndpointReferencesMap,
      IDictionary<string, ReleaseEnvironmentData> newEnvironmentServiceEndpointTasksReferencesMap)
    {
      if (newEnvironmentServiceEndpointTasksReferencesMap == null || !newEnvironmentServiceEndpointTasksReferencesMap.Any<KeyValuePair<string, ReleaseEnvironmentData>>())
        return;
      List<ServiceEndpointTasksReference> list = newEnvironmentServiceEndpointTasksReferencesMap.Values.SelectMany<ReleaseEnvironmentData, ServiceEndpointTasksReference>((Func<ReleaseEnvironmentData, IEnumerable<ServiceEndpointTasksReference>>) (e => (IEnumerable<ServiceEndpointTasksReference>) e.ServiceEndpointTasksReferences)).ToList<ServiceEndpointTasksReference>();
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> existingEndpoints;
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> authorizedEndpoints;
      ServiceEndpointPermissionValidator.FilterReferencedServiceEndpoints(context, projectId, (IList<ServiceEndpointTasksReference>) list, out existingEndpoints, out authorizedEndpoints);
      ServiceEndpointPermissionValidator.PopulateServiceEndpointIds(newEnvironmentServiceEndpointTasksReferencesMap, existingEndpoints);
      foreach (KeyValuePair<string, ReleaseEnvironmentData> endpointTasksReferences in (IEnumerable<KeyValuePair<string, ReleaseEnvironmentData>>) newEnvironmentServiceEndpointTasksReferencesMap)
      {
        if (endpointTasksReferences.Value.ServiceEndpointTasksReferences.Any<ServiceEndpointTasksReference>())
          ServiceEndpointPermissionValidator.EnsureServiceEndpointSecurityPermissionInEnvironment(context, endpointTasksReferences.Key, existingEnvironmentServiceEndpointReferencesMap, newEnvironmentServiceEndpointTasksReferencesMap, existingEndpoints, authorizedEndpoints);
      }
    }

    private static void EnsureServiceEndpointSecurityPermissionInEnvironment(
      IVssRequestContext context,
      string environmentKey,
      IDictionary<string, ReleaseEnvironmentData> existingEnvironmentServiceEndpointReferencesMap,
      IDictionary<string, ReleaseEnvironmentData> newEnvironmentServiceEndpointTasksReferencesMap,
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> existingEndpoints,
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> authorizedEndpoints)
    {
      IList<ServiceEndpointTasksReference> endpointTasksReferences = newEnvironmentServiceEndpointTasksReferencesMap[environmentKey].ServiceEndpointTasksReferences;
      if (!endpointTasksReferences.Any<ServiceEndpointTasksReference>())
        return;
      List<Guid> list1 = existingEndpoints.Select<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Guid>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, Guid>) (endpoint => endpoint.Id)).ToList<Guid>();
      List<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>> dirtyEndpoints = new List<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>>();
      List<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>> unauthorizedEndpoints = new List<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>>();
      List<Tuple<Guid, List<string>>> deletedEndpoints = new List<Tuple<Guid, List<string>>>();
      foreach (ServiceEndpointTasksReference endpointTasksReference in (IEnumerable<ServiceEndpointTasksReference>) endpointTasksReferences)
      {
        Guid endpointId = endpointTasksReference.EndpointId;
        List<string> list2 = endpointTasksReference.TasksUsingEndpoint.ToList<string>();
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint1 = authorizedEndpoints.FirstOrDefault<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, bool>) (ep => ep.Id == endpointId));
        if (serviceEndpoint1 == null)
        {
          if (!context.IsFeatureEnabled("AzureDevOps.ReleaseManagement.DeletedEndpointValidationsForServiceEndpoints") && !string.IsNullOrWhiteSpace(endpointTasksReference.EndpointName) && !list1.Contains(endpointId))
          {
            Tuple<Guid, List<string>> tuple = Tuple.Create<Guid, List<string>>(endpointId, list2);
            deletedEndpoints.Add(tuple);
          }
          else if (ServiceEndpointPermissionValidator.IsEndpointNewlyAdded(existingEnvironmentServiceEndpointReferencesMap, environmentKey, endpointId))
          {
            Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint2 = existingEndpoints.FirstOrDefault<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, bool>) (ep => ep.Id == endpointId));
            if (serviceEndpoint2 != null)
            {
              Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>> tuple = Tuple.Create<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>(serviceEndpoint2, list2);
              unauthorizedEndpoints.Add(tuple);
            }
          }
        }
        else if (!serviceEndpoint1.IsReady)
        {
          Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>> tuple = Tuple.Create<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>(serviceEndpoint1, list2);
          dirtyEndpoints.Add(tuple);
        }
      }
      ServiceEndpointPermissionValidator.CheckForAnyUnauthorizedEndpoints(newEnvironmentServiceEndpointTasksReferencesMap[environmentKey].EnvironmentName, dirtyEndpoints, unauthorizedEndpoints, deletedEndpoints);
    }

    private static void CheckForAnyUnauthorizedEndpoints(
      string environmentName,
      List<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>> dirtyEndpoints,
      List<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>> unauthorizedEndpoints,
      List<Tuple<Guid, List<string>>> deletedEndpoints)
    {
      if (dirtyEndpoints.Any<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>>())
      {
        List<string> list = dirtyEndpoints.SelectMany<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>, string>((Func<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>, IEnumerable<string>>) (endpoint => (IEnumerable<string>) ServiceEndpointPermissionValidator.GetEndpointErrorStrings(endpoint.Item1.Name, (IList<string>) endpoint.Item2))).ToList<string>();
        throw new ReleaseManagementUnauthorizedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ServiceEndpointInDirtyState, (object) environmentName, (object) string.Join(",", (IEnumerable<string>) list)));
      }
      if (unauthorizedEndpoints.Any<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>>())
      {
        List<string> list = unauthorizedEndpoints.SelectMany<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>, string>((Func<Tuple<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, List<string>>, IEnumerable<string>>) (endpoint => (IEnumerable<string>) ServiceEndpointPermissionValidator.GetEndpointErrorStrings(endpoint.Item1.Name, (IList<string>) endpoint.Item2))).ToList<string>();
        throw new ReleaseManagementUnauthorizedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ServiceEndpointNotAccessible, (object) environmentName, (object) string.Join(",", (IEnumerable<string>) list)));
      }
      if (deletedEndpoints.Any<Tuple<Guid, List<string>>>())
      {
        List<string> list = deletedEndpoints.SelectMany<Tuple<Guid, List<string>>, string>((Func<Tuple<Guid, List<string>>, IEnumerable<string>>) (endpoint => (IEnumerable<string>) ServiceEndpointPermissionValidator.GetEndpointErrorStrings(endpoint.Item1.ToString("D"), (IList<string>) endpoint.Item2))).ToList<string>();
        throw new ReleaseManagementUnauthorizedException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.ServiceEndpointNotAccessible, (object) environmentName, (object) string.Join(",", (IEnumerable<string>) list)));
      }
    }

    private static void GetListOfReferencedServiceEndpointIdsAndName(
      IList<ServiceEndpointTasksReference> endpointReferences,
      out IList<Guid> referencedEndpointIds,
      out IList<string> referencedEndpointNames)
    {
      referencedEndpointIds = (IList<Guid>) new List<Guid>();
      referencedEndpointNames = (IList<string>) new List<string>();
      foreach (ServiceEndpointTasksReference endpointReference in (IEnumerable<ServiceEndpointTasksReference>) endpointReferences)
      {
        if (endpointReference.EndpointId != Guid.Empty)
          referencedEndpointIds.Add(endpointReference.EndpointId);
        else if (!string.IsNullOrEmpty(endpointReference.EndpointName))
          referencedEndpointNames.Add(endpointReference.EndpointName);
      }
      referencedEndpointIds = (IList<Guid>) referencedEndpointIds.Distinct<Guid>().ToList<Guid>();
      referencedEndpointNames = (IList<string>) referencedEndpointNames.Distinct<string>().ToList<string>();
    }

    internal static void FilterReferencedServiceEndpoints(
      IVssRequestContext context,
      Guid projectId,
      IList<ServiceEndpointTasksReference> serviceEndpointTaskReferences,
      out IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> existingEndpoints,
      out IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> authorizedEndpoints)
    {
      IList<Guid> referencedEndpointIds;
      IList<string> referencedEndpointNames;
      ServiceEndpointPermissionValidator.GetListOfReferencedServiceEndpointIdsAndName(serviceEndpointTaskReferences, out referencedEndpointIds, out referencedEndpointNames);
      IServiceEndpointService2 service = context.GetService<IServiceEndpointService2>();
      existingEndpoints = (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) new List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>();
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> serviceEndpoints1 = ServiceEndpointPermissionValidator.GetServiceEndpoints(context, projectId, service, referencedEndpointIds);
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> serviceEndpoints2 = ServiceEndpointPermissionValidator.GetServiceEndpoints(context, projectId, service, referencedEndpointNames);
      existingEndpoints.AddRange<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>((IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) serviceEndpoints1);
      foreach (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint in (IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) serviceEndpoints2)
      {
        Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpointByName = serviceEndpoint;
        List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> list = existingEndpoints.Where<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, bool>) (endpoint => string.Equals(endpoint.Name, serviceEndpointByName.Name))).ToList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>();
        if (list == null || !list.Any<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>())
          existingEndpoints.Add(serviceEndpointByName);
      }
      authorizedEndpoints = (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) existingEndpoints.Where<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, bool>) (endpoint => endpoint.Url != (Uri) null)).ToList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>();
    }

    private static bool IsEndpointNewlyAdded(
      IDictionary<string, ReleaseEnvironmentData> existingEndpointsEnvironmentMap,
      string environmentKey,
      Guid endpointId)
    {
      bool flag = true;
      if (existingEndpointsEnvironmentMap.ContainsKey(environmentKey))
        flag = existingEndpointsEnvironmentMap[environmentKey].ServiceEndpointTasksReferences.Select<ServiceEndpointTasksReference, Guid>((Func<ServiceEndpointTasksReference, Guid>) (endpoint => endpoint.EndpointId)).ToList<Guid>().FirstOrDefault<Guid>((Func<Guid, bool>) (ep => ep == endpointId)) == Guid.Empty;
      return flag;
    }

    private static void PopulateServiceEndpointIds(
      IDictionary<string, ReleaseEnvironmentData> newEndpointsEnvironmentMap,
      IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> endpoints)
    {
      if (newEndpointsEnvironmentMap == null || endpoints == null)
        return;
      foreach (ServiceEndpointTasksReference endpointTasksReference in newEndpointsEnvironmentMap.Values.SelectMany<ReleaseEnvironmentData, ServiceEndpointTasksReference>((Func<ReleaseEnvironmentData, IEnumerable<ServiceEndpointTasksReference>>) (e => (IEnumerable<ServiceEndpointTasksReference>) e.ServiceEndpointTasksReferences)).ToList<ServiceEndpointTasksReference>())
      {
        Guid endpointId = endpointTasksReference.EndpointId;
        string endpointName = endpointTasksReference.EndpointName;
        Guid empty = Guid.Empty;
        if (endpointId == empty)
        {
          Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint = endpoints.Where<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>((Func<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint, bool>) (endpoint => string.Equals(endpoint.Name, endpointName, StringComparison.OrdinalIgnoreCase))).SingleOrDefault<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>();
          if (serviceEndpoint != null)
            endpointTasksReference.EndpointId = serviceEndpoint.Id;
        }
      }
    }

    private static IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> GetServiceEndpoints(
      IVssRequestContext context,
      Guid projectId,
      IServiceEndpointService2 endpointService,
      IList<Guid> endpointIds)
    {
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> result = new List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>();
      endpointIds.Batch<Guid>(40).ForEach<IList<Guid>>((Action<IList<Guid>>) (batch => result.AddRange((IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) endpointService.QueryServiceEndpoints(context, projectId, (string) null, (IEnumerable<string>) null, (IEnumerable<Guid>) batch, (string) null, true))));
      return (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) result;
    }

    private static IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> GetServiceEndpoints(
      IVssRequestContext context,
      Guid projectId,
      IServiceEndpointService2 endpointService,
      IList<string> endpointNames)
    {
      List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint> result = new List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>();
      endpointNames.Batch<string>(40).ForEach<IList<string>>((Action<IList<string>>) (batch => result.AddRange(context.RunSynchronously<IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>((Func<Task<IEnumerable<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>>) (() => endpointService.QueryServiceEndpointsAsync(context, projectId, (string) null, (IEnumerable<string>) null, (IEnumerable<string>) batch, (string) null, true))))));
      return (IList<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>) result;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "Using int id as a string key, we do not need localization")]
    private static void PopulateEndpointsEnvironmentMap(
      IVssRequestContext context,
      ReleaseEnvironmentData environmentData,
      Dictionary<string, ReleaseEnvironmentData> endpointsEnvironmentMap,
      IList<TaskDefinition> taskDefinitions)
    {
      IList<ServiceEndpointTasksReference> serviceEndpointsInUse = environmentData.DeployPhases.SelectMany<DeployPhase, WorkflowTask>((Func<DeployPhase, IEnumerable<WorkflowTask>>) (phase => (IEnumerable<WorkflowTask>) phase.WorkflowTasks)).GetServiceEndpointsInUse(context, environmentData.Variables, taskDefinitions);
      if (!serviceEndpointsInUse.Any<ServiceEndpointTasksReference>())
        return;
      string environmentKey = ServiceEndpointVariablesHelper.GetEnvironmentKey(environmentData);
      ReleaseEnvironmentData releaseEnvironmentData = new ReleaseEnvironmentData()
      {
        EnvironmentId = environmentData.EnvironmentId,
        EnvironmentName = environmentData.EnvironmentName,
        DeployPhases = environmentData.DeployPhases,
        Variables = environmentData.Variables,
        ServiceEndpointTasksReferences = serviceEndpointsInUse
      };
      endpointsEnvironmentMap.Add(environmentKey, releaseEnvironmentData);
    }

    private static void MergeReleaseDefinitionEnvironmentVariables(
      IVssRequestContext context,
      Guid projectId,
      ReleaseDefinitionEnvironment definitionEnvironment)
    {
      IList<Microsoft.TeamFoundation.DistributedTask.WebApi.VariableGroup> variableGroups = ReleaseDefinitionVariableGroupUtility.GetVariableGroups(context, projectId, definitionEnvironment.VariableGroups);
      IDictionary<string, ConfigurationVariableValue> variableGroupVariables = ReleaseDefinitionVariableGroupUtility.GetVariableGroupVariables(definitionEnvironment.VariableGroups, variableGroups);
      definitionEnvironment.Variables = (IDictionary<string, ConfigurationVariableValue>) definitionEnvironment.Variables.Concat<KeyValuePair<string, ConfigurationVariableValue>>((IEnumerable<KeyValuePair<string, ConfigurationVariableValue>>) variableGroupVariables).GroupBy<KeyValuePair<string, ConfigurationVariableValue>, string>((Func<KeyValuePair<string, ConfigurationVariableValue>, string>) (x => x.Key)).ToDictionary<IGrouping<string, KeyValuePair<string, ConfigurationVariableValue>>, string, ConfigurationVariableValue>((Func<IGrouping<string, KeyValuePair<string, ConfigurationVariableValue>>, string>) (y => y.Key), (Func<IGrouping<string, KeyValuePair<string, ConfigurationVariableValue>>, ConfigurationVariableValue>) (y => y.First<KeyValuePair<string, ConfigurationVariableValue>>().Value));
    }

    private static void MergeReleaseEnvironmentVariables(ReleaseEnvironment environment)
    {
      foreach (KeyValuePair<string, ConfigurationVariableValue> mergedGroupVariable in (IEnumerable<KeyValuePair<string, ConfigurationVariableValue>>) VariableGroupsMerger.GetMergedGroupVariables(environment.VariableGroups))
      {
        if (!environment.Variables.ContainsKey(mergedGroupVariable.Key))
          environment.Variables.Add(mergedGroupVariable.Key, mergedGroupVariable.Value);
      }
    }

    private static IList<string> GetEndpointErrorStrings(
      string endpointString,
      IList<string> taskFriendlyNames)
    {
      List<string> endpointErrorStrings = new List<string>();
      foreach (string taskFriendlyName in (IEnumerable<string>) taskFriendlyNames)
        endpointErrorStrings.Add(string.Format((IFormatProvider) CultureInfo.InvariantCulture, Resources.TaskIsUsingServiceEndpoint, (object) taskFriendlyName, (object) endpointString));
      return (IList<string>) endpointErrorStrings;
    }
  }
}
