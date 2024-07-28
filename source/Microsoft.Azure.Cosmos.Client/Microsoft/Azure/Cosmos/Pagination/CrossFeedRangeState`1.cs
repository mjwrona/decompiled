// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.CrossFeedRangeState`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal sealed class CrossFeedRangeState<TState> : State where TState : State
  {
    public CrossFeedRangeState(ReadOnlyMemory<FeedRangeState<TState>> value) => this.Value = !value.IsEmpty ? value : throw new ArgumentException("value is empty.");

    public ReadOnlyMemory<FeedRangeState<TState>> Value { get; }
  }
}
