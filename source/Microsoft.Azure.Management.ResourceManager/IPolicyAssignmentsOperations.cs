// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.IPolicyAssignmentsOperations
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
  public interface IPolicyAssignmentsOperations
  {
    Task<AzureOperationResponse<PolicyAssignment>> DeleteWithHttpMessagesAsync(
      string scope,
      string policyAssignmentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<PolicyAssignment>> CreateWithHttpMessagesAsync(
      string scope,
      string policyAssignmentName,
      PolicyAssignment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<PolicyAssignment>> GetWithHttpMessagesAsync(
      string scope,
      string policyAssignmentName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyAssignment>>> ListForResourceGroupWithHttpMessagesAsync(
      string resourceGroupName,
      string filter = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyAssignment>>> ListForResourceWithHttpMessagesAsync(
      string resourceGroupName,
      string resourceProviderNamespace,
      string parentResourcePath,
      string resourceType,
      string resourceName,
      ODataQuery<PolicyAssignment> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyAssignment>>> ListWithHttpMessagesAsync(
      ODataQuery<PolicyAssignment> odataQuery = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<PolicyAssignment>> DeleteByIdWithHttpMessagesAsync(
      string policyAssignmentId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<PolicyAssignment>> CreateByIdWithHttpMessagesAsync(
      string policyAssignmentId,
      PolicyAssignment parameters,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<PolicyAssignment>> GetByIdWithHttpMessagesAsync(
      string policyAssignmentId,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyAssignment>>> ListForResourceGroupNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyAssignment>>> ListForResourceNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<PolicyAssignment>>> ListNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
