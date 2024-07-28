// Decompiled with JetBrains decompiler
// Type: Nest.OverallBucket
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class OverallBucket
  {
    [DataMember(Name = "bucket_span")]
    public long BucketSpan { get; internal set; }

    [DataMember(Name = "is_interim")]
    public bool IsInterim { get; internal set; }

    [DataMember(Name = "jobs")]
    public IReadOnlyCollection<OverallBucketJobInfo> Jobs { get; internal set; } = EmptyReadOnly<OverallBucketJobInfo>.Collection;

    [DataMember(Name = "overall_score")]
    public double OverallScore { get; internal set; }

    [DataMember(Name = "result_type")]
    public string ResultType { get; internal set; }

    [DataMember(Name = "timestamp")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset Timestamp { get; internal set; }
  }
}
