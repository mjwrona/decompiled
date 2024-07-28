// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.SubscriptionsOperationsExtensions
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
  public static class SubscriptionsOperationsExtensions
  {
    public static IEnumerable<Location> ListLocations(
      this ISubscriptionsOperations operations,
      string subscriptionId)
    {
      return operations.ListLocationsAsync(subscriptionId).GetAwaiter().GetResult();
    }

    public static async Task<IEnumerable<Location>> ListLocationsAsync(
      this ISubscriptionsOperations operations,
      string subscriptionId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IEnumerable<Location> body;
      using (AzureOperationResponse<IEnumerable<Location>> _result = await operations.ListLocationsWithHttpMessagesAsync(subscriptionId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static Subscription Get(this ISubscriptionsOperations operations, string subscriptionId) => operations.GetAsync(subscriptionId).GetAwaiter().GetResult();

    public static async Task<Subscription> GetAsync(
      this ISubscriptionsOperations operations,
      string subscriptionId,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      Subscription body;
      using (AzureOperationResponse<Subscription> _result = await operations.GetWithHttpMessagesAsync(subscriptionId, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<Subscription> List(this ISubscriptionsOperations operations) => operations.ListAsync().GetAwaiter().GetResult();

    public static async Task<IPage<Subscription>> ListAsync(
      this ISubscriptionsOperations operations,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<Subscription> body;
      using (AzureOperationResponse<IPage<Subscription>> _result = await operations.ListWithHttpMessagesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<Subscription> ListNext(
      this ISubscriptionsOperations operations,
      string nextPageLink)
    {
      return operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<Subscription>> ListNextAsync(
      this ISubscriptionsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<Subscription> body;
      using (AzureOperationResponse<IPage<Subscription>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
