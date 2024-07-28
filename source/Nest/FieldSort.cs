// Decompiled with JetBrains decompiler
// Type: Nest.FieldSort
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Nest
{
  public class FieldSort : SortBase, IFieldSort, ISort
  {
    private const string ShardDoc = "_shard_doc";
    public static readonly IList<ISort> ByDocumentOrder = (IList<ISort>) new ReadOnlyCollection<ISort>((IList<ISort>) new List<ISort>()
    {
      (ISort) new FieldSort() { Field = (Field) "_doc" }
    });
    public static readonly IList<ISort> ByShardDocumentOrder = (IList<ISort>) new ReadOnlyCollection<ISort>((IList<ISort>) new List<ISort>()
    {
      (ISort) new FieldSort()
      {
        Field = (Field) "_shard_doc"
      }
    });
    public static readonly FieldSort ShardDocumentOrderAscending;
    public static readonly FieldSort ShardDocumentOrderDescending;

    public Field Field { get; set; }

    public bool? IgnoreUnmappedFields { get; set; }

    public FieldType? UnmappedType { get; set; }

    protected override Field SortKey => this.Field;

    static FieldSort()
    {
      FieldSort fieldSort1 = new FieldSort();
      fieldSort1.Field = (Field) "_shard_doc";
      fieldSort1.Order = new SortOrder?(SortOrder.Ascending);
      FieldSort.ShardDocumentOrderAscending = fieldSort1;
      FieldSort fieldSort2 = new FieldSort();
      fieldSort2.Field = (Field) "_shard_doc";
      fieldSort2.Order = new SortOrder?(SortOrder.Descending);
      FieldSort.ShardDocumentOrderDescending = fieldSort2;
    }
  }
}
