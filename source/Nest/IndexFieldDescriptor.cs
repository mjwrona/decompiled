// Decompiled with JetBrains decompiler
// Type: Nest.IndexFieldDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  [Obsolete("Configuration for the _index field is no longer supported in Elasticsearch 7.x and will be removed in the next major release.")]
  public class IndexFieldDescriptor : 
    DescriptorBase<IndexFieldDescriptor, IIndexField>,
    IIndexField,
    IFieldMapping
  {
    bool? IIndexField.Enabled { get; set; }

    public IndexFieldDescriptor Enabled(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<IIndexField, bool?>) ((a, v) => a.Enabled = v));
  }
}
