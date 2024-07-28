// Decompiled with JetBrains decompiler
// Type: Nest.ICustomAnalyzer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  public interface ICustomAnalyzer : IAnalyzer
  {
    [DataMember(Name = "char_filter")]
    [JsonFormatter(typeof (SingleOrEnumerableFormatter<string>))]
    IEnumerable<string> CharFilter { get; set; }

    [DataMember(Name = "filter")]
    [JsonFormatter(typeof (SingleOrEnumerableFormatter<string>))]
    IEnumerable<string> Filter { get; set; }

    [Obsolete("Deprecated, use PositionIncrementGap instead")]
    [DataMember(Name = "position_offset_gap")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? PositionOffsetGap { get; set; }

    [DataMember(Name = "position_increment_gap")]
    [JsonFormatter(typeof (NullableStringIntFormatter))]
    int? PositionIncrementGap { get; set; }

    [DataMember(Name = "tokenizer")]
    string Tokenizer { get; set; }
  }
}
