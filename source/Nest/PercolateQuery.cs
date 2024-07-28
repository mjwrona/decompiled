// Decompiled with JetBrains decompiler
// Type: Nest.PercolateQuery
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class PercolateQuery : QueryBase, IPercolateQuery, IQuery
  {
    private Routing _routing;

    public object Document { get; set; }

    public IEnumerable<object> Documents { get; set; }

    public Field Field { get; set; }

    public Id Id { get; set; }

    public IndexName Index { get; set; }

    public string Preference { get; set; }

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

    public long? Version { get; set; }

    protected override bool Conditionless => PercolateQuery.IsConditionless((IPercolateQuery) this);

    internal override void InternalWrapInContainer(IQueryContainer c) => c.Percolate = (IPercolateQuery) this;

    internal static bool IsConditionless(IPercolateQuery q)
    {
      if (q.Document != null || q.Documents != null)
        return false;
      return q.Index == (IndexName) null || q.Id.IsConditionless() || q.Field.IsConditionless();
    }
  }
}
