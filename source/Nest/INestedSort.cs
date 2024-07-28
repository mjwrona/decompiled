// Decompiled with JetBrains decompiler
// Type: Nest.INestedSort
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [ReadAs(typeof (NestedSort))]
  public interface INestedSort
  {
    [DataMember(Name = "filter")]
    QueryContainer Filter { get; set; }

    [DataMember(Name = "nested")]
    INestedSort Nested { get; set; }

    [DataMember(Name = "path")]
    Field Path { get; set; }

    [DataMember(Name = "max_children")]
    int? MaxChildren { get; set; }
  }
}
