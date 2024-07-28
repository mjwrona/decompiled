// Decompiled with JetBrains decompiler
// Type: Nest.ApiKeys
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Runtime.Serialization;

namespace Nest
{
  public class ApiKeys
  {
    [DataMember(Name = "creation")]
    [JsonFormatter(typeof (DateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset Creation { get; set; }

    [DataMember(Name = "expiration")]
    [JsonFormatter(typeof (NullableDateTimeOffsetEpochMillisecondsFormatter))]
    public DateTimeOffset? Expiration { get; set; }

    [DataMember(Name = "id")]
    public string Id { get; set; }

    [DataMember(Name = "invalidated")]
    public bool Invalidated { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "realm")]
    public string Realm { get; set; }

    [DataMember(Name = "username")]
    public string Username { get; set; }
  }
}
