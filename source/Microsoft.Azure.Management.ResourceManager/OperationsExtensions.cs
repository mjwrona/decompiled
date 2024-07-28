// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.OperationsExtensions
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Microsoft.Azure.Management.ResourceManager.Models;
using Microsoft.Rest.Azure;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Management.ResourceManager
{
  public static class OperationsExtensions
  {
    public static IPage<Operation> List(this IOperations operations) => operations.ListAsync().GetAwaiter().GetResult();

    public static async Task<IPage<Operation>> ListAsync(
      this IOperations operations,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<Operation> body;
      using (AzureOperationResponse<IPage<Operation>> _result = await operations.ListWithHttpMessagesAsync(cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }

    public static IPage<Operation> ListNext(this IOperations operations, string nextPageLink) => operations.ListNextAsync(nextPageLink).GetAwaiter().GetResult();

    public static async Task<IPage<Operation>> ListNextAsync(
      this IOperations operations,
      string nextPageLink,
      CancellationToken cancellationToken = default (CancellationToken))
    {
      IPage<Operation> body;
      using (AzureOperationResponse<IPage<Operation>> _result = await operations.ListNextWithHttpMessagesAsync(nextPageLink, cancellationToken: cancellationToken).ConfigureAwait(false))
        body = _result.Body;
      return body;
    }
  }
}
