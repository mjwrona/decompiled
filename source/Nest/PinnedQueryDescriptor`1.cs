// Decompiled with JetBrains decompiler
// Type: Nest.PinnedQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class PinnedQueryDescriptor<T> : 
    QueryDescriptorBase<PinnedQueryDescriptor<T>, IPinnedQuery>,
    IPinnedQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => PinnedQuery.IsConditionless((IPinnedQuery) this);

    IEnumerable<Id> IPinnedQuery.Ids { get; set; }

    QueryContainer IPinnedQuery.Organic { get; set; }

    public PinnedQueryDescriptor<T> Ids(params Id[] ids) => this.Assign<Id[]>(ids, (Action<IPinnedQuery, Id[]>) ((a, v) => a.Ids = (IEnumerable<Id>) v));

    public PinnedQueryDescriptor<T> Ids(IEnumerable<Id> ids) => this.Assign<IEnumerable<Id>>(ids, (Action<IPinnedQuery, IEnumerable<Id>>) ((a, v) => a.Ids = v));

    public PinnedQueryDescriptor<T> Ids(IEnumerable<string> ids) => this.Assign<IEnumerable<Id>>(ids != null ? ids.Select<string, Id>((Func<string, Id>) (i => (Id) i)) : (IEnumerable<Id>) null, (Action<IPinnedQuery, IEnumerable<Id>>) ((a, v) => a.Ids = v));

    public PinnedQueryDescriptor<T> Ids(IEnumerable<long> ids) => this.Assign<IEnumerable<Id>>(ids != null ? ids.Select<long, Id>((Func<long, Id>) (i => (Id) i)) : (IEnumerable<Id>) null, (Action<IPinnedQuery, IEnumerable<Id>>) ((a, v) => a.Ids = v));

    public PinnedQueryDescriptor<T> Ids(IEnumerable<Guid> ids) => this.Assign<IEnumerable<Id>>(ids != null ? ids.Select<Guid, Id>((Func<Guid, Id>) (i => (Id) i)) : (IEnumerable<Id>) null, (Action<IPinnedQuery, IEnumerable<Id>>) ((a, v) => a.Ids = v));

    public PinnedQueryDescriptor<T> Organic(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<IPinnedQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Organic = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}
