// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.IDeploymentScriptsOperations
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
  public interface IDeploymentScriptsOperations
  {
    Task<AzureOperationResponse<DeploymentScript>> CreateWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      DeploymentScript deploymentScript,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentScript>> UpdateWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      DeploymentScriptUpdateParameter deploymentScript = null,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentScript>> GetWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse> DeleteWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentScript>>> ListBySubscriptionWithHttpMessagesAsync(
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ScriptLogsList>> GetLogsWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<ScriptLog>> GetLogsDefaultWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentScript>>> ListByResourceGroupWithHttpMessagesAsync(
      string resourceGroupName,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<DeploymentScript>> BeginCreateWithHttpMessagesAsync(
      string resourceGroupName,
      string scriptName,
      DeploymentScript deploymentScript,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentScript>>> ListBySubscriptionNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));

    Task<AzureOperationResponse<IPage<DeploymentScript>>> ListByResourceGroupNextWithHttpMessagesAsync(
      string nextPageLink,
      Dictionary<string, List<string>> customHeaders = null,
      CancellationToken cancellationToken = default (CancellationToken));
  }
}
