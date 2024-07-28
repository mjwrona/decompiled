// Decompiled with JetBrains decompiler
// Type: Nest.AllField
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  [Obsolete("The _all field is no longer supported in Elasticsearch 7.x and will be removed in the next major release. The value will not be sent in a request. An _all like field can be achieved using copy_to")]
  public class AllField : IAllField, IFieldMapping
  {
    public string Analyzer { get; set; }

    public bool? Enabled { get; set; }

    public bool? OmitNorms { get; set; }

    public string SearchAnalyzer { get; set; }

    public string Similarity { get; set; }

    public bool? Store { get; set; }

    public bool? StoreTermVectorOffsets { get; set; }

    public bool? StoreTermVectorPayloads { get; set; }

    public bool? StoreTermVectorPositions { get; set; }

    public bool? StoreTermVectors { get; set; }
  }
}
