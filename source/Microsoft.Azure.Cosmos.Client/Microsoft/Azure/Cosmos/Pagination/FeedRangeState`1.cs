// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.FeedRangeState`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal readonly struct FeedRangeState<TState> where TState : Microsoft.Azure.Cosmos.Pagination.State
  {
    public FeedRangeState(FeedRangeInternal feedRange, TState state)
    {
      this.FeedRange = feedRange ?? throw new ArgumentNullException(nameof (feedRange));
      this.State = state;
    }

    public FeedRangeInternal FeedRange { get; }

    public TState State { get; }
  }
}
