// Decompiled with JetBrains decompiler
// Type: Nest.IPathHierarchyTokenizer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IPathHierarchyTokenizer : ITokenizer
  {
    [DataMember(Name = "buffer_size")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? BufferSize { get; set; }

    [DataMember(Name = "delimiter")]
    char? Delimiter { get; set; }

    [DataMember(Name = "replacement")]
    char? Replacement { get; set; }

    [DataMember(Name = "reverse")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? Reverse { get; set; }

    [DataMember(Name = "skip")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? Skip { get; set; }
  }
}
