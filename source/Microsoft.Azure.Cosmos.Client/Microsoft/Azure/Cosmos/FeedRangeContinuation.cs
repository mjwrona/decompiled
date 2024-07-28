// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.FeedRangeContinuation
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Query.Core.Monads;
using Microsoft.Azure.Documents;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  internal abstract class FeedRangeContinuation
  {
    public string ContainerRid { get; }

    public virtual FeedRangeInternal FeedRange { get; }

    protected FeedRangeContinuation()
    {
    }

    public FeedRangeContinuation(string containerRid, FeedRangeInternal feedRange)
    {
      this.FeedRange = feedRange ?? throw new ArgumentNullException(nameof (feedRange));
      this.ContainerRid = containerRid;
    }

    public abstract string GetContinuation();

    public abstract Microsoft.Azure.Cosmos.FeedRange GetFeedRange();

    public abstract void ReplaceContinuation(string continuationToken);

    public abstract bool IsDone { get; }

    public abstract TryCatch ValidateContainer(string containerRid);

    public static bool TryParse(string toStringValue, out FeedRangeContinuation parsedToken)
    {
      if (FeedRangeCompositeContinuation.TryParse(toStringValue, out parsedToken))
        return true;
      parsedToken = (FeedRangeContinuation) null;
      return false;
    }

    public abstract ShouldRetryResult HandleChangeFeedNotModified(ResponseMessage responseMessage);

    public abstract Task<ShouldRetryResult> HandleSplitAsync(
      ContainerInternal containerCore,
      ResponseMessage responseMessage,
      CancellationToken cancellationToken);

    public abstract void Accept(IFeedRangeContinuationVisitor visitor);
  }
}
