// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.ResourceLinksOperationsExtensions
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
  public static class ResourceLinksOperationsExtensions
  {
    public static void Delete(this IResourceLinksOperations operations, string linkId) => operations.DeleteAsync(linkId).GetAwaiter().GetResult();

    public static async Task DeleteAsync(
      this IResourceLinksOperations operations,
      string linkId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      AzureOperationResponse operationResponse = await operations.DeleteWithHttpMessagesAsync(linkId, cancellationToken: cancellationToken).ConfigureAwait(false);
      operationResponse.Dispose();
      operationResponse = (AzureOperationResponse) null;
    }

    public static ResourceLink CreateOrUpdate(
      this IResourceLinksOperations operations,
      string linkId,
      ResourceLink parameters)
    {
      return operations.CreateOrUpdateAsync(linkId, parameters).GetAwaiter().GetResult();
    }

    public static async Task<ResourceLink> CreateOrUpdateAsync(
      this IResourceLinksOperations operations,
      string linkId,
      ResourceLink parameters,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResourceLink body;
      using (AzureOperationResponse<ResourceLink> _result = await operations.CreateOrUpdateWithHttpMessagesAsync(linkId, parameters, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static ResourceLink Get(this IResourceLinksOperations operations, string linkId) => operations.GetAsync(linkId).GetAwaiter().GetResult();

    public static async Task<ResourceLink> GetAsync(
      this IResourceLinksOperations operations,
      string linkId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      ResourceLink body;
      using (AzureOperationResponse<ResourceLink> _result = await operations.GetWithHttpMessagesAsync(linkId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ResourceLink> ListAtSubscription(
      this IResourceLinksOperations operations,
      ODataQuery<ResourceLinkFilter> odataQuery = null)
    {
      return operations.ListAtSubscriptionAsync(odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ResourceLink>> ListAtSubscriptionAsync(
      this IResourceLinksOperations operations,
      ODataQuery<ResourceLinkFilter> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ResourceLink> body;
      using (AzureOperationResponse<IPage<ResourceLink>> _result = await operations.ListAtSubscriptionWithHttpMessagesAsync(odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ResourceLink> ListAtSourceScope(
      this IResourceLinksOperations operations,
      string scope,
      ODataQuery<ResourceLinkFilter> odataQuery = null)
    {
      return operations.ListAtSourceScopeAsync(scope, odataQuery).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ResourceLink>> ListAtSourceScopeAsync(
      this IResourceLinksOperations operations,
      string scope,
      ODataQuery<ResourceLinkFilter> odataQuery = null,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ResourceLink> body;
      using (AzureOperationResponse<IPage<ResourceLink>> _result = await operations.ListAtSourceScopeWithHttpMessagesAsync(scope, odataQuery, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ResourceLink> ListAtSubscriptionNext(
      this IResourceLinksOperations operations,
      string nextPageLink)
    {
      return operations.ListAtSubscriptionNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ResourceLink>> ListAtSubscriptionNextAsync(
      this IResourceLinksOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ResourceLink> body;
      using (AzureOperationResponse<IPage<ResourceLink>> _result = await operations.ListAtSubscriptionNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<ResourceLink> ListAtSourceScopeNext(
      this IResourceLinksOperations operations,
      string nextPageLink)
    {
      return operations.ListAtSourceScopeNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<ResourceLink>> ListAtSourceScopeNextAsync(
      this IResourceLinksOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<ResourceLink> body;
      using (AzureOperationResponse<IPage<ResourceLink>> _result = await operations.ListAtSourceScopeNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
