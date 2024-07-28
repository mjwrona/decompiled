// Decompiled with JetBrains decompiler
// Type: Nest.AllFieldDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  [Obsolete("The _all field is no longer supported in Elasticsearch 7.x and will be removed in the next major release. The value will not be sent in a request. An _all like field can be achieved using copy_to")]
  public class AllFieldDescriptor : 
    DescriptorBase<AllFieldDescriptor, IAllField>,
    IAllField,
    IFieldMapping
  {
    string IAllField.Analyzer { get; set; }

    bool? IAllField.Enabled { get; set; }

    bool? IAllField.OmitNorms { get; set; }

    string IAllField.SearchAnalyzer { get; set; }

    string IAllField.Similarity { get; set; }

    bool? IAllField.Store { get; set; }

    bool? IAllField.StoreTermVectorOffsets { get; set; }

    bool? IAllField.StoreTermVectorPayloads { get; set; }

    bool? IAllField.StoreTermVectorPositions { get; set; }

    bool? IAllField.StoreTermVectors { get; set; }

    public AllFieldDescriptor Enabled(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<IAllField, bool?>) ((a, v) => a.Enabled = v));

    public AllFieldDescriptor Store(bool? store = true) => this.Assign<bool?>(store, (Action<IAllField, bool?>) ((a, v) => a.Store = v));

    public AllFieldDescriptor StoreTermVectors(bool? store = true) => this.Assign<bool?>(store, (Action<IAllField, bool?>) ((a, v) => a.StoreTermVectors = v));

    public AllFieldDescriptor StoreTermVectorOffsets(bool? store = true) => this.Assign<bool?>(store, (Action<IAllField, bool?>) ((a, v) => a.StoreTermVectorOffsets = v));

    public AllFieldDescriptor StoreTermVectorPositions(bool? store = true) => this.Assign<bool?>(store, (Action<IAllField, bool?>) ((a, v) => a.StoreTermVectorPositions = v));

    public AllFieldDescriptor StoreTermVectorPayloads(bool? store = true) => this.Assign<bool?>(store, (Action<IAllField, bool?>) ((a, v) => a.StoreTermVectorPayloads = v));

    public AllFieldDescriptor Analyzer(string analyzer) => this.Assign<string>(analyzer, (Action<IAllField, string>) ((a, v) => a.Analyzer = v));

    public AllFieldDescriptor SearchAnalyzer(string searchAnalyzer) => this.Assign<string>(searchAnalyzer, (Action<IAllField, string>) ((a, v) => a.SearchAnalyzer = v));

    public AllFieldDescriptor Similarity(string similarity) => this.Assign<string>(similarity, (Action<IAllField, string>) ((a, v) => a.Similarity = v));
  }
}
