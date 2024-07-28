// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.DeploymentScriptsOperationsExtensions
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public static class DeploymentScriptsOperationsExtensions
  {
    public static DeploymentScript Create(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName,
      DeploymentScript deploymentScript)
    {
      return operations.CreateAsync(resourceGroupName, scriptName, deploymentScript).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentScript> CreateAsync(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName,
      DeploymentScript deploymentScript,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentScript body;
      using (AzureOperationResponse<DeploymentScript> _result = await operations.CreateWithHttpMessagesAsync(resourceGroupName, scriptName, deploymentScript, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentScript Update(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName,
      DeploymentScriptUpdateParameter deploymentScript = null)
    {
      return operations.UpdateAsync(resourceGroupName, scriptName, deploymentScript).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentScript> UpdateAsync(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName,
      DeploymentScriptUpdateParameter deploymentScript = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentScript body;
      using (AzureOperationResponse<DeploymentScript> _result = await operations.UpdateWithHttpMessagesAsync(resourceGroupName, scriptName, deploymentScript, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentScript Get(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName)
    {
      return operations.GetAsync(resourceGroupName, scriptName).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentScript> GetAsync(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentScript body;
      using (AzureOperationResponse<DeploymentScript> _result = await operations.GetWithHttpMessagesAsync(resourceGroupName, scriptName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static void Delete(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName)
    {
      operations.DeleteAsync(resourceGroupName, scriptName).GetAwaiter().GetResult();
    }

    public static async Task DeleteAsync(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteWithHttpMessagesAsync(resourceGroupName, scriptName, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static IPage<DeploymentScript> ListBySubscription(
      this IDeploymentScriptsOperations operations)
    {
      return operations.ListBySubscriptionAsync().GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentScript>> ListBySubscriptionAsync(
      this IDeploymentScriptsOperations operations,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentScript> body;
      using (AzureOperationResponse<IPage<DeploymentScript>> _result = await operations.ListBySubscriptionWithHttpMessagesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static ScriptLogsList GetLogs(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName)
    {
      return operations.GetLogsAsync(resourceGroupName, scriptName).GetAwaiter().GetResult();
    }

    public static async Task<ScriptLogsList> GetLogsAsync(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ScriptLogsList body;
      using (AzureOperationResponse<ScriptLogsList> _result = await operations.GetLogsWithHttpMessagesAsync(resourceGroupName, scriptName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static ScriptLog GetLogsDefault(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName)
    {
      return operations.GetLogsDefaultAsync(resourceGroupName, scriptName).GetAwaiter().GetResult();
    }

    public static async Task<ScriptLog> GetLogsDefaultAsync(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ScriptLog body;
      using (AzureOperationResponse<ScriptLog> _result = await operations.GetLogsDefaultWithHttpMessagesAsync(resourceGroupName, scriptName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentScript> ListByResourceGroup(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName)
    {
      return operations.ListByResourceGroupAsync(resourceGroupName).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentScript>> ListByResourceGroupAsync(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentScript> body;
      using (AzureOperationResponse<IPage<DeploymentScript>> _result = await operations.ListByResourceGroupWithHttpMessagesAsync(resourceGroupName, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static DeploymentScript BeginCreate(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName,
      DeploymentScript deploymentScript)
    {
      return operations.BeginCreateAsync(resourceGroupName, scriptName, deploymentScript).GetAwaiter().GetResult();
    }

    public static async Task<DeploymentScript> BeginCreateAsync(
      this IDeploymentScriptsOperations operations,
      string resourceGroupName,
      string scriptName,
      DeploymentScript deploymentScript,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      DeploymentScript body;
      using (AzureOperationResponse<DeploymentScript> _result = await operations.BeginCreateWithHttpMessagesAsync(resourceGroupName, scriptName, deploymentScript, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentScript> ListBySubscriptionNext(
      this IDeploymentScriptsOperations operations,
      string nextPageLink)
    {
      return operations.ListBySubscriptionNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentScript>> ListBySubscriptionNextAsync(
      this IDeploymentScriptsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentScript> body;
      using (AzureOperationResponse<IPage<DeploymentScript>> _result = await operations.ListBySubscriptionNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<DeploymentScript> ListByResourceGroupNext(
      this IDeploymentScriptsOperations operations,
      string nextPageLink)
    {
      return operations.ListByResourceGroupNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<DeploymentScript>> ListByResourceGroupNextAsync(
      this IDeploymentScriptsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<DeploymentScript> body;
      using (AzureOperationResponse<IPage<DeploymentScript>> _result = await operations.ListByResourceGroupNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
