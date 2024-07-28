// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedStartFromAsyncVisitor`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal abstract class ChangeFeedStartFromAsyncVisitor<TInput, TResult>
  {
    public abstract Task<TResult> VisitAsync(
      ChangeFeedStartFromNow startFromNow,
      TInput input,
      CancellationToken cancellationToken);

    public abstract Task<TResult> VisitAsync(
      ChangeFeedStartFromTime startFromTime,
      TInput input,
      CancellationToken cancellationToken);

    public abstract Task<TResult> VisitAsync(
      ChangeFeedStartFromContinuation startFromContinuation,
      TInput input,
      CancellationToken cancellationToken);

    public abstract Task<TResult> VisitAsync(
      ChangeFeedStartFromBeginning startFromBeginning,
      TInput input,
      CancellationToken cancellationToken);

    public abstract Task<TResult> VisitAsync(
      ChangeFeedStartFromContinuationAndFeedRange startFromContinuationAndFeedRange,
      TInput input,
      CancellationToken cancellationToken);
  }
}
