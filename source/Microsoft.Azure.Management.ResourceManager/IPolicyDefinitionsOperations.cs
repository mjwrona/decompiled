// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.IPolicyDefinitionsOperations
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
  public interface IPolicyDefinitionsOperations
  {
    Task<AzureOperationResponse<PolicyDefinition>> CreateOrUpdateWithHttpMessagesAsync(
      string policyDefinitionName,
      PolicyDefinition parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteWithHttpMessagesAsync(
      string policyDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<PolicyDefinition>> GetWithHttpMessagesAsync(
      string policyDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<PolicyDefinition>> GetBuiltInWithHttpMessagesAsync(
      string policyDefinitionName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<PolicyDefinition>> CreateOrUpdateAtManagementGroupWithHttpMessagesAsync(
      string policyDefinitionName,
      PolicyDefinition parameters,
      string managementGroupId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteAtManagementGroupWithHttpMessagesAsync(
      string policyDefinitionName,
      string managementGroupId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<PolicyDefinition>> GetAtManagementGroupWithHttpMessagesAsync(
      string policyDefinitionName,
      string managementGroupId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListWithHttpMessagesAsync(
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListBuiltInWithHttpMessagesAsync(
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListByManagementGroupWithHttpMessagesAsync(
      string managementGroupId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListBuiltInNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyDefinition>>> ListByManagementGroupNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
