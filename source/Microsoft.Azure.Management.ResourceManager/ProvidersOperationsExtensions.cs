// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.ProvidersOperationsExtensions
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public static class ProvidersOperationsExtensions
  {
    public static Provider Unregister(
      this IProvidersOperations operations,
      string resourceProviderNamespace)
    {
      return operations.UnregisterAsync(resourceProviderNamespace).GetAwaiter().GetResult();
    }

    public static async Task<Provider> UnregisterAsync(
      this IProvidersOperations operations,
      string resourceProviderNamespace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Provider body;
      using (AzureOperationResponse<Provider> _result = await operations.UnregisterWithHttpMessagesAsync(resourceProviderNamespace, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static Provider Register(
      this IProvidersOperations operations,
      string resourceProviderNamespace)
    {
      return operations.RegisterAsync(resourceProviderNamespace).GetAwaiter().GetResult();
    }

    public static async Task<Provider> RegisterAsync(
      this IProvidersOperations operations,
      string resourceProviderNamespace,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Provider body;
      using (AzureOperationResponse<Provider> _result = await operations.RegisterWithHttpMessagesAsync(resourceProviderNamespace, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<Provider> List(
      this IProvidersOperations operations,
      int? top = null,
      string expand = null)
    {
      return operations.ListAsync(top, expand).GetAwaiter().GetResult();
    }

    public static async Task<IPage<Provider>> ListAsync(
      this IProvidersOperations operations,
      int? top = null,
      string expand = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<Provider> body;
      using (AzureOperationResponse<IPage<Provider>> _result = await operations.ListWithHttpMessagesAsync(top, expand, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<Provider> ListAtTenantScope(
      this IProvidersOperations operations,
      int? top = null,
      string expand = null)
    {
      return operations.ListAtTenantScopeAsync(top, expand).GetAwaiter().GetResult();
    }

    public static async Task<IPage<Provider>> ListAtTenantScopeAsync(
      this IProvidersOperations operations,
      int? top = null,
      string expand = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<Provider> body;
      using (AzureOperationResponse<IPage<Provider>> _result = await operations.ListAtTenantScopeWithHttpMessagesAsync(top, expand, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static Provider Get(
      this IProvidersOperations operations,
      string resourceProviderNamespace,
      string expand = null)
    {
      return operations.GetAsync(resourceProviderNamespace, expand).GetAwaiter().GetResult();
    }

    public static async Task<Provider> GetAsync(
      this IProvidersOperations operations,
      string resourceProviderNamespace,
      string expand = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Provider body;
      using (AzureOperationResponse<Provider> _result = await operations.GetWithHttpMessagesAsync(resourceProviderNamespace, expand, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static Provider GetAtTenantScope(
      this IProvidersOperations operations,
      string resourceProviderNamespace,
      string expand = null)
    {
      return operations.GetAtTenantScopeAsync(resourceProviderNamespace, expand).GetAwaiter().GetResult();
    }

    public static async Task<Provider> GetAtTenantScopeAsync(
      this IProvidersOperations operations,
      string resourceProviderNamespace,
      string expand = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Provider body;
      using (AzureOperationResponse<Provider> _result = await operations.GetAtTenantScopeWithHttpMessagesAsync(resourceProviderNamespace, expand, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<Provider> ListNext(
      this IProvidersOperations operations,
      string nextPageLink)
    {
      return operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<Provider>> ListNextAsync(
      this IProvidersOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<Provider> body;
      using (AzureOperationResponse<IPage<Provider>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<Provider> ListAtTenantScopeNext(
      this IProvidersOperations operations,
      string nextPageLink)
    {
      return operations.ListAtTenantScopeNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<Provider>> ListAtTenantScopeNextAsync(
      this IProvidersOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<Provider> body;
      using (AzureOperationResponse<IPage<Provider>> _result = await operations.ListAtTenantScopeNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
