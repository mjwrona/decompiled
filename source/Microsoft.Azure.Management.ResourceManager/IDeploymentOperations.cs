// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.IDeploymentOperations
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public interface IDeploymentOperations
  {
    Task<AzureOperationResponse<DeploymentOperation>> GetAtScopeWithHttpMessagesAsync(
      string scope,
      string deploymentName,
      string operationId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentOperation>>> ListAtScopeWithHttpMessagesAsync(
      string scope,
      string deploymentName,
      int? top = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentOperation>> GetAtTenantScopeWithHttpMessagesAsync(
      string deploymentName,
      string operationId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentOperation>>> ListAtTenantScopeWithHttpMessagesAsync(
      string deploymentName,
      int? top = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentOperation>> GetAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      string deploymentName,
      string operationId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentOperation>>> ListAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      string deploymentName,
      int? top = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentOperation>> GetAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      string operationId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentOperation>>> ListAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      int? top = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentOperation>> GetWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      string operationId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentOperation>>> ListWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      int? top = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentOperation>>> ListAtScopeNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentOperation>>> ListAtTenantScopeNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentOperation>>> ListAtManagementGroupScopeNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentOperation>>> ListAtSubscriptionScopeNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentOperation>>> ListNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
