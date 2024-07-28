// Decompiled with JetBrains decompiler
// Type: Nest.IIcuCollationTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  public interface IIcuCollationTokenFilter : ITokenFilter
  {
    [DataMember(Name = "alternate")]
    IcuCollationAlternate? Alternate { get; set; }

    [DataMember(Name = "caseFirst")]
    IcuCollationCaseFirst? CaseFirst { get; set; }

    [DataMember(Name = "caseLevel")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? CaseLevel { get; set; }

    [DataMember(Name = "country")]
    string Country { get; set; }

    [DataMember(Name = "decomposition")]
    IcuCollationDecomposition? Decomposition { get; set; }

    [DataMember(Name = "hiraganaQuaternaryMode")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? HiraganaQuaternaryMode { get; set; }

    [DataMember(Name = "language")]
    string Language { get; set; }

    [DataMember(Name = "numeric")]
    [JsonFormatter(typeof (NullableStringBooleanFormatter))]
    bool? Numeric { get; set; }

    [DataMember(Name = "strength")]
    IcuCollationStrength? Strength { get; set; }

    [DataMember(Name = "variableTop")]
    string VariableTop { get; set; }

    [DataMember(Name = "variant")]
    string Variant { get; set; }
  }
}
