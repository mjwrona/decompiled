// Decompiled with JetBrains decompiler
// Type: Microsoft.Cloud.Metrics.Client.Query.QueryResultListV3
// Assembly: Microsoft.Cloud.Metrics.Client, Version=2.2023.705.2051, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 06B39E1C-7DF0-4BC1-AFBA-9AD635E73CB0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Cloud.Metrics.Client.dll

using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Cloud.Metrics.Client.Query
{
  public sealed class QueryResultListV3 : IQueryResultListV3
  {
    [JsonConstructor]
    internal QueryResultListV3(
      DateTime startTimeUtc,
      DateTime endTimeUtc,
      int timeResolutionInMinutes,
      IReadOnlyList<IQueryResultV3> results)
    {
      this.StartTimeUtc = startTimeUtc;
      this.EndTimeUtc = endTimeUtc;
      this.TimeResolutionInMinutes = timeResolutionInMinutes;
      this.Results = results;
    }

    public DateTime EndTimeUtc { get; }

    public DateTime StartTimeUtc { get; }

    public int TimeResolutionInMinutes { get; }

    public IReadOnlyList<IQueryResultV3> Results { get; }
  }
}
