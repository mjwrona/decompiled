// Decompiled with JetBrains decompiler
// Type: Nest.MultiTermVectorOperation`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class MultiTermVectorOperation<T> : IMultiTermVectorOperation where T : class
  {
    private Routing _routing;

    public MultiTermVectorOperation(Id id)
    {
      this.Id = id;
      this.Index = (IndexName) typeof (T);
    }

    public object Document { get; set; }

    public bool? FieldStatistics { get; set; }

    public ITermVectorFilter Filter { get; set; }

    public Id Id { get; set; }

    public IndexName Index { get; set; }

    public bool? Offsets { get; set; }

    public bool? Payloads { get; set; }

    public bool? Positions { get; set; }

    public Routing Routing
    {
      get
      {
        Routing routing = this._routing;
        if ((object) routing != null)
          return routing;
        return this.Document != null ? new Routing(this.Document) : (Routing) null;
      }
      set => this._routing = value;
    }

    public Fields Fields { get; set; }

    public bool? TermStatistics { get; set; }

    public long? Version { get; set; }

    public Elasticsearch.Net.VersionType? VersionType { get; set; }
  }
}
