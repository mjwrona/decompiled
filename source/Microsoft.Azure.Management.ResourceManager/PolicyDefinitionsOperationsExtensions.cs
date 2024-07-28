// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.PolicyDefinitionsOperationsExtensions
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public static class PolicyDefinitionsOperationsExtensions
  {
    public static PolicyDefinition CreateOrUpdate(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName,
      PolicyDefinition parameters)
    {
      return operations.CreateOrUpdateAsync(policyDefinitionName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<PolicyDefinition> CreateOrUpdateAsync(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName,
      PolicyDefinition parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicyDefinition body;
      using (AzureOperationResponse<PolicyDefinition> _result = await operations.CreateOrUpdateWithHttpMessagesAsync(policyDefinitionName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void Delete(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName)
    {
      operations.DeleteAsync(policyDefinitionName).GetAwaiter().GetResult();
    }

    public static async Task DeleteAsync(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteWithHttpMessagesAsync(policyDefinitionName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static PolicyDefinition Get(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName)
    {
      return operations.GetAsync(policyDefinitionName).GetAwaiter().GetResult();
    }

    public static async Task<PolicyDefinition> GetAsync(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicyDefinition body;
      using (AzureOperationResponse<PolicyDefinition> _result = await operations.GetWithHttpMessagesAsync(policyDefinitionName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static PolicyDefinition GetBuiltIn(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName)
    {
      return operations.GetBuiltInAsync(policyDefinitionName).GetAwaiter().GetResult();
    }

    public static async Task<PolicyDefinition> GetBuiltInAsync(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicyDefinition body;
      using (AzureOperationResponse<PolicyDefinition> _result = await operations.GetBuiltInWithHttpMessagesAsync(policyDefinitionName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static PolicyDefinition CreateOrUpdateAtManagementGroup(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName,
      PolicyDefinition parameters,
      string managementGroupId)
    {
      return operations.CreateOrUpdateAtManagementGroupAsync(policyDefinitionName, parameters, managementGroupId).GetAwaiter().GetResult();
    }

    public static async Task<PolicyDefinition> CreateOrUpdateAtManagementGroupAsync(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName,
      PolicyDefinition parameters,
      string managementGroupId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicyDefinition body;
      using (AzureOperationResponse<PolicyDefinition> _result = await operations.CreateOrUpdateAtManagementGroupWithHttpMessagesAsync(policyDefinitionName, parameters, managementGroupId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void DeleteAtManagementGroup(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName,
      string managementGroupId)
    {
      operations.DeleteAtManagementGroupAsync(policyDefinitionName, managementGroupId).GetAwaiter().GetResult();
    }

    public static async Task DeleteAtManagementGroupAsync(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName,
      string managementGroupId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteAtManagementGroupWithHttpMessagesAsync(policyDefinitionName, managementGroupId, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static PolicyDefinition GetAtManagementGroup(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName,
      string managementGroupId)
    {
      return operations.GetAtManagementGroupAsync(policyDefinitionName, managementGroupId).GetAwaiter().GetResult();
    }

    public static async Task<PolicyDefinition> GetAtManagementGroupAsync(
      this IPolicyDefinitionsOperations operations,
      string policyDefinitionName,
      string managementGroupId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicyDefinition body;
      using (AzureOperationResponse<PolicyDefinition> _result = await operations.GetAtManagementGroupWithHttpMessagesAsync(policyDefinitionName, managementGroupId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyDefinition> List(this IPolicyDefinitionsOperations operations) => operations.ListAsync().GetAwaiter().GetResult();

    public static async Task<IPage<PolicyDefinition>> ListAsync(
      this IPolicyDefinitionsOperations operations,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyDefinition> body;
      using (AzureOperationResponse<IPage<PolicyDefinition>> _result = await operations.ListWithHttpMessagesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyDefinition> ListBuiltIn(this IPolicyDefinitionsOperations operations) => operations.ListBuiltInAsync().GetAwaiter().GetResult();

    public static async Task<IPage<PolicyDefinition>> ListBuiltInAsync(
      this IPolicyDefinitionsOperations operations,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyDefinition> body;
      using (AzureOperationResponse<IPage<PolicyDefinition>> _result = await operations.ListBuiltInWithHttpMessagesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyDefinition> ListByManagementGroup(
      this IPolicyDefinitionsOperations operations,
      string managementGroupId)
    {
      return operations.ListByManagementGroupAsync(managementGroupId).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicyDefinition>> ListByManagementGroupAsync(
      this IPolicyDefinitionsOperations operations,
      string managementGroupId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyDefinition> body;
      using (AzureOperationResponse<IPage<PolicyDefinition>> _result = await operations.ListByManagementGroupWithHttpMessagesAsync(managementGroupId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyDefinition> ListNext(
      this IPolicyDefinitionsOperations operations,
      string nextPageLink)
    {
      return operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicyDefinition>> ListNextAsync(
      this IPolicyDefinitionsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyDefinition> body;
      using (AzureOperationResponse<IPage<PolicyDefinition>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyDefinition> ListBuiltInNext(
      this IPolicyDefinitionsOperations operations,
      string nextPageLink)
    {
      return operations.ListBuiltInNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicyDefinition>> ListBuiltInNextAsync(
      this IPolicyDefinitionsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyDefinition> body;
      using (AzureOperationResponse<IPage<PolicyDefinition>> _result = await operations.ListBuiltInNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyDefinition> ListByManagementGroupNext(
      this IPolicyDefinitionsOperations operations,
      string nextPageLink)
    {
      return operations.ListByManagementGroupNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicyDefinition>> ListByManagementGroupNextAsync(
      this IPolicyDefinitionsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyDefinition> body;
      using (AzureOperationResponse<IPage<PolicyDefinition>> _result = await operations.ListByManagementGroupNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
