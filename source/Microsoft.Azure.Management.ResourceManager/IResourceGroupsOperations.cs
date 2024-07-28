// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.IResourceGroupsOperations
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
  public interface IResourceGroupsOperations
  {
    Task<AzureOperationResponse<bool>> CheckExistenceWithHttpMessagesAsync(
      string resourceGroupName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ResourceGroup>> CreateOrUpdateWithHttpMessagesAsync(
      string resourceGroupName,
      ResourceGroup parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteWithHttpMessagesAsync(
      string resourceGroupName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ResourceGroup>> GetWithHttpMessagesAsync(
      string resourceGroupName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ResourceGroup>> UpdateWithHttpMessagesAsync(
      string resourceGroupName,
      ResourceGroupPatchable parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ResourceGroupExportResult>> ExportTemplateWithHttpMessagesAsync(
      string resourceGroupName,
      ExportTemplateRequest parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<ResourceGroup>>> ListWithHttpMessagesAsync(
      ODataQuery<ResourceGroupFilter> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> BeginDeleteWithHttpMessagesAsync(
      string resourceGroupName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ResourceGroupExportResult>> BeginExportTemplateWithHttpMessagesAsync(
      string resourceGroupName,
      ExportTemplateRequest parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<ResourceGroup>>> ListNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
