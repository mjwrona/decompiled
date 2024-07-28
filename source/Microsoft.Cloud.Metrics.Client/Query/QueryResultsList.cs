// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.QueryResultsList
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Microsoft.Cloud.Metrics.Client.Utility;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.Query
{
  public sealed class QueryResultsList
  {
    [JsonConstructor]
    internal QueryResultsList(
      long startTimeUtc,
      long endTimeUtc,
      long timeResolutionInMilliseconds,
      IReadOnlyList<QueryResult> results)
    {
      this.StartTimeUtc = UnixEpochHelper.FromMillis(startTimeUtc);
      this.EndTimeUtc = UnixEpochHelper.FromMillis(endTimeUtc);
      this.TimeResolutionInMilliseconds = timeResolutionInMilliseconds;
      this.Results = (IReadOnlyList<IQueryResult>) results;
    }

    public DateTime EndTimeUtc { get; private set; }

    public DateTime StartTimeUtc { get; private set; }

    public long TimeResolutionInMilliseconds { get; private set; }

    public IReadOnlyList<IQueryResult> Results { get; private set; }
  }
}
