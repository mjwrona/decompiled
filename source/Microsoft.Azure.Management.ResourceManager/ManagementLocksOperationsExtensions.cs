// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.ManagementLocksOperationsExtensions
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
  public static class ManagementLocksOperationsExtensions
  {
    public static ManagementLockObject CreateOrUpdateAtResourceGroupLevel(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string lockName,
      ManagementLockObject parameters)
    {
      return operations.CreateOrUpdateAtResourceGroupLevelAsync(resourceGroupName, lockName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<ManagementLockObject> CreateOrUpdateAtResourceGroupLevelAsync(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string lockName,
      ManagementLockObject parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ManagementLockObject body;
      using (AzureOperationResponse<ManagementLockObject> _result = await operations.CreateOrUpdateAtResourceGroupLevelWithHttpMessagesAsync(resourceGroupName, lockName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void DeleteAtResourceGroupLevel(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string lockName)
    {
      operations.DeleteAtResourceGroupLevelAsync(resourceGroupName, lockName).GetAwaiter().GetResult();
    }

    public static async Task DeleteAtResourceGroupLevelAsync(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string lockName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteAtResourceGroupLevelWithHttpMessagesAsync(resourceGroupName, lockName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static ManagementLockObject GetAtResourceGroupLevel(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string lockName)
    {
      return operations.GetAtResourceGroupLevelAsync(resourceGroupName, lockName).GetAwaiter().GetResult();
    }

    public static async Task<ManagementLockObject> GetAtResourceGroupLevelAsync(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string lockName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ManagementLockObject body;
      using (AzureOperationResponse<ManagementLockObject> _result = await operations.GetAtResourceGroupLevelWithHttpMessagesAsync(resourceGroupName, lockName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static ManagementLockObject CreateOrUpdateByScope(
      this IManagementLocksOperations operations,
      string scope,
      string lockName,
      ManagementLockObject parameters)
    {
      return operations.CreateOrUpdateByScopeAsync(scope, lockName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<ManagementLockObject> CreateOrUpdateByScopeAsync(
      this IManagementLocksOperations operations,
      string scope,
      string lockName,
      ManagementLockObject parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ManagementLockObject body;
      using (AzureOperationResponse<ManagementLockObject> _result = await operations.CreateOrUpdateByScopeWithHttpMessagesAsync(scope, lockName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void DeleteByScope(
      this IManagementLocksOperations operations,
      string scope,
      string lockName)
    {
      operations.DeleteByScopeAsync(scope, lockName).GetAwaiter().GetResult();
    }

    public static async Task DeleteByScopeAsync(
      this IManagementLocksOperations operations,
      string scope,
      string lockName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteByScopeWithHttpMessagesAsync(scope, lockName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static ManagementLockObject GetByScope(
      this IManagementLocksOperations operations,
      string scope,
      string lockName)
    {
      return operations.GetByScopeAsync(scope, lockName).GetAwaiter().GetResult();
    }

    public static async Task<ManagementLockObject> GetByScopeAsync(
      this IManagementLocksOperations operations,
      string scope,
      string lockName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ManagementLockObject body;
      using (AzureOperationResponse<ManagementLockObject> _result = await operations.GetByScopeWithHttpMessagesAsync(scope, lockName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static ManagementLockObject CreateOrUpdateAtResourceLevel(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName,
      ManagementLockObject parameters)
    {
      return operations.CreateOrUpdateAtResourceLevelAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, lockName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<ManagementLockObject> CreateOrUpdateAtResourceLevelAsync(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName,
      ManagementLockObject parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ManagementLockObject body;
      using (AzureOperationResponse<ManagementLockObject> _result = await operations.CreateOrUpdateAtResourceLevelWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, lockName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void DeleteAtResourceLevel(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName)
    {
      operations.DeleteAtResourceLevelAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, lockName).GetAwaiter().GetResult();
    }

    public static async Task DeleteAtResourceLevelAsync(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteAtResourceLevelWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, lockName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static ManagementLockObject GetAtResourceLevel(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName)
    {
      return operations.GetAtResourceLevelAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, lockName).GetAwaiter().GetResult();
    }

    public static async Task<ManagementLockObject> GetAtResourceLevelAsync(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ManagementLockObject body;
      using (AzureOperationResponse<ManagementLockObject> _result = await operations.GetAtResourceLevelWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, lockName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static ManagementLockObject CreateOrUpdateAtSubscriptionLevel(
      this IManagementLocksOperations operations,
      string lockName,
      ManagementLockObject parameters)
    {
      return operations.CreateOrUpdateAtSubscriptionLevelAsync(lockName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<ManagementLockObject> CreateOrUpdateAtSubscriptionLevelAsync(
      this IManagementLocksOperations operations,
      string lockName,
      ManagementLockObject parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ManagementLockObject body;
      using (AzureOperationResponse<ManagementLockObject> _result = await operations.CreateOrUpdateAtSubscriptionLevelWithHttpMessagesAsync(lockName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void DeleteAtSubscriptionLevel(
      this IManagementLocksOperations operations,
      string lockName)
    {
      operations.DeleteAtSubscriptionLevelAsync(lockName).GetAwaiter().GetResult();
    }

    public static async Task DeleteAtSubscriptionLevelAsync(
      this IManagementLocksOperations operations,
      string lockName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteAtSubscriptionLevelWithHttpMessagesAsync(lockName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static ManagementLockObject GetAtSubscriptionLevel(
      this IManagementLocksOperations operations,
      string lockName)
    {
      return operations.GetAtSubscriptionLevelAsync(lockName).GetAwaiter().GetResult();
    }

    public static async Task<ManagementLockObject> GetAtSubscriptionLevelAsync(
      this IManagementLocksOperations operations,
      string lockName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ManagementLockObject body;
      using (AzureOperationResponse<ManagementLockObject> _result = await operations.GetAtSubscriptionLevelWithHttpMessagesAsync(lockName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ManagementLockObject> ListAtResourceGroupLevel(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      ODataQuery<ManagementLockObject> odataQuery = null)
    {
      return operations.ListAtResourceGroupLevelAsync(resourceGroupName, odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ManagementLockObject>> ListAtResourceGroupLevelAsync(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      ODataQuery<ManagementLockObject> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ManagementLockObject> body;
      using (AzureOperationResponse<IPage<ManagementLockObject>> _result = await operations.ListAtResourceGroupLevelWithHttpMessagesAsync(resourceGroupName, odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ManagementLockObject> ListAtResourceLevel(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      ODataQuery<ManagementLockObject> odataQuery = null)
    {
      return operations.ListAtResourceLevelAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ManagementLockObject>> ListAtResourceLevelAsync(
      this IManagementLocksOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      ODataQuery<ManagementLockObject> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ManagementLockObject> body;
      using (AzureOperationResponse<IPage<ManagementLockObject>> _result = await operations.ListAtResourceLevelWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ManagementLockObject> ListAtSubscriptionLevel(
      this IManagementLocksOperations operations,
      ODataQuery<ManagementLockObject> odataQuery = null)
    {
      return operations.ListAtSubscriptionLevelAsync(odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ManagementLockObject>> ListAtSubscriptionLevelAsync(
      this IManagementLocksOperations operations,
      ODataQuery<ManagementLockObject> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ManagementLockObject> body;
      using (AzureOperationResponse<IPage<ManagementLockObject>> _result = await operations.ListAtSubscriptionLevelWithHttpMessagesAsync(odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ManagementLockObject> ListAtResourceGroupLevelNext(
      this IManagementLocksOperations operations,
      string nextPageLink)
    {
      return operations.ListAtResourceGroupLevelNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ManagementLockObject>> ListAtResourceGroupLevelNextAsync(
      this IManagementLocksOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ManagementLockObject> body;
      using (AzureOperationResponse<IPage<ManagementLockObject>> _result = await operations.ListAtResourceGroupLevelNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ManagementLockObject> ListAtResourceLevelNext(
      this IManagementLocksOperations operations,
      string nextPageLink)
    {
      return operations.ListAtResourceLevelNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ManagementLockObject>> ListAtResourceLevelNextAsync(
      this IManagementLocksOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ManagementLockObject> body;
      using (AzureOperationResponse<IPage<ManagementLockObject>> _result = await operations.ListAtResourceLevelNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ManagementLockObject> ListAtSubscriptionLevelNext(
      this IManagementLocksOperations operations,
      string nextPageLink)
    {
      return operations.ListAtSubscriptionLevelNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ManagementLockObject>> ListAtSubscriptionLevelNextAsync(
      this IManagementLocksOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ManagementLockObject> body;
      using (AzureOperationResponse<IPage<ManagementLockObject>> _result = await operations.ListAtSubscriptionLevelNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
