// Decompiled with JetBrains decompiler
// Type: Nest.IAllField
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [Obsolete("The _all field is no longer supported in Elasticsearch 7.x and will be removed in the next major release. The value will not be sent in a request. An _all like field can be achieved using copy_to")]
  [ReadAs(typeof (AllField))]
  public interface IAllField : IFieldMapping
  {
    [DataMember(Name = "analyzer")]
    string Analyzer { get; set; }

    [DataMember(Name = "enabled")]
    bool? Enabled { get; set; }

    [DataMember(Name = "omit_norms")]
    bool? OmitNorms { get; set; }

    [DataMember(Name = "search_analyzer")]
    string SearchAnalyzer { get; set; }

    [DataMember(Name = "similarity")]
    string Similarity { get; set; }

    [DataMember(Name = "store")]
    bool? Store { get; set; }

    [DataMember(Name = "store_term_vector_offsets")]
    bool? StoreTermVectorOffsets { get; set; }

    [DataMember(Name = "store_term_vector_payloads")]
    bool? StoreTermVectorPayloads { get; set; }

    [DataMember(Name = "store_term_vector_positions")]
    bool? StoreTermVectorPositions { get; set; }

    [DataMember(Name = "store_term_vectors")]
    bool? StoreTermVectors { get; set; }
  }
}
