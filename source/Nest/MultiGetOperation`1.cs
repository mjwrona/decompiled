// Decompiled with JetBrains decompiler
// Type: Nest.MultiGetOperation`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MultiGetOperation<T> : IMultiGetOperation
  {
    public MultiGetOperation(Id id)
    {
      this.Id = id;
      this.Index = (IndexName) typeof (T);
    }

    public object Document { get; set; }

    public Id Id { get; set; }

    public IndexName Index { get; set; }

    public string Routing { get; set; }

    public Union<bool, ISourceFilter> Source { get; set; }

    public Fields StoredFields { get; set; }

    public long? Version { get; set; }

    public Elasticsearch.Net.VersionType? VersionType { get; set; }

    bool IMultiGetOperation.CanBeFlattened => this.Index == (IndexName) null && this.Routing == null && this.Source == null && this.StoredFields == (Fields) null;

    Type IMultiGetOperation.ClrType => typeof (T);
  }
}
