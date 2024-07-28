// Decompiled with JetBrains decompiler
// Type: Nest.TextQueryType
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum TextQueryType
  {
    [EnumMember(Value = "best_fields")] BestFields,
    [EnumMember(Value = "most_fields")] MostFields,
    [EnumMember(Value = "cross_fields")] CrossFields,
    [EnumMember(Value = "phrase")] Phrase,
    [EnumMember(Value = "phrase_prefix")] PhrasePrefix,
    [EnumMember(Value = "bool_prefix")] BoolPrefix,
  }
}
