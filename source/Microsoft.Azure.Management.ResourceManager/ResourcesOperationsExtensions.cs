// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.ResourcesOperationsExtensions
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
  public static class ResourcesOperationsExtensions
  {
    public static IPage<GenericResource> ListByResourceGroup(
      this IResourcesOperations operations,
      string resourceGroupName,
      ODataQuery<GenericResourceFilter> odataQuery = null)
    {
      return operations.ListByResourceGroupAsync(resourceGroupName, odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<GenericResource>> ListByResourceGroupAsync(
      this IResourcesOperations operations,
      string resourceGroupName,
      ODataQuery<GenericResourceFilter> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<GenericResource> body;
      using (AzureOperationResponse<IPage<GenericResource>> _result = await operations.ListByResourceGroupWithHttpMessagesAsync(resourceGroupName, odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void MoveResources(
      this IResourcesOperations operations,
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters)
    {
      operations.MoveResourcesAsync(sourceResourceGroupName, parameters).GetAwaiter().GetResult();
    }

    public static async Task MoveResourcesAsync(
      this IResourcesOperations operations,
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.MoveResourcesWithHttpMessagesAsync(sourceResourceGroupName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static void ValidateMoveResources(
      this IResourcesOperations operations,
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters)
    {
      operations.ValidateMoveResourcesAsync(sourceResourceGroupName, parameters).GetAwaiter().GetResult();
    }

    public static async Task ValidateMoveResourcesAsync(
      this IResourcesOperations operations,
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.ValidateMoveResourcesWithHttpMessagesAsync(sourceResourceGroupName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static IPage<GenericResource> List(
      this IResourcesOperations operations,
      ODataQuery<GenericResourceFilter> odataQuery = null)
    {
      return operations.ListAsync(odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<GenericResource>> ListAsync(
      this IResourcesOperations operations,
      ODataQuery<GenericResourceFilter> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<GenericResource> body;
      using (AzureOperationResponse<IPage<GenericResource>> _result = await operations.ListWithHttpMessagesAsync(odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static bool CheckExistence(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion)
    {
      return operations.CheckExistenceAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion).GetAwaiter().GetResult();
    }

    public static async Task<bool> CheckExistenceAsync(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      bool body;
      using (AzureOperationResponse<bool> _result = await operations.CheckExistenceWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void Delete(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion)
    {
      operations.DeleteAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion).GetAwaiter().GetResult();
    }

    public static async Task DeleteAsync(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static GenericResource CreateOrUpdate(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters)
    {
      return operations.CreateOrUpdateAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, parameters).GetAwaiter().GetResult();
    }

    public static async Task<GenericResource> CreateOrUpdateAsync(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GenericResource body;
      using (AzureOperationResponse<GenericResource> _result = await operations.CreateOrUpdateWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static GenericResource Update(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters)
    {
      return operations.UpdateAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, parameters).GetAwaiter().GetResult();
    }

    public static async Task<GenericResource> UpdateAsync(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GenericResource body;
      using (AzureOperationResponse<GenericResource> _result = await operations.UpdateWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static GenericResource Get(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion)
    {
      return operations.GetAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion).GetAwaiter().GetResult();
    }

    public static async Task<GenericResource> GetAsync(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GenericResource body;
      using (AzureOperationResponse<GenericResource> _result = await operations.GetWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static bool CheckExistenceById(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion)
    {
      return operations.CheckExistenceByIdAsync(resourceId, apiVersion).GetAwaiter().GetResult();
    }

    public static async Task<bool> CheckExistenceByIdAsync(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      bool body;
      using (AzureOperationResponse<bool> _result = await operations.CheckExistenceByIdWithHttpMessagesAsync(resourceId, apiVersion, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void DeleteById(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion)
    {
      operations.DeleteByIdAsync(resourceId, apiVersion).GetAwaiter().GetResult();
    }

    public static async Task DeleteByIdAsync(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteByIdWithHttpMessagesAsync(resourceId, apiVersion, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static GenericResource CreateOrUpdateById(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      GenericResource parameters)
    {
      return operations.CreateOrUpdateByIdAsync(resourceId, apiVersion, parameters).GetAwaiter().GetResult();
    }

    public static async Task<GenericResource> CreateOrUpdateByIdAsync(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      GenericResource parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GenericResource body;
      using (AzureOperationResponse<GenericResource> _result = await operations.CreateOrUpdateByIdWithHttpMessagesAsync(resourceId, apiVersion, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static GenericResource UpdateById(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      GenericResource parameters)
    {
      return operations.UpdateByIdAsync(resourceId, apiVersion, parameters).GetAwaiter().GetResult();
    }

    public static async Task<GenericResource> UpdateByIdAsync(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      GenericResource parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GenericResource body;
      using (AzureOperationResponse<GenericResource> _result = await operations.UpdateByIdWithHttpMessagesAsync(resourceId, apiVersion, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static GenericResource GetById(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion)
    {
      return operations.GetByIdAsync(resourceId, apiVersion).GetAwaiter().GetResult();
    }

    public static async Task<GenericResource> GetByIdAsync(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GenericResource body;
      using (AzureOperationResponse<GenericResource> _result = await operations.GetByIdWithHttpMessagesAsync(resourceId, apiVersion, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void BeginMoveResources(
      this IResourcesOperations operations,
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters)
    {
      operations.BeginMoveResourcesAsync(sourceResourceGroupName, parameters).GetAwaiter().GetResult();
    }

    public static async Task BeginMoveResourcesAsync(
      this IResourcesOperations operations,
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.BeginMoveResourcesWithHttpMessagesAsync(sourceResourceGroupName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static void BeginValidateMoveResources(
      this IResourcesOperations operations,
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters)
    {
      operations.BeginValidateMoveResourcesAsync(sourceResourceGroupName, parameters).GetAwaiter().GetResult();
    }

    public static async Task BeginValidateMoveResourcesAsync(
      this IResourcesOperations operations,
      string sourceResourceGroupName,
      ResourcesMoveInfo parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.BeginValidateMoveResourcesWithHttpMessagesAsync(sourceResourceGroupName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static void BeginDelete(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion)
    {
      operations.BeginDeleteAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion).GetAwaiter().GetResult();
    }

    public static async Task BeginDeleteAsync(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.BeginDeleteWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static GenericResource BeginCreateOrUpdate(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters)
    {
      return operations.BeginCreateOrUpdateAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, parameters).GetAwaiter().GetResult();
    }

    public static async Task<GenericResource> BeginCreateOrUpdateAsync(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GenericResource body;
      using (AzureOperationResponse<GenericResource> _result = await operations.BeginCreateOrUpdateWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static GenericResource BeginUpdate(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters)
    {
      return operations.BeginUpdateAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, parameters).GetAwaiter().GetResult();
    }

    public static async Task<GenericResource> BeginUpdateAsync(
      this IResourcesOperations operations,
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      string apiVersion,
      GenericResource parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GenericResource body;
      using (AzureOperationResponse<GenericResource> _result = await operations.BeginUpdateWithHttpMessagesAsync(resourceGroupName, resourceProviderNamespace, parentResourcePath, resourceType, resourceName, apiVersion, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void BeginDeleteById(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion)
    {
      operations.BeginDeleteByIdAsync(resourceId, apiVersion).GetAwaiter().GetResult();
    }

    public static async Task BeginDeleteByIdAsync(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.BeginDeleteByIdWithHttpMessagesAsync(resourceId, apiVersion, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static GenericResource BeginCreateOrUpdateById(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      GenericResource parameters)
    {
      return operations.BeginCreateOrUpdateByIdAsync(resourceId, apiVersion, parameters).GetAwaiter().GetResult();
    }

    public static async Task<GenericResource> BeginCreateOrUpdateByIdAsync(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      GenericResource parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GenericResource body;
      using (AzureOperationResponse<GenericResource> _result = await operations.BeginCreateOrUpdateByIdWithHttpMessagesAsync(resourceId, apiVersion, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static GenericResource BeginUpdateById(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      GenericResource parameters)
    {
      return operations.BeginUpdateByIdAsync(resourceId, apiVersion, parameters).GetAwaiter().GetResult();
    }

    public static async Task<GenericResource> BeginUpdateByIdAsync(
      this IResourcesOperations operations,
      string resourceId,
      string apiVersion,
      GenericResource parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      GenericResource body;
      using (AzureOperationResponse<GenericResource> _result = await operations.BeginUpdateByIdWithHttpMessagesAsync(resourceId, apiVersion, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<GenericResource> ListByResourceGroupNext(
      this IResourcesOperations operations,
      string nextPageLink)
    {
      return operations.ListByResourceGroupNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<GenericResource>> ListByResourceGroupNextAsync(
      this IResourcesOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<GenericResource> body;
      using (AzureOperationResponse<IPage<GenericResource>> _result = await operations.ListByResourceGroupNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<GenericResource> ListNext(
      this IResourcesOperations operations,
      string nextPageLink)
    {
      return operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<GenericResource>> ListNextAsync(
      this IResourcesOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<GenericResource> body;
      using (AzureOperationResponse<IPage<GenericResource>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
