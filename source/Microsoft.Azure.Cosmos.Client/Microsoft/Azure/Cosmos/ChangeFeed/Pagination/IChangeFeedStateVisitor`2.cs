// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.ChangeFeed.Pagination.IChangeFeedStateVisitor`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

namespace Microsoft.Azure.Cosmos.ChangeFeed.Pagination
{
  internal interface IChangeFeedStateVisitor<TInput, TOutput>
  {
    TOutput Visit(ChangeFeedStateBeginning changeFeedStateBeginning, TInput input);

    TOutput Visit(ChangeFeedStateTime changeFeedStateTime, TInput input);

    TOutput Visit(
      ChangeFeedStateContinuation changeFeedStateContinuation,
      TInput input);

    TOutput Visit(ChangeFeedStateNow changeFeedStateNow, TInput input);
  }
}
