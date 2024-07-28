// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.VariableGroupExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server
{
  public static class VariableGroupExtensions
  {
    private static readonly Guid AzureKeyVaultTaskId = new Guid("{1E244D32-2DD4-4165-96FB-B7441CA9331E}");

    public static void Validate(
      this AzureKeyVaultVariableGroupProviderData azureKeyVaultProviderData)
    {
      Guid guid = azureKeyVaultProviderData != null ? azureKeyVaultProviderData.ServiceEndpointId : throw new TeamFoundationServerInvalidRequestException(nameof (azureKeyVaultProviderData));
      if (azureKeyVaultProviderData.ServiceEndpointId.Equals(Guid.Empty))
        throw new TeamFoundationServerInvalidRequestException(TaskResources.AzureKeyVaultServiceEndpointIdMustBeValidGuid());
      DateTime dateTime = !azureKeyVaultProviderData.Vault.IsNullOrEmpty<char>() ? azureKeyVaultProviderData.LastRefreshedOn : throw new TeamFoundationServerInvalidRequestException(TaskResources.AzureKeyVaultKeyVaultNameMustBeValid());
    }

    public static void ReadExternalVariables(
      this VariableGroup group,
      IVssRequestContext context,
      Guid projectId)
    {
      switch (group.Type)
      {
        case "Vsts":
          break;
        case "AzureKeyVault":
          group.PopulateKeyVaultSecretValues(context, projectId);
          break;
        default:
          throw new DistributedTaskException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.VariableGroupTypeNotSupported((object) group.Type)));
      }
    }

    public static IList<TaskInstance> GetVariableGroupTasks(
      this VariableGroup group,
      IList<string> externalVariablesFilter)
    {
      IList<TaskInstance> variableGroupTasks = (IList<TaskInstance>) new List<TaskInstance>();
      switch (group.Type)
      {
        case "Vsts":
          return variableGroupTasks;
        case "AzureKeyVault":
          TaskInstance workflowTask = ((AzureKeyVaultVariableGroupProviderData) group.ProviderData).GetWorkflowTask(externalVariablesFilter);
          variableGroupTasks.Add(workflowTask);
          goto case "Vsts";
        default:
          throw new DistributedTaskException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, TaskResources.VariableGroupTypeNotSupported((object) group.Type)));
      }
    }

    public static TaskInstance GetWorkflowTask(
      this AzureKeyVaultVariableGroupProviderData azureKeyVaultVariableGroupProvider,
      IList<string> secretsFilter)
    {
      TaskInstance workflowTask = new TaskInstance();
      workflowTask.Enabled = true;
      workflowTask.Name = "AzureKeyVault";
      workflowTask.Id = VariableGroupExtensions.AzureKeyVaultTaskId;
      workflowTask.DisplayName = string.Format((IFormatProvider) CultureInfo.InvariantCulture, TaskResources.AzureKeyVaultTaskName((object) azureKeyVaultVariableGroupProvider.Vault));
      workflowTask.Version = "*";
      string str = "*";
      if (!secretsFilter.IsNullOrEmpty<string>())
        str = string.Join(",", (IEnumerable<string>) secretsFilter);
      workflowTask.Inputs["ConnectedServiceName"] = azureKeyVaultVariableGroupProvider.ServiceEndpointId.ToString();
      workflowTask.Inputs["KeyVaultName"] = azureKeyVaultVariableGroupProvider.Vault;
      workflowTask.Inputs["SecretsFilter"] = str;
      return workflowTask;
    }
  }
}
