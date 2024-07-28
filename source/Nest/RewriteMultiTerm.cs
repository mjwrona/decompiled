// Decompiled with JetBrains decompiler
// Type: Nest.RewriteMultiTerm
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System.Runtime.Serialization;

namespace Nest
{
  [StringEnum]
  public enum RewriteMultiTerm
  {
    [EnumMember(Value = "constant_score")] ConstantScore,
    [EnumMember(Value = "scoring_boolean")] ScoringBoolean,
    [EnumMember(Value = "constant_score_boolean")] ConstantScoreBoolean,
    [EnumMember(Value = "top_terms_N")] TopTermsN,
    [EnumMember(Value = "top_terms_boost_N")] TopTermsBoostN,
    [EnumMember(Value = "top_terms_blended_freqs_N")] TopTermsBlendedFreqsN,
  }
}
