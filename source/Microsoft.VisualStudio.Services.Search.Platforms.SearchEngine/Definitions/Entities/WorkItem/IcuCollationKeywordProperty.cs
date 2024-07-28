// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem.IcuCollationKeywordProperty
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using Nest;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.WorkItem
{
  public class IcuCollationKeywordProperty : IProperty, IFieldMapping
  {
    public string Type { get; set; } = "icu_collation_keyword";

    public PropertyName Name { get; set; }

    public IDictionary<string, object> LocalMetadata { get; set; }

    public IDictionary<string, string> Meta { get; set; }

    [PropertyName("doc_values")]
    public bool? DocValues { get; set; }

    [PropertyName("index")]
    public bool? Index { get; set; }

    [PropertyName("null_value")]
    public string NullValue { get; set; }

    [PropertyName("Store")]
    public bool? Store { get; set; }

    [PropertyName("fields")]
    public IProperties Fields { get; set; }

    [PropertyName("language")]
    public string Language { get; set; }

    [PropertyName("country")]
    public string Country { get; set; }

    [PropertyName("variant")]
    public string Variant { get; set; }

    [PropertyName("strength")]
    public IcuCollationStrength? Strength { get; set; }

    [PropertyName("decomposition")]
    public IcuCollationDecomposition? Decomposition { get; set; }

    [PropertyName("numeric")]
    public bool? Numeric { get; set; }

    [PropertyName("alternate")]
    public IcuCollationAlternate? Alternate { get; set; }

    [PropertyName("case_level")]
    public bool? CaseLevel { get; set; }

    [PropertyName("case_first")]
    public IcuCollationCaseFirst? CaseFirst { get; set; }

    [PropertyName("variable_top")]
    public string VariableTop { get; set; }

    [PropertyName("hiragana_quaternary_mode")]
    public bool? HiraganaQuaternaryMode { get; set; }

    public IcuCollationKeywordProperty(string name) => this.Name = new PropertyName(name);
  }
}
