// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.PolicySetDefinitionsOperationsExtensions
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public static class PolicySetDefinitionsOperationsExtensions
  {
    public static PolicySetDefinition CreateOrUpdate(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName,
      PolicySetDefinition parameters)
    {
      return operations.CreateOrUpdateAsync(policySetDefinitionName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<PolicySetDefinition> CreateOrUpdateAsync(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName,
      PolicySetDefinition parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicySetDefinition body;
      using (AzureOperationResponse<PolicySetDefinition> _result = await operations.CreateOrUpdateWithHttpMessagesAsync(policySetDefinitionName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void Delete(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName)
    {
      operations.DeleteAsync(policySetDefinitionName).GetAwaiter().GetResult();
    }

    public static async Task DeleteAsync(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteWithHttpMessagesAsync(policySetDefinitionName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static PolicySetDefinition Get(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName)
    {
      return operations.GetAsync(policySetDefinitionName).GetAwaiter().GetResult();
    }

    public static async Task<PolicySetDefinition> GetAsync(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicySetDefinition body;
      using (AzureOperationResponse<PolicySetDefinition> _result = await operations.GetWithHttpMessagesAsync(policySetDefinitionName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static PolicySetDefinition GetBuiltIn(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName)
    {
      return operations.GetBuiltInAsync(policySetDefinitionName).GetAwaiter().GetResult();
    }

    public static async Task<PolicySetDefinition> GetBuiltInAsync(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicySetDefinition body;
      using (AzureOperationResponse<PolicySetDefinition> _result = await operations.GetBuiltInWithHttpMessagesAsync(policySetDefinitionName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicySetDefinition> List(this IPolicySetDefinitionsOperations operations) => operations.ListAsync().GetAwaiter().GetResult();

    public static async Task<IPage<PolicySetDefinition>> ListAsync(
      this IPolicySetDefinitionsOperations operations,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicySetDefinition> body;
      using (AzureOperationResponse<IPage<PolicySetDefinition>> _result = await operations.ListWithHttpMessagesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicySetDefinition> ListBuiltIn(
      this IPolicySetDefinitionsOperations operations)
    {
      return operations.ListBuiltInAsync().GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicySetDefinition>> ListBuiltInAsync(
      this IPolicySetDefinitionsOperations operations,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicySetDefinition> body;
      using (AzureOperationResponse<IPage<PolicySetDefinition>> _result = await operations.ListBuiltInWithHttpMessagesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static PolicySetDefinition CreateOrUpdateAtManagementGroup(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName,
      PolicySetDefinition parameters,
      string managementGroupId)
    {
      return operations.CreateOrUpdateAtManagementGroupAsync(policySetDefinitionName, parameters, managementGroupId).GetAwaiter().GetResult();
    }

    public static async Task<PolicySetDefinition> CreateOrUpdateAtManagementGroupAsync(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName,
      PolicySetDefinition parameters,
      string managementGroupId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicySetDefinition body;
      using (AzureOperationResponse<PolicySetDefinition> _result = await operations.CreateOrUpdateAtManagementGroupWithHttpMessagesAsync(policySetDefinitionName, parameters, managementGroupId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void DeleteAtManagementGroup(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName,
      string managementGroupId)
    {
      operations.DeleteAtManagementGroupAsync(policySetDefinitionName, managementGroupId).GetAwaiter().GetResult();
    }

    public static async Task DeleteAtManagementGroupAsync(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName,
      string managementGroupId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteAtManagementGroupWithHttpMessagesAsync(policySetDefinitionName, managementGroupId, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static PolicySetDefinition GetAtManagementGroup(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName,
      string managementGroupId)
    {
      return operations.GetAtManagementGroupAsync(policySetDefinitionName, managementGroupId).GetAwaiter().GetResult();
    }

    public static async Task<PolicySetDefinition> GetAtManagementGroupAsync(
      this IPolicySetDefinitionsOperations operations,
      string policySetDefinitionName,
      string managementGroupId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicySetDefinition body;
      using (AzureOperationResponse<PolicySetDefinition> _result = await operations.GetAtManagementGroupWithHttpMessagesAsync(policySetDefinitionName, managementGroupId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicySetDefinition> ListByManagementGroup(
      this IPolicySetDefinitionsOperations operations,
      string managementGroupId)
    {
      return operations.ListByManagementGroupAsync(managementGroupId).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicySetDefinition>> ListByManagementGroupAsync(
      this IPolicySetDefinitionsOperations operations,
      string managementGroupId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicySetDefinition> body;
      using (AzureOperationResponse<IPage<PolicySetDefinition>> _result = await operations.ListByManagementGroupWithHttpMessagesAsync(managementGroupId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicySetDefinition> ListNext(
      this IPolicySetDefinitionsOperations operations,
      string nextPageLink)
    {
      return operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicySetDefinition>> ListNextAsync(
      this IPolicySetDefinitionsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicySetDefinition> body;
      using (AzureOperationResponse<IPage<PolicySetDefinition>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicySetDefinition> ListBuiltInNext(
      this IPolicySetDefinitionsOperations operations,
      string nextPageLink)
    {
      return operations.ListBuiltInNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicySetDefinition>> ListBuiltInNextAsync(
      this IPolicySetDefinitionsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicySetDefinition> body;
      using (AzureOperationResponse<IPage<PolicySetDefinition>> _result = await operations.ListBuiltInNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicySetDefinition> ListByManagementGroupNext(
      this IPolicySetDefinitionsOperations operations,
      string nextPageLink)
    {
      return operations.ListByManagementGroupNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicySetDefinition>> ListByManagementGroupNextAsync(
      this IPolicySetDefinitionsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicySetDefinition> body;
      using (AzureOperationResponse<IPage<PolicySetDefinition>> _result = await operations.ListByManagementGroupNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
