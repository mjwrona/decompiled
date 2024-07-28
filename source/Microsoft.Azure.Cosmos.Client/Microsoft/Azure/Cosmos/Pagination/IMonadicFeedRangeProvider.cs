// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.IMonadicFeedRangeProvider
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Cosmos.Tracing;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal interface IMonadicFeedRangeProvider
  {
    Task<TryCatch<List<FeedRangeEpk>>> MonadicGetChildRangeAsync(
      FeedRangeInternal feedRange,
      ITrace trace,
      CancellationToken cancellationToken);

    Task<TryCatch<List<FeedRangeEpk>>> MonadicGetFeedRangesAsync(
      ITrace trace,
      CancellationToken cancellationToken);

    Task<TryCatch> MonadicRefreshProviderAsync(ITrace trace, CancellationToken cancellationToken);
  }
}
