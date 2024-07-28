// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Server.ArgumentValidation
// Assembly: Microsoft.TeamFoundation.DistributedTask.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: EDA28B25-3B93-49F7-A194-C85AAF98B83A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.DistributedTask.Server
{
  internal static class ArgumentValidation
  {
    public static void CheckAgent(TaskAgent agent, string parameterName, bool isAdd = true)
    {
      ArgumentUtility.CheckForNull<TaskAgent>(agent, parameterName, "DistributedTask");
      string name = agent.Name;
      ArgumentValidation.CheckAgentName(ref name, ArgumentValidation.GetPropertyName(parameterName, "Name"), !isAdd);
      agent.Name = name;
      ArgumentValidation.CheckAgentVersion(agent.Version, ArgumentValidation.GetPropertyName(parameterName, "Version"), !isAdd);
      ArgumentValidation.CheckAgentCapabilities(agent.SystemCapabilities, ArgumentValidation.GetPropertyName(parameterName, "SystemCapabilities"));
      ArgumentValidation.CheckAgentCapabilities(agent.UserCapabilities, ArgumentValidation.GetPropertyName(parameterName, "UserCapabilities"));
    }

    public static void CheckDeploymentMachine(
      DeploymentMachine machine,
      string parameterName,
      bool isAdd = true,
      bool validateAgent = true)
    {
      ArgumentUtility.CheckForNull<DeploymentMachine>(machine, parameterName, "DistributedTask");
      ArgumentUtility.CheckForNull<TaskAgent>(machine.Agent, ArgumentValidation.GetPropertyName(parameterName, "Agent"), "DistributedTask");
      if (validateAgent)
        ArgumentValidation.CheckAgent(machine.Agent, ArgumentValidation.GetPropertyName(parameterName, "Agent"), isAdd);
      ArgumentValidation.CheckDeploymentTargetTags(machine.Tags);
    }

    public static void CheckVirtualMachineResource(
      VirtualMachineResource resource,
      string parameterName,
      bool isAdd = true,
      bool validateAgent = true)
    {
      ArgumentUtility.CheckForNull<VirtualMachineResource>(resource, parameterName, "DistributedTask");
      ArgumentUtility.CheckForNull<string>(resource.Name, "virtual machine resource name", "DistributedTask");
      ArgumentUtility.CheckForNull<TaskAgent>(resource.Agent, ArgumentValidation.GetPropertyName(parameterName, "Agent"), "DistributedTask");
      string name = resource.Name;
      ArgumentValidation.CheckVirtualMachineResourceName(ref name, "vmResourceName", false);
      if (validateAgent)
        ArgumentValidation.CheckAgent(resource.Agent, ArgumentValidation.GetPropertyName(parameterName, "Agent"), isAdd);
      ArgumentValidation.CheckDeploymentTargetTags(resource.Tags);
    }

    public static void CheckAgentSession(TaskAgentSession session, string parameterName)
    {
      ArgumentUtility.CheckForNull<TaskAgentSession>(session, parameterName, "DistributedTask");
      ArgumentUtility.CheckForNull<TaskAgentReference>(session.Agent, ArgumentValidation.GetPropertyName(parameterName, "Agent"), "DistributedTask");
      if (string.IsNullOrEmpty(session.OwnerName) || session.OwnerName.Length > 512)
        throw new ArgumentException(TaskResources.InvalidSessionOwnerName(), ArgumentValidation.GetPropertyName(parameterName, "OwnerName")).Expected("DistributedTask");
      ArgumentValidation.CheckAgentVersion(session.Agent.Version, ArgumentValidation.GetPropertyName(parameterName, PipelineConstants.AgentVersionDemandName), true);
      ArgumentValidation.CheckAgentCapabilities(session.SystemCapabilities, ArgumentValidation.GetPropertyName(parameterName, "SystemCapabilities"));
    }

    public static void CheckPool(TaskAgentPool pool, string parameterName, bool isAdd = true)
    {
      ArgumentUtility.CheckForNull<TaskAgentPool>(pool, parameterName, "DistributedTask");
      string name = pool.Name;
      ArgumentValidation.CheckPoolName(ref name, ArgumentValidation.GetPropertyName(parameterName, "Name"), !isAdd);
      pool.Name = name;
    }

    public static void CheckQueue(TaskAgentQueue queue, string parameterName, bool isAdd = true)
    {
      ArgumentUtility.CheckForNull<TaskAgentQueue>(queue, parameterName, "DistributedTask");
      string name = queue.Name;
      ArgumentValidation.CheckQueueName(ref name, ArgumentValidation.GetPropertyName(parameterName, "Name"), !isAdd);
      queue.Name = name;
    }

    public static void CheckDeploymentGroup(
      DeploymentGroup machineGroup,
      string parameterName,
      bool isAdd = true)
    {
      ArgumentUtility.CheckForNull<DeploymentGroup>(machineGroup, parameterName, "DistributedTask");
      string name = machineGroup.Name;
      ArgumentValidation.CheckDeploymentGroupName(ref name, ArgumentValidation.GetPropertyName(parameterName, "Name"), !isAdd);
      machineGroup.Name = name;
    }

    public static void CheckAgentCapability(string name, string value, string parameterName)
    {
      if (string.IsNullOrEmpty(name))
        throw new ArgumentException(TaskResources.InvalidCapabilityName(), parameterName).Expected("DistributedTask");
      if (name.Length > 256 || char.IsWhiteSpace(name[0]) || char.IsWhiteSpace(name[name.Length - 1]))
        throw new ArgumentException(TaskResources.InvalidCapabilityNameWithValue((object) name), parameterName).Expected("DistributedTask");
      if (!string.IsNullOrEmpty(value) && value.Length > 1024)
        throw new ArgumentException(TaskResources.InvalidCapabilityValue((object) name), parameterName).Expected("DistributedTask");
    }

    public static void CheckAgentCapabilities(
      IDictionary<string, string> capabilities,
      string parameterName)
    {
      if (capabilities == null || capabilities.Count == 0)
        return;
      foreach (string str in capabilities.Keys.ToArray<string>())
      {
        string capability = capabilities[str];
        if (capability == null)
          capabilities.Remove(str);
        else
          ArgumentValidation.CheckAgentCapability(str, capability, parameterName);
      }
    }

    public static void CheckAgentName(ref string agentName, string parameterName, bool allowNull)
    {
      if (allowNull && string.IsNullOrEmpty(agentName))
      {
        agentName = (string) null;
      }
      else
      {
        ArgumentUtility.CheckStringForNullOrEmpty(agentName, parameterName, "DistributedTask");
        if (agentName.Length > 64)
          throw new ArgumentException(TaskResources.InvalidAgentName((object) agentName), parameterName).Expected("DistributedTask");
        if (agentName.IndexOfAny(FileSpec.IllegalNtfsCharsAndWildcards) >= 0)
          throw new ArgumentException(TaskResources.InvalidAgentName((object) agentName), parameterName).Expected("DistributedTask");
        agentName = agentName.Trim();
      }
    }

    public static void CheckAgentVersion(string agentVersion, string parameterName, bool allowNull)
    {
      if (string.IsNullOrEmpty(agentVersion))
      {
        if (!allowNull)
          throw new ArgumentException(TaskResources.InvalidAgentVersion(), parameterName).Expected("DistributedTask");
      }
      else if (DemandMinimumVersion.ParseVersion(agentVersion) == (Version) null)
        throw new ArgumentException(TaskResources.InvalidAgentVersion(), parameterName).Expected("DistributedTask");
    }

    public static void CheckPoolName(ref string poolName, string parameterName, bool allowNull)
    {
      if (allowNull && string.IsNullOrEmpty(poolName))
      {
        poolName = (string) null;
      }
      else
      {
        if (poolName != null)
          poolName = poolName.Trim();
        ArgumentUtility.CheckStringForNullOrEmpty(poolName, parameterName, "DistributedTask");
        ArgumentValidation.CheckInvalidCharacters(ref poolName, parameterName, allowNull, TaskResources.InvalidPoolName((object) poolName));
      }
    }

    public static void CheckQueueName(ref string queueName, string parameterName, bool allowNull) => ArgumentValidation.CheckInvalidCharacters(ref queueName, parameterName, allowNull, TaskResources.InvalidQueueName((object) queueName));

    public static void CheckDeploymentGroupName(
      ref string machineGroupName,
      string parameterName,
      bool allowNull)
    {
      ArgumentValidation.CheckInvalidCharacters(ref machineGroupName, parameterName, allowNull, TaskResources.InvalidDeploymentMachineGroupName((object) machineGroupName));
    }

    public static void CheckEnvironmentName(
      ref string environmentName,
      string parameterName,
      bool allowNull)
    {
      ArgumentValidation.CheckInvalidCharacters(ref environmentName, parameterName, allowNull, TaskResources.InvalidEnvironmentName((object) environmentName), illegalChars: ArgumentValidation.IllegalNameCharsForEnvironmentService);
    }

    public static void CheckVirtualMachineResourceName(
      ref string vmResourceName,
      string parameterName,
      bool allowNull)
    {
      ArgumentValidation.CheckInvalidCharacters(ref vmResourceName, parameterName, allowNull, TaskResources.InvalidEnvironmentVirtualMachineResourceName((object) vmResourceName), illegalChars: ArgumentValidation.IllegalNameCharsForVirtualMachineResource);
    }

    public static void CheckVariableGroup(VariableGroup group, string parameterName)
    {
      if (group == null)
        throw new ArgumentException(TaskResources.InvalidVariableGroup()).Expected("DistributedTask");
      ArgumentValidation.ValidateVariableGroupProjectReferences(group, parameterName);
      string name = group.Name;
      ArgumentValidation.CheckVariableGroupName(ref name, ArgumentValidation.GetPropertyName(parameterName, "Name"), false);
      group.Name = name;
      if (group.Description != null)
      {
        group.Description = group.Description.Trim();
        ArgumentUtility.CheckStringLength(group.Description, ArgumentValidation.GetPropertyName(parameterName, "Description"), 1024, expectedServiceArea: "DistributedTask");
      }
      if (group.Variables.IsNullOrEmpty<KeyValuePair<string, VariableValue>>())
        throw new ArgumentException(TaskResources.VariableGroupEmptyError()).Expected("DistributedTask");
      switch (group.Type)
      {
        case "Vsts":
          ArgumentValidation.CheckAndSanitizeVariables(group, parameterName);
          break;
        case "AzureKeyVault":
          (group.ProviderData as AzureKeyVaultVariableGroupProviderData).Validate();
          goto case "Vsts";
        default:
          throw new ArgumentException(Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.TaskResources.VariableGroupTypeNotSupported((object) group.Type)).Expected("DistributedTask");
      }
    }

    private static void ValidateVariableGroupProjectReferences(
      VariableGroup group,
      string parameterName)
    {
      if (group.VariableGroupProjectReferences.IsNullOrEmpty<VariableGroupProjectReference>())
        return;
      foreach (VariableGroupProjectReference projectReference in (IEnumerable<VariableGroupProjectReference>) group.VariableGroupProjectReferences)
        ArgumentValidation.CheckVariableGroupProjectReference(projectReference, parameterName);
      IDictionary<Guid, bool> dictionary = (IDictionary<Guid, bool>) new Dictionary<Guid, bool>();
      foreach (VariableGroupProjectReference projectReference in (IEnumerable<VariableGroupProjectReference>) group.VariableGroupProjectReferences)
      {
        if (dictionary.ContainsKey(projectReference.ProjectReference.Id))
          throw new ArgumentException(TaskResources.CannotCreateSameVariableGroupMultipleTime()).Expected("DistributedTask");
        dictionary.Add(projectReference.ProjectReference.Id, true);
      }
    }

    public static void CheckVariableGroupProjectReference(
      VariableGroupProjectReference variableGroupProjectReference,
      string parameterName)
    {
      string groupName = variableGroupProjectReference != null ? variableGroupProjectReference.Name : throw new ArgumentException(TaskResources.InvalidVariableGroup()).Expected("DistributedTask");
      ArgumentValidation.CheckVariableGroupName(ref groupName, ArgumentValidation.GetPropertyName(parameterName, "Name"), false);
      variableGroupProjectReference.Name = groupName;
      if (variableGroupProjectReference.Description == null)
        return;
      variableGroupProjectReference.Description = variableGroupProjectReference.Description.Trim();
      ArgumentUtility.CheckStringLength(variableGroupProjectReference.Description, ArgumentValidation.GetPropertyName(parameterName, "Description"), 1024, expectedServiceArea: "DistributedTask");
    }

    public static void CheckDeploymentTargetTags(IList<string> tags)
    {
      if (tags == null || !tags.Any<string>())
        return;
      if (tags.Any<string>((Func<string, bool>) (t => t == null)))
        throw new ArgumentException(TaskResources.InvalidDeploymentTagWithNullValue());
      string str = tags.FirstOrDefault<string>((Func<string, bool>) (tag => tag.Length > 256));
      if (str != null)
        throw new ArgumentException(TaskResources.InvalidDeploymentTargetTagLength((object) str));
      if (tags.Count<string>() != tags.Distinct<string>().Count<string>())
        throw new ArgumentException(TaskResources.DuplicateTagsNotSupported((object) string.Join(", ", (IEnumerable<string>) tags)));
    }

    public static void CheckEnvironment(
      EnvironmentInstance environment,
      string parameterName,
      bool isAdd = true)
    {
      ArgumentUtility.CheckForNull<EnvironmentInstance>(environment, parameterName, "DistributedTask");
      string name = environment.Name;
      ArgumentValidation.CheckEnvironmentName(ref name, ArgumentValidation.GetPropertyName(parameterName, "Name"), !isAdd);
      if (environment.Description != null)
      {
        environment.Description = environment.Description.Trim();
        ArgumentUtility.CheckStringLength(environment.Description, ArgumentValidation.GetPropertyName(parameterName, "Description"), 1024, expectedServiceArea: "DistributedTask");
      }
      environment.Name = name;
    }

    public static void CheckEnvironmentDeploymentExecutionRecord(
      EnvironmentDeploymentExecutionRecord executionRecord,
      string parameterName)
    {
      ArgumentUtility.CheckForNull<EnvironmentDeploymentExecutionRecord>(executionRecord, parameterName, "DistributedTask");
      ArgumentUtility.CheckForNonPositiveInt(executionRecord.EnvironmentId, ArgumentValidation.GetPropertyName(parameterName, "EnvironmentId"), "DistributedTask");
      ArgumentUtility.CheckForEmptyGuid(executionRecord.ScopeId, ArgumentValidation.GetPropertyName(parameterName, "ScopeId"), "DistributedTask");
      ArgumentUtility.CheckForEmptyGuid(executionRecord.ServiceOwner, ArgumentValidation.GetPropertyName(parameterName, "ServiceOwner"), "DistributedTask");
      ArgumentUtility.CheckForEmptyGuid(executionRecord.PlanId, ArgumentValidation.GetPropertyName(parameterName, "PlanId"), "DistributedTask");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(executionRecord.PlanType, ArgumentValidation.GetPropertyName(parameterName, "PlanType"), "DistributedTask");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(executionRecord.StageName, ArgumentValidation.GetPropertyName(parameterName, "StageName"), "DistributedTask");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(executionRecord.JobName, ArgumentValidation.GetPropertyName(parameterName, "JobName"), "DistributedTask");
      ArgumentUtility.CheckForNull<TaskOrchestrationOwner>(executionRecord.Owner, ArgumentValidation.GetPropertyName(parameterName, "Owner"), "DistributedTask");
      ArgumentUtility.CheckForNull<TaskOrchestrationOwner>(executionRecord.Definition, ArgumentValidation.GetPropertyName(parameterName, "Definition"), "DistributedTask");
    }

    public static void CheckEnvironmentResourceDeploymentExecutionRecord(
      EnvironmentResourceDeploymentExecutionRecord executionRecord,
      string parameterName)
    {
      ArgumentUtility.CheckForNull<EnvironmentResourceDeploymentExecutionRecord>(executionRecord, parameterName, "DistributedTask");
      ArgumentUtility.CheckForNonPositiveInt(executionRecord.EnvironmentId, ArgumentValidation.GetPropertyName(parameterName, "EnvironmentId"), "DistributedTask");
      ArgumentUtility.CheckForNonPositiveInt(executionRecord.ResourceId, ArgumentValidation.GetPropertyName(parameterName, "ResourceId"), "DistributedTask");
    }

    public static void CheckKubernetesResource(KubernetesResource resource)
    {
      ArgumentValidation.CheckResource((EnvironmentResource) resource);
      ArgumentUtility.CheckForNull<string>(resource.Namespace, "resourceNamespace", "DistributedTask");
      resource.Namespace = resource.Namespace.Trim();
      ArgumentUtility.CheckStringLength(resource.Namespace, "resourceNamespace", 256, 1, "DistributedTask");
    }

    public static void CheckResource(EnvironmentResource resource)
    {
      ArgumentUtility.CheckForNull<EnvironmentResource>(resource, nameof (resource), "DistributedTask");
      string name = resource.Name;
      ArgumentValidation.CheckInvalidCharacters(ref name, "resourceName", false, TaskResources.InvalidResourceName((object) resource.Name), 256, ArgumentValidation.IllegalNameCharsForEnvironmentService);
      resource.Name = name;
    }

    private static void CheckAndSanitizeVariables(VariableGroup group, string parameterName)
    {
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) group.Variables)
      {
        ArgumentUtility.CheckForNull<string>(variable.Key, "Key");
        ArgumentUtility.CheckForNull<VariableValue>(variable.Value, "Value");
        ArgumentUtility.CheckStringLength(variable.Key.Trim(), ArgumentValidation.GetPropertyName(parameterName, "variable.Key"), 400, 1, "DistributedTask");
        ArgumentValidation.CheckVariableValueLength(variable.Value.Value, variable.Key, 4096, 0);
      }
      List<string> list = group.Variables.GroupBy<KeyValuePair<string, VariableValue>, string>((Func<KeyValuePair<string, VariableValue>, string>) (x => x.Key.Trim()), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Where<IGrouping<string, KeyValuePair<string, VariableValue>>>((Func<IGrouping<string, KeyValuePair<string, VariableValue>>, bool>) (x => x.Count<KeyValuePair<string, VariableValue>>() > 1)).Select<IGrouping<string, KeyValuePair<string, VariableValue>>, string>((Func<IGrouping<string, KeyValuePair<string, VariableValue>>, string>) (x => x.Key)).ToList<string>();
      if (list.Count > 0)
        throw new ArgumentException(TaskResources.DuplicateKeyInVariableGroup((object) group.Name, (object) string.Join<string>(", ", (IEnumerable<string>) list)));
      Dictionary<string, VariableValue> dictionary = new Dictionary<string, VariableValue>();
      foreach (KeyValuePair<string, VariableValue> variable in (IEnumerable<KeyValuePair<string, VariableValue>>) group.Variables)
        dictionary.Add(variable.Key.Trim(), variable.Value);
      group.Variables = (IDictionary<string, VariableValue>) dictionary;
    }

    private static void CheckVariableValueLength(
      string variableValue,
      string variableKey,
      int maxLength,
      int minLength)
    {
      if (variableValue != null && (variableValue.Length < minLength || variableValue.Length > maxLength))
        throw new ArgumentException(string.Format(TaskResources.InvalidVariableGroupVariableLength((object) variableKey, (object) minLength, (object) maxLength)));
    }

    private static string GetPropertyName(string objectName, string propertyName) => string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}.{1}", (object) objectName, (object) propertyName);

    private static void CheckVariableGroupName(
      ref string groupName,
      string parameterName,
      bool allowNull)
    {
      ArgumentValidation.CheckInvalidCharacters(ref groupName, parameterName, allowNull, TaskResources.InvalidVariableGroupName((object) groupName));
    }

    public static void CheckSecureFile(SecureFile secureFile, string parameterName)
    {
      ArgumentUtility.CheckForNull<SecureFile>(secureFile, parameterName, "DistributedTask");
      string name = secureFile.Name;
      ArgumentValidation.CheckSecureFileName(ref name, "Name");
      secureFile.Name = name;
    }

    private static void CheckSecureFileName(ref string secureFileName, string parameterName)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(secureFileName, parameterName, "DistributedTask");
      secureFileName = secureFileName.Trim();
      if (secureFileName.Length > (int) byte.MaxValue)
        throw new ArgumentException(TaskResources.InvalidSecureFileName((object) secureFileName), parameterName).Expected("DistributedTask");
      if (secureFileName.IndexOfAny(FileSpec.IllegalNtfsCharsAndWildcards) >= 0)
        throw new ArgumentException(TaskResources.InvalidSecureFileName((object) secureFileName), parameterName).Expected("DistributedTask");
    }

    private static void CheckInvalidCharacters(
      ref string parameterValue,
      string parameterName,
      bool allowNull,
      string errorMessage,
      int maxLength = 128,
      char[] illegalChars = null)
    {
      if (allowNull && string.IsNullOrEmpty(parameterValue))
      {
        parameterValue = (string) null;
      }
      else
      {
        if (parameterValue != null)
          parameterValue = parameterValue.Trim();
        ArgumentUtility.CheckStringForNullOrEmpty(parameterValue, parameterName, "DistributedTask");
        if (illegalChars == null)
          illegalChars = SidIdentityHelper.IllegalNameChars;
        if (parameterValue.Length > maxLength || parameterValue.IndexOfAny(illegalChars) >= 0)
        {
          string str = TaskResources.ValidNameSuggestion((object) maxLength, (object) string.Join(", ", ((IEnumerable<char>) illegalChars).Where<char>((Func<char, bool>) (c => !char.IsControl(c) && c < '\uFFFE')).Select<char, string>((Func<char, string>) (c => string.Format("'{0}'", (object) c)))));
          throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "{0} {1}", (object) errorMessage, (object) str)).Expected("DistributedTask");
        }
      }
    }

    private static char[] IllegalNameCharsForEnvironmentService => new List<char>((IEnumerable<char>) SidIdentityHelper.IllegalNameChars)
    {
      '.'
    }.ToArray();

    private static char[] IllegalNameCharsForVirtualMachineResource => new List<char>((IEnumerable<char>) SidIdentityHelper.IllegalNameChars)
    {
      '.'
    }.ToArray();
  }
}
