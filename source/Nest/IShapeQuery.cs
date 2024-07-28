// Decompiled with JetBrains decompiler
// Type: Nest.IShapeQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net.Utf8Json;
using System.Runtime.Serialization;

namespace Nest
{
  [InterfaceDataContract]
  [JsonFormatter(typeof (CompositeFormatter<IShapeQuery, ShapeQueryFormatter, ShapeQueryFieldNameFormatter>))]
  public interface IShapeQuery : IFieldNameQuery, IQuery
  {
    [DataMember(Name = "ignore_unmapped")]
    bool? IgnoreUnmapped { get; set; }

    [DataMember(Name = "indexed_shape")]
    IFieldLookup IndexedShape { get; set; }

    [DataMember(Name = "relation")]
    ShapeRelation? Relation { get; set; }

    [DataMember(Name = "shape")]
    IGeoShape Shape { get; set; }
  }
}
