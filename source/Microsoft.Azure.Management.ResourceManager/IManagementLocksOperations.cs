// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.IManagementLocksOperations
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using Microsoft.Rest.Azure.OData;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public interface IManagementLocksOperations
  {
    Task<AzureOperationResponse<ManagementLockObject>> CreateOrUpdateAtResourceGroupLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string lockName,
      ManagementLockObject parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteAtResourceGroupLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ManagementLockObject>> GetAtResourceGroupLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ManagementLockObject>> CreateOrUpdateByScopeWithHttpMessagesAsync(
      string scope,
      string lockName,
      ManagementLockObject parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteByScopeWithHttpMessagesAsync(
      string scope,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ManagementLockObject>> GetByScopeWithHttpMessagesAsync(
      string scope,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ManagementLockObject>> CreateOrUpdateAtResourceLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName,
      ManagementLockObject parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteAtResourceLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ManagementLockObject>> GetAtResourceLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ManagementLockObject>> CreateOrUpdateAtSubscriptionLevelWithHttpMessagesAsync(
      string lockName,
      ManagementLockObject parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteAtSubscriptionLevelWithHttpMessagesAsync(
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ManagementLockObject>> GetAtSubscriptionLevelWithHttpMessagesAsync(
      string lockName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtResourceGroupLevelWithHttpMessagesAsync(
      string resourceGroupName,
      ODataQuery<ManagementLockObject> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtResourceLevelWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      ODataQuery<ManagementLockObject> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtSubscriptionLevelWithHttpMessagesAsync(
      ODataQuery<ManagementLockObject> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtResourceGroupLevelNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtResourceLevelNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<ManagementLockObject>>> ListAtSubscriptionLevelNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
