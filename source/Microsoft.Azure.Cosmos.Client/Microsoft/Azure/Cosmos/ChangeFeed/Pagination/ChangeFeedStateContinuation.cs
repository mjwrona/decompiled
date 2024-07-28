// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedStateContinuation
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Pagination
{
  internal sealed class ChangeFeedStateContinuation : ChangeFeedState
  {
    public ChangeFeedStateContinuation(CosmosElement continuation) => this.ContinuationToken = continuation ?? throw new ArgumentNullException(nameof (continuation));

    public CosmosElement ContinuationToken { get; }

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
