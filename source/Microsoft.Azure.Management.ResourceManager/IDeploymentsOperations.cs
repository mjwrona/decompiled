// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.IDeploymentsOperations
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
  public interface IDeploymentsOperations
  {
    Task<AzureOperationResponse> DeleteAtScopeWithHttpMessagesAsync(
      string scope,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<bool>> CheckExistenceAtScopeWithHttpMessagesAsync(
      string scope,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> CreateOrUpdateAtScopeWithHttpMessagesAsync(
      string scope,
      string deploymentName,
      Deployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> GetAtScopeWithHttpMessagesAsync(
      string scope,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> CancelAtScopeWithHttpMessagesAsync(
      string scope,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentValidateResult>> ValidateAtScopeWithHttpMessagesAsync(
      string scope,
      string deploymentName,
      Deployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExportResult>> ExportTemplateAtScopeWithHttpMessagesAsync(
      string scope,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentExtended>>> ListAtScopeWithHttpMessagesAsync(
      string scope,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteAtTenantScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<bool>> CheckExistenceAtTenantScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> CreateOrUpdateAtTenantScopeWithHttpMessagesAsync(
      string deploymentName,
      ScopedDeployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> GetAtTenantScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> CancelAtTenantScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentValidateResult>> ValidateAtTenantScopeWithHttpMessagesAsync(
      string deploymentName,
      ScopedDeployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExportResult>> ExportTemplateAtTenantScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentExtended>>> ListAtTenantScopeWithHttpMessagesAsync(
      ODataQuery<DeploymentExtendedFilter> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<bool>> CheckExistenceAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> CreateOrUpdateAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      string deploymentName,
      ScopedDeployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> GetAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> CancelAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentValidateResult>> ValidateAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      string deploymentName,
      ScopedDeployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExportResult>> ExportTemplateAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentExtended>>> ListAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<bool>> CheckExistenceAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> CreateOrUpdateAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      Deployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> GetAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> CancelAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentValidateResult>> ValidateAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      Deployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<WhatIfOperationResult, DeploymentsWhatIfAtSubscriptionScopeHeaders>> WhatIfAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      DeploymentWhatIf parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExportResult>> ExportTemplateAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentExtended>>> ListAtSubscriptionScopeWithHttpMessagesAsync(
      ODataQuery<DeploymentExtendedFilter> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<bool>> CheckExistenceWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> CreateOrUpdateWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      Deployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> GetWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> CancelWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentValidateResult>> ValidateWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      Deployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<WhatIfOperationResult, DeploymentsWhatIfHeaders>> WhatIfWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      DeploymentWhatIf parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExportResult>> ExportTemplateWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentExtended>>> ListByResourceGroupWithHttpMessagesAsync(
      string resourceGroupName,
      ODataQuery<DeploymentExtendedFilter> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<TemplateHashResult>> CalculateTemplateHashWithHttpMessagesAsync(
      object template,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> BeginDeleteAtScopeWithHttpMessagesAsync(
      string scope,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> BeginCreateOrUpdateAtScopeWithHttpMessagesAsync(
      string scope,
      string deploymentName,
      Deployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> BeginDeleteAtTenantScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> BeginCreateOrUpdateAtTenantScopeWithHttpMessagesAsync(
      string deploymentName,
      ScopedDeployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> BeginDeleteAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> BeginCreateOrUpdateAtManagementGroupScopeWithHttpMessagesAsync(
      string groupId,
      string deploymentName,
      ScopedDeployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> BeginDeleteAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> BeginCreateOrUpdateAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      Deployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<WhatIfOperationResult, DeploymentsWhatIfAtSubscriptionScopeHeaders>> BeginWhatIfAtSubscriptionScopeWithHttpMessagesAsync(
      string deploymentName,
      DeploymentWhatIf parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> BeginDeleteWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentExtended>> BeginCreateOrUpdateWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      Deployment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<WhatIfOperationResult, DeploymentsWhatIfHeaders>> BeginWhatIfWithHttpMessagesAsync(
      string resourceGroupName,
      string deploymentName,
      DeploymentWhatIf parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentExtended>>> ListAtScopeNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentExtended>>> ListAtTenantScopeNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentExtended>>> ListAtManagementGroupScopeNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentExtended>>> ListAtSubscriptionScopeNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentExtended>>> ListByResourceGroupNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
