// Decompiled with JetBrains decompiler
// Type: Nest.IAsyncSearchResponse`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface IAsyncSearchResponse<TDocument> : IResponse, IElasticsearchResponse where TDocument : class
  {
    [DataMember(Name = "id")]
    string Id { get; }

    [DataMember(Name = "is_partial")]
    bool IsPartial { get; }

    [DataMember(Name = "start_time_in_millis")]
    long StartTimeInMilliseconds { get; }

    [IgnoreDataMember]
    DateTimeOffset StartTime { get; }

    [DataMember(Name = "is_running")]
    bool IsRunning { get; }

    [DataMember(Name = "expiration_time_in_millis")]
    long ExpirationTimeInMilliseconds { get; }

    [IgnoreDataMember]
    DateTimeOffset ExpirationTime { get; }

    [DataMember(Name = "response")]
    AsyncSearch<TDocument> Response { get; }
  }
}
