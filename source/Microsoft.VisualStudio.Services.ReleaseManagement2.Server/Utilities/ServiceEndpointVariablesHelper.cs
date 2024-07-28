// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities.ServiceEndpointVariablesHelper
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Validations;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Utilities
{
  public static class ServiceEndpointVariablesHelper
  {
    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> GetReleaseEnvironmentsImpactedByVariableUpdate(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release,
      IList<TaskDefinition> taskDefinitions)
    {
      VariablesView variablesView = VariablesViewExtension.GetVariablesView(releaseDefinition);
      return ServiceEndpointVariablesHelper.GetReleaseEnvironmentsImpactedByVariableUpdate(requestContext, variablesView, release, taskDefinitions);
    }

    public static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> GetReleaseEnvironmentsImpactedByVariableUpdate(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release existingRelease,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release updatedRelease,
      IList<TaskDefinition> taskDefinitions)
    {
      VariablesView variablesView = VariablesViewExtension.GetVariablesView(existingRelease);
      return ServiceEndpointVariablesHelper.GetReleaseEnvironmentsImpactedByVariableUpdate(requestContext, variablesView, updatedRelease, taskDefinitions);
    }

    public static IList<ReleaseDefinitionEnvironment> GetReleaseEnvironmentsImpactedByVariableUpdate(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition existingReleaseDefinition,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition updatedReleaseDefinition,
      IList<TaskDefinition> taskDefinitions)
    {
      IList<ReleaseDefinitionEnvironment> byVariableUpdate = (IList<ReleaseDefinitionEnvironment>) new List<ReleaseDefinitionEnvironment>();
      if (updatedReleaseDefinition != null)
      {
        VariablesView variablesView = VariablesViewExtension.GetVariablesView(updatedReleaseDefinition);
        VariablesView diff;
        if (VariablesViewExtension.Diff(VariablesViewExtension.GetVariablesView(existingReleaseDefinition), variablesView, out diff))
        {
          IDictionary<string, IList<string>> endpointVariableMap = ServiceEndpointVariablesHelper.GetEnvironmentServiceEndpointVariableMap(requestContext, updatedReleaseDefinition, taskDefinitions);
          foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) updatedReleaseDefinition.Environments)
          {
            if (ServiceEndpointVariablesHelper.IsEnvironmentImpactedByVariableUpdate(variablesView, diff, ServiceEndpointVariablesHelper.GetEnvironmentKey(environment), endpointVariableMap))
              byVariableUpdate.Add(environment);
          }
        }
      }
      return byVariableUpdate;
    }

    public static IDictionary<string, IList<string>> GetEnvironmentServiceEndpointVariableMap(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition,
      IList<TaskDefinition> taskDefinitions)
    {
      Dictionary<string, IList<string>> endpointVariableMap = new Dictionary<string, IList<string>>();
      if (releaseDefinition != null && releaseDefinition.Environments != null)
      {
        foreach (ReleaseDefinitionEnvironment environment in (IEnumerable<ReleaseDefinitionEnvironment>) releaseDefinition.Environments)
        {
          string environmentKey = ServiceEndpointVariablesHelper.GetEnvironmentKey(environment);
          IList<string> namesUsedInTasks = environment.DeployPhases.SelectMany<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, WorkflowTask>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, IEnumerable<WorkflowTask>>) (phase => (IEnumerable<WorkflowTask>) phase.WorkflowTasks)).GetServiceEndpointVariableNamesUsedInTasks(requestContext, taskDefinitions);
          endpointVariableMap.Add(environmentKey, namesUsedInTasks);
        }
      }
      return (IDictionary<string, IList<string>>) endpointVariableMap;
    }

    public static IDictionary<string, IList<string>> GetEnvironmentServiceEndpointVariableMap(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release,
      IList<TaskDefinition> taskDefinitions)
    {
      Dictionary<string, IList<string>> endpointVariableMap = new Dictionary<string, IList<string>>();
      if (release != null && release.Environments != null)
      {
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) release.Environments)
        {
          string environmentKey = ServiceEndpointVariablesHelper.GetEnvironmentKey(environment);
          IList<string> namesUsedInTasks = environment.DeployPhasesSnapshot.SelectMany<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, WorkflowTask>((Func<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts.DeployPhase, IEnumerable<WorkflowTask>>) (phase => (IEnumerable<WorkflowTask>) phase.WorkflowTasks)).GetServiceEndpointVariableNamesUsedInTasks(requestContext, taskDefinitions);
          endpointVariableMap.Add(environmentKey, namesUsedInTasks);
        }
      }
      return (IDictionary<string, IList<string>>) endpointVariableMap;
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentData ConvertToReleaseEnvironmentData(
      ReleaseDefinitionEnvironment environment,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseVariables)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentData releaseEnvironmentData = (Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentData) null;
      if (environment == null)
        return releaseEnvironmentData;
      IDictionary<string, string> allVariables = ServiceEndpointVariablesHelper.GetAllVariables(releaseVariables, environment);
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentData()
      {
        EnvironmentId = environment.Id,
        EnvironmentName = environment.Name,
        DeployPhases = environment.DeployPhases,
        Variables = allVariables
      };
    }

    public static Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentData ConvertToReleaseEnvironmentData(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment,
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseVariables)
    {
      Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentData releaseEnvironmentData = (Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentData) null;
      if (environment == null)
        return releaseEnvironmentData;
      IDictionary<string, string> allVariables = ServiceEndpointVariablesHelper.GetAllVariables(releaseVariables, environment);
      return new Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentData()
      {
        EnvironmentId = environment.DefinitionEnvironmentId,
        EnvironmentName = environment.Name,
        DeployPhases = environment.DeployPhasesSnapshot,
        Variables = allVariables
      };
    }

    public static IDictionary<string, string> GetAllVariables(
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseVariablesConfigurationDictionary,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      IDictionary<string, string> stringDictionary1 = ServiceEndpointVariablesHelper.GetStringDictionary(releaseVariablesConfigurationDictionary);
      IDictionary<string, string> stringDictionary2 = ServiceEndpointVariablesHelper.GetStringDictionary(environment?.Variables);
      return DictionaryMerger.MergeDictionaries<string, string>((IEnumerable<IDictionary<string, string>>) new List<IDictionary<string, string>>()
      {
        (IDictionary<string, string>) (environment != null ? environment.ProcessParameters.GetProcessParametersInputs() : (Dictionary<string, string>) null),
        stringDictionary2,
        stringDictionary1
      });
    }

    public static IDictionary<string, string> GetAllVariables(
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> releaseVariablesConfigurationDictionary,
      Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ReleaseEnvironment environment)
    {
      IDictionary<string, string> stringDictionary1 = ServiceEndpointVariablesHelper.GetStringDictionary(releaseVariablesConfigurationDictionary);
      IDictionary<string, string> stringDictionary2 = ServiceEndpointVariablesHelper.GetStringDictionary(environment?.Variables);
      return DictionaryMerger.MergeDictionaries<string, string>((IEnumerable<IDictionary<string, string>>) new List<IDictionary<string, string>>()
      {
        (IDictionary<string, string>) (environment != null ? environment.ProcessParameters.GetProcessParametersInputs() : (Dictionary<string, string>) null),
        stringDictionary2,
        stringDictionary1
      });
    }

    public static IDictionary<string, string> GetAllVariables(
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> releaseVariablesConfigurationDictionary,
      ReleaseDefinitionEnvironment environment)
    {
      IDictionary<string, string> stringDictionary1 = ServiceEndpointVariablesHelper.GetStringDictionary(releaseVariablesConfigurationDictionary);
      IDictionary<string, string> stringDictionary2 = ServiceEndpointVariablesHelper.GetStringDictionary(environment?.Variables);
      return DictionaryMerger.MergeDictionaries<string, string>((IEnumerable<IDictionary<string, string>>) new List<IDictionary<string, string>>()
      {
        (IDictionary<string, string>) (environment != null ? environment.ProcessParameters.GetProcessParametersInputs() : (Dictionary<string, string>) null),
        stringDictionary2,
        stringDictionary1
      });
    }

    public static IDictionary<string, string> GetMergedEnvironmentVariables(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release release,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      IDictionary<string, string> stringDictionary1 = ServiceEndpointVariablesHelper.GetStringDictionary(release?.Variables);
      IDictionary<string, string> stringDictionary2 = ServiceEndpointVariablesHelper.GetStringDictionary(environment?.Variables);
      return DictionaryMerger.MergeDictionaries<string, string>((IEnumerable<IDictionary<string, string>>) new List<IDictionary<string, string>>()
      {
        (IDictionary<string, string>) (environment != null ? environment.ProcessParameters.GetProcessParametersInputs() : (Dictionary<string, string>) null),
        stringDictionary2,
        stringDictionary1
      });
    }

    public static IDictionary<string, string> GetMergedEnvironmentVariables(
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseDefinition releaseDefinition,
      ReleaseDefinitionEnvironment environment)
    {
      IDictionary<string, string> stringDictionary1 = ServiceEndpointVariablesHelper.GetStringDictionary(releaseDefinition?.Variables);
      IDictionary<string, string> stringDictionary2 = ServiceEndpointVariablesHelper.GetStringDictionary(environment?.Variables);
      return DictionaryMerger.MergeDictionaries<string, string>((IEnumerable<IDictionary<string, string>>) new List<IDictionary<string, string>>()
      {
        (IDictionary<string, string>) (environment != null ? environment.ProcessParameters.GetProcessParametersInputs() : (Dictionary<string, string>) null),
        stringDictionary2,
        stringDictionary1
      });
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "Using int id as a string key, we do not need localization")]
    public static string GetEnvironmentKey(Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment)
    {
      string environmentKey = (string) null;
      if (environment != null)
        environmentKey = environment.DefinitionEnvironmentId.ToString();
      return environmentKey;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "Using int id as a string key, we do not need localization")]
    public static string GetEnvironmentKey(ReleaseDefinitionEnvironment environment)
    {
      string environmentKey = (string) null;
      if (environment != null)
        environmentKey = environment.Id == 0 || environment.Id == -1 ? environment.Name : environment.Id.ToString();
      return environmentKey;
    }

    [SuppressMessage("Microsoft.Globalization", "CA1305:SpecifyIFormatProvider", Justification = "Using int id as a string key, we do not need localization")]
    public static string GetEnvironmentKey(Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseEnvironmentData environmentData)
    {
      string environmentKey = (string) null;
      if (environmentData != null)
        environmentKey = environmentData.EnvironmentId == 0 || environmentData.EnvironmentId == -1 ? environmentData.EnvironmentName : environmentData.EnvironmentId.ToString();
      return environmentKey;
    }

    private static IDictionary<string, string> GetStringDictionary(
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variables)
    {
      Dictionary<string, string> stringDictionary = new Dictionary<string, string>();
      if (variables != null)
      {
        foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ConfigurationVariableValue>>) variables)
        {
          string str = variable.Value.Value;
          stringDictionary.Add(variable.Key, str);
        }
      }
      return (IDictionary<string, string>) stringDictionary;
    }

    private static IDictionary<string, string> GetStringDictionary(
      IDictionary<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variables)
    {
      Dictionary<string, string> stringDictionary = new Dictionary<string, string>();
      if (variables != null)
      {
        foreach (KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue> variable in (IEnumerable<KeyValuePair<string, Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.ConfigurationVariableValue>>) variables)
        {
          string str = variable.Value.Value;
          stringDictionary.Add(variable.Key, str);
        }
      }
      return (IDictionary<string, string>) stringDictionary;
    }

    private static IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> GetReleaseEnvironmentsImpactedByVariableUpdate(
      IVssRequestContext requestContext,
      VariablesView existingVariableView,
      Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Release updatedRelease,
      IList<TaskDefinition> taskDefinitions)
    {
      IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment> byVariableUpdate = (IList<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) new List<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>();
      VariablesView variablesView = VariablesViewExtension.GetVariablesView(updatedRelease);
      VariablesView diff;
      if (VariablesViewExtension.Diff(existingVariableView, variablesView, out diff))
      {
        IDictionary<string, IList<string>> endpointVariableMap = ServiceEndpointVariablesHelper.GetEnvironmentServiceEndpointVariableMap(requestContext, updatedRelease, taskDefinitions);
        foreach (Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment environment in (IEnumerable<Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.ReleaseEnvironment>) updatedRelease.Environments)
        {
          if (ServiceEndpointVariablesHelper.IsEnvironmentImpactedByVariableUpdate(variablesView, diff, ServiceEndpointVariablesHelper.GetEnvironmentKey(environment), endpointVariableMap))
            byVariableUpdate.Add(environment);
        }
      }
      return byVariableUpdate;
    }

    private static bool IsEnvironmentImpactedByVariableUpdate(
      VariablesView newVariableView,
      VariablesView diffVariableView,
      string environmentKey,
      IDictionary<string, IList<string>> newEnvironmentServiceEndpointVariableMap)
    {
      bool flag = false;
      IDictionary<string, string> dictionary1;
      diffVariableView.MergedEnvironmentVariables.TryGetValue(environmentKey, out dictionary1);
      IDictionary<string, string> dictionary2;
      newVariableView.MergedEnvironmentVariables.TryGetValue(environmentKey, out dictionary2);
      IList<string> serviceEndpointVariables;
      if (newEnvironmentServiceEndpointVariableMap.TryGetValue(environmentKey, out serviceEndpointVariables) && ServiceEndpointVariablesHelper.ContainsEnvironmentLevelServiceEndpointVariables(dictionary1?.Keys, (ICollection<string>) serviceEndpointVariables, dictionary2?.Keys))
        flag = true;
      return flag;
    }

    private static bool ContainsEnvironmentLevelServiceEndpointVariables(
      ICollection<string> updatedVariables,
      ICollection<string> serviceEndpointVariables,
      ICollection<string> environmentVariables)
    {
      bool flag = false;
      if (updatedVariables == null || serviceEndpointVariables == null)
        return flag;
      IEnumerable<string> strings = updatedVariables.Intersect<string>((IEnumerable<string>) serviceEndpointVariables);
      if (strings.Any<string>() && environmentVariables != null && strings.Intersect<string>((IEnumerable<string>) environmentVariables).Any<string>())
        flag = true;
      return flag;
    }
  }
}
