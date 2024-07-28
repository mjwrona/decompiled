// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Pagination.ChangeFeedState
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.CosmosElements;
using Microsoft.Azure.Cosmos.Pagination;
using System;

namespace Microsoft.Azure.Cosmos.ChangeFeed.Pagination
{
  internal abstract class ChangeFeedState : State
  {
    public abstract void Accept<TInput>(IChangeFeedStateVisitor<TInput> visitor, TInput input);

    public abstract TOutput Accept<TInput, TOutput>(
      IChangeFeedStateVisitor<TInput, TOutput> visitor,
      TInput input);

    public abstract TResult Accept<TResult>(IChangeFeedStateTransformer<TResult> visitor);

    public static ChangeFeedState Now() => (ChangeFeedState) ChangeFeedStateNow.Singleton;

    public static ChangeFeedState Beginning() => (ChangeFeedState) ChangeFeedStateBeginning.Singleton;

    public static ChangeFeedState Time(DateTime time) => (ChangeFeedState) new ChangeFeedStateTime(time);

    public static ChangeFeedState Continuation(CosmosElement continuation) => (ChangeFeedState) new ChangeFeedStateContinuation(continuation);
  }
}
