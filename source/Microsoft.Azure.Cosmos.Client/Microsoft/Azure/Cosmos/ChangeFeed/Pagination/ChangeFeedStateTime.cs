// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedStateTime
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Pagination
{
  internal sealed class ChangeFeedStateTime : ChangeFeedState
  {
    public ChangeFeedStateTime(DateTime time) => this.StartTime = time.Kind == DateTimeKind.Utc ? time : throw new ArgumentOutOfRangeException("time.Kind must be DateTimeKind.Utc");

    public DateTime StartTime { get; }

    public override void Accept<TInput>(IChangeFeedStateVisitor<TInput> visitor, TInput input) => visitor.Visit(this, input);

    public override TOutput Accept<TInput, TOutput>(
      IChangeFeedStateVisitor<TInput, TOutput> visitor,
      TInput input)
    {
      return visitor.Visit(this, input);
    }

    public override TResult Accept<TResult>(IChangeFeedStateTransformer<TResult> visitor) => visitor.Transform(this);
  }
}
