// Decompiled with JetBrains decompiler
// Type: Nest.DeleteByQueryResponse
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public class DeleteByQueryResponse : ResponseBase
  {
    public override bool IsValid
    {
      get
      {
        IApiCallDetails apiCall = this.ApiCall;
        int num1;
        if (apiCall == null)
        {
          num1 = 0;
        }
        else
        {
          int? httpStatusCode = apiCall.HttpStatusCode;
          int num2 = 200;
          num1 = httpStatusCode.GetValueOrDefault() == num2 & httpStatusCode.HasValue ? 1 : 0;
        }
        return num1 != 0 || !this.Failures.HasAny<BulkIndexByScrollFailure>();
      }
    }

    [DataMember(Name = "batches")]
    public long Batches { get; internal set; }

    [DataMember(Name = "deleted")]
    public long Deleted { get; internal set; }

    [DataMember(Name = "failures")]
    public IReadOnlyCollection<BulkIndexByScrollFailure> Failures { get; internal set; } = EmptyReadOnly<BulkIndexByScrollFailure>.Collection;

    [DataMember(Name = "noops")]
    public long Noops { get; internal set; }

    [DataMember(Name = "requests_per_second")]
    public float RequestsPerSecond { get; internal set; }

    [DataMember(Name = "retries")]
    public Retries Retries { get; internal set; }

    [DataMember(Name = "slice_id")]
    public int? SliceId { get; internal set; }

    [DataMember(Name = "task")]
    public TaskId Task { get; internal set; }

    [DataMember(Name = "throttled_millis")]
    public long ThrottledMilliseconds { get; internal set; }

    [DataMember(Name = "throttled_until_millis")]
    public long ThrottledUntilMilliseconds { get; internal set; }

    [DataMember(Name = "timed_out")]
    public bool TimedOut { get; internal set; }

    [DataMember(Name = "took")]
    public long Took { get; internal set; }

    [DataMember(Name = "total")]
    public long Total { get; internal set; }

    [DataMember(Name = "version_conflicts")]
    public long VersionConflicts { get; internal set; }
  }
}
