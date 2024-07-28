// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.TenantsOperationsExtensions
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public static class TenantsOperationsExtensions
  {
    public static IPage<TenantIdDescription> List(this ITenantsOperations operations) => operations.ListAsync().GetAwaiter().GetResult();

    public static async Task<IPage<TenantIdDescription>> ListAsync(
      this ITenantsOperations operations,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<TenantIdDescription> body;
      using (AzureOperationResponse<IPage<TenantIdDescription>> _result = await operations.ListWithHttpMessagesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<TenantIdDescription> ListNext(
      this ITenantsOperations operations,
      string nextPageLink)
    {
      return operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();
    }

    public static async Task<IPage<TenantIdDescription>> ListNextAsync(
      this ITenantsOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<TenantIdDescription> body;
      using (AzureOperationResponse<IPage<TenantIdDescription>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
