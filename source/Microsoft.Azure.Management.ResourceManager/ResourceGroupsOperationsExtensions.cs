// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.ResourceGroupsOperationsExtensions
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
  public static class ResourceGroupsOperationsExtensions
  {
    public static bool CheckExistence(
      this IResourceGroupsOperations operations,
      string resourceGroupName)
    {
      return operations.CheckExistenceAsync(resourceGroupName).GetAwaiter().GetResult();
    }

    public static async Task<bool> CheckExistenceAsync(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      bool body;
      using (AzureOperationResponse<bool> _result = await operations.CheckExistenceWithHttpMessagesAsync(resourceGroupName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static ResourceGroup CreateOrUpdate(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      ResourceGroup parameters)
    {
      return operations.CreateOrUpdateAsync(resourceGroupName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<ResourceGroup> CreateOrUpdateAsync(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      ResourceGroup parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResourceGroup body;
      using (AzureOperationResponse<ResourceGroup> _result = await operations.CreateOrUpdateWithHttpMessagesAsync(resourceGroupName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void Delete(this IResourceGroupsOperations operations, string resourceGroupName) => operations.DeleteAsync(resourceGroupName).GetAwaiter().GetResult();

    public static async Task DeleteAsync(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteWithHttpMessagesAsync(resourceGroupName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static ResourceGroup Get(
      this IResourceGroupsOperations operations,
      string resourceGroupName)
    {
      return operations.GetAsync(resourceGroupName).GetAwaiter().GetResult();
    }

    public static async Task<ResourceGroup> GetAsync(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResourceGroup body;
      using (AzureOperationResponse<ResourceGroup> _result = await operations.GetWithHttpMessagesAsync(resourceGroupName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static ResourceGroup Update(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      ResourceGroupPatchable parameters)
    {
      return operations.UpdateAsync(resourceGroupName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<ResourceGroup> UpdateAsync(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      ResourceGroupPatchable parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResourceGroup body;
      using (AzureOperationResponse<ResourceGroup> _result = await operations.UpdateWithHttpMessagesAsync(resourceGroupName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static ResourceGroupExportResult ExportTemplate(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      ExportTemplateRequest parameters)
    {
      return operations.ExportTemplateAsync(resourceGroupName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<ResourceGroupExportResult> ExportTemplateAsync(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      ExportTemplateRequest parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResourceGroupExportResult body;
      using (AzureOperationResponse<ResourceGroupExportResult> _result = await operations.ExportTemplateWithHttpMessagesAsync(resourceGroupName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ResourceGroup> List(
      this IResourceGroupsOperations operations,
      ODataQuery<ResourceGroupFilter> odataQuery = null)
    {
      return operations.ListAsync(odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ResourceGroup>> ListAsync(
      this IResourceGroupsOperations operations,
      ODataQuery<ResourceGroupFilter> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ResourceGroup> body;
      using (AzureOperationResponse<IPage<ResourceGroup>> _result = await operations.ListWithHttpMessagesAsync(odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void BeginDelete(
      this IResourceGroupsOperations operations,
      string resourceGroupName)
    {
      operations.BeginDeleteAsync(resourceGroupName).GetAwaiter().GetResult();
    }

    public static async Task BeginDeleteAsync(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.BeginDeleteWithHttpMessagesAsync(resourceGroupName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static ResourceGroupExportResult BeginExportTemplate(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      ExportTemplateRequest parameters)
    {
      return operations.BeginExportTemplateAsync(resourceGroupName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<ResourceGroupExportResult> BeginExportTemplateAsync(
      this IResourceGroupsOperations operations,
      string resourceGroupName,
      ExportTemplateRequest parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResourceGroupExportResult body;
      using (AzureOperationResponse<ResourceGroupExportResult> _result = await operations.BeginExportTemplateWithHttpMessagesAsync(resourceGroupName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ResourceGroup> ListNext(
      this IResourceGroupsOperations operations,
      string nextPageLink)
    {
      return operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ResourceGroup>> ListNextAsync(
      this IResourceGroupsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ResourceGroup> body;
      using (AzureOperationResponse<IPage<ResourceGroup>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
