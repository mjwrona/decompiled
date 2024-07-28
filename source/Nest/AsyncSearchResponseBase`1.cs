// Decompiled with JetBrains decompiler
// Type: Nest.AsyncSearchResponseBase`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public abstract class AsyncSearchResponseBase<TDocument> : 
    ResponseBase,
    IAsyncSearchResponse<TDocument>,
    IResponse,
    IElasticsearchResponse
    where TDocument : class
  {
    [DataMember(Name = "id")]
    public string Id { get; internal set; }

    [DataMember(Name = "is_partial")]
    public bool IsPartial { get; internal set; }

    [DataMember(Name = "start_time_in_millis")]
    public long StartTimeInMilliseconds { get; internal set; }

    [IgnoreDataMember]
    public DateTimeOffset StartTime => DateTimeUtil.UnixEpoch.AddMilliseconds((double) this.StartTimeInMilliseconds);

    [DataMember(Name = "is_running")]
    public bool IsRunning { get; internal set; }

    [DataMember(Name = "expiration_time_in_millis")]
    public long ExpirationTimeInMilliseconds { get; internal set; }

    [IgnoreDataMember]
    public DateTimeOffset ExpirationTime => DateTimeUtil.UnixEpoch.AddMilliseconds((double) this.ExpirationTimeInMilliseconds);

    [DataMember(Name = "response")]
    public AsyncSearch<TDocument> Response { get; internal set; }
  }
}
