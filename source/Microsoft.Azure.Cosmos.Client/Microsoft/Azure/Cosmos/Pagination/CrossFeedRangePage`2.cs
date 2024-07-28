// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.CrossFeedRangePage`2
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal sealed class CrossFeedRangePage<TBackendPage, TBackendState> : 
    Microsoft.Azure.Cosmos.Pagination.Page<CrossFeedRangeState<TBackendState>>
    where TBackendPage : Microsoft.Azure.Cosmos.Pagination.Page<TBackendState>
    where TBackendState : Microsoft.Azure.Cosmos.Pagination.State
  {
    private static readonly ImmutableHashSet<string> bannedHeaders = new HashSet<string>().ToImmutableHashSet<string>();

    public CrossFeedRangePage(TBackendPage backendEndPage, CrossFeedRangeState<TBackendState> state)
      : base(backendEndPage.RequestCharge, backendEndPage.ActivityId, backendEndPage.AdditionalHeaders, state)
    {
      this.Page = backendEndPage;
    }

    public TBackendPage Page { get; }

    protected override ImmutableHashSet<string> DerivedClassBannedHeaders => CrossFeedRangePage<TBackendPage, TBackendState>.bannedHeaders;
  }
}
