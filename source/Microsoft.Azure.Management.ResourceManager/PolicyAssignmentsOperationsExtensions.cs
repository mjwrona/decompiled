// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.PolicyAssignmentsOperationsExtensions
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using Microsoft.Rest.Azure.OData;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public static class PolicyAssignmentsOperationsExtensions
  {
    public static PolicyAssignment Delete(
      this IPolicyAssignmentsOperations operations,
      string scope,
      string policyAssignmentName)
    {
      return operations.DeleteAsync(scope, policyAssignmentName).GetAwaiter().GetResult();
    }

    public static async Task<PolicyAssignment> DeleteAsync(
      this IPolicyAssignmentsOperations operations,
      string scope,
      string policyAssignmentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicyAssignment body;
      using (AzureOperationResponse<PolicyAssignment> _result = await operations.DeleteWithHttpMessagesAsync(scope, policyAssignmentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static PolicyAssignment Create(
      this IPolicyAssignmentsOperations operations,
      string scope,
      string policyAssignmentName,
      PolicyAssignment parameters)
    {
      return operations.CreateAsync(scope, policyAssignmentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<PolicyAssignment> CreateAsync(
      this IPolicyAssignmentsOperations operations,
      string scope,
      string policyAssignmentName,
      PolicyAssignment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicyAssignment body;
      using (AzureOperationResponse<PolicyAssignment> _result = await operations.CreateWithHttpMessagesAsync(scope, policyAssignmentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static PolicyAssignment Get(
      this IPolicyAssignmentsOperations operations,
      string scope,
      string policyAssignmentName)
    {
      return operations.GetAsync(scope, policyAssignmentName).GetAwaiter().GetResult();
    }

    public static async Task<PolicyAssignment> GetAsync(
      this IPolicyAssignmentsOperations operations,
      string scope,
      string policyAssignmentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicyAssignment body;
      using (AzureOperationResponse<PolicyAssignment> _result = await operations.GetWithHttpMessagesAsync(scope, policyAssignmentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyAssignment> ListForResourceGroup(
      this IPolicyAssignmentsOperations operations,
      string resourceGroupName,
      string filter = null)
    {
      return operations.ListForResourceGroupAsync(resourceGroupName, filter).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicyAssignment>> ListForResourceGroupAsync(
      this IPolicyAssignmentsOperations operations,
      string resourceGroupName,
      string filter = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyAssignment> body;
      using (AzureOperationResponse<IPage<PolicyAssignment>> _result = await operations.ListForResourceGroupWithHttpMessagesAsync(resourceGroupName, filter, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyAssignment> ListForResource(
      this IPolicyAssignmentsOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      ODataQuery<PolicyAssignment> odataQuery = null)
    {
      return operations.ListForResourceAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicyAssignment>> ListForResourceAsync(
      this IPolicyAssignmentsOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      ODataQuery<PolicyAssignment> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyAssignment> body;
      using (AzureOperationResponse<IPage<PolicyAssignment>> _result = await operations.ListForResourceWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyAssignment> List(
      this IPolicyAssignmentsOperations operations,
      ODataQuery<PolicyAssignment> odataQuery = null)
    {
      return operations.ListAsync(odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicyAssignment>> ListAsync(
      this IPolicyAssignmentsOperations operations,
      ODataQuery<PolicyAssignment> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyAssignment> body;
      using (AzureOperationResponse<IPage<PolicyAssignment>> _result = await operations.ListWithHttpMessagesAsync(odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static PolicyAssignment DeleteById(
      this IPolicyAssignmentsOperations operations,
      string policyAssignmentId)
    {
      return operations.DeleteByIdAsync(policyAssignmentId).GetAwaiter().GetResult();
    }

    public static async Task<PolicyAssignment> DeleteByIdAsync(
      this IPolicyAssignmentsOperations operations,
      string policyAssignmentId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicyAssignment body;
      using (AzureOperationResponse<PolicyAssignment> _result = await operations.DeleteByIdWithHttpMessagesAsync(policyAssignmentId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static PolicyAssignment CreateById(
      this IPolicyAssignmentsOperations operations,
      string policyAssignmentId,
      PolicyAssignment parameters)
    {
      return operations.CreateByIdAsync(policyAssignmentId, parameters).GetAwaiter().GetResult();
    }

    public static async Task<PolicyAssignment> CreateByIdAsync(
      this IPolicyAssignmentsOperations operations,
      string policyAssignmentId,
      PolicyAssignment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicyAssignment body;
      using (AzureOperationResponse<PolicyAssignment> _result = await operations.CreateByIdWithHttpMessagesAsync(policyAssignmentId, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static PolicyAssignment GetById(
      this IPolicyAssignmentsOperations operations,
      string policyAssignmentId)
    {
      return operations.GetByIdAsync(policyAssignmentId).GetAwaiter().GetResult();
    }

    public static async Task<PolicyAssignment> GetByIdAsync(
      this IPolicyAssignmentsOperations operations,
      string policyAssignmentId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      PolicyAssignment body;
      using (AzureOperationResponse<PolicyAssignment> _result = await operations.GetByIdWithHttpMessagesAsync(policyAssignmentId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyAssignment> ListForResourceGroupNext(
      this IPolicyAssignmentsOperations operations,
      string nextPageLink)
    {
      return operations.ListForResourceGroupNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicyAssignment>> ListForResourceGroupNextAsync(
      this IPolicyAssignmentsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyAssignment> body;
      using (AzureOperationResponse<IPage<PolicyAssignment>> _result = await operations.ListForResourceGroupNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyAssignment> ListForResourceNext(
      this IPolicyAssignmentsOperations operations,
      string nextPageLink)
    {
      return operations.ListForResourceNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicyAssignment>> ListForResourceNextAsync(
      this IPolicyAssignmentsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyAssignment> body;
      using (AzureOperationResponse<IPage<PolicyAssignment>> _result = await operations.ListForResourceNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<PolicyAssignment> ListNext(
      this IPolicyAssignmentsOperations operations,
      string nextPageLink)
    {
      return operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<PolicyAssignment>> ListNextAsync(
      this IPolicyAssignmentsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<PolicyAssignment> body;
      using (AzureOperationResponse<IPage<PolicyAssignment>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
