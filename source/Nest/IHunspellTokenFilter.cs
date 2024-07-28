// Decompiled with JetBrains decompiler
// Type: Nest.IHunspellTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IHunspellTokenFilter : ITokenFilter
  {
    [DataMember(Name = "dedup")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? Dedup { get; set; }

    [DataMember(Name = "dictionary")]
    string Dictionary { get; set; }

    [DataMember(Name = "locale")]
    string Locale { get; set; }

    [DataMember(Name = "longest_only")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? LongestOnly { get; set; }
  }
}
