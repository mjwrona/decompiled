// Decompiled with JetBrains decompiler
// Type: Nest.PercolateQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class PercolateQueryDescriptor<T> : 
    QueryDescriptorBase<PercolateQueryDescriptor<T>, IPercolateQuery>,
    IPercolateQuery,
    IQuery
    where T : class
  {
    private Nest.Routing _routing;

    protected override bool Conditionless => PercolateQuery.IsConditionless((IPercolateQuery) this);

    object IPercolateQuery.Document { get; set; }

    IEnumerable<object> IPercolateQuery.Documents { get; set; }

    Nest.Field IPercolateQuery.Field { get; set; }

    Nest.Id IPercolateQuery.Id { get; set; }

    IndexName IPercolateQuery.Index { get; set; }

    string IPercolateQuery.Preference { get; set; }

    Nest.Routing IPercolateQuery.Routing
    {
      get
      {
        Nest.Routing routing = this._routing;
        if ((object) routing != null)
          return routing;
        return this.Self.Document != null ? new Nest.Routing(this.Self.Document) : (Nest.Routing) null;
      }
      set => this._routing = value;
    }

    long? IPercolateQuery.Version { get; set; }

    public PercolateQueryDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IPercolateQuery, Nest.Field>) ((a, v) => a.Field = v));

    public PercolateQueryDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IPercolateQuery, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public PercolateQueryDescriptor<T> Document<TDocument>(TDocument document) => this.Assign<TDocument>(document, (Action<IPercolateQuery, TDocument>) ((a, v) => a.Document = (object) v));

    public PercolateQueryDescriptor<T> Documents<TDocument>(params TDocument[] documents) => this.Assign<IEnumerable<object>>(documents.Cast<object>(), (Action<IPercolateQuery, IEnumerable<object>>) ((a, v) => a.Documents = v));

    public PercolateQueryDescriptor<T> Documents<TDocument>(IEnumerable<TDocument> documents) => this.Assign<IEnumerable<object>>(documents.Cast<object>(), (Action<IPercolateQuery, IEnumerable<object>>) ((a, v) => a.Documents = v));

    public PercolateQueryDescriptor<T> Id(string id) => this.Assign<string>(id, (Action<IPercolateQuery, string>) ((a, v) => a.Id = (Nest.Id) v));

    public PercolateQueryDescriptor<T> Index(IndexName index) => this.Assign<IndexName>(index, (Action<IPercolateQuery, IndexName>) ((a, v) => a.Index = v));

    public PercolateQueryDescriptor<T> Index<TDocument>() => this.Assign<Type>(typeof (TDocument), (Action<IPercolateQuery, Type>) ((a, v) => a.Index = (IndexName) v));

    public PercolateQueryDescriptor<T> Routing(Nest.Routing routing) => this.Assign<Nest.Routing>(routing, (Action<IPercolateQuery, Nest.Routing>) ((a, v) => a.Routing = v));

    public PercolateQueryDescriptor<T> Preference(string preference) => this.Assign<string>(preference, (Action<IPercolateQuery, string>) ((a, v) => a.Preference = v));

    public PercolateQueryDescriptor<T> Version(long? version) => this.Assign<long?>(version, (Action<IPercolateQuery, long?>) ((a, v) => a.Version = v));
  }
}
