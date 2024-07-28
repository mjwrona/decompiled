// Decompiled with JetBrains decompiler
// Type: Nest.ISort
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (SortFormatter))]
  public interface ISort
  {
    [DataMember(Name = "format")]
    string Format { get; set; }

    [DataMember(Name = "missing")]
    object Missing { get; set; }

    [DataMember(Name = "mode")]
    SortMode? Mode { get; set; }

    [DataMember(Name = "numeric_type")]
    Nest.NumericType? NumericType { get; set; }

    [DataMember(Name = "nested")]
    INestedSort Nested { get; set; }

    [DataMember(Name = "order")]
    SortOrder? Order { get; set; }

    [IgnoreDataMember]
    Field SortKey { get; }
  }
}
