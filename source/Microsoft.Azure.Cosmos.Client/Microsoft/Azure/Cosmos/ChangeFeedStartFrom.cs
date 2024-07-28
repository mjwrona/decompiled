// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeedStartFrom
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.ChangeFeed;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.Azure.Cosmos
{
  public abstract class ChangeFeedStartFrom
  {
    internal ChangeFeedStartFrom(FeedRange feedRange) => this.FeedRange = feedRange;

    internal FeedRange FeedRange { get; }

    internal abstract void Accept(ChangeFeedStartFromVisitor visitor);

    internal abstract TResult Accept<TResult>(ChangeFeedStartFromVisitor<TResult> visitor);

    internal abstract Task<TResult> AcceptAsync<TInput, TResult>(
      ChangeFeedStartFromAsyncVisitor<TInput, TResult> visitor,
      TInput input,
      CancellationToken cancellationToken);

    public static ChangeFeedStartFrom Now() => ChangeFeedStartFrom.Now((FeedRange) FeedRangeEpk.FullRange);

    public static ChangeFeedStartFrom Now(FeedRange feedRange) => feedRange is FeedRangeInternal feedRange1 ? (ChangeFeedStartFrom) new ChangeFeedStartFromNow(feedRange1) : throw new ArgumentException("feedRange needs to be a FeedRangeInternal.");

    public static ChangeFeedStartFrom Time(DateTime dateTimeUtc) => ChangeFeedStartFrom.Time(dateTimeUtc, (FeedRange) FeedRangeEpk.FullRange);

    public static ChangeFeedStartFrom Time(DateTime dateTimeUtc, FeedRange feedRange) => feedRange is FeedRangeInternal feedRange1 ? (ChangeFeedStartFrom) new ChangeFeedStartFromTime(dateTimeUtc, feedRange1) : throw new ArgumentException("feedRange needs to be a FeedRangeInternal.");

    public static ChangeFeedStartFrom ContinuationToken(string continuationToken) => (ChangeFeedStartFrom) new ChangeFeedStartFromContinuation(continuationToken);

    public static ChangeFeedStartFrom Beginning() => ChangeFeedStartFrom.Beginning((FeedRange) FeedRangeEpk.FullRange);

    public static ChangeFeedStartFrom Beginning(FeedRange feedRange) => feedRange is FeedRangeInternal feedRange1 ? (ChangeFeedStartFrom) new ChangeFeedStartFromBeginning(feedRange1) : throw new ArgumentException("feedRange needs to be a FeedRangeInternal.");
  }
}
