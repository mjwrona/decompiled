// Decompiled with JetBrains decompiler
// Type: Nest.IStopTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IStopTokenFilter : ITokenFilter
  {
    [DataMember(Name = "ignore_case")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? IgnoreCase { get; set; }

    [DataMember(Name = "remove_trailing")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? RemoveTrailing { get; set; }

    [DataMember(Name = "stopwords")]
    StopWords StopWords { get; set; }

    [DataMember(Name = "stopwords_path")]
    string StopWordsPath { get; set; }
  }
}
