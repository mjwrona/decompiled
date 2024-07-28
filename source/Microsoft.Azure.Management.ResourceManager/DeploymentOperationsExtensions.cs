// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.DeploymentOperationsExtensions
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public static class DeploymentOperationsExtensions
  {
    public static DeploymentOperation GetAtScope(
      this IDeploymentOperations operations,
      string scope,
      string deploymentName,
      string operationId)
    {
      return operations.GetAtScopeAsync(scope, deploymentName, operationId).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentOperation> GetAtScopeAsync(
      this IDeploymentOperations operations,
      string scope,
      string deploymentName,
      string operationId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentOperation body;
      using (AzureOperationResponse<DeploymentOperation> _result = await operations.GetAtScopeWithHttpMessagesAsync(scope, deploymentName, operationId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentOperation> ListAtScope(
      this IDeploymentOperations operations,
      string scope,
      string deploymentName,
      int? top = null)
    {
      return operations.ListAtScopeAsync(scope, deploymentName, top).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentOperation>> ListAtScopeAsync(
      this IDeploymentOperations operations,
      string scope,
      string deploymentName,
      int? top = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentOperation> body;
      using (AzureOperationResponse<IPage<DeploymentOperation>> _result = await operations.ListAtScopeWithHttpMessagesAsync(scope, deploymentName, top, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentOperation GetAtTenantScope(
      this IDeploymentOperations operations,
      string deploymentName,
      string operationId)
    {
      return operations.GetAtTenantScopeAsync(deploymentName, operationId).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentOperation> GetAtTenantScopeAsync(
      this IDeploymentOperations operations,
      string deploymentName,
      string operationId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentOperation body;
      using (AzureOperationResponse<DeploymentOperation> _result = await operations.GetAtTenantScopeWithHttpMessagesAsync(deploymentName, operationId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentOperation> ListAtTenantScope(
      this IDeploymentOperations operations,
      string deploymentName,
      int? top = null)
    {
      return operations.ListAtTenantScopeAsync(deploymentName, top).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentOperation>> ListAtTenantScopeAsync(
      this IDeploymentOperations operations,
      string deploymentName,
      int? top = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentOperation> body;
      using (AzureOperationResponse<IPage<DeploymentOperation>> _result = await operations.ListAtTenantScopeWithHttpMessagesAsync(deploymentName, top, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentOperation GetAtManagementGroupScope(
      this IDeploymentOperations operations,
      string groupId,
      string deploymentName,
      string operationId)
    {
      return operations.GetAtManagementGroupScopeAsync(groupId, deploymentName, operationId).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentOperation> GetAtManagementGroupScopeAsync(
      this IDeploymentOperations operations,
      string groupId,
      string deploymentName,
      string operationId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentOperation body;
      using (AzureOperationResponse<DeploymentOperation> _result = await operations.GetAtManagementGroupScopeWithHttpMessagesAsync(groupId, deploymentName, operationId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentOperation> ListAtManagementGroupScope(
      this IDeploymentOperations operations,
      string groupId,
      string deploymentName,
      int? top = null)
    {
      return operations.ListAtManagementGroupScopeAsync(groupId, deploymentName, top).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentOperation>> ListAtManagementGroupScopeAsync(
      this IDeploymentOperations operations,
      string groupId,
      string deploymentName,
      int? top = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentOperation> body;
      using (AzureOperationResponse<IPage<DeploymentOperation>> _result = await operations.ListAtManagementGroupScopeWithHttpMessagesAsync(groupId, deploymentName, top, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentOperation GetAtSubscriptionScope(
      this IDeploymentOperations operations,
      string deploymentName,
      string operationId)
    {
      return operations.GetAtSubscriptionScopeAsync(deploymentName, operationId).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentOperation> GetAtSubscriptionScopeAsync(
      this IDeploymentOperations operations,
      string deploymentName,
      string operationId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentOperation body;
      using (AzureOperationResponse<DeploymentOperation> _result = await operations.GetAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, operationId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentOperation> ListAtSubscriptionScope(
      this IDeploymentOperations operations,
      string deploymentName,
      int? top = null)
    {
      return operations.ListAtSubscriptionScopeAsync(deploymentName, top).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentOperation>> ListAtSubscriptionScopeAsync(
      this IDeploymentOperations operations,
      string deploymentName,
      int? top = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentOperation> body;
      using (AzureOperationResponse<IPage<DeploymentOperation>> _result = await operations.ListAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, top, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentOperation Get(
      this IDeploymentOperations operations,
      string resourceGroupName,
      string deploymentName,
      string operationId)
    {
      return operations.GetAsync(resourceGroupName, deploymentName, operationId).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentOperation> GetAsync(
      this IDeploymentOperations operations,
      string resourceGroupName,
      string deploymentName,
      string operationId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentOperation body;
      using (AzureOperationResponse<DeploymentOperation> _result = await operations.GetWithHttpMessagesAsync(resourceGroupName, deploymentName, operationId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentOperation> List(
      this IDeploymentOperations operations,
      string resourceGroupName,
      string deploymentName,
      int? top = null)
    {
      return operations.ListAsync(resourceGroupName, deploymentName, top).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentOperation>> ListAsync(
      this IDeploymentOperations operations,
      string resourceGroupName,
      string deploymentName,
      int? top = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentOperation> body;
      using (AzureOperationResponse<IPage<DeploymentOperation>> _result = await operations.ListWithHttpMessagesAsync(resourceGroupName, deploymentName, top, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentOperation> ListAtScopeNext(
      this IDeploymentOperations operations,
      string nextPageLink)
    {
      return operations.ListAtScopeNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentOperation>> ListAtScopeNextAsync(
      this IDeploymentOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentOperation> body;
      using (AzureOperationResponse<IPage<DeploymentOperation>> _result = await operations.ListAtScopeNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentOperation> ListAtTenantScopeNext(
      this IDeploymentOperations operations,
      string nextPageLink)
    {
      return operations.ListAtTenantScopeNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentOperation>> ListAtTenantScopeNextAsync(
      this IDeploymentOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentOperation> body;
      using (AzureOperationResponse<IPage<DeploymentOperation>> _result = await operations.ListAtTenantScopeNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentOperation> ListAtManagementGroupScopeNext(
      this IDeploymentOperations operations,
      string nextPageLink)
    {
      return operations.ListAtManagementGroupScopeNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentOperation>> ListAtManagementGroupScopeNextAsync(
      this IDeploymentOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentOperation> body;
      using (AzureOperationResponse<IPage<DeploymentOperation>> _result = await operations.ListAtManagementGroupScopeNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentOperation> ListAtSubscriptionScopeNext(
      this IDeploymentOperations operations,
      string nextPageLink)
    {
      return operations.ListAtSubscriptionScopeNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentOperation>> ListAtSubscriptionScopeNextAsync(
      this IDeploymentOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentOperation> body;
      using (AzureOperationResponse<IPage<DeploymentOperation>> _result = await operations.ListAtSubscriptionScopeNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentOperation> ListNext(
      this IDeploymentOperations operations,
      string nextPageLink)
    {
      return operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentOperation>> ListNextAsync(
      this IDeploymentOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentOperation> body;
      using (AzureOperationResponse<IPage<DeploymentOperation>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
