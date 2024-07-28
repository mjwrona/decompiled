// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions.ReleaseWorkflowExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.DistributedTask.Common.Contracts;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Threading;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Exceptions;
using Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.DistributedTask;
using Microsoft.VisualStudio.Services.ReleaseManagement.Server.Properties;
using Microsoft.VisualStudio.Services.ReleaseManagement.WebApi.Contracts;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.Extensions
{
  public static class ReleaseWorkflowExtensions
  {
    [SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification = "This is required for strong typing")]
    public static IList<ServiceEndpointTasksReference> GetServiceEndpointsInUse(
      this IEnumerable<WorkflowTask> tasks,
      IVssRequestContext requestContext,
      IDictionary<string, string> variables,
      IList<TaskDefinition> taskDefinitions)
    {
      return tasks.GetServiceEndpointsInUseWithTypeAndTasksUsingThem(requestContext, variables, taskDefinitions);
    }

    public static IEnumerable<string> GetUniqueEndpointTypesInUse(
      this IEnumerable<WorkflowTask> tasks,
      IVssRequestContext requestContext,
      ProcessParameters processParameters)
    {
      return tasks.GetServiceEndpointsInUseWithTypeAndTasksUsingThem(requestContext, (IDictionary<string, string>) processParameters.GetProcessParametersInputs(), (IList<TaskDefinition>) null).Select<ServiceEndpointTasksReference, string>((Func<ServiceEndpointTasksReference, string>) (x => x.EndpointType)).Distinct<string>();
    }

    public static string GetUniqueAzureSubscriptionsInUse(
      this IEnumerable<WorkflowTask> tasks,
      IVssRequestContext requestContext,
      ProcessParameters processParameters,
      Guid projectId)
    {
      IEnumerable<Guid> azureServiceEndpointsInUse = tasks.GetServiceEndpointsInUseWithTypeAndTasksUsingThem(requestContext, (IDictionary<string, string>) processParameters.GetProcessParametersInputs(), (IList<TaskDefinition>) null).Where<ServiceEndpointTasksReference>((Func<ServiceEndpointTasksReference, bool>) (se => se.EndpointType == "AzureRM" || se.EndpointType == "Azure")).Select<ServiceEndpointTasksReference, Guid>((Func<ServiceEndpointTasksReference, Guid>) (se => se.EndpointId));
      StringBuilder stringBuilder = new StringBuilder();
      if (azureServiceEndpointsInUse.Count<Guid>() > 0)
      {
        ServiceEndpointHttpClient serviceEndpointHttpClient = requestContext.GetClient<ServiceEndpointHttpClient>();
        foreach (Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint serviceEndpoint in requestContext.RunSynchronously<List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>((Func<Task<List<Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi.ServiceEndpoint>>>) (() =>
        {
          ServiceEndpointHttpClient endpointHttpClient = serviceEndpointHttpClient;
          Guid project = projectId;
          IEnumerable<Guid> endpointIds = azureServiceEndpointsInUse;
          bool? nullable = new bool?(true);
          bool? includeFailed = new bool?();
          bool? includeDetails = nullable;
          ServiceEndpointActionFilter? actionFilter = new ServiceEndpointActionFilter?();
          CancellationToken cancellationToken = new CancellationToken();
          return endpointHttpClient.GetServiceEndpointsAsync(project, endpointIds: endpointIds, includeFailed: includeFailed, includeDetails: includeDetails, actionFilter: actionFilter, cancellationToken: cancellationToken);
        })))
        {
          string str;
          if (serviceEndpoint.Data.TryGetValue("SubscriptionId", out str))
            stringBuilder.Append(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0},", (object) str));
        }
      }
      return stringBuilder.ToString().Trim(',');
    }

    public static IList<string> GetServiceEndpointVariableNamesUsedInTasks(
      this IEnumerable<WorkflowTask> inputTasks,
      IVssRequestContext requestContext,
      IList<TaskDefinition> availableTaskDefinitions)
    {
      List<string> namesUsedInTasks = new List<string>();
      if (inputTasks == null)
        return (IList<string>) namesUsedInTasks;
      availableTaskDefinitions = ReleaseWorkflowExtensions.EnsureTaskDefinitions(requestContext, availableTaskDefinitions);
      Dictionary<Guid, IList<TaskVersion>> taskVersions;
      Dictionary<Guid, IDictionary<string, TaskDefinition>> taskDefinitions;
      ReleaseWorkflowExtensions.FetchTaskDefinitionsInUse((IList<TaskInstance>) inputTasks.Select<WorkflowTask, TaskInstance>((Func<WorkflowTask, TaskInstance>) (x => x.ToTaskInstance())).ToList<TaskInstance>(), availableTaskDefinitions, out taskVersions, out taskDefinitions);
      TaskVersionResolver taskVersionResolver = new TaskVersionResolver((IDictionary<Guid, IList<TaskVersion>>) taskVersions);
      foreach (WorkflowTask inputTask in inputTasks)
      {
        TaskDefinition taskDefinition;
        if (ReleaseWorkflowExtensions.IsValidTask(inputTask, taskVersionResolver, taskDefinitions, out taskDefinition))
        {
          foreach (TaskInputDefinition endpointTypeInput in (IEnumerable<TaskInputDefinition>) ReleaseWorkflowExtensions.GetServiceEndpointTypeInputs(taskDefinition))
          {
            string input;
            if (ReleaseWorkflowExtensions.TryGetServiceEndpointInputValue(inputTask, endpointTypeInput, out input) && VariableUtility.IsVariable(input))
            {
              List<string> referencedVariables = VariableUtility.GetReferencedVariables(input);
              for (int index = 0; index < referencedVariables.Count; ++index)
              {
                string str = referencedVariables[index].Substring(2, referencedVariables[index].Length - 3);
                if (!namesUsedInTasks.Contains(str))
                  namesUsedInTasks.Add(str);
              }
            }
          }
        }
      }
      return (IList<string>) namesUsedInTasks;
    }

    private static IList<ServiceEndpointTasksReference> GetServiceEndpointsInUseWithTypeAndTasksUsingThem(
      this IEnumerable<WorkflowTask> inputTasks,
      IVssRequestContext requestContext,
      IDictionary<string, string> variables,
      IList<TaskDefinition> availableTaskDefinitions)
    {
      List<ServiceEndpointTasksReference> serviceEndpointReferences = new List<ServiceEndpointTasksReference>();
      availableTaskDefinitions = ReleaseWorkflowExtensions.EnsureTaskDefinitions(requestContext, availableTaskDefinitions);
      Dictionary<Guid, IList<TaskVersion>> taskVersions;
      Dictionary<Guid, IDictionary<string, TaskDefinition>> taskDefinitions;
      ReleaseWorkflowExtensions.FetchTaskDefinitionsInUse((IList<TaskInstance>) inputTasks.Select<WorkflowTask, TaskInstance>((Func<WorkflowTask, TaskInstance>) (x => x.ToTaskInstance())).ToList<TaskInstance>(), availableTaskDefinitions, out taskVersions, out taskDefinitions);
      TaskVersionResolver taskVersionResolver = new TaskVersionResolver((IDictionary<Guid, IList<TaskVersion>>) taskVersions);
      foreach (WorkflowTask inputTask in inputTasks)
      {
        TaskDefinition taskDefinition;
        if (ReleaseWorkflowExtensions.IsValidTask(inputTask, taskVersionResolver, taskDefinitions, out taskDefinition))
        {
          foreach (TaskInputDefinition endpointTypeInput in (IEnumerable<TaskInputDefinition>) ReleaseWorkflowExtensions.GetServiceEndpointTypeInputs(taskDefinition))
          {
            string originalValue;
            if (ReleaseWorkflowExtensions.TryGetServiceEndpointInputValue(inputTask, endpointTypeInput, out originalValue))
            {
              string str1 = VariableUtility.ExpandVariables(originalValue.Trim(), variables);
              string str2 = !requestContext.IsFeatureEnabled("AzureDevOps.ReleaseManagement.VariableNameValidationsForServiceEndpoints") || !VariableUtility.IsVariable(str1) ? str1 : throw new FailedToResolveEndpointVariableException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, Resources.FailedToResolveServiceEndpointVariable, (object) str1));
              char[] chArray = new char[1]{ ',' };
              foreach (string inputValue in str2.Split(chArray))
                ReleaseWorkflowExtensions.UpdateServiceEndpointTaskReference(inputTask.Name, inputValue, originalValue, endpointTypeInput, (IList<ServiceEndpointTasksReference>) serviceEndpointReferences);
            }
          }
        }
      }
      return (IList<ServiceEndpointTasksReference>) serviceEndpointReferences;
    }

    private static void FetchTaskDefinitionsInUse(
      IList<TaskInstance> tasks,
      IList<TaskDefinition> availableTaskDefinitions,
      out Dictionary<Guid, IList<TaskVersion>> taskVersions,
      out Dictionary<Guid, IDictionary<string, TaskDefinition>> taskDefinitions)
    {
      taskVersions = new Dictionary<Guid, IList<TaskVersion>>();
      taskDefinitions = new Dictionary<Guid, IDictionary<string, TaskDefinition>>();
      if (availableTaskDefinitions == null)
        return;
      foreach (Guid guid in tasks.Select<TaskInstance, Guid>((Func<TaskInstance, Guid>) (task => task.Id)).Distinct<Guid>())
      {
        Guid taskId = guid;
        List<TaskVersion> taskVersionList = new List<TaskVersion>();
        Dictionary<string, TaskDefinition> dictionary = new Dictionary<string, TaskDefinition>();
        foreach (TaskDefinition taskDefinition in availableTaskDefinitions.Where<TaskDefinition>((Func<TaskDefinition, bool>) (x => x.Id.Equals(taskId))))
        {
          taskVersionList.Add(taskDefinition.Version);
          dictionary.Add((string) taskDefinition.Version, taskDefinition);
        }
        taskVersions.Add(taskId, (IList<TaskVersion>) taskVersionList);
        taskDefinitions.Add(taskId, (IDictionary<string, TaskDefinition>) dictionary);
      }
    }

    private static IList<TaskDefinition> EnsureTaskDefinitions(
      IVssRequestContext requestContext,
      IList<TaskDefinition> taskDefinitions)
    {
      IDistributedTaskPoolService service = requestContext.GetService<IDistributedTaskPoolService>();
      if (taskDefinitions == null)
        taskDefinitions = service.GetTaskDefinitions(requestContext);
      return taskDefinitions;
    }

    private static bool IsValidTask(
      WorkflowTask inputTask,
      TaskVersionResolver taskVersionResolver,
      Dictionary<Guid, IDictionary<string, TaskDefinition>> taskDefinitions,
      out TaskDefinition taskDefinition)
    {
      taskDefinition = (TaskDefinition) null;
      TaskVersion taskVersion;
      IDictionary<string, TaskDefinition> dictionary;
      return inputTask.Enabled && taskVersionResolver.TryResolveVersion(inputTask.TaskId, inputTask.Version, out taskVersion) && taskDefinitions.TryGetValue(inputTask.TaskId, out dictionary) && dictionary.TryGetValue((string) taskVersion, out taskDefinition);
    }

    private static IList<TaskInputDefinition> GetServiceEndpointTypeInputs(
      TaskDefinition taskDefinition)
    {
      List<TaskInputDefinition> endpointTypeInputs = new List<TaskInputDefinition>();
      if (taskDefinition != null)
        endpointTypeInputs = taskDefinition.Inputs.Where<TaskInputDefinition>((Func<TaskInputDefinition, bool>) (x => x.IsEndpointTypeInput())).ToList<TaskInputDefinition>();
      return (IList<TaskInputDefinition>) endpointTypeInputs;
    }

    private static string GetEndpointType(TaskInputDefinition serviceEndpointTypeInput)
    {
      string endpointType = string.Empty;
      string[] strArray = serviceEndpointTypeInput.InputType.Split(':');
      if (strArray.Length > 1 && !string.IsNullOrWhiteSpace(strArray[1]))
        endpointType = strArray[1].Trim();
      return endpointType;
    }

    private static bool TryGetServiceEndpointInputValue(
      WorkflowTask inputTask,
      TaskInputDefinition serviceEndpointTypeInput,
      out string value)
    {
      return inputTask.Inputs.TryGetValue(serviceEndpointTypeInput.Name, out value) && !string.IsNullOrEmpty(value) && serviceEndpointTypeInput.IsEndpointInputVisible((IDictionary<string, string>) inputTask.Inputs);
    }

    private static void UpdateServiceEndpointTaskReference(
      string inputTaskName,
      string inputValue,
      string originalValue,
      TaskInputDefinition serviceEndpointTypeInput,
      IList<ServiceEndpointTasksReference> serviceEndpointReferences)
    {
      string endpointName = (string) null;
      ServiceEndpointTasksReference endpointTasksReference;
      Guid endpointId;
      if (Guid.TryParse(inputValue, out endpointId))
      {
        endpointTasksReference = serviceEndpointReferences.Where<ServiceEndpointTasksReference>((Func<ServiceEndpointTasksReference, bool>) (endpoint => endpoint.EndpointId == endpointId)).SingleOrDefault<ServiceEndpointTasksReference>();
      }
      else
      {
        endpointName = inputValue;
        endpointTasksReference = serviceEndpointReferences.Where<ServiceEndpointTasksReference>((Func<ServiceEndpointTasksReference, bool>) (endpoint => string.Equals(endpoint.EndpointName, endpointName))).SingleOrDefault<ServiceEndpointTasksReference>();
      }
      if (endpointTasksReference == null)
      {
        string endpointType = ReleaseWorkflowExtensions.GetEndpointType(serviceEndpointTypeInput);
        endpointTasksReference = new ServiceEndpointTasksReference()
        {
          EndpointName = endpointName,
          EndpointId = endpointId,
          EndpointType = endpointType,
          EndpointReference = originalValue
        };
        serviceEndpointReferences.Add(endpointTasksReference);
      }
      endpointTasksReference?.TasksUsingEndpoint.Add(inputTaskName);
    }
  }
}
