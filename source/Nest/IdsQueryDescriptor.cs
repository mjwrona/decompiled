// Decompiled with JetBrains decompiler
// Type: Nest.IdsQueryDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class IdsQueryDescriptor : 
    QueryDescriptorBase<IdsQueryDescriptor, IIdsQuery>,
    IIdsQuery,
    IQuery
  {
    protected override bool Conditionless => IdsQuery.IsConditionless((IIdsQuery) this);

    IEnumerable<Id> IIdsQuery.Values { get; set; }

    public IdsQueryDescriptor Values(params Id[] values) => this.Assign<Id[]>(values, (Action<IIdsQuery, Id[]>) ((a, v) => a.Values = (IEnumerable<Id>) v));

    public IdsQueryDescriptor Values(IEnumerable<Id> values) => this.Values(values != null ? values.ToArray<Id>() : (Id[]) null);

    public IdsQueryDescriptor Values(params string[] values) => this.Assign<IEnumerable<Id>>(values != null ? ((IEnumerable<string>) values).Select<string, Id>((Func<string, Id>) (v => (Id) v)) : (IEnumerable<Id>) null, (Action<IIdsQuery, IEnumerable<Id>>) ((a, v) => a.Values = v));

    public IdsQueryDescriptor Values(IEnumerable<string> values) => this.Values(values.ToArray<string>());

    public IdsQueryDescriptor Values(params long[] values) => this.Assign<IEnumerable<Id>>(values != null ? ((IEnumerable<long>) values).Select<long, Id>((Func<long, Id>) (v => (Id) v)) : (IEnumerable<Id>) null, (Action<IIdsQuery, IEnumerable<Id>>) ((a, v) => a.Values = v));

    public IdsQueryDescriptor Values(IEnumerable<long> values) => this.Values(values.ToArray<long>());

    public IdsQueryDescriptor Values(params Guid[] values) => this.Assign<IEnumerable<Id>>(values != null ? ((IEnumerable<Guid>) values).Select<Guid, Id>((Func<Guid, Id>) (v => (Id) v)) : (IEnumerable<Id>) null, (Action<IIdsQuery, IEnumerable<Id>>) ((a, v) => a.Values = v));

    public IdsQueryDescriptor Values(IEnumerable<Guid> values) => this.Values(values.ToArray<Guid>());
  }
}
