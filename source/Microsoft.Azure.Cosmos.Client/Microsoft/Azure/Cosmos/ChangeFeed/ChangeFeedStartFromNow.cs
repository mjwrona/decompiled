// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.ChangeFeedStartFromNow
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos.ChangeFeed
{
  internal sealed class ChangeFeedStartFromNow : ChangeFeedStartFrom
  {
    public ChangeFeedStartFromNow(FeedRangeInternal feedRange)
      : base((FeedRange) feedRange)
    {
    }

    internal override void Accept(ChangeFeedStartFromVisitor visitor) => visitor.Visit(this);

    internal override TResult Accept<TResult>(ChangeFeedStartFromVisitor<TResult> visitor) => visitor.Visit(this);

    internal override Task<TOutput> AcceptAsync<TInput, TOutput>(
      ChangeFeedStartFromAsyncVisitor<TInput, TOutput> visitor,
      TInput input,
      CancellationToken cancellationToken)
    {
      return visitor.VisitAsync(this, input, cancellationToken);
    }
  }
}
