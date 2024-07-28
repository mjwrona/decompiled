// Decompiled with JetBrains decompiler
// Type: Nest.ICompletionProperty
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  public interface ICompletionProperty : IDocValuesProperty, ICoreProperty, IProperty, IFieldMapping
  {
    [DataMember(Name = "analyzer")]
    string Analyzer { get; set; }

    [DataMember(Name = "contexts")]
    IList<ISuggestContext> Contexts { get; set; }

    [DataMember(Name = "max_input_length")]
    int? MaxInputLength { get; set; }

    [DataMember(Name = "preserve_position_increments")]
    bool? PreservePositionIncrements { get; set; }

    [DataMember(Name = "preserve_separators")]
    bool? PreserveSeparators { get; set; }

    [DataMember(Name = "search_analyzer")]
    string SearchAnalyzer { get; set; }
  }
}
