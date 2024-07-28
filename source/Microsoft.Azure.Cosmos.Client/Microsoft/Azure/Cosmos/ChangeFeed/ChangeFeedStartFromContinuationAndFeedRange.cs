// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedStartFromContinuationAndFeedRange
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedStartFromContinuationAndFeedRange : ChangeFeedStartFrom
  {
    public ChangeFeedStartFromContinuationAndFeedRange(string etag, FeedRangeInternal feedRange)
      : base((FeedRange) feedRange)
    {
      this.Etag = etag;
    }

    public string Etag { get; }

    internal override void Accept(ChangeFeedStartFromVisitor visitor) => visitor.Visit(this);

    internal override TResult Accept<TResult>(ChangeFeedStartFromVisitor<TResult> visitor) => visitor.Visit(this);

    internal override Task<TResult> AcceptAsync<TInput, TResult>(
      ChangeFeedStartFromAsyncVisitor<TInput, TResult> visitor,
      TInput input,
      CancellationToken cancellationToken)
    {
      return visitor.VisitAsync(this, input, cancellationToken);
    }
  }
}
