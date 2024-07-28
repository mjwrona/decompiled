// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Pagination.Page`1
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.Azure.Cosmos.Pagination
{
  internal abstract class Page<TState> where TState : Microsoft.Azure.Cosmos.Pagination.State
  {
    protected static readonly ImmutableHashSet<string> BannedHeadersBase = new HashSet<string>()
    {
      "x-ms-request-charge",
      "x-ms-activity-id"
    }.ToImmutableHashSet<string>();
    private static readonly IReadOnlyDictionary<string, string> EmptyDictionary = (IReadOnlyDictionary<string, string>) new Dictionary<string, string>();

    protected Page(
      double requestCharge,
      string activityId,
      IReadOnlyDictionary<string, string> additionalHeaders,
      TState state)
    {
      this.RequestCharge = requestCharge >= 0.0 ? requestCharge : throw new ArgumentOutOfRangeException(nameof (requestCharge));
      this.ActivityId = activityId;
      this.State = state;
      this.AdditionalHeaders = additionalHeaders ?? Page<TState>.EmptyDictionary;
    }

    public double RequestCharge { get; }

    public string ActivityId { get; }

    public IReadOnlyDictionary<string, string> AdditionalHeaders { get; }

    public TState State { get; }

    protected abstract ImmutableHashSet<string> DerivedClassBannedHeaders { get; }
  }
}
