// Decompiled with JetBrains decompiler
// Type: Nest.AutoFollowPattern
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class AutoFollowPattern : IAutoFollowPattern
  {
    public string FollowIndexPattern { get; set; }

    public IEnumerable<string> LeaderIndexPatterns { get; set; }

    public IEnumerable<string> LeaderIndexExclusionPatterns { get; set; }

    public IIndexSettings Settings { get; set; }

    public long? MaxOutstandingReadRequests { get; set; }

    public int? MaxOutstandingWriteRequests { get; set; }

    public Time MaxPollTimeout { get; set; }

    public int? MaxReadRequestOperationCount { get; set; }

    public string MaxReadRequestSize { get; set; }

    public Time MaxRetryDelay { get; set; }

    public int? MaxWriteBufferCount { get; set; }

    public string MaxWriteBufferSize { get; set; }

    public int? MaxWriteRequestOperationCount { get; set; }

    public string MaxWriteRequestSize { get; set; }

    public string RemoteCluster { get; set; }
  }
}
