// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.IResourcesOperations
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
  public interface IResourcesOperations
  {
    Task<AzureOperationResponse<IPage<GenericResource>>> ListByResourceGroupWithHttpMessagesAsync(
      string resourceGroupName,
      ODataQuery<GenericResourceFilter> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> MoveResourcesWithHttpMessagesAsync(
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> ValidateMoveResourcesWithHttpMessagesAsync(
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<GenericResource>>> ListWithHttpMessagesAsync(
      ODataQuery<GenericResourceFilter> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<bool>> CheckExistenceWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<GenericResource>> CreateOrUpdateWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<GenericResource>> UpdateWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<GenericResource>> GetWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<bool>> CheckExistenceByIdWithHttpMessagesAsync(
      string resourceId,
      string apiVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteByIdWithHttpMessagesAsync(
      string resourceId,
      string apiVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<GenericResource>> CreateOrUpdateByIdWithHttpMessagesAsync(
      string resourceId,
      string apiVersion,
      GenericResource parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<GenericResource>> UpdateByIdWithHttpMessagesAsync(
      string resourceId,
      string apiVersion,
      GenericResource parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<GenericResource>> GetByIdWithHttpMessagesAsync(
      string resourceId,
      string apiVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> BeginMoveResourcesWithHttpMessagesAsync(
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> BeginValidateMoveResourcesWithHttpMessagesAsync(
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> BeginDeleteWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<GenericResource>> BeginCreateOrUpdateWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<GenericResource>> BeginUpdateWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> BeginDeleteByIdWithHttpMessagesAsync(
      string resourceId,
      string apiVersion,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<GenericResource>> BeginCreateOrUpdateByIdWithHttpMessagesAsync(
      string resourceId,
      string apiVersion,
      GenericResource parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<GenericResource>> BeginUpdateByIdWithHttpMessagesAsync(
      string resourceId,
      string apiVersion,
      GenericResource parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<GenericResource>>> ListByResourceGroupNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<GenericResource>>> ListNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
