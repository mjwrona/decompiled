// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.DeploymentsOperationsExtensions
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
  public static class DeploymentsOperationsExtensions
  {
    public static void DeleteAtScope(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName)
    {
      operations.DeleteAtScopeAsync(scope, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task DeleteAtScopeAsync(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteAtScopeWithHttpMessagesAsync(scope, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static bool CheckExistenceAtScope(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName)
    {
      return operations.CheckExistenceAtScopeAsync(scope, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<bool> CheckExistenceAtScopeAsync(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      bool body;
      using (AzureOperationResponse<bool> _result = await operations.CheckExistenceAtScopeWithHttpMessagesAsync(scope, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExtended CreateOrUpdateAtScope(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      Deployment parameters)
    {
      return operations.CreateOrUpdateAtScopeAsync(scope, deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> CreateOrUpdateAtScopeAsync(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      Deployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.CreateOrUpdateAtScopeWithHttpMessagesAsync(scope, deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExtended GetAtScope(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName)
    {
      return operations.GetAtScopeAsync(scope, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> GetAtScopeAsync(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.GetAtScopeWithHttpMessagesAsync(scope, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void CancelAtScope(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName)
    {
      operations.CancelAtScopeAsync(scope, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task CancelAtScopeAsync(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.CancelAtScopeWithHttpMessagesAsync(scope, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static DeploymentValidateResult ValidateAtScope(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      Deployment parameters)
    {
      return operations.ValidateAtScopeAsync(scope, deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentValidateResult> ValidateAtScopeAsync(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      Deployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentValidateResult body;
      using (AzureOperationResponse<DeploymentValidateResult> _result = await operations.ValidateAtScopeWithHttpMessagesAsync(scope, deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExportResult ExportTemplateAtScope(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName)
    {
      return operations.ExportTemplateAtScopeAsync(scope, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExportResult> ExportTemplateAtScopeAsync(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExportResult body;
      using (AzureOperationResponse<DeploymentExportResult> _result = await operations.ExportTemplateAtScopeWithHttpMessagesAsync(scope, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentExtended> ListAtScope(
      this IDeploymentsOperations operations,
      string scope,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null)
    {
      return operations.ListAtScopeAsync(scope, odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentExtended>> ListAtScopeAsync(
      this IDeploymentsOperations operations,
      string scope,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentExtended> body;
      using (AzureOperationResponse<IPage<DeploymentExtended>> _result = await operations.ListAtScopeWithHttpMessagesAsync(scope, odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void DeleteAtTenantScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      operations.DeleteAtTenantScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task DeleteAtTenantScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteAtTenantScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static bool CheckExistenceAtTenantScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      return operations.CheckExistenceAtTenantScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<bool> CheckExistenceAtTenantScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      bool body;
      using (AzureOperationResponse<bool> _result = await operations.CheckExistenceAtTenantScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExtended CreateOrUpdateAtTenantScope(
      this IDeploymentsOperations operations,
      string deploymentName,
      ScopedDeployment parameters)
    {
      return operations.CreateOrUpdateAtTenantScopeAsync(deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> CreateOrUpdateAtTenantScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      ScopedDeployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.CreateOrUpdateAtTenantScopeWithHttpMessagesAsync(deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExtended GetAtTenantScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      return operations.GetAtTenantScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> GetAtTenantScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.GetAtTenantScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void CancelAtTenantScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      operations.CancelAtTenantScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task CancelAtTenantScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.CancelAtTenantScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static DeploymentValidateResult ValidateAtTenantScope(
      this IDeploymentsOperations operations,
      string deploymentName,
      ScopedDeployment parameters)
    {
      return operations.ValidateAtTenantScopeAsync(deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentValidateResult> ValidateAtTenantScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      ScopedDeployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentValidateResult body;
      using (AzureOperationResponse<DeploymentValidateResult> _result = await operations.ValidateAtTenantScopeWithHttpMessagesAsync(deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExportResult ExportTemplateAtTenantScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      return operations.ExportTemplateAtTenantScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExportResult> ExportTemplateAtTenantScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExportResult body;
      using (AzureOperationResponse<DeploymentExportResult> _result = await operations.ExportTemplateAtTenantScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentExtended> ListAtTenantScope(
      this IDeploymentsOperations operations,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null)
    {
      return operations.ListAtTenantScopeAsync(odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentExtended>> ListAtTenantScopeAsync(
      this IDeploymentsOperations operations,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentExtended> body;
      using (AzureOperationResponse<IPage<DeploymentExtended>> _result = await operations.ListAtTenantScopeWithHttpMessagesAsync(odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void DeleteAtManagementGroupScope(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName)
    {
      operations.DeleteAtManagementGroupScopeAsync(groupId, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task DeleteAtManagementGroupScopeAsync(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteAtManagementGroupScopeWithHttpMessagesAsync(groupId, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static bool CheckExistenceAtManagementGroupScope(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName)
    {
      return operations.CheckExistenceAtManagementGroupScopeAsync(groupId, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<bool> CheckExistenceAtManagementGroupScopeAsync(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      bool body;
      using (AzureOperationResponse<bool> _result = await operations.CheckExistenceAtManagementGroupScopeWithHttpMessagesAsync(groupId, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExtended CreateOrUpdateAtManagementGroupScope(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      ScopedDeployment parameters)
    {
      return operations.CreateOrUpdateAtManagementGroupScopeAsync(groupId, deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> CreateOrUpdateAtManagementGroupScopeAsync(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      ScopedDeployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.CreateOrUpdateAtManagementGroupScopeWithHttpMessagesAsync(groupId, deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExtended GetAtManagementGroupScope(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName)
    {
      return operations.GetAtManagementGroupScopeAsync(groupId, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> GetAtManagementGroupScopeAsync(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.GetAtManagementGroupScopeWithHttpMessagesAsync(groupId, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void CancelAtManagementGroupScope(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName)
    {
      operations.CancelAtManagementGroupScopeAsync(groupId, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task CancelAtManagementGroupScopeAsync(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.CancelAtManagementGroupScopeWithHttpMessagesAsync(groupId, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static DeploymentValidateResult ValidateAtManagementGroupScope(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      ScopedDeployment parameters)
    {
      return operations.ValidateAtManagementGroupScopeAsync(groupId, deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentValidateResult> ValidateAtManagementGroupScopeAsync(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      ScopedDeployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentValidateResult body;
      using (AzureOperationResponse<DeploymentValidateResult> _result = await operations.ValidateAtManagementGroupScopeWithHttpMessagesAsync(groupId, deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExportResult ExportTemplateAtManagementGroupScope(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName)
    {
      return operations.ExportTemplateAtManagementGroupScopeAsync(groupId, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExportResult> ExportTemplateAtManagementGroupScopeAsync(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExportResult body;
      using (AzureOperationResponse<DeploymentExportResult> _result = await operations.ExportTemplateAtManagementGroupScopeWithHttpMessagesAsync(groupId, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentExtended> ListAtManagementGroupScope(
      this IDeploymentsOperations operations,
      string groupId,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null)
    {
      return operations.ListAtManagementGroupScopeAsync(groupId, odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentExtended>> ListAtManagementGroupScopeAsync(
      this IDeploymentsOperations operations,
      string groupId,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentExtended> body;
      using (AzureOperationResponse<IPage<DeploymentExtended>> _result = await operations.ListAtManagementGroupScopeWithHttpMessagesAsync(groupId, odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void DeleteAtSubscriptionScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      operations.DeleteAtSubscriptionScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task DeleteAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static bool CheckExistenceAtSubscriptionScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      return operations.CheckExistenceAtSubscriptionScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<bool> CheckExistenceAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      bool body;
      using (AzureOperationResponse<bool> _result = await operations.CheckExistenceAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExtended CreateOrUpdateAtSubscriptionScope(
      this IDeploymentsOperations operations,
      string deploymentName,
      Deployment parameters)
    {
      return operations.CreateOrUpdateAtSubscriptionScopeAsync(deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> CreateOrUpdateAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      Deployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.CreateOrUpdateAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExtended GetAtSubscriptionScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      return operations.GetAtSubscriptionScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> GetAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.GetAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void CancelAtSubscriptionScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      operations.CancelAtSubscriptionScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task CancelAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.CancelAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static DeploymentValidateResult ValidateAtSubscriptionScope(
      this IDeploymentsOperations operations,
      string deploymentName,
      Deployment parameters)
    {
      return operations.ValidateAtSubscriptionScopeAsync(deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentValidateResult> ValidateAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      Deployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentValidateResult body;
      using (AzureOperationResponse<DeploymentValidateResult> _result = await operations.ValidateAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static WhatIfOperationResult WhatIfAtSubscriptionScope(
      this IDeploymentsOperations operations,
      string deploymentName,
      DeploymentWhatIf parameters)
    {
      return operations.WhatIfAtSubscriptionScopeAsync(deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<WhatIfOperationResult> WhatIfAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      DeploymentWhatIf parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WhatIfOperationResult body;
      using (AzureOperationResponse<WhatIfOperationResult, DeploymentsWhatIfAtSubscriptionScopeHeaders> _result = await operations.WhatIfAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExportResult ExportTemplateAtSubscriptionScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      return operations.ExportTemplateAtSubscriptionScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExportResult> ExportTemplateAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExportResult body;
      using (AzureOperationResponse<DeploymentExportResult> _result = await operations.ExportTemplateAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentExtended> ListAtSubscriptionScope(
      this IDeploymentsOperations operations,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null)
    {
      return operations.ListAtSubscriptionScopeAsync(odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentExtended>> ListAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentExtended> body;
      using (AzureOperationResponse<IPage<DeploymentExtended>> _result = await operations.ListAtSubscriptionScopeWithHttpMessagesAsync(odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void Delete(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName)
    {
      operations.DeleteAsync(resourceGroupName, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task DeleteAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteWithHttpMessagesAsync(resourceGroupName, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static bool CheckExistence(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName)
    {
      return operations.CheckExistenceAsync(resourceGroupName, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<bool> CheckExistenceAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      bool body;
      using (AzureOperationResponse<bool> _result = await operations.CheckExistenceWithHttpMessagesAsync(resourceGroupName, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExtended CreateOrUpdate(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      Deployment parameters)
    {
      return operations.CreateOrUpdateAsync(resourceGroupName, deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> CreateOrUpdateAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      Deployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.CreateOrUpdateWithHttpMessagesAsync(resourceGroupName, deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExtended Get(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName)
    {
      return operations.GetAsync(resourceGroupName, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> GetAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.GetWithHttpMessagesAsync(resourceGroupName, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void Cancel(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName)
    {
      operations.CancelAsync(resourceGroupName, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task CancelAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.CancelWithHttpMessagesAsync(resourceGroupName, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static DeploymentValidateResult Validate(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      Deployment parameters)
    {
      return operations.ValidateAsync(resourceGroupName, deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentValidateResult> ValidateAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      Deployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentValidateResult body;
      using (AzureOperationResponse<DeploymentValidateResult> _result = await operations.ValidateWithHttpMessagesAsync(resourceGroupName, deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static WhatIfOperationResult WhatIf(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      DeploymentWhatIf parameters)
    {
      return operations.WhatIfAsync(resourceGroupName, deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<WhatIfOperationResult> WhatIfAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      DeploymentWhatIf parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WhatIfOperationResult body;
      using (AzureOperationResponse<WhatIfOperationResult, DeploymentsWhatIfHeaders> _result = await operations.WhatIfWithHttpMessagesAsync(resourceGroupName, deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentExportResult ExportTemplate(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName)
    {
      return operations.ExportTemplateAsync(resourceGroupName, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExportResult> ExportTemplateAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExportResult body;
      using (AzureOperationResponse<DeploymentExportResult> _result = await operations.ExportTemplateWithHttpMessagesAsync(resourceGroupName, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentExtended> ListByResourceGroup(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null)
    {
      return operations.ListByResourceGroupAsync(resourceGroupName, odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentExtended>> ListByResourceGroupAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentExtended> body;
      using (AzureOperationResponse<IPage<DeploymentExtended>> _result = await operations.ListByResourceGroupWithHttpMessagesAsync(resourceGroupName, odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static TemplateHashResult CalculateTemplateHash(
      this IDeploymentsOperations operations,
      object template)
    {
      return operations.CalculateTemplateHashAsync(template).GetAwaiter().GetResult();
    }

    public static async Task<TemplateHashResult> CalculateTemplateHashAsync(
      this IDeploymentsOperations operations,
      object template,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      TemplateHashResult body;
      using (AzureOperationResponse<TemplateHashResult> _result = await operations.CalculateTemplateHashWithHttpMessagesAsync(template, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void BeginDeleteAtScope(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName)
    {
      operations.BeginDeleteAtScopeAsync(scope, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task BeginDeleteAtScopeAsync(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.BeginDeleteAtScopeWithHttpMessagesAsync(scope, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static DeploymentExtended BeginCreateOrUpdateAtScope(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      Deployment parameters)
    {
      return operations.BeginCreateOrUpdateAtScopeAsync(scope, deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> BeginCreateOrUpdateAtScopeAsync(
      this IDeploymentsOperations operations,
      string scope,
      string deploymentName,
      Deployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.BeginCreateOrUpdateAtScopeWithHttpMessagesAsync(scope, deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void BeginDeleteAtTenantScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      operations.BeginDeleteAtTenantScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task BeginDeleteAtTenantScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.BeginDeleteAtTenantScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static DeploymentExtended BeginCreateOrUpdateAtTenantScope(
      this IDeploymentsOperations operations,
      string deploymentName,
      ScopedDeployment parameters)
    {
      return operations.BeginCreateOrUpdateAtTenantScopeAsync(deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> BeginCreateOrUpdateAtTenantScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      ScopedDeployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.BeginCreateOrUpdateAtTenantScopeWithHttpMessagesAsync(deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void BeginDeleteAtManagementGroupScope(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName)
    {
      operations.BeginDeleteAtManagementGroupScopeAsync(groupId, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task BeginDeleteAtManagementGroupScopeAsync(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.BeginDeleteAtManagementGroupScopeWithHttpMessagesAsync(groupId, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static DeploymentExtended BeginCreateOrUpdateAtManagementGroupScope(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      ScopedDeployment parameters)
    {
      return operations.BeginCreateOrUpdateAtManagementGroupScopeAsync(groupId, deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> BeginCreateOrUpdateAtManagementGroupScopeAsync(
      this IDeploymentsOperations operations,
      string groupId,
      string deploymentName,
      ScopedDeployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.BeginCreateOrUpdateAtManagementGroupScopeWithHttpMessagesAsync(groupId, deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void BeginDeleteAtSubscriptionScope(
      this IDeploymentsOperations operations,
      string deploymentName)
    {
      operations.BeginDeleteAtSubscriptionScopeAsync(deploymentName).GetAwaiter().GetResult();
    }

    public static async Task BeginDeleteAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.BeginDeleteAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static DeploymentExtended BeginCreateOrUpdateAtSubscriptionScope(
      this IDeploymentsOperations operations,
      string deploymentName,
      Deployment parameters)
    {
      return operations.BeginCreateOrUpdateAtSubscriptionScopeAsync(deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> BeginCreateOrUpdateAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      Deployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.BeginCreateOrUpdateAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static WhatIfOperationResult BeginWhatIfAtSubscriptionScope(
      this IDeploymentsOperations operations,
      string deploymentName,
      DeploymentWhatIf parameters)
    {
      return operations.BeginWhatIfAtSubscriptionScopeAsync(deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<WhatIfOperationResult> BeginWhatIfAtSubscriptionScopeAsync(
      this IDeploymentsOperations operations,
      string deploymentName,
      DeploymentWhatIf parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WhatIfOperationResult body;
      using (AzureOperationResponse<WhatIfOperationResult, DeploymentsWhatIfAtSubscriptionScopeHeaders> _result = await operations.BeginWhatIfAtSubscriptionScopeWithHttpMessagesAsync(deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void BeginDelete(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName)
    {
      operations.BeginDeleteAsync(resourceGroupName, deploymentName).GetAwaiter().GetResult();
    }

    public static async Task BeginDeleteAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.BeginDeleteWithHttpMessagesAsync(resourceGroupName, deploymentName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static DeploymentExtended BeginCreateOrUpdate(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      Deployment parameters)
    {
      return operations.BeginCreateOrUpdateAsync(resourceGroupName, deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentExtended> BeginCreateOrUpdateAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      Deployment parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentExtended body;
      using (AzureOperationResponse<DeploymentExtended> _result = await operations.BeginCreateOrUpdateWithHttpMessagesAsync(resourceGroupName, deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static WhatIfOperationResult BeginWhatIf(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      DeploymentWhatIf parameters)
    {
      return operations.BeginWhatIfAsync(resourceGroupName, deploymentName, parameters).GetAwaiter().GetResult();
    }

    public static async Task<WhatIfOperationResult> BeginWhatIfAsync(
      this IDeploymentsOperations operations,
      string resourceGroupName,
      string deploymentName,
      DeploymentWhatIf parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      WhatIfOperationResult body;
      using (AzureOperationResponse<WhatIfOperationResult, DeploymentsWhatIfHeaders> _result = await operations.BeginWhatIfWithHttpMessagesAsync(resourceGroupName, deploymentName, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentExtended> ListAtScopeNext(
      this IDeploymentsOperations operations,
      string nextPageLink)
    {
      return operations.ListAtScopeNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentExtended>> ListAtScopeNextAsync(
      this IDeploymentsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentExtended> body;
      using (AzureOperationResponse<IPage<DeploymentExtended>> _result = await operations.ListAtScopeNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentExtended> ListAtTenantScopeNext(
      this IDeploymentsOperations operations,
      string nextPageLink)
    {
      return operations.ListAtTenantScopeNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentExtended>> ListAtTenantScopeNextAsync(
      this IDeploymentsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentExtended> body;
      using (AzureOperationResponse<IPage<DeploymentExtended>> _result = await operations.ListAtTenantScopeNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentExtended> ListAtManagementGroupScopeNext(
      this IDeploymentsOperations operations,
      string nextPageLink)
    {
      return operations.ListAtManagementGroupScopeNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentExtended>> ListAtManagementGroupScopeNextAsync(
      this IDeploymentsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentExtended> body;
      using (AzureOperationResponse<IPage<DeploymentExtended>> _result = await operations.ListAtManagementGroupScopeNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentExtended> ListAtSubscriptionScopeNext(
      this IDeploymentsOperations operations,
      string nextPageLink)
    {
      return operations.ListAtSubscriptionScopeNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentExtended>> ListAtSubscriptionScopeNextAsync(
      this IDeploymentsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentExtended> body;
      using (AzureOperationResponse<IPage<DeploymentExtended>> _result = await operations.ListAtSubscriptionScopeNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentExtended> ListByResourceGroupNext(
      this IDeploymentsOperations operations,
      string nextPageLink)
    {
      return operations.ListByResourceGroupNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentExtended>> ListByResourceGroupNextAsync(
      this IDeploymentsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentExtended> body;
      using (AzureOperationResponse<IPage<DeploymentExtended>> _result = await operations.ListByResourceGroupNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
